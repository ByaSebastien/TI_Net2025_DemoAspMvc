using System.ComponentModel.DataAnnotations;

namespace TI_Net2025_DemoAspMvc.Models.Dtos.User
{
    public class RegisterFormDto
    {
        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
