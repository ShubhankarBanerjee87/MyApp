namespace MyNewApp.Domain.Entities
{
    public class UserRole : AuditableEntities
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;

        //Foreign key navigation properties
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
