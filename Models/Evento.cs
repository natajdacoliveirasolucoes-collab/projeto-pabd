using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Models
{
    [Table("evento"), PrimaryKey(nameof(Id))]
    public class Evento
    {
        [Column("id")]
        public int Id { get; set; }

        [JsonIgnore]
        [Column("organizador_id")]
        public int OrganizadorId { get; set; }

        [Column("categoria_id")]
        public int CategoriaId { get; set; }

        [Column("local_id")]
        public int LocalId { get; set; }

        [Column("titulo")]
        public required string Titulo { get; set; }

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("data_inicio")]
        public DateTime DataInicio { get; set; }

        [Column("data_fim")]
        public DateTime DataFim { get; set; }

        [Column("status")]
        public string Status { get; set; } = "rascunho";

        public Organizador? Organizador { get; set; }

        public Categoria? Categoria { get; set; }

        public Local? Local { get; set; }

        public ICollection<Programacao>? Programacao { get; set; }

        public ICollection<TipoIngresso>? TiposIngresso { get; set; }

        [JsonIgnore]
        public ICollection<EventoPalestrante>? EventoPalestrantes { get; set; }

        [JsonIgnore]
        public ICollection<Inscricao>? Inscricoes { get; set; }

        public ICollection<Avaliacao>? Avaliacoes { get; set; }
    }
}
