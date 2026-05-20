using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Dtos;
using ApiFinanceiro.Models;
using ApiFinanceiro.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, PasswordService passwordService, TokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario is null || !_passwordService.Verify(dto.Senha, usuario.Senha))
            {
                return Unauthorized(new { message = "Credenciais invalidas" });
            }

            return Ok(new
            {
                token = _tokenService.Generate(usuario),
                expiresInHours = 24,
                usuario = new UsuarioResponseDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Telefone = usuario.Telefone,
                    CreatedAt = usuario.CreatedAt
                }
            });
        }
    }
}
