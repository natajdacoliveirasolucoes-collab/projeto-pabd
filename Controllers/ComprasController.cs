using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Dtos;
using ApiFinanceiro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Controllers
{
    [Route("api/compras")]
    [ApiController]
    [Authorize]
    public class ComprasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComprasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            var usuarioId = this.GetUsuarioId();

            if (usuarioId is null)
            {
                return Unauthorized(new { message = "Token invalido" });
            }

            var compras = await _context.Compras
                .AsNoTracking()
                .Include(c => c.Itens)
                .ThenInclude(i => i.TipoIngresso)
                .ThenInclude(t => t!.Evento)
                .Where(c => c.UsuarioId == usuarioId.Value)
                .OrderByDescending(c => c.DataCompra)
                .ToListAsync();

            return Ok(compras);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> FindById(int id)
        {
            var usuarioId = this.GetUsuarioId();

            if (usuarioId is null)
            {
                return Unauthorized(new { message = "Token invalido" });
            }

            var compra = await _context.Compras
                .AsNoTracking()
                .Include(c => c.Itens)
                .ThenInclude(i => i.TipoIngresso)
                .ThenInclude(t => t!.Evento)
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId.Value);

            return compra is null ? NotFound(new { message = "Compra nao encontrada" }) : Ok(compra);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompraCreateDto dto)
        {
            var usuarioId = this.GetUsuarioId();

            if (usuarioId is null)
            {
                return Unauthorized(new { message = "Token invalido" });
            }

            if (dto.Itens is null || dto.Itens.Count == 0)
            {
                return BadRequest(new { message = "Compra deve possuir pelo menos um item" });
            }

            var itensAgrupados = dto.Itens
                .GroupBy(i => i.TipoIngressoId)
                .Select(g => new
                {
                    TipoIngressoId = g.Key,
                    Quantidade = g.Sum(i => i.Quantidade)
                })
                .ToList();

            if (itensAgrupados.Any(i => i.Quantidade <= 0))
            {
                return BadRequest(new { message = "Quantidade de ingresso deve ser maior que zero" });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var tipoIds = itensAgrupados.Select(i => i.TipoIngressoId).ToList();
            var tiposIngresso = await _context.TiposIngresso
                .Include(t => t.Evento)
                .Where(t => tipoIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id);

            var compra = new Compra
            {
                UsuarioId = usuarioId.Value,
                StatusPagamento = "pendente"
            };

            foreach (var item in itensAgrupados)
            {
                if (!tiposIngresso.TryGetValue(item.TipoIngressoId, out var tipoIngresso))
                {
                    return NotFound(new { message = $"Tipo de ingresso #{item.TipoIngressoId} nao encontrado" });
                }

                if (!string.Equals(tipoIngresso.Evento?.Status, "publicado", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = $"Evento do ingresso #{item.TipoIngressoId} nao esta publicado" });
                }

                if (tipoIngresso.QtdDisponivel < item.Quantidade)
                {
                    return BadRequest(new { message = $"Ingresso #{item.TipoIngressoId} indisponivel na quantidade solicitada" });
                }

                tipoIngresso.QtdDisponivel -= item.Quantidade;
                compra.ValorTotal += tipoIngresso.Preco * item.Quantidade;
                compra.Itens.Add(new ItemCompra
                {
                    TipoIngressoId = item.TipoIngressoId,
                    Quantidade = item.Quantidade,
                    ValorUnitario = tipoIngresso.Preco
                });
            }

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(FindById), new { id = compra.Id }, compra);
        }
    }
}
