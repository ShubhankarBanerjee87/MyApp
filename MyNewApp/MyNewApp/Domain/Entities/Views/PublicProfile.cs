namespace MyNewApp.Domain.Entities.Views
{
    public class PublicProfile
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ProfileImage { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
