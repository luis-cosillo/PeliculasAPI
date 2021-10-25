using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Dtos
{
    public class ActorPathDTO
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
       
    }
}
