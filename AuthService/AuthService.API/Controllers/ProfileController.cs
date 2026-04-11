using AuthService.API.Data;
using AuthService.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMINISTRADOR")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/profile
        [HttpGet]
        public IActionResult GetAll()
        {
            var profiles = _context.Profiles.ToList();
            return Ok(profiles);
        }

        // GET: api/profile/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var profile = _context.Profiles.FirstOrDefault(x => x.Id == id);

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
