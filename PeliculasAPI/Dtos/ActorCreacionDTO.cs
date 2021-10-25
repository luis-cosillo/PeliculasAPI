using PeliculasAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Dtos
{
    public class ActorCreacionDTO: ActorPathDTO
    {
        [PesoArchivoValidacion(PesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen )]
        public IFormFile? Foto { get; set; }
    }
}
