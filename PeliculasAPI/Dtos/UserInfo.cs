using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Dtos
{
    public class UserInfo
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
