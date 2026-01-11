using Microsoft.EntityFrameworkCore;
using MyNewApp.Domain.Entities;

namespace MyNewApp.Data
{
    public class MyNewAppDbContext(DbContextOptions options) : DbContext(options)
    {

        //DbSets for entities
        public DbSet<User> Users { get; set; } 
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Role> RolesMaster { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserDetail)
                .WithOne(ud => ud.User)
                .HasForeignKey<UserDetail>(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(u => u.User)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(r => r.Role)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicate role assignment
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            //Unique constraints
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            //Other way for unique constraints
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleTitle)
                .IsUnique();

            // Seed initial roles or can say default data
            modelBuilder.Entity<Role>()
                .HasData(
                new Role { Id = 1, RoleId = 1, RoleTitle = "SuperAdmin", RoleDescription = "Super Administrator with full access over all organizations", CreatedAt = new DateTime(2026, 1, 10), IsActive = true },
                new Role { Id = 2,  RoleId = 2, RoleTitle = "OrganizationAdmin", RoleDescription = "Administrator with access to manage organization data", CreatedAt = new DateTime(2026, 1, 10), IsActive = true },
                new Role { Id = 3, RoleId = 3, RoleTitle = "Faculty", RoleDescription = "Faculty connected to some organization", CreatedAt = new DateTime(2026, 1, 10), IsActive = true },
                new Role { Id = 4, RoleId = 4, RoleTitle = "Student", RoleDescription = "Student connected to some organization", CreatedAt = new DateTime(2026, 1, 10), IsActive = true },
                new Role { Id = 5, RoleId = 5, RoleTitle = "Guest", RoleDescription = "Guest user with limited access", CreatedAt = new DateTime(2026, 1, 10), IsActive = true }
                );
        }
    }
}
