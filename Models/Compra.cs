using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Models
{
    [Table("compra"), PrimaryKey(nameof(Id))]
    public class Compra
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("data_compra")]
        public DateTime DataCompra { get; set; } = DateTime.UtcNow;

        [Column("valor_total", TypeName = "decimal(10,2)")]
        public decimal ValorTotal { get; set; }

        [Column("status_pagamento")]
        public string StatusPagamento { get; set; } = "pendente";

        public Usuario? Usuario { get; set; }

        public ICollection<ItemCompra> Itens { get; set; } = new List<ItemCompra>();
    }
}
