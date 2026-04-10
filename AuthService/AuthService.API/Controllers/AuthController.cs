using AuthService.API.Data;
using AuthService.API.DTOs;
using AuthService.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var profileId = User.FindFirst("ProfileId")?.Value;
            var profileName = User.FindFirst("ProfileName")?.Value;

            return Ok(new
            {
                userId,
                email,
                profileId,
                profileName
            });
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterUserDto dto)
        {
            // validação básica
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email e senha são obrigatórios.");

            // verifica se já existe
            var profileExists = _context.Profiles.Any(x => x.Id == dto.ProfileId);
            if (!profileExists)
                return BadRequest("Perfil inválido.");

            var exists = _context.Users.Any(x => x.Email == dto.Email);
            if (exists)
                return BadRequest("Usuário já existe.");

            // gera hash da senha
            var passwordHash = HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                ProfileId = dto.ProfileId
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Usuário cadastrado com sucesso.");
        }

        [HttpPost("login")]
        public IActionResult Login(RegisterUserDto dto)
        {
            var user = _context.Users
                .Include(x => x.Profile)
                .FirstOrDefault(x => x.Email == dto.Email);

            if (user == null)
                return Unauthorized("Usuário ou senha inválidos.");

            var passwordHash = HashPassword(dto.Password);

            if (user.PasswordHash != passwordHash)
                return Unauthorized("Usuário ou senha inválidos.");

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok(new
            {
                token = accessToken,
                refreshToken = refreshToken
            });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken(RefreshTokenRequestDto dto)
        {
            var user = _context.Users.FirstOrDefault(x => x.RefreshToken == dto.RefreshToken);

            if (user == null)
                return Unauthorized("Refresh token inválido.");

            if (user.RefreshTokenExpiresAt == null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return Unauthorized("Refresh token expirado.");

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok(new
            {
                token = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUsers()
        {
            var users = _context.Users
                .Select(x => new
                {
                    x.Id,
                    x.Email,
                    x.CreatedAt,
                    x.ProfileId,
                    ProfileName = x.Profile.Name
                })
                .ToList();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Email,
                    x.CreatedAt,
                    x.ProfileId,
                    ProfileName = x.Profile.Name
                })
                .FirstOrDefault();

            if (user == null)
                return NotFound("Usuário não encontrado.");

            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, RegisterUserDto dto)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email e senha são obrigatórios.");

            var emailJaExiste = _context.Users.Any(x => x.Email == dto.Email && x.Id != id);
            if (emailJaExiste)
                return BadRequest("Já existe outro usuário com esse email.");

            var profileExists = _context.Profiles.Any(x => x.Id == dto.ProfileId);
            if (!profileExists)
                return BadRequest("Perfil inválido.");

            user.Email = dto.Email;
            user.PasswordHash = HashPassword(dto.Password);
            user.ProfileId = dto.ProfileId;

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok("Usuário alterado com sucesso.");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok("Usuário removido com sucesso.");
        }

        #region FUNÇÕES
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()
                .GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("ProfileId", user.ProfileId.ToString()),
                new Claim("ProfileName", user.Profile?.Name ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        #endregion
    }
}
