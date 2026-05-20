using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Models
{
    [Table("item_compra"), PrimaryKey(nameof(Id))]
    public class ItemCompra
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("compra_id")]
        public int CompraId { get; set; }

        [Column("tipo_ingresso_id")]
        public int TipoIngressoId { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("valor_unitario", TypeName = "decimal(10,2)")]
        public decimal ValorUnitario { get; set; }

        public Compra? Compra { get; set; }

        public TipoIngresso? TipoIngresso { get; set; }
    }
}
