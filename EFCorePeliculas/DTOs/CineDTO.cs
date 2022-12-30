namespace EFCorePeliculas.DTOs
{
    public class CineDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Juntos formamos el data type: geography
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}
