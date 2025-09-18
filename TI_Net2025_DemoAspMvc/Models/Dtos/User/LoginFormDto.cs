using System.ComponentModel.DataAnnotations;

namespace TI_Net2025_DemoAspMvc.Models.Dtos.User
{
    public class LoginFormDto
    {
        [Required]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = "Mot de passe requis")]
        public string Password { get; set; } = null!;
    }
}
