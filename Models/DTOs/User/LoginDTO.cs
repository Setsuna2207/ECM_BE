using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.User
{
    public class LoginDTO
    {

        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
