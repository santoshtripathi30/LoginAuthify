using Microsoft.AspNetCore.Authentication;

using System.ComponentModel.DataAnnotations;

namespace LoginAuthify.Models
{
    public class LoginVM
    {

        [Required(ErrorMessage = "Email address is required")]
        [MinLength(2)]
        public string EmailAddress { get; set; } = "";


        [Required(ErrorMessage = "Password is required")]
        [MinLength(2)]
        public string Password { get; set; } = "";


        // Third-party provider
        public IEnumerable<AuthenticationScheme> Schemes { get; set; } = [];
    }
}
