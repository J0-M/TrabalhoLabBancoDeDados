using LabBancoDeDados.Data;
using LabBancoDeDados.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabBancoDeDados.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusicaPlaylistsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MusicaPlaylistsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MusicaPlaylist>>> GetMusicaPlaylists()
        {
            return await _context.MusicaPlaylists
                .Include(mp => mp.Musica)
                .Include(mp => mp.Playlist)
                .ToListAsync();
        }

        [HttpGet("{musicaId}/{playlistId}")]
        public async Task<ActionResult<MusicaPlaylist>> GetMusicaPlaylist(int musicaId, int playlistId)
        {
            var musicaPlaylist = await _context.MusicaPlaylists
                .Include(mp => mp.Musica)
                .Include(mp => mp.Playlist)
                .FirstOrDefaultAsync(mp => mp.MusicaId == musicaId && mp.PlaylistId == playlistId);

            if (musicaPlaylist == null)
                return NotFound();

            return musicaPlaylist;
        }

        [HttpPost]
        public async Task<ActionResult<MusicaPlaylist>> PostMusicaPlaylist(MusicaPlaylist musicaPlaylist)
        {
            _context.MusicaPlaylists.Add(musicaPlaylist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMusicaPlaylist), new { musicaId = musicaPlaylist.MusicaId, playlistId = musicaPlaylist.PlaylistId }, musicaPlaylist);
        }

        [HttpDelete("{musicaId}/{playlistId}")]
        public async Task<IActionResult> DeleteMusicaPlaylist(int musicaId, int playlistId)
        {
            var musicaPlaylist = await _context.MusicaPlaylists
                .FirstOrDefaultAsync(mp => mp.MusicaId == musicaId && mp.PlaylistId == playlistId);

            if (musicaPlaylist == null)
                return NotFound();

            _context.MusicaPlaylists.Remove(musicaPlaylist);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
