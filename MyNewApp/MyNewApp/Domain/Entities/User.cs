namespace MyNewApp.Domain.Entities
{
    public class User : BaseEntities
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; } = true;

        //collection navigation properties
        public UserDetail? UserDetail { get; set; }
        public ICollection<UserRole>? UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
