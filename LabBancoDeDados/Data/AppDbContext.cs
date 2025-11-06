using Microsoft.EntityFrameworkCore;
using LabBancoDeDados.Models;

namespace LabBancoDeDados.Data
{
    public class AppDbContext : DbContext{

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Artista> Artistas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Musica> Musicas { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<MusicaPlaylist> MusicaPlaylists { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder){ //Criação das constraints das tabelas
            modelBuilder.Entity<Playlist>()
                .HasKey(p => new { p.PlaylistId, p.UsuarioId }); //PK de playlist = playlist_id, usuario_id

            modelBuilder.Entity<MusicaPlaylist>()
                .HasKey(mp => new { mp.MusicaId, mp.PlaylistId, mp.UsuarioId });  //PK de MusicaPlaylist = playlist_id, usuario_id

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Playlists) //Usuario tem N playlists
                .WithOne(p => p.Usuario) // Playlists tem 1 usuario
                .HasForeignKey(p => p.UsuarioId) // Playlist FK = usuario_id
                .OnDelete(DeleteBehavior.Cascade); // on delete cascade

            modelBuilder.Entity<Musica>()
                .HasOne(m => m.Artista) // musica tem 1 artista
                .WithMany(a => a.Musicas) // artista tem n musicas
                .HasForeignKey(m => m.ArtistaId) // musica FK = artista_id
                .OnDelete(DeleteBehavior.Restrict); // on delete restrict

            modelBuilder.Entity<MusicaPlaylist>()
                .HasOne(mp => mp.Musica)
                .WithMany(m => m.MusicaPlaylists)
                .HasForeignKey(mp => mp.MusicaId);

            modelBuilder.Entity<MusicaPlaylist>()
                .HasOne(mp => mp.Playlist)
                .WithMany(p => p.MusicaPlaylists)
                .HasForeignKey(mp => new { mp.PlaylistId, mp.UsuarioId });
        }
    }
}