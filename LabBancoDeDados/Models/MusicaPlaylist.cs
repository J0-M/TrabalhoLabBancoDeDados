using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabBancoDeDados.Models
{
    [Table("MUSICA_PLAYLIST")] //Entidade fraca
    public class MusicaPlaylist
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Musica")]
        public int MusicaId { get; set; }
        public Musica Musica { get; set; }

        [Key, Column(Order = 1)]
        public int PlaylistId { get; set; }

        [Key, Column(Order = 2)]
        public int UsuarioId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int OrdemNaPlaylist { get; set; }

        [ForeignKey("PlaylistId, UsuarioId")]
        public Playlist Playlist { get; set; }
    }
}
