using System.ComponentModel.DataAnnotations;

namespace ApiFinanceiro.Dtos
{
    public class DespesaUpdateDto : DespesaDto
    {
        [Required]
        public required string Situacao { get; set; }

        [Required]
        public DateOnly DataPagamento { get; set; }
    }
}
