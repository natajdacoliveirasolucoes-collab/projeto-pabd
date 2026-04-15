using System.ComponentModel.DataAnnotations;

namespace ApiFinanceiro.Dtos
{
    public class DespesaDto
    {
        [Required(ErrorMessage = "O campo Categoria é obrigatório")]
        [MinLength(5, ErrorMessage = "Obrigatório mínimo de 5 caracteres")]
        public required string Descricao { get; set; }

        [Required(ErrorMessage = "O campo Categoria é obrigatório")]
        public required int CategoriaId { get; set; }

        [Required(ErrorMessage = "O campo Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public required decimal Valor { get; set; }

        [Required(ErrorMessage = "O campo DataVencimento é obrigatório")]
        public required DateOnly DataVencimento { get; set; }
    }
}
