using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCorePeliculas.DTOs;
using EFCorePeliculas.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EFCorePeliculas.Controllers
{
    [Route("api/cines")]
    [ApiController]
    public class CinesController : ControllerBase
    {
        // Variables _globales
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        // Constructor
        public CinesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Métodos

        [HttpGet]
        public async Task<IEnumerable<CineDTO>> Get()
        {
            return await _context.Cines.ProjectTo<CineDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        // Vamos a ordenar los cines por cercania a mi ubicacion
        [HttpGet("cercanos")]
        public async Task<ActionResult> Get(double latitud, double longitud)
        {
            var geomeryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326); // Configuracion
            var miUbicacion = geomeryFactory.CreatePoint(new Coordinate(latitud, longitud)); // Definimos el punto
            var distanciaMaxMetros = 2000;

            var cines = await _context.Cines
                .OrderBy(c => c.Ubicacion.Distance(miUbicacion))
                .Where(c => c.Ubicacion.IsWithinDistance(miUbicacion, distanciaMaxMetros)) // Filtra por cines que están a menos de [distanciaMaxMetros]
                .Select(c => new
                {
                    Nombre = c.Nombre,
                    Ubicacion = Math.Round(c.Ubicacion.Distance(miUbicacion))
                }).ToListAsync();

            return Ok(cines);
        }
    }
}
