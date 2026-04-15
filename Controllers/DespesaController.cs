using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Dtos;
using ApiFinanceiro.Exceptions;
using ApiFinanceiro.Models;
using ApiFinanceiro.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Controllers
{
    [Route("/despesas")]
    [ApiController]
    public class DespesaController : ControllerBase
    {
        private readonly DespesaService _service;

       public DespesaController(DespesaService service)
        {
            _service = service;
        }

        [HttpGet()]
        public async Task<IActionResult> FindAll()
        {
            try
            {
                var despesas = await _service.FindAll();

                return Ok(despesas);
            } catch(Exception e)
            {
                return Problem(e.Message);
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindById(int id)
        {
            try
            {
                var despesa = await _service.FindById(id);

                return Ok(despesa);
            }
            catch(ErrorServiceException e)
            {
                return e.ToActionResult(this);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] DespesaDto novaDespesa)
        {
            try
            {
                var despesa = await _service.Create(novaDespesa);

                return Created("", despesa);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DespesaUpdateDto despesaDto)
        {
            try
            {
                var despesa = await _service.Update(id, despesaDto);

                return Ok(despesa);

            } catch (ErrorServiceException e)
            {
                return e.ToActionResult(this);
            } 
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(int id)
        {
            try
            {
                await _service.Remove(id);

                return NoContent();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }            
        }
    }
}
