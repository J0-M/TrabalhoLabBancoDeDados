using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabBancoDeDados.Models
{
    [Table("MUSICA")]
    public class Musica
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Titulo { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A duração deve ser maior que 0!")]
        public int DuracaoSegundos { get; set; }

        [ForeignKey("Artista")]
        public int ArtistaId { get; set; }
        public Artista Artista { get; set; }

        public ICollection<MusicaPlaylist>? MusicaPlaylists { get; set; } //N:N
    }
}