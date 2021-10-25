using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Dtos
{
    public class GeneroCreacionDTO
    {
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }
    }
}
