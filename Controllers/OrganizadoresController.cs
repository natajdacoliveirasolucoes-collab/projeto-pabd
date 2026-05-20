using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Dtos;
using ApiFinanceiro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Controllers
{
    [Route("api/organizadores")]
    [ApiController]
    [Authorize]
    public class OrganizadoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrganizadoresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrganizadorCreateDto dto)
        {
            var usuarioId = this.GetUsuarioId();

            if (usuarioId is null)
            {
                return Unauthorized(new { message = "Token invalido" });
            }

            var usuarioExists = await _context.Usuarios.AnyAsync(u => u.Id == usuarioId.Value);

            if (!usuarioExists)
            {
                return NotFound(new { message = "Usuario nao encontrado" });
            }

            var alreadyOrganizer = await _context.Organizadores.AnyAsync(o => o.UsuarioId == usuarioId.Value);

            if (alreadyOrganizer)
            {
                return Conflict(new { message = "Usuario ja possui perfil de organizador" });
            }

            var cnpj = dto.Cnpj.Trim();
            var cnpjExists = await _context.Organizadores.AnyAsync(o => o.Cnpj == cnpj);

            if (cnpjExists)
            {
                return Conflict(new { message = "CNPJ ja cadastrado" });
            }

            var organizador = new Organizador
            {
                UsuarioId = usuarioId.Value,
                RazaoSocial = dto.RazaoSocial.Trim(),
                Cnpj = cnpj,
                Descricao = dto.Descricao
            };

            _context.Organizadores.Add(organizador);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(FindById), new { id = organizador.Id }, organizador);
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            var organizadores = await _context.Organizadores
                .AsNoTracking()
                .Include(o => o.Usuario)
                .ToListAsync();

            return Ok(organizadores);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> FindById(int id)
        {
            var organizador = await _context.Organizadores
                .AsNoTracking()
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(o => o.Id == id);

            return organizador is null ? NotFound(new { message = "Organizador nao encontrado" }) : Ok(organizador);
        }
    }
}
