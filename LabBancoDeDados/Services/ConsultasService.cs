using Microsoft.EntityFrameworkCore;
using LabBancoDeDados.Data;
using LabBancoDeDados.Models;

namespace LabBancoDeDados.Services
{
    public class ConsultasService
    {
        private readonly AppDbContext _context;

        public ConsultasService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlaylistUsuarioDTO>> GetPlaylistsPorUsername(string username)
        {
            return await _context.Playlists
                .Where(p => p.Usuario.Username == username)
                .Select(p => new PlaylistUsuarioDTO
                (
                    p.PlaylistId,
                    p.UsuarioId,
                    p.Nome,
                    p.DataCriacao,
                    p.Usuario.Username
                ))
                .ToListAsync();
        }

        public async Task<List<MusicaPlaylistUsuarioDTO>> GetMusicasEmPlaylistsDeUsuarioPorArtista(
            string username, string artistaNome)
        {
            return await _context.MusicaPlaylists
                .Where(mp => mp.Playlist.Usuario.Username == username)
                .Where(mp => mp.Musica.Artista.Nome == artistaNome)
                .Select(mp => new MusicaPlaylistUsuarioDTO
                (
                    mp.Musica.Titulo,
                    mp.Musica.Artista.Nome,
                    mp.Playlist.Nome,
                    mp.Playlist.Usuario.Username
                ))
                .ToListAsync();
        }

        public async Task<List<ContagemMusicasPlaylistDTO>> GetContagemMusicasPorPlaylist()
        {
            return await _context.Playlists
                .Select(p => new ContagemMusicasPlaylistDTO
                (
                    p.Nome,
                    p.MusicaPlaylists.Count,
                    p.Usuario.Username
                ))
                .OrderByDescending(p => p.QuantidadeMusicas)
                .ToListAsync();
        }

        public async Task<List<ArtistaSemMusicasDTO>> GetArtistasSemMusicasEmPlaylists()
        {
            return await _context.Artistas
                .Where(a => !a.Musicas.Any(m => m.MusicaPlaylists.Any()))
                .Select(a => new ArtistaSemMusicasDTO
                (
                    a.Id,
                    a.Nome,
                    a.Nacionalidade
                ))
                .ToListAsync();
        }

        public async Task<MusicaDetalhadaDTO> GetMusicaComArtista(int musicaId)
        {
            return await _context.Musicas
                .Include(m => m.Artista)
                .Where(m => m.Id == musicaId)
                .Select(m => new MusicaDetalhadaDTO
                (
                    m.Id,
                    m.Titulo,
                    m.DuracaoSegundos,
                    m.Artista.Id,
                    m.Artista.Nome,
                    m.Artista.Nacionalidade
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<List<TempoTotalPlaylistDTO>> GetTempoTotalReproducaoPlaylists()
        {
            var resultados = await _context.Playlists
            .Select(p => new 
            {
                p.Nome,
                Username = p.Usuario!.Username,
                TempoTotalSegundos = p.MusicaPlaylists != null 
                    ? p.MusicaPlaylists.Sum(mp => mp.Musica!.DuracaoSegundos) 
                    : 0
            })
            .OrderByDescending(x => x.TempoTotalSegundos)
            .ToListAsync();

            return resultados
            .Select(x => new TempoTotalPlaylistDTO(
                x.Nome,
                x.Username,
                x.TempoTotalSegundos
            ))
            .ToList();
        }

        public async Task<List<MusicaMaisCurtaQueMediaDTO>> GetMusicasMaisCurtaQueMediaArtista()
        {
            var query = from m in _context.Musicas
                       let mediaArtista = _context.Musicas
                           .Where(m2 => m2.ArtistaId == m.ArtistaId)
                           .Average(m2 => (double?)m2.DuracaoSegundos) ?? 0
                       where m.DuracaoSegundos < mediaArtista
                       select new MusicaMaisCurtaQueMediaDTO
                       (
                           m.Id,
                           m.Titulo,
                           m.DuracaoSegundos,
                           m.Artista.Nome,
                           (int)mediaArtista
                       );

            return await query.ToListAsync();
        }

        public async Task<List<MusicaNaPlaylistDTO>> GetMusicasNaPlaylistComOrdem(string playlistNome)
        {
            return await _context.MusicaPlaylists
                .Where(mp => mp.Playlist.Nome == playlistNome)
                .OrderBy(mp => mp.OrdemNaPlaylist)
                .Select(mp => new MusicaNaPlaylistDTO
                (
                    mp.Musica.Titulo,
                    mp.OrdemNaPlaylist,
                    mp.Musica.DuracaoSegundos,
                    mp.Musica.Artista.Nome
                ))
                .ToListAsync();
        }

        public async Task<string> GetUsuarioDonoDaPlaylistComMusica(string musicaTitulo)
        {
            return await _context.MusicaPlaylists
                .Where(mp => mp.Musica.Titulo == musicaTitulo)
                .Select(mp => mp.Playlist.Usuario.Username)
                .FirstOrDefaultAsync();
        }

        public async Task<List<RankArtistaDTO>> GetRankPopularidadeArtistas()
        {
            var artistasComPlaylists = await _context.Artistas
                .Select(a => new
                {
                    Artista = a,
                    QuantidadePlaylists = a.Musicas
                        .SelectMany(m => m.MusicaPlaylists)
                        .Select(mp => new { mp.PlaylistId, mp.UsuarioId })
                        .Distinct()
                        .Count()
                })
                .OrderByDescending(x => x.QuantidadePlaylists)
                .ToListAsync();

            return artistasComPlaylists
                .Select((x, index) => new RankArtistaDTO
                (
                    index + 1,
                    x.Artista.Nome,
                    x.Artista.Nacionalidade,
                    x.QuantidadePlaylists
                ))
                .ToList();
        }

        public async Task<List<MusicaComparacaoDTO>> GetMusicasLedZeppelinMaisLongasQueQueen()
        {
            var maiorDuracaoQueen = await _context.Musicas
                .Where(m => m.Artista.Nome == "Queen")
                .MaxAsync(m => (int?)m.DuracaoSegundos) ?? 0;

            return await _context.Musicas
                .Where(m => m.Artista.Nome == "Led Zeppelin")
                .Where(m => m.DuracaoSegundos > maiorDuracaoQueen)
                .Select(m => new MusicaComparacaoDTO
                (
                    m.Titulo,
                    m.DuracaoSegundos,
                    m.Artista.Nome,
                    maiorDuracaoQueen
                ))
                .ToListAsync();
        }

        public async Task<bool> TransferirMusicaEntrePlaylists(
            int musicaId, int playlistOrigemId, int playlistDestinoId, 
            int usuarioId, int novaOrdem)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var musicaPlaylistOrigem = await _context.MusicaPlaylists
                    .FirstOrDefaultAsync(mp => mp.MusicaId == musicaId 
                        && mp.PlaylistId == playlistOrigemId 
                        && mp.UsuarioId == usuarioId);

                if (musicaPlaylistOrigem == null)
                    return false;

                _context.MusicaPlaylists.Remove(musicaPlaylistOrigem);

                var existeNaDestino = await _context.MusicaPlaylists
                    .AnyAsync(mp => mp.MusicaId == musicaId 
                        && mp.PlaylistId == playlistDestinoId 
                        && mp.UsuarioId == usuarioId);

                if (existeNaDestino)
                    throw new Exception("Música já existe na playlist destino");

                var musicaPlaylistDestino = new MusicaPlaylist
                {
                    MusicaId = musicaId,
                    PlaylistId = playlistDestinoId,
                    UsuarioId = usuarioId,
                    OrdemNaPlaylist = novaOrdem
                };

                _context.MusicaPlaylists.Add(musicaPlaylistDestino);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }

    public record PlaylistUsuarioDTO(
        int PlaylistId,
        int UsuarioId,
        string NomePlaylist,
        DateTime DataCriacao,
        string Username);

    public record MusicaPlaylistUsuarioDTO(
        string MusicaTitulo,
        string ArtistaNome,
        string PlaylistNome,
        string Username);

    public record ContagemMusicasPlaylistDTO(
        string NomePlaylist,
        int QuantidadeMusicas,
        string Username);

    public record ArtistaSemMusicasDTO(
        int ArtistaId,
        string ArtistaNome,
        string Nacionalidade);

    public record MusicaDetalhadaDTO(
        int MusicaId,
        string Titulo,
        int DuracaoSegundos,
        int ArtistaId,
        string ArtistaNome,
        string ArtistaNacionalidade);

    public record TempoTotalPlaylistDTO(
        string PlaylistNome,
        string Username,
        int TempoTotalSegundos);

    public record MusicaMaisCurtaQueMediaDTO(
        int MusicaId,
        string MusicaTitulo,
        int DuracaoSegundos,
        string ArtistaNome,
        int MediaArtista);

    public record MusicaNaPlaylistDTO(
        string MusicaTitulo,
        int OrdemNaPlaylist,
        int DuracaoSegundos,
        string Artista);

    public record RankArtistaDTO(
        int Posicao,
        string ArtistaNome,
        string Nacionalidade,
        int QuantidadePlaylists);

    public record MusicaComparacaoDTO(
        string MusicaTitulo,
        int DuracaoSegundos,
        string ArtistaNome,
        int MaiorDuracaoQueen);
}