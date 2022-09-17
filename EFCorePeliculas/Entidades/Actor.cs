namespace EFCorePeliculas.Entidades
{
    public class Actor
    {
        private string _nombre;

        public int Id { get; set; }
        public string Nombre // Ba3
        { 
            get
            {
                return _nombre;
            }
            set
            {
                _nombre = string.Join(' ', value
                    .Split(' ')
                    .Select(x => x[0].ToString().ToUpper() + x.Substring(1).ToLower())
                    .ToArray());
            }
        }
        public string Biografia { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public HashSet<PeliculaActor> PeliculasActores { get; set; }
    }
}
