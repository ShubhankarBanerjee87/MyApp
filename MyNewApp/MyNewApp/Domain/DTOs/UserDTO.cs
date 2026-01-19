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
        [Required, MinLength(8)]
        public string Password { get; set; } = null!;
    }

    public class  LoginDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(8)]
        public string Password { get; set; } = null!;
    }

    public class LoginResponseDTO
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public List<string> Roles { get; set; } = null!;
        public int AcceessTokenExpiresIn { get; set; }
    }

    public class UserDetailsDTO 
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; } 
        public DateTime? DOB { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }

}
