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
        //Playlists de um Usuário Específico usando o username como filtro (ex: 'Pablo')
        //O retorno deve incluir o nome da Playlist e a data de criação
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
        //Encontre todas as Músicas que pertencem a qualquer Playlist criada por um USUARIO específico (ex: 'Josue'), e cujo ARTISTA seja 'Queen'
        //Esta consulta requer atravessar múltiplos relacionamentos e aplicar filtros em diferentes entidades.
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
        //Liste o nome de todas as Playlists e o número total de Músicas que cada uma contém.
        //A listagem deve ser ordenada da Playlist mais longa para a mais curta.
        public async Task<List<ContagemMusicasPlaylistDTO>> GetContagemMusicasPorPlaylist()
        {
            var resultados = await _context.Playlists
                .Select(p => new
                {
                    p.Nome,
                    QuantidadeMusicas = p.MusicaPlaylists!.Count,
                    Username = p.Usuario!.Username
                })
                .OrderByDescending(x => x.QuantidadeMusicas)
                .ToListAsync();

            return resultados
                .Select(x => new ContagemMusicasPlaylistDTO(
                    x.Nome,
                    x.QuantidadeMusicas,
                    x.Username
                ))
                .ToList();
        }

        //Identifique e liste todos os Artistas que não possuem nenhuma de suas Músicas adicionadas a nenhuma Playlist no sistema.
        //(Foco em operadores NOT IN, LEFT JOIN ou EXCEPT)
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

        //Crie uma função para buscar uma Música por seu id e, em uma única operação de consulta (evitando o problema N+1),
        //carregue (fetch) automaticamente todos os detalhes do Artista relacionado. (Foco em Eager Loading ou Fetching Join)
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

        //Para cada PLAYLIST no sistema, calcule e retorne o tempo total de reprodução
        //(soma de duracao_segundos de todas as músicas).
        //A saída deve listar o nome da Playlist, o username do Dono e o tempo total de reprodução.
        //(Foco em agregação SUM e GROUP BY sobre o N:N).
        public async Task<List<TempoTotalPlaylistDTO>> GetTempoTotalReproducaoPlaylists()
        {
            var playlists = await _context.Playlists
                .Include(p => p.Usuario)
                .ToListAsync();

            var resultado = new List<TempoTotalPlaylistDTO>();

            foreach (var playlist in playlists)
            {   
                var tempoTotal = await _context.MusicaPlaylists
                    .Where(mp => mp.PlaylistId == playlist.PlaylistId && mp.UsuarioId == playlist.UsuarioId)
                    .Select(mp => mp.Musica!.DuracaoSegundos)
                    .SumAsync();

                resultado.Add(new TempoTotalPlaylistDTO(
                    playlist.Nome,
                    playlist.Usuario!.Username,
                    tempoTotal
                ));
            }

            return resultado.OrderByDescending(r => r.TempoTotalSegundos).ToList();
        }

        //Liste todas as Músicas cujo tempo de duração (duracao_segundos) é menor que o tempo de duração médio de todas as músicas do seu próprio Artista
        //(ex: listar músicas do AC/DC que são mais curtas que a média do AC/DC).
        //(Foco em subconsultas ou Window Functions se o ORM suportar)
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

        //Liste o título de todas as Músicas na playlist 'Rock do Pablo', incluindo a ordem_na_playlist de cada música.
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

        //Encontre o username do Usuário que é o dono da Playlist que contém a MUSICA 'Bohemian Rhapsody'.
        //O filtro deve começar pela MUSICA e navegar de volta para o USUARIO.
        public async Task<string> GetUsuarioDonoDaPlaylistComMusica(string musicaTitulo)
        {
            return await _context.MusicaPlaylists
                .Where(mp => mp.Musica.Titulo == musicaTitulo)
                .Select(mp => mp.Playlist.Usuario.Username)
                .FirstOrDefaultAsync();
        }

        //Liste todos os Artistas e seu ranking baseado no número de Playlists em que suas músicas estão presentes
        //(o Artista com músicas na maior quantidade de playlists fica em 1º)
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

        //Liste todas as Músicas do Artista 'Led Zeppelin' cuja duração é maior que a duração
        //da música mais longa do Artista 'Queen'.
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

        //Implemente uma função que mova uma MUSICA de uma PLAYLIST para outra PLAYLIST
        //(ambas do mesmo USUARIO), garantindo que o processo seja Atômico (ou tudo acontece ou nada acontece)
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
