using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Models
{
    [Table("programacao"), PrimaryKey(nameof(Id))]
    public class Programacao
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("evento_id")]
        public int EventoId { get; set; }

        [Column("palestrante_id")]
        public int PalestranteId { get; set; }

        [Column("titulo")]
        public required string Titulo { get; set; }

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("horario_inicio")]
        public DateTime HorarioInicio { get; set; }

        [Column("horario_fim")]
        public DateTime HorarioFim { get; set; }

        [Column("sala")]
        public string? Sala { get; set; }

        public Evento? Evento { get; set; }

        public Palestrante? Palestrante { get; set; }
    }
}
