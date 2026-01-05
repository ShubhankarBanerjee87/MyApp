using Microsoft.EntityFrameworkCore;
using MyApp.API.Entities;

namespace MyApp.API.Data
{
    public class MyAppDbContext : DbContext
    {
        public MyAppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role() {Id = 1, Name = "General", Description = "This is the general user which is created on normal signup" },
                new Role() {Id = 2, Name = "Student", Description = "This says the user is a student in the application and has access to student features" },
                new Role() {Id = 3, Name = "Faculty", Description = "This says the user is a faculty in the application and has access to faculty features" },
                new Role() {Id = 4, Name = "Admin", Description = "This says the user is a admin in the application and has access to admin features" }
            );
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles_Master { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
