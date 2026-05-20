using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiFinanceiro.Models
{
    [Table("categoria"), PrimaryKey(nameof(Id))]
    public class Categoria
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nome")]
        public required string Nome { get; set; }

        [Column("descricao")]
        public string? Descricao { get; set; }

        [JsonIgnore]
        public ICollection<Despesa>? Despesas { get; set; }

        [JsonIgnore]
        public ICollection<Evento>? Eventos { get; set; }
    }
}
