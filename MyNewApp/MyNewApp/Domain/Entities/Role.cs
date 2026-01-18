namespace MyNewApp.Domain.Entities
{
    public class Role : BaseEntities
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string RoleTitle { get; set; } = null!;
        public string? RoleDescription { get; set; }

        //Collection navigation property
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
