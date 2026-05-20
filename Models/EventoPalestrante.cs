using System.ComponentModel.DataAnnotations.Schema;

namespace ApiFinanceiro.Models
{
    [Table("evento_palestrante")]
    public class EventoPalestrante
    {
        [Column("evento_id")]
        public int EventoId { get; set; }

        [Column("palestrante_id")]
        public int PalestranteId { get; set; }

        [Column("papel")]
        public string Papel { get; set; } = "palestrante";

        public Evento? Evento { get; set; }

        public Palestrante? Palestrante { get; set; }
    }
}
