using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Dtos;
using ApiFinanceiro.Exceptions;
using ApiFinanceiro.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiFinanceiro.Services
{
    public class DespesaService
    {
        private readonly AppDbContext _context;

        private readonly IMapper _mapper;

        public DespesaService(AppDbContext context, IMapper mapper) 
        { 
            _context = context;
            _mapper = mapper;
        }

        public async Task<ICollection<Despesa>> FindAll()
        {
            try
            {
                return await _context.Despesas
                    .Include(d => d.Categoria)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Despesa> Create(DespesaDto data)
        {
            try
            {
                var categoriaExiste = await _context.Categorias.AnyAsync(x => x.Id == data.CategoriaId);

                if(!categoriaExiste)
                {
                    throw new ErrorServiceException($"Categoria não encontrada",
                        c => c.NotFound(new { message = $"Categoria #{data.CategoriaId} não encontrada" }));
                }

                var despesa = _mapper.Map<Despesa>(data);

                await _context.Despesas.AddAsync(despesa);
                await _context.SaveChangesAsync();

                return despesa;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task<Despesa> FindById(int id)
        {
            try
            {
                var despesa = await _context.Despesas.FirstOrDefaultAsync(x => x.Id == id);

                if (despesa is null)
                {
                    throw new ErrorServiceException($"Despesa ${id} não encontrada", 
                        c => c.NotFound(new { message = $"Despesa #{id} não encontrada" }));
                }

                return despesa;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Despesa> Update(int id, DespesaUpdateDto data)
        {
            try
            {
                var despesa = await FindById(id);

                var categoriaExiste = await _context.Categorias.AnyAsync(x => x.Id == data.CategoriaId);

                if (!categoriaExiste)
                {
                    throw new ErrorServiceException($"Categoria não encontrada",
                        c => c.NotFound(new { message = $"Categoria #{data.CategoriaId} não encontrada" }));
                }

                //var dataVencimento = new DateTime(despesa.DataVencimento.Year, despesa.DataVencimento.Month, despesa.DataVencimento.Day);
                //var dataPagamento = new DateTime(despesaDto.DataPagamento.Year, despesaDto.DataPagamento.Month, despesaDto.DataPagamento.Day);

                //// TODO: adicionar data de emissão
                //if(dataPagamento < dataVencimento)
                //{
                //    throw new ErrorServiceException("Somente é possível realizar o pagamento no dia de vencimento", 
                //        c => c.Conflict(new { message = "Somente é possível realizar o pagamento no dia de vencimento" }));
                //}

                _mapper.Map(data, despesa);

                _context.Despesas.Update(despesa);
                await _context.SaveChangesAsync();

                return despesa;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Remove(int id)
        {
            try
            {
                var despesa = await FindById(id);

                _context.Despesas.Remove(despesa);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
