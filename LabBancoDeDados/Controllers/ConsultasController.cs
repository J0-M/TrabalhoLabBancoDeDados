using Microsoft.AspNetCore.Mvc;
using LabBancoDeDados.Services;

namespace LabBancoDeDados.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultasController : ControllerBase
    {
        private readonly ConsultasService _consultasService;

        public ConsultasController(ConsultasService consultasService)
        {
            _consultasService = consultasService;
        }

        [HttpGet("playlists-por-usuario/{username}")]
        public async Task<IActionResult> GetPlaylistsPorUsuario(string username)
        {
            var result = await _consultasService.GetPlaylistsPorUsername(username);
            return Ok(result);
        }

        [HttpGet("musicas-em-playlists/{username}/{artista}")]
        public async Task<IActionResult> GetMusicasEmPlaylistsDeUsuarioPorArtista(
            string username, string artista)
        {
            var result = await _consultasService.GetMusicasEmPlaylistsDeUsuarioPorArtista(username, artista);
            return Ok(result);
        }

        [HttpGet("contagem-musicas-playlist")]
        public async Task<IActionResult> GetContagemMusicasPorPlaylist()
        {
            var result = await _consultasService.GetContagemMusicasPorPlaylist();
            return Ok(result);
        }

        [HttpGet("artistas-sem-musicas-playlists")]
        public async Task<IActionResult> GetArtistasSemMusicasEmPlaylists()
        {
            var result = await _consultasService.GetArtistasSemMusicasEmPlaylists();
            return Ok(result);
        }

        [HttpGet("musica-com-artista/{id}")]
        public async Task<IActionResult> GetMusicaComArtista(int id)
        {
            var result = await _consultasService.GetMusicaComArtista(id);
            return Ok(result);
        }

        [HttpGet("tempo-total-playlists")]
        public async Task<IActionResult> GetTempoTotalReproducaoPlaylists()
        {
            var result = await _consultasService.GetTempoTotalReproducaoPlaylists();
            return Ok(result);
        }

        [HttpGet("musicas-curtas-que-media")]
        public async Task<IActionResult> GetMusicasMaisCurtaQueMediaArtista()
        {
            var result = await _consultasService.GetMusicasMaisCurtaQueMediaArtista();
            return Ok(result);
        }

        [HttpGet("musicas-na-playlist/{playlistNome}")]
        public async Task<IActionResult> GetMusicasNaPlaylistComOrdem(string playlistNome)
        {
            var result = await _consultasService.GetMusicasNaPlaylistComOrdem(playlistNome);
            return Ok(result);
        }

        [HttpGet("usuario-dono-playlist/{musicaTitulo}")]
        public async Task<IActionResult> GetUsuarioDonoDaPlaylistComMusica(string musicaTitulo)
        {
            var result = await _consultasService.GetUsuarioDonoDaPlaylistComMusica(musicaTitulo);
            return Ok(result);
        }

        [HttpGet("rank-artistas")]
        public async Task<IActionResult> GetRankPopularidadeArtistas()
        {
            var result = await _consultasService.GetRankPopularidadeArtistas();
            return Ok(result);
        }

        [HttpGet("led-zeppelin-maior-que-queen")]
        public async Task<IActionResult> GetMusicasLedZeppelinMaisLongasQueQueen()
        {
            var result = await _consultasService.GetMusicasLedZeppelinMaisLongasQueQueen();
            return Ok(result);
        }

        [HttpPost("transferir-musica")]
        public async Task<IActionResult> TransferirMusicaEntrePlaylists(
            [FromBody] TransferenciaMusicaRequest request)
        {
            var success = await _consultasService.TransferirMusicaEntrePlaylists(
                request.MusicaId,
                request.PlaylistOrigemId,
                request.PlaylistDestinoId,
                request.UsuarioId,
                request.NovaOrdem);

            return success ? Ok("Transferência realizada com sucesso!") 
                          : BadRequest("Erro na transferência.");
        }
    }

    public class TransferenciaMusicaRequest
    {
        public int MusicaId { get; set; }
        public int PlaylistOrigemId { get; set; }
        public int PlaylistDestinoId { get; set; }
        public int UsuarioId { get; set; }
        public int NovaOrdem { get; set; }
    }
}