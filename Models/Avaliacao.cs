using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Models
{
    [Table("avaliacao"), PrimaryKey(nameof(Id))]
    public class Avaliacao
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("evento_id")]
        public int EventoId { get; set; }

        [Column("nota")]
        public int Nota { get; set; }

        [Column("comentario")]
        public string? Comentario { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Usuario? Usuario { get; set; }

        public Evento? Evento { get; set; }
    }
}
