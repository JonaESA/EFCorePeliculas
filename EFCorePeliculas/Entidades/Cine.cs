using NetTopologySuite.Geometries;

namespace EFCorePeliculas.Entidades
{
    public class Cine
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Point Ubicacion { get; set; }

        // Propiedades de navegacion | the others tablas con que se relaciona Cine
        public CineOferta CineOferta { get; set; }
        public HashSet<SalaDeCine> SalasDeCine { get; set; }
    }
}
