using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Models
{
    [Table("usuario"), PrimaryKey(nameof(Id))]
    public class Usuario
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nome")]
        public required string Nome { get; set; }

        [Column("email")]
        public required string Email { get; set; }

        [JsonIgnore]
        [Column("senha")]
        public required string Senha { get; set; }

        [Column("telefone")]
        public string? Telefone { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Organizador? Organizador { get; set; }

        [JsonIgnore]
        public ICollection<Compra>? Compras { get; set; }

        [JsonIgnore]
        public ICollection<Inscricao>? Inscricoes { get; set; }

        [JsonIgnore]
        public ICollection<Avaliacao>? Avaliacoes { get; set; }
    }
}
