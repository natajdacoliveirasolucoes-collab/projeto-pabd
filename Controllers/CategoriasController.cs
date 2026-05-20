using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Dtos;
using ApiFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Controllers
{
    [Route("api/categorias")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            return Ok(await _context.Categorias.AsNoTracking().OrderBy(c => c.Nome).ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> FindById(int id)
        {
            var categoria = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return categoria is null ? NotFound(new { message = "Categoria nao encontrada" }) : Ok(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoriaDto dto)
        {
            var nome = dto.Nome.Trim();

            if (await _context.Categorias.AnyAsync(c => c.Nome == nome))
            {
                return Conflict(new { message = "Categoria ja cadastrada" });
            }

            var categoria = new Categoria
            {
                Nome = nome,
                Descricao = dto.Descricao
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(FindById), new { id = categoria.Id }, categoria);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoriaDto dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria is null)
            {
                return NotFound(new { message = "Categoria nao encontrada" });
            }

            var nome = dto.Nome.Trim();
            var exists = await _context.Categorias.AnyAsync(c => c.Id != id && c.Nome == nome);

            if (exists)
            {
                return Conflict(new { message = "Categoria ja cadastrada" });
            }

            categoria.Nome = nome;
            categoria.Descricao = dto.Descricao;

            await _context.SaveChangesAsync();

            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Remove(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria is null)
            {
                return NotFound(new { message = "Categoria nao encontrada" });
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
