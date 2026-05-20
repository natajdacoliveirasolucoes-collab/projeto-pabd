using System.ComponentModel.DataAnnotations;

namespace ApiFinanceiro.Dtos
{
    public class UsuarioCreateDto
    {
        [Required, MaxLength(100)]
        public required string Nome { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public required string Email { get; set; }

        [Required, MinLength(6), MaxLength(100)]
        public required string Senha { get; set; }

        [MaxLength(20)]
        public string? Telefone { get; set; }
    }

    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public string? Telefone { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class LoginDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Senha { get; set; }
    }

    public class OrganizadorCreateDto
    {
        [Required, MaxLength(200)]
        public required string RazaoSocial { get; set; }

        [Required, MaxLength(18)]
        public required string Cnpj { get; set; }

        public string? Descricao { get; set; }
    }

    public class CategoriaDto
    {
        [Required, MaxLength(80)]
        public required string Nome { get; set; }

        public string? Descricao { get; set; }
    }

    public class LocalDto
    {
        [Required, MaxLength(150)]
        public required string Nome { get; set; }

        [Required, MaxLength(255)]
        public required string Endereco { get; set; }

        [Required, MaxLength(100)]
        public required string Cidade { get; set; }

        [Required, MinLength(2), MaxLength(2)]
        public required string Estado { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacidade { get; set; }
    }

    public class PalestranteDto
    {
        [Required, MaxLength(100)]
        public required string Nome { get; set; }

        public string? Bio { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public required string Email { get; set; }

        [MaxLength(500)]
        public string? FotoUrl { get; set; }
    }

    public class EventoCreateDto
    {
        [Required, MaxLength(200)]
        public required string Titulo { get; set; }

        public string? Descricao { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public int LocalId { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }
    }

    public class EventoUpdateDto : EventoCreateDto
    {
        [Required, MaxLength(20)]
        public required string Status { get; set; }
    }

    public class VincularPalestranteDto
    {
        [Required]
        public int PalestranteId { get; set; }

        [MaxLength(50)]
        public string? Papel { get; set; }
    }

    public class ProgramacaoCreateDto
    {
        [Required, MaxLength(200)]
        public required string Titulo { get; set; }

        public string? Descricao { get; set; }

        [Required]
        public int PalestranteId { get; set; }

        [Required]
        public DateTime HorarioInicio { get; set; }

        [Required]
        public DateTime HorarioFim { get; set; }

        [MaxLength(50)]
        public string? Sala { get; set; }
    }

    public class TipoIngressoCreateDto
    {
        [Required, MaxLength(80)]
        public required string Nome { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Preco { get; set; }

        [Range(0, int.MaxValue)]
        public int QtdDisponivel { get; set; }
    }

    public class CompraCreateDto
    {
        [Required, MinLength(1)]
        public required List<ItemCompraCreateDto> Itens { get; set; }
    }

    public class ItemCompraCreateDto
    {
        [Required]
        public int TipoIngressoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }
    }

    public class AvaliacaoCreateDto
    {
        [Range(1, 5)]
        public int Nota { get; set; }

        public string? Comentario { get; set; }
    }
}
