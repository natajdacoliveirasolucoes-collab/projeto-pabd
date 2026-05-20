using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Dtos;
using ApiFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Controllers
{
    [Route("api/locais")]
    [ApiController]
    public class LocaisController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocaisController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await _context.Locais.AsNoTracking().OrderBy(l => l.Nome).ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> FindById(int id)
        {
            var local = await _context.Locais.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
            return local is null ? NotFound(new { message = "Local nao encontrado" }) : Ok(local);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LocalDto dto)
        {
            var local = new Local
            {
                Nome = dto.Nome.Trim(),
                Endereco = dto.Endereco.Trim(),
                Cidade = dto.Cidade.Trim(),
                Estado = dto.Estado.Trim().ToUpperInvariant(),
                Capacidade = dto.Capacidade
            };

            _context.Locais.Add(local);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(FindById), new { id = local.Id }, local);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] LocalDto dto)
        {
            var local = await _context.Locais.FindAsync(id);

            if (local is null)
            {
                return NotFound(new { message = "Local nao encontrado" });
            }

            local.Nome = dto.Nome.Trim();
            local.Endereco = dto.Endereco.Trim();
            local.Cidade = dto.Cidade.Trim();
            local.Estado = dto.Estado.Trim().ToUpperInvariant();
            local.Capacidade = dto.Capacidade;

            await _context.SaveChangesAsync();

            return Ok(local);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Remove(int id)
        {
            var local = await _context.Locais.FindAsync(id);

            if (local is null)
            {
                return NotFound(new { message = "Local nao encontrado" });
            }

            _context.Locais.Remove(local);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
