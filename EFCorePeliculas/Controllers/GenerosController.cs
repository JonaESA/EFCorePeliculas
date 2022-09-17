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

        public GenerosController(ApplicationDbContext context)
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

        [HttpPost]
        public async Task<ActionResult> Post(Genero genero)
        {
            _context.Add(genero); // Marca Genero as Add
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var genero = await _context.Generos.FirstOrDefaultAsync(g => g.Identificador == id);
            if (genero is null)
            {
                return NotFound();
            }
            _context.Remove(genero);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
