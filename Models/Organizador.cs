using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.Models
{
    [Table("organizador"), PrimaryKey(nameof(Id))]
    public class Organizador
    {
        [Column("id")]
        public int Id { get; set; }

        [JsonIgnore]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("razao_social")]
        public required string RazaoSocial { get; set; }

        [Column("cnpj")]
        public required string Cnpj { get; set; }

        [Column("descricao")]
        public string? Descricao { get; set; }

        public Usuario? Usuario { get; set; }

        [JsonIgnore]
        public ICollection<Evento>? Eventos { get; set; }
    }
}
