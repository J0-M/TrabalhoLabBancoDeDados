[ApiController]
[Route("api/artista")]
public class ArtistaController : ControllerBase
{
    private readonly AppDbContext _context;

    public ArtistaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_context.Artista.ToList());
}