namespace MyApp.API.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; } 
        public string UserName { get; set; }
        public string Password { get; set; }

        //For one-to-many relationship
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
