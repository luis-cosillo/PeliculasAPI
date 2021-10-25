namespace PeliculasAPI.Entidades
{
    public class PeliculasSalasDeCine
    {
        public int PeliculaId { get; set; }
        public int SalaCineId { get; set; }

        public Pelicula Pelicula { get; set; }
        public SalaDeCine SalaDeCine { get; set; }
    }
}
