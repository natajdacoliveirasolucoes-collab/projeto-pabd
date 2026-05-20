using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Models
{
    [Table("palestrante"), PrimaryKey(nameof(Id))]
    public class Palestrante
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nome")]
        public required string Nome { get; set; }

        [Column("bio")]
        public string? Bio { get; set; }

        [Column("email")]
        public required string Email { get; set; }

        [Column("foto_url")]
        public string? FotoUrl { get; set; }

        [JsonIgnore]
        public ICollection<Programacao>? Programacao { get; set; }

        [JsonIgnore]
        public ICollection<EventoPalestrante>? EventoPalestrantes { get; set; }
    }
}
