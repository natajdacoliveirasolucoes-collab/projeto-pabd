using ApiFinanceiro.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceiro.DataContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Despesa> Despesas { get; set; }

        public DbSet<Categoria> Categorias { get; set; }
    }
}
