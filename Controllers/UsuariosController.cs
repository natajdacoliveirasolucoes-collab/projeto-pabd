using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Dtos;
using ApiFinanceiro.Models;
using ApiFinanceiro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;

        public UsuariosController(AppDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var emailExists = await _context.Usuarios.AnyAsync(u => u.Email == email);

            if (emailExists)
            {
                return Conflict(new { message = "E-mail ja cadastrado" });
            }

            var usuario = new Usuario
            {
                Nome = dto.Nome.Trim(),
                Email = email,
                Senha = _passwordService.Hash(dto.Senha),
                Telefone = dto.Telefone
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(FindById), new { id = usuario.Id }, ToResponse(usuario));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> FindAll()
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .Select(u => new UsuarioResponseDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Telefone = u.Telefone,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> FindById(int id)
        {
            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            return usuario is null ? NotFound(new { message = "Usuario nao encontrado" }) : Ok(ToResponse(usuario));
        }

        private static UsuarioResponseDto ToResponse(Usuario usuario)
        {
            return new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                CreatedAt = usuario.CreatedAt
            };
        }
    }
}
