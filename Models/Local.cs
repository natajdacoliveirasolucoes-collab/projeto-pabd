using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Models
{
    [Table("local"), PrimaryKey(nameof(Id))]
    public class Local
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nome")]
        public required string Nome { get; set; }

        [Column("endereco")]
        public required string Endereco { get; set; }

        [Column("cidade")]
        public required string Cidade { get; set; }

        [Column("estado")]
        public required string Estado { get; set; }

        [Column("capacidade")]
        public int Capacidade { get; set; }

        [JsonIgnore]
        public ICollection<Evento>? Eventos { get; set; }
    }
}
