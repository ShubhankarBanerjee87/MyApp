using MyApp.API.Entities;

namespace MyApp.API.DTOs
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public List<int> RoleIds { get; set; }

    }

    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
