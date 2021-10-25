namespace PeliculasAPI.Dtos
{
    public class PeliculaRelacionadaDTO: PeliculaDTO
    {

        public List<GeneroDTO> Generos { get; set; }
        public List<ActorPeliculaDetalleDTO> Actores { get; set; }

    }
}
