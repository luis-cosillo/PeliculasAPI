using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Dtos
{
    public class PeliculaPathDTO
    {
        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
    }
}
