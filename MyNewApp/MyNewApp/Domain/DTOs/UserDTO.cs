using System.ComponentModel.DataAnnotations;

namespace MyNewApp.Domain.DTOs
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

    }

    public class RegisterUserDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        public string? UserName { get; set; }
        [Required, MinLength(8)]
        public string Password { get; set; } = null!;
    }

}
