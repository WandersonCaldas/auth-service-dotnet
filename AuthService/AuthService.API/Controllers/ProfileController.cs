using AuthService.API.Data;
using AuthService.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "ADMINISTRADOR")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly int pageSize = 0;

        public ProfileController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            pageSize = _configuration.GetValue<int>("Pagination:PageSize");
        }

        // GET: api/profile
        [HttpGet]
        public IActionResult GetAll(int page = 1)
        {           
            if (page < 1)
                page = 1;

            var totalRecords = _context.Profiles.AsNoTracking().Count();

            var profiles = _context.Profiles
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                page,
                pageSize,
                totalRecords,
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                data = profiles
            });
        }

        // GET: api/profile/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var profile = _context.Profiles.AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (profile == null)
                return NotFound("Perfil não encontrado.");

            return Ok(profile);
        }

        // POST: api/profile
        [HttpPost]
        public IActionResult Create(Profile profile)
        {
            if (string.IsNullOrWhiteSpace(profile.Name))
                return BadRequest("Nome é obrigatório.");

            var exists = _context.Profiles.Any(x => x.Name.ToUpper().Trim() == profile.Name.ToUpper().Trim());
            if (exists)
                return BadRequest("Perfil já existe.");

            _context.Profiles.Add(profile);
            _context.SaveChanges();

            return Ok(profile);
        }

        // PUT: api/profile/1
        [HttpPut("{id}")]
        public IActionResult Update(int id, Profile profile)
        {
            var existing = _context.Profiles.FirstOrDefault(x => x.Id == id);

            if (existing == null)
                return NotFound("Perfil não encontrado.");

            if (string.IsNullOrWhiteSpace(profile.Name))
                return BadRequest("Nome é obrigatório.");

            var profileJaExiste = _context.Profiles.Any(x => x.Name.ToUpper().Trim() == profile.Name.ToUpper().Trim() && x.Id != id);
            if (profileJaExiste)
                return BadRequest("Já existe outro perfil com esse nome.");

            existing.Name = profile.Name;

            _context.Profiles.Update(existing);
            _context.SaveChanges();

            return Ok(existing);
        }

        // DELETE: api/profile/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var profile = _context.Profiles.FirstOrDefault(x => x.Id == id);

            if (profile == null)
                return NotFound("Perfil não encontrado.");

            _context.Profiles.Remove(profile);
            _context.SaveChanges();

            return Ok("Perfil removido com sucesso.");
        }
    }
}
