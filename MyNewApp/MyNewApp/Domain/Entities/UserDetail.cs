namespace MyNewApp.Domain.Entities
{
    public class UserDetail : AuditableEntities
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;

        //Foreign key navigation property to User
        public User User { get; set; } = null!;
    }
}
