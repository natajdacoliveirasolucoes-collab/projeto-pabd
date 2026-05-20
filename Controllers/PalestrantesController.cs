using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Dtos;
using ApiFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Controllers
{
    [Route("api/palestrantes")]
    [ApiController]
    public class PalestrantesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PalestrantesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await _context.Palestrantes.AsNoTracking().OrderBy(p => p.Nome).ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> FindById(int id)
        {
            var palestrante = await _context.Palestrantes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return palestrante is null ? NotFound(new { message = "Palestrante nao encontrado" }) : Ok(palestrante);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PalestranteDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _context.Palestrantes.AnyAsync(p => p.Email == email))
            {
                return Conflict(new { message = "E-mail ja cadastrado para palestrante" });
            }

            var palestrante = new Palestrante
            {
                Nome = dto.Nome.Trim(),
                Bio = dto.Bio,
                Email = email,
                FotoUrl = dto.FotoUrl
            };

            _context.Palestrantes.Add(palestrante);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(FindById), new { id = palestrante.Id }, palestrante);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PalestranteDto dto)
        {
            var palestrante = await _context.Palestrantes.FindAsync(id);

            if (palestrante is null)
            {
                return NotFound(new { message = "Palestrante nao encontrado" });
            }

            var email = dto.Email.Trim().ToLowerInvariant();
            var exists = await _context.Palestrantes.AnyAsync(p => p.Id != id && p.Email == email);

            if (exists)
            {
                return Conflict(new { message = "E-mail ja cadastrado para palestrante" });
            }

            palestrante.Nome = dto.Nome.Trim();
            palestrante.Bio = dto.Bio;
            palestrante.Email = email;
            palestrante.FotoUrl = dto.FotoUrl;

            await _context.SaveChangesAsync();

            return Ok(palestrante);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Remove(int id)
        {
            var palestrante = await _context.Palestrantes.FindAsync(id);

            if (palestrante is null)
            {
                return NotFound(new { message = "Palestrante nao encontrado" });
            }

            _context.Palestrantes.Remove(palestrante);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
