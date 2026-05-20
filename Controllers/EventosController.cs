using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Dtos;
using ApiFinanceiro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Controllers
{
    [Route("api/eventos")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        private static readonly HashSet<string> StatusPermitidos = new(StringComparer.OrdinalIgnoreCase)
        {
            "rascunho",
            "publicado",
            "cancelado",
            "encerrado"
        };

        private readonly AppDbContext _context;

        public EventosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> FindAll([FromQuery] int? categoriaId, [FromQuery] string? cidade, [FromQuery] DateTime? data, [FromQuery] string? status)
        {
            var query = QueryEventos();

            if (categoriaId.HasValue)
            {
                query = query.Where(e => e.CategoriaId == categoriaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(cidade))
            {
                var cidadeFiltro = cidade.Trim();
                query = query.Where(e => e.Local != null && e.Local.Cidade.Contains(cidadeFiltro));
            }

            if (data.HasValue)
            {
                var inicioDia = data.Value.Date;
                var fimDia = inicioDia.AddDays(1);
                query = query.Where(e => e.DataInicio < fimDia && e.DataFim >= inicioDia);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var statusFiltro = status.Trim().ToLowerInvariant();
                query = query.Where(e => e.Status == statusFiltro);
            }

            var eventos = await query.OrderBy(e => e.DataInicio).ToListAsync();
            return Ok(eventos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> FindById(int id)
        {
            var evento = await QueryEventos()
                .Include(e => e.EventoPalestrantes!)
                .ThenInclude(ep => ep.Palestrante)
                .FirstOrDefaultAsync(e => e.Id == id);

            return evento is null ? NotFound(new { message = "Evento nao encontrado" }) : Ok(evento);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] EventoCreateDto dto)
        {
            var organizador = await GetOrganizadorAtual();

            if (organizador is null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Usuario nao possui perfil de organizador" });
            }

            var validationResult = await ValidateEvento(dto.CategoriaId, dto.LocalId, dto.DataInicio, dto.DataFim);

            if (validationResult is not null)
            {
                return validationResult;
            }

            var hasConflict = await HasLocalDateConflict(dto.LocalId, dto.DataInicio, dto.DataFim);

            if (hasConflict)
            {
                return Conflict(new { message = "Conflito de datas para o local informado" });
            }

            var evento = new Evento
            {
                OrganizadorId = organizador.Id,
                CategoriaId = dto.CategoriaId,
                LocalId = dto.LocalId,
                Titulo = dto.Titulo.Trim(),
                Descricao = dto.Descricao,
                DataInicio = dto.DataInicio,
                DataFim = dto.DataFim,
                Status = "rascunho"
            };

            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(FindById), new { id = evento.Id }, evento);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] EventoUpdateDto dto)
        {
            var organizador = await GetOrganizadorAtual();

            if (organizador is null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Usuario nao possui perfil de organizador" });
            }

            var evento = await _context.Eventos.FindAsync(id);

            if (evento is null)
            {
                return NotFound(new { message = "Evento nao encontrado" });
            }

            if (evento.OrganizadorId != organizador.Id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Evento pertence a outro organizador" });
            }

            var status = dto.Status.Trim().ToLowerInvariant();

            if (!StatusPermitidos.Contains(status))
            {
                return BadRequest(new { message = "Status invalido" });
            }

            var validationResult = await ValidateEvento(dto.CategoriaId, dto.LocalId, dto.DataInicio, dto.DataFim);

            if (validationResult is not null)
            {
                return validationResult;
            }

            var hasConflict = await HasLocalDateConflict(dto.LocalId, dto.DataInicio, dto.DataFim, id);

            if (hasConflict)
            {
                return Conflict(new { message = "Conflito de datas para o local informado" });
            }

            evento.CategoriaId = dto.CategoriaId;
            evento.LocalId = dto.LocalId;
            evento.Titulo = dto.Titulo.Trim();
            evento.Descricao = dto.Descricao;
            evento.DataInicio = dto.DataInicio;
            evento.DataFim = dto.DataFim;
            evento.Status = status;

            await _context.SaveChangesAsync();

            return Ok(evento);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Remove(int id)
        {
            var organizador = await GetOrganizadorAtual();

            if (organizador is null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Usuario nao possui perfil de organizador" });
            }

            var evento = await _context.Eventos.FindAsync(id);

            if (evento is null)
            {
                return NotFound(new { message = "Evento nao encontrado" });
            }

            if (evento.OrganizadorId != organizador.Id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Evento pertence a outro organizador" });
            }

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id:int}/inscricoes")]
        [Authorize]
        public async Task<IActionResult> CreateInscricao(int id)
        {
            var usuarioId = this.GetUsuarioId();

            if (usuarioId is null)
            {
                return Unauthorized(new { message = "Token invalido" });
            }

            var evento = await _context.Eventos.FindAsync(id);

            if (evento is null)
            {
                return NotFound(new { message = "Evento nao encontrado" });
            }

            if (!string.Equals(evento.Status, "publicado", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Evento precisa estar publicado para inscricao" });
            }

            var exists = await _context.Inscricoes.AnyAsync(i => i.UsuarioId == usuarioId.Value && i.EventoId == id);

            if (exists)
            {
                return Conflict(new { message = "Usuario ja inscrito neste evento" });
            }

            var inscricao = new Inscricao
            {
                UsuarioId = usuarioId.Value,
                EventoId = id
            };

            _context.Inscricoes.Add(inscricao);
            await _context.SaveChangesAsync();

            return Created($"/api/eventos/{id}/inscricoes", inscricao);
        }

        [HttpPost("{id:int}/palestrantes")]
        [Authorize]
        public async Task<IActionResult> VincularPalestrante(int id, [FromBody] VincularPalestranteDto dto)
        {
            var evento = await FindEventoDoOrganizador(id);

            if (evento.Result is not null)
            {
                return evento.Result;
            }

            var palestranteExists = await _context.Palestrantes.AnyAsync(p => p.Id == dto.PalestranteId);

            if (!palestranteExists)
            {
                return NotFound(new { message = "Palestrante nao encontrado" });
            }

            var alreadyLinked = await _context.EventosPalestrantes.AnyAsync(ep => ep.EventoId == id && ep.PalestranteId == dto.PalestranteId);

            if (alreadyLinked)
            {
                return Conflict(new { message = "Palestrante ja vinculado ao evento" });
            }

            var eventoPalestrante = new EventoPalestrante
            {
                EventoId = id,
                PalestranteId = dto.PalestranteId,
                Papel = string.IsNullOrWhiteSpace(dto.Papel) ? "palestrante" : dto.Papel.Trim()
            };

            _context.EventosPalestrantes.Add(eventoPalestrante);
            await _context.SaveChangesAsync();

            return Created($"/api/eventos/{id}/palestrantes/{dto.PalestranteId}", eventoPalestrante);
        }

        [HttpPost("{id:int}/programacao")]
        [Authorize]
        public async Task<IActionResult> CreateProgramacao(int id, [FromBody] ProgramacaoCreateDto dto)
        {
            var evento = await FindEventoDoOrganizador(id);

            if (evento.Result is not null)
            {
                return evento.Result;
            }

            if (dto.HorarioFim <= dto.HorarioInicio)
            {
                return BadRequest(new { message = "Horario final deve ser maior que o horario inicial" });
            }

            if (dto.HorarioInicio < evento.Value!.DataInicio || dto.HorarioFim > evento.Value.DataFim)
            {
                return BadRequest(new { message = "Programacao deve ocorrer dentro do periodo do evento" });
            }

            var palestranteExists = await _context.Palestrantes.AnyAsync(p => p.Id == dto.PalestranteId);

            if (!palestranteExists)
            {
                return NotFound(new { message = "Palestrante nao encontrado" });
            }

            var sala = string.IsNullOrWhiteSpace(dto.Sala) ? null : dto.Sala.Trim();

            if (sala is not null)
            {
                var hasRoomConflict = await _context.Programacoes.AnyAsync(p =>
                    p.EventoId == id &&
                    p.Sala == sala &&
                    p.HorarioInicio < dto.HorarioFim &&
                    dto.HorarioInicio < p.HorarioFim);

                if (hasRoomConflict)
                {
                    return Conflict(new { message = "Conflito de sala e horario na programacao" });
                }
            }

            var programacao = new Programacao
            {
                EventoId = id,
                PalestranteId = dto.PalestranteId,
                Titulo = dto.Titulo.Trim(),
                Descricao = dto.Descricao,
                HorarioInicio = dto.HorarioInicio,
                HorarioFim = dto.HorarioFim,
                Sala = sala
            };

            _context.Programacoes.Add(programacao);
            await _context.SaveChangesAsync();

            return Created($"/api/eventos/{id}/programacao/{programacao.Id}", programacao);
        }

        [HttpPost("{id:int}/tipos-ingresso")]
        [Authorize]
        public async Task<IActionResult> CreateTipoIngresso(int id, [FromBody] TipoIngressoCreateDto dto)
        {
            var evento = await FindEventoDoOrganizador(id);

            if (evento.Result is not null)
            {
                return evento.Result;
            }

            var tipoIngresso = new TipoIngresso
            {
                EventoId = id,
                Nome = dto.Nome.Trim(),
                Preco = dto.Preco,
                QtdDisponivel = dto.QtdDisponivel
            };

            _context.TiposIngresso.Add(tipoIngresso);
            await _context.SaveChangesAsync();

            return Created($"/api/eventos/{id}/tipos-ingresso/{tipoIngresso.Id}", tipoIngresso);
        }

        [HttpPost("{id:int}/avaliacoes")]
        [Authorize]
        public async Task<IActionResult> CreateAvaliacao(int id, [FromBody] AvaliacaoCreateDto dto)
        {
            var usuarioId = this.GetUsuarioId();

            if (usuarioId is null)
            {
                return Unauthorized(new { message = "Token invalido" });
            }

            var evento = await _context.Eventos.FindAsync(id);

            if (evento is null)
            {
                return NotFound(new { message = "Evento nao encontrado" });
            }

            if (evento.DataFim > DateTime.UtcNow)
            {
                return BadRequest(new { message = "Evento ainda nao foi realizado" });
            }

            var inscrito = await _context.Inscricoes.AnyAsync(i => i.UsuarioId == usuarioId.Value && i.EventoId == id);

            if (!inscrito)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Usuario nao esta inscrito neste evento" });
            }

            var exists = await _context.Avaliacoes.AnyAsync(a => a.UsuarioId == usuarioId.Value && a.EventoId == id);

            if (exists)
            {
                return Conflict(new { message = "Usuario ja avaliou este evento" });
            }

            var avaliacao = new Avaliacao
            {
                UsuarioId = usuarioId.Value,
                EventoId = id,
                Nota = dto.Nota,
                Comentario = dto.Comentario
            };

            _context.Avaliacoes.Add(avaliacao);
            await _context.SaveChangesAsync();

            return Created($"/api/eventos/{id}/avaliacoes/{avaliacao.Id}", avaliacao);
        }

        private IQueryable<Evento> QueryEventos()
        {
            return _context.Eventos
                .AsNoTracking()
                .Include(e => e.Categoria)
                .Include(e => e.Local)
                .Include(e => e.Organizador)
                .ThenInclude(o => o!.Usuario)
                .Include(e => e.Programacao!)
                .ThenInclude(p => p.Palestrante)
                .Include(e => e.TiposIngresso)
                .Include(e => e.Avaliacoes);
        }

        private async Task<Organizador?> GetOrganizadorAtual()
        {
            var usuarioId = this.GetUsuarioId();

            if (usuarioId is null)
            {
                return null;
            }

            return await _context.Organizadores.FirstOrDefaultAsync(o => o.UsuarioId == usuarioId.Value);
        }

        private async Task<bool> HasLocalDateConflict(int localId, DateTime dataInicio, DateTime dataFim, int? ignoredEventoId = null)
        {
            return await _context.Eventos.AnyAsync(e =>
                e.LocalId == localId &&
                (!ignoredEventoId.HasValue || e.Id != ignoredEventoId.Value) &&
                e.Status != "cancelado" &&
                e.DataInicio < dataFim &&
                dataInicio < e.DataFim);
        }

        private async Task<IActionResult?> ValidateEvento(int categoriaId, int localId, DateTime dataInicio, DateTime dataFim)
        {
            if (dataFim <= dataInicio)
            {
                return BadRequest(new { message = "Data final deve ser maior que a data inicial" });
            }

            var categoriaExists = await _context.Categorias.AnyAsync(c => c.Id == categoriaId);

            if (!categoriaExists)
            {
                return NotFound(new { message = "Categoria nao encontrada" });
            }

            var localExists = await _context.Locais.AnyAsync(l => l.Id == localId);

            if (!localExists)
            {
                return NotFound(new { message = "Local nao encontrado" });
            }

            return null;
        }

        private async Task<(Evento? Value, IActionResult? Result)> FindEventoDoOrganizador(int id)
        {
            var organizador = await GetOrganizadorAtual();

            if (organizador is null)
            {
                return (null, StatusCode(StatusCodes.Status403Forbidden, new { message = "Usuario nao possui perfil de organizador" }));
            }

            var evento = await _context.Eventos.FindAsync(id);

            if (evento is null)
            {
                return (null, NotFound(new { message = "Evento nao encontrado" }));
            }

            if (evento.OrganizadorId != organizador.Id)
            {
                return (null, StatusCode(StatusCodes.Status403Forbidden, new { message = "Evento pertence a outro organizador" }));
            }

            return (evento, null);
        }
    }
}
