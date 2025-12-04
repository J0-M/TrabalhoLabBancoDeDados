using Microsoft.EntityFrameworkCore;
using LabBancoDeDados.Models;

namespace LabBancoDeDados.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Artista> Artistas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Musica> Musicas { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<MusicaPlaylist> MusicaPlaylists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Playlist>()
            .Property(p => p.DataCriacao)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Playlist>()
                .HasKey(p => new { p.PlaylistId, p.UsuarioId });

            modelBuilder.Entity<MusicaPlaylist>()
                .HasKey(mp => new { mp.MusicaId, mp.PlaylistId, mp.UsuarioId });

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Playlists)
                .WithOne(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Musica>()
                .HasOne(m => m.Artista)
                .WithMany(a => a.Musicas)
                .HasForeignKey(m => m.ArtistaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MusicaPlaylist>()
                .HasOne(mp => mp.Musica)
                .WithMany(m => m.MusicaPlaylists)
                .HasForeignKey(mp => mp.MusicaId);

            modelBuilder.Entity<MusicaPlaylist>()
                .HasOne(mp => mp.Playlist)
                .WithMany(p => p.MusicaPlaylists)
                .HasForeignKey(mp => new { mp.PlaylistId, mp.UsuarioId });

            // ARTISTAS
            modelBuilder.Entity<Artista>().HasData(
                new Artista { Id = 1, Nome = "Queen", Nacionalidade = "Britânica" },
                new Artista { Id = 2, Nome = "Led Zeppelin", Nacionalidade = "Britânica" },
                new Artista { Id = 3, Nome = "AC/DC", Nacionalidade = "Australiana" },
                new Artista { Id = 4, Nome = "Banda X (Pop)", Nacionalidade = "Brasileira" }
            );

            // USUÁRIOS
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { Id = 1, Username = "Pablo", Email = "pablo@aluno.com" },
                new Usuario { Id = 2, Username = "Josue", Email = "josue@aluno.com" },
                new Usuario { Id = 3, Username = "Alexandre", Email = "alexandre@aluno.com" }
            );

            // MÚSICAS
            modelBuilder.Entity<Musica>().HasData(
                new Musica { Id = 1, Titulo = "Bohemian Rhapsody", DuracaoSegundos = 354, ArtistaId = 1 },
                new Musica { Id = 2, Titulo = "Stairway to Heaven", DuracaoSegundos = 482, ArtistaId = 2 },
                new Musica { Id = 3, Titulo = "Back In Black", DuracaoSegundos = 255, ArtistaId = 3 },
                new Musica { Id = 4, Titulo = "We Will Rock You", DuracaoSegundos = 160, ArtistaId = 1 },
                new Musica { Id = 5, Titulo = "Musica Pop Brasileira", DuracaoSegundos = 180, ArtistaId = 4 },
                new Musica { Id = 6, Titulo = "Thunderstruck", DuracaoSegundos = 292, ArtistaId = 3 }
            );

            // PLAYLISTS
            modelBuilder.Entity<Playlist>().HasData(
                new Playlist { PlaylistId = 1, UsuarioId = 1, Nome = "Rock do Pablo", DataCriacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc) },
                new Playlist { PlaylistId = 2, UsuarioId = 2, Nome = "Baladas do Josue", DataCriacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc) },
                new Playlist { PlaylistId = 3, UsuarioId = 1, Nome = "Heavy Riffs",   DataCriacao = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc) }
            );

            // MUSICA_PLAYLIST
            modelBuilder.Entity<MusicaPlaylist>().HasData(
                new { MusicaId = 1, PlaylistId = 1, UsuarioId = 1, OrdemNaPlaylist = 1 },
                new { MusicaId = 3, PlaylistId = 1, UsuarioId = 1, OrdemNaPlaylist = 2 },
                new { MusicaId = 4, PlaylistId = 1, UsuarioId = 1, OrdemNaPlaylist = 3 },

                new { MusicaId = 2, PlaylistId = 2, UsuarioId = 2, OrdemNaPlaylist = 1 },

                new { MusicaId = 3, PlaylistId = 3, UsuarioId = 1, OrdemNaPlaylist = 1 },
                new { MusicaId = 6, PlaylistId = 3, UsuarioId = 1, OrdemNaPlaylist = 2 }
            );
        }
    }
}
