using LabBancoDeDados.Data;
using LabBancoDeDados.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabBancoDeDados.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlaylistsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {
            return await _context.Playlists
                .Include(p => p.Usuario)
                .Include(p => p.MusicaPlaylists)
                .ThenInclude(mp => mp.Musica)
                .ToListAsync();
        }

        [HttpGet("{playlistId:int}/{usuarioId:int}")]
        public async Task<ActionResult<Playlist>> GetPlaylist(int playlistId, int usuarioId)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Usuario)
                .Include(p => p.MusicaPlaylists)
                    .ThenInclude(mp => mp.Musica)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId && p.UsuarioId == usuarioId);

            if (playlist == null) return NotFound();

            return playlist;
        }

        [HttpPost]
        public async Task<ActionResult<Playlist>> PostPlaylist(Playlist playlist)
        {
            var usuarioExists = await _context.Usuarios.AnyAsync(u => u.Id == playlist.UsuarioId);
            if (!usuarioExists) return BadRequest("Usuário inválido.");

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaylist), new { playlistId = playlist.PlaylistId, usuarioId = playlist.UsuarioId }, playlist);
        }

        [HttpPut("{playlistId:int}/{usuarioId:int}")]
        public async Task<IActionResult> PutPlaylist(int playlistId, int usuarioId, Playlist updated)
        {
            if (playlistId != updated.PlaylistId || usuarioId != updated.UsuarioId)
                return BadRequest("Chave composta divergente.");

            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId && p.UsuarioId == usuarioId);

            if (playlist == null) return NotFound();

            playlist.Nome = updated.Nome;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{playlistId:int}/{usuarioId:int}")]
        public async Task<IActionResult> DeletePlaylist(int playlistId, int usuarioId)
        {
            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId && p.UsuarioId == usuarioId);

            if (playlist == null) return NotFound();

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
