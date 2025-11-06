using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabBancoDeDados.Models{
    [Table("ARTISTA")]
    public class Artista
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nome { get; set; }

        [MaxLength(100)]
        public string? Nacionalidade { get; set; }

        public ICollection<Musica>? Musicas { get; set; } //1:N
    }
}