using LabBancoDeDados.Data;
using LabBancoDeDados.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabBancoDeDados.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusicasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MusicasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Musica>>> GetMusicas()
        {
            return await _context.Musicas
                .Include(m => m.Artista)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Musica>> GetMusica(int id)
        {
            var musica = await _context.Musicas
                .Include(m => m.Artista)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (musica == null)
                return NotFound();

            return musica;
        }

        [HttpPost]
        public async Task<ActionResult<Musica>> PostMusica(Musica musica)
        {
            _context.Musicas.Add(musica);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMusica), new { id = musica.Id }, musica);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMusica(int id, Musica musica)
        {
            if (id != musica.Id)
                return BadRequest();

            _context.Entry(musica).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMusica(int id)
        {
            var musica = await _context.Musicas.FindAsync(id);
            if (musica == null)
                return NotFound();

            _context.Musicas.Remove(musica);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
