namespace MyApp.API.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        //For one-to-many relationship
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
