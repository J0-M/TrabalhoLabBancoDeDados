using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabBancoDeDados.Models
{
    [Table("PLAYLIST")]
    public class Playlist
    {
        [Key, Column(Order = 0)]
        public int PlaylistId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nome { get; set; }

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public Usuario Usuario { get; set; }

        public ICollection<MusicaPlaylist>? MusicaPlaylists { get; set; } = new List<MusicaPlaylist>(); //N:N
    }
}
