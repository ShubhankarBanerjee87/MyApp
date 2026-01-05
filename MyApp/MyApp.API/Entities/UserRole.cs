namespace MyApp.API.Entities
{
    public class UserRole
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int RoleId { get; set; }

        //For foreign key constraints
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
