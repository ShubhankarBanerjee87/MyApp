namespace MyNewApp.Domain.Entities
{
    public class UserRole : BaseEntities
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int RoleId { get; set; }

        //Foreign key navigation properties
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
