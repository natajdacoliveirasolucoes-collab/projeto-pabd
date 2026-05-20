using ApiFinanceiro.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.DataContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Despesa> Despesas { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Organizador> Organizadores { get; set; }

        public DbSet<Local> Locais { get; set; }

        public DbSet<Evento> Eventos { get; set; }

        public DbSet<Palestrante> Palestrantes { get; set; }

        public DbSet<Programacao> Programacoes { get; set; }

        public DbSet<TipoIngresso> TiposIngresso { get; set; }

        public DbSet<Compra> Compras { get; set; }

        public DbSet<ItemCompra> ItensCompra { get; set; }

        public DbSet<EventoPalestrante> EventosPalestrantes { get; set; }

        public DbSet<Inscricao> Inscricoes { get; set; }

        public DbSet<Avaliacao> Avaliacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Nome).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Senha).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Organizador>(entity =>
            {
                entity.HasIndex(e => e.UsuarioId).IsUnique();
                entity.HasIndex(e => e.Cnpj).IsUnique();
                entity.Property(e => e.RazaoSocial).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Cnpj).HasMaxLength(18).IsRequired();
                entity.HasOne(e => e.Usuario)
                    .WithOne(e => e.Organizador)
                    .HasForeignKey<Organizador>(e => e.UsuarioId);
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasIndex(e => e.Nome).IsUnique();
                entity.Property(e => e.Nome).HasMaxLength(80).IsRequired();
            });

            modelBuilder.Entity<Local>(entity =>
            {
                entity.Property(e => e.Nome).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Endereco).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Cidade).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Estado).HasMaxLength(2).IsRequired();
            });

            modelBuilder.Entity<Evento>(entity =>
            {
                entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("rascunho");
                entity.HasOne(e => e.Organizador)
                    .WithMany(e => e.Eventos)
                    .HasForeignKey(e => e.OrganizadorId);
                entity.HasOne(e => e.Categoria)
                    .WithMany(e => e.Eventos)
                    .HasForeignKey(e => e.CategoriaId);
                entity.HasOne(e => e.Local)
                    .WithMany(e => e.Eventos)
                    .HasForeignKey(e => e.LocalId);
            });

            modelBuilder.Entity<Palestrante>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Nome).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
                entity.Property(e => e.FotoUrl).HasMaxLength(500);
            });

            modelBuilder.Entity<Programacao>(entity =>
            {
                entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Sala).HasMaxLength(50);
                entity.HasOne(e => e.Evento)
                    .WithMany(e => e.Programacao)
                    .HasForeignKey(e => e.EventoId);
                entity.HasOne(e => e.Palestrante)
                    .WithMany(e => e.Programacao)
                    .HasForeignKey(e => e.PalestranteId);
            });

            modelBuilder.Entity<TipoIngresso>(entity =>
            {
                entity.Property(e => e.Nome).HasMaxLength(80).IsRequired();
                entity.Property(e => e.Preco).HasPrecision(10, 2);
                entity.HasOne(e => e.Evento)
                    .WithMany(e => e.TiposIngresso)
                    .HasForeignKey(e => e.EventoId);
            });

            modelBuilder.Entity<Compra>(entity =>
            {
                entity.Property(e => e.ValorTotal).HasPrecision(10, 2);
                entity.Property(e => e.StatusPagamento).HasMaxLength(20).HasDefaultValue("pendente");
                entity.Property(e => e.DataCompra).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(e => e.Usuario)
                    .WithMany(e => e.Compras)
                    .HasForeignKey(e => e.UsuarioId);
            });

            modelBuilder.Entity<ItemCompra>(entity =>
            {
                entity.Property(e => e.ValorUnitario).HasPrecision(10, 2);
                entity.HasOne(e => e.Compra)
                    .WithMany(e => e.Itens)
                    .HasForeignKey(e => e.CompraId);
                entity.HasOne(e => e.TipoIngresso)
                    .WithMany(e => e.ItensCompra)
                    .HasForeignKey(e => e.TipoIngressoId);
            });

            modelBuilder.Entity<EventoPalestrante>(entity =>
            {
                entity.HasKey(e => new { e.EventoId, e.PalestranteId });
                entity.Property(e => e.Papel).HasMaxLength(50).HasDefaultValue("palestrante");
                entity.HasOne(e => e.Evento)
                    .WithMany(e => e.EventoPalestrantes)
                    .HasForeignKey(e => e.EventoId);
                entity.HasOne(e => e.Palestrante)
                    .WithMany(e => e.EventoPalestrantes)
                    .HasForeignKey(e => e.PalestranteId);
            });

            modelBuilder.Entity<Inscricao>(entity =>
            {
                entity.HasKey(e => new { e.UsuarioId, e.EventoId });
                entity.Property(e => e.DataInscricao).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(e => e.Usuario)
                    .WithMany(e => e.Inscricoes)
                    .HasForeignKey(e => e.UsuarioId);
                entity.HasOne(e => e.Evento)
                    .WithMany(e => e.Inscricoes)
                    .HasForeignKey(e => e.EventoId);
            });

            modelBuilder.Entity<Avaliacao>(entity =>
            {
                entity.HasIndex(e => new { e.UsuarioId, e.EventoId }).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.ToTable(t => t.HasCheckConstraint("CK_avaliacao_nota", "nota >= 1 AND nota <= 5"));
                entity.HasOne(e => e.Usuario)
                    .WithMany(e => e.Avaliacoes)
                    .HasForeignKey(e => e.UsuarioId);
                entity.HasOne(e => e.Evento)
                    .WithMany(e => e.Avaliacoes)
                    .HasForeignKey(e => e.EventoId);
            });
        }
    }
}
