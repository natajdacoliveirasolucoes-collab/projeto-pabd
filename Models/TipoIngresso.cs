using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Models
{
    [Table("tipo_ingresso"), PrimaryKey(nameof(Id))]
    public class TipoIngresso
    {
        [Column("id")]
        public int Id { get; set; }

        [JsonIgnore]
        [Column("evento_id")]
        public int EventoId { get; set; }

        [Column("nome")]
        public required string Nome { get; set; }

        [Column("preco", TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        [Column("qtd_disponivel")]
        public int QtdDisponivel { get; set; }

        public Evento? Evento { get; set; }

        [JsonIgnore]
        public ICollection<ItemCompra>? ItensCompra { get; set; }
    }
}
