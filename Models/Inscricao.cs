using System.ComponentModel.DataAnnotations.Schema;

namespace ApiFinanceiro.Models
{
    [Table("inscricao")]
    public class Inscricao
    {
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("evento_id")]
        public int EventoId { get; set; }

        [Column("data_inscricao")]
        public DateTime DataInscricao { get; set; } = DateTime.UtcNow;

        public Usuario? Usuario { get; set; }

        public Evento? Evento { get; set; }
    }
}
