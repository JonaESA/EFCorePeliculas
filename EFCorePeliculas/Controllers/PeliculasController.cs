using AutoMapper;
using EFCorePeliculas.DTOs;
using EFCorePeliculas.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCorePeliculas.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        // Variables globales
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        // Bob contruye
        public PeliculasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Métodos

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var pelicula = await _context.Peliculas
                .Include(p => p.Generos.OrderByDescending(g => g.Nombre)) // Porque tiene la propiedad de navegación: public HashSet<Genero> Generos { get; set; }
                .Include(p => p.SalasDeCines)
                    .ThenInclude(s => s.Cine)
                .Include(p => p.PeliculasActores.Where(pa => pa.Actor.FechaNacimiento.Value.Year >= 1980))
                    .ThenInclude(pa => pa.Actor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                return NotFound();
            }
            // Hacemos el mapeo
            var peliculaDTO = _mapper.Map<PeliculaDTO>(pelicula);
            peliculaDTO.Cines = peliculaDTO.Cines.DistinctBy(x => x.Id).ToList(); // Eliminamos los cines repetidos

            return peliculaDTO;
        }

        [HttpGet("cargadoselectivo/{id:int}")]
        public async Task<ActionResult> GetSelectivo(int id)
        {
            // Obtenemos la pelicula
            var pelicula = await _context.Peliculas.Select(p => new
            {
                Id = p.Id,
                Titulo = p.Titulo,
                Generos = p.Generos.OrderByDescending(g => g.Nombre).Select(g => g.Nombre).ToList(),
                CantidadActores = p.PeliculasActores.Count(),
                CantidadCines = p.SalasDeCines.Select(s => s.CineId).Distinct().Count()
            }).FirstOrDefaultAsync(p => p.Id == id);

            // Validamos
            if (pelicula is null)
            {
                return NotFound();
            }
            return Ok(pelicula);
        }

        //[HttpGet("lazyloading/{id:int}")]
        //public async Task<ActionResult<PeliculaDTO>> GetLazyLoading(int id)
        //{
        //    // Buscamos la peli
        //    var pelicula = await _context.Peliculas.AsTracking().FirstOrDefaultAsync(p => p.Id == id);

        //    // Validamos
        //    if (pelicula is null)
        //    {
        //        return NotFound();
        //    }

        //    // Mapeamos
        //    var peliculaDTO = _mapper.Map<PeliculaDTO>(pelicula);
        //    peliculaDTO.Cines = peliculaDTO.Cines.DistinctBy(c => c.Id).ToList();
        //    return peliculaDTO;
        //}

        [HttpGet("agrupadasPorEstreno")]
        public async Task<ActionResult> GetAgrupadasPorCartelera()
        {
            var peliculasAgrupadas = await _context.Peliculas.GroupBy(p => p.EnCartelera)
                .Select(g => new
                {
                    EnCartelera = g.Key,
                    Conteo = g.Count(),
                    Peliculas = g.ToList()
                }).ToListAsync();

            return Ok(peliculasAgrupadas);
        }

        [HttpGet("agrupadasPorCantidadDeGeneros")]
        public async Task<ActionResult> GetAgrupadasPorCantidadDeGeneros()
        {
            var peliculasAgrupadas = await _context.Peliculas.GroupBy(p => p.Generos.Count())
                .Select(g => new
                {
                    Conteo = g.Key,
                    Titulos = g.Select(x => x.Titulo),
                    Generos = g.Select(p => p.Generos)
                    .SelectMany(gen => gen) // Una coleccion de colecciones me las aplana
                    .Select(gen => gen.Nombre)
                    .Distinct()
                }).ToListAsync();

            return Ok(peliculasAgrupadas);
        }

        [HttpGet("filtrar")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] PeliculasFiltroDTO peliculasFiltroDTO)
        {
            var peliculasQueryable = _context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(peliculasFiltroDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.Titulo.Contains(peliculasFiltroDTO.Titulo));
            }

            if (peliculasFiltroDTO.EnCartelera)
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.EnCartelera);
            }

            if (peliculasFiltroDTO.ProximosEstrenos)
            {
                DateTime hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(p => p.FechaEstreno > hoy);
            }

            // El filtro por GeneroId
            if (peliculasFiltroDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(p => p.Generos.Select(g => g.Identificador).Contains(peliculasFiltroDTO.GeneroId));
            }

            var peliculas = await peliculasQueryable.Include(p => p.Generos).ToListAsync();
            return _mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpPost] // Ba3
        public async Task<ActionResult> Post(PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = _mapper.Map<Pelicula>(peliculaCreacionDTO);
            pelicula.Generos.ForEach(g => _context.Entry(g).State = EntityState.Unchanged);
            pelicula.SalasDeCines.ForEach(s => _context.Entry(s).State = EntityState.Unchanged);

            if (pelicula.PeliculasActores is not null)
            {
                for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i + 1;
                }
            }

            _context.Add(pelicula);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
