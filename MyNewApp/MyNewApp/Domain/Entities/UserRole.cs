namespace MyNewApp.Domain.Entities
{
    public class UserRole
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int RoleId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        //Foreign key navigation properties
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
