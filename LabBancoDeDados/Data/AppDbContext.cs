using Microsoft.EntityFrameworkCore;
using LabBancoDeDados.Models;

namespace LabBancoDeDados.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Artista> Artista { get; set; }
    }
}