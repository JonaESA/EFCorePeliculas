namespace EFCorePeliculas.Entidades
{
    public class Genero
    {
        public int Identificador { get; set; }
        public string Nombre { get; set; }
        public HashSet<Pelicula> Peliculas { get; set; }
    }
}
