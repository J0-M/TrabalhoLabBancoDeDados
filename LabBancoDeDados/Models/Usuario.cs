using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabBancoDeDados.Models{
    [Table("USUARIO")]
    public class Usuario{
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; }

        public ICollection<Playlist> Playlists { get; set; } //1:N
    }
}