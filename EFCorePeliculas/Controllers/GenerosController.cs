using EFCorePeliculas.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCorePeliculas.Controllers
{
    [Route("api/generos")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenerosController(ApplicationDbContext context) // Ba3
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Genero>> Get()
        {
            return await _context.Generos
                .OrderBy(g => g.Nombre)
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genero>> Get(int id)
        {
            var genero = await _context.Generos.FirstOrDefaultAsync(g => g.Identificador == id);
            if (genero is null)
            {
                return NotFound();
            }
            return genero;
        }

        [HttpGet("primer")]
        public async Task<ActionResult<Genero>> Primer()
        {
            var genero = await _context.Generos.FirstOrDefaultAsync(g => g.Nombre.StartsWith("X"));
            if (genero is null)
            {
                return NotFound();
            }
            return genero;
        }

        [HttpGet("filtrar")]
        public async Task<IEnumerable<Genero>> Filtrar(string nombre)
        {
            return await _context.Generos.Where(g => g.Nombre.Contains(nombre)).ToListAsync();
        }

        [HttpGet("paginacion")]
        public async Task<ActionResult<IEnumerable<Genero>>> GetPaginacion(int pagina = 1)
        {
            int cantidadRegistrosPorPagina = 2;
            var generos = await _context.Generos
                .Skip((pagina - 1) * cantidadRegistrosPorPagina) // Bonita logica de paginación
                .Take(cantidadRegistrosPorPagina)
                .ToListAsync();
            return generos;
        }
    }
}
