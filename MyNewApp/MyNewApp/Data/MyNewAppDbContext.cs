using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyNewApp.Domain.Entities;
using System.Security.Claims;

namespace MyNewApp.Data
{
    public class MyNewAppDbContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public MyNewAppDbContext(
            DbContextOptions<MyNewAppDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //DbSets for entities
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Role> RolesMaster { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }


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

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            // ---------- PREVENT CREATED FIELDS FROM UPDATE ----------
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntities).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(AuditableEntities.CreatedBy))
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(BaseEntities.CreatedAt))
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                }
            }

            // Seed initial roles or can say default data
            modelBuilder.Entity<Role>()
                .HasData(
                new Role { Id = 1, RoleId = 1, RoleTitle = "SuperAdmin", RoleDescription = "Super Administrator with full access over all organizations", CreatedAt = new DateTime(2026, 1, 10), IsActive = true },
                new Role { Id = 2, RoleId = 2, RoleTitle = "OrganizationAdmin", RoleDescription = "Administrator with access to manage organization data", CreatedAt = new DateTime(2026, 1, 10), IsActive = true },
                new Role { Id = 3, RoleId = 3, RoleTitle = "Faculty", RoleDescription = "Faculty connected to some organization", CreatedAt = new DateTime(2026, 1, 10), IsActive = true },
                new Role { Id = 4, RoleId = 4, RoleTitle = "Student", RoleDescription = "Student connected to some organization", CreatedAt = new DateTime(2026, 1, 10), IsActive = true },
                new Role { Id = 5, RoleId = 5, RoleTitle = "Guest", RoleDescription = "Guest user with limited access", CreatedAt = new DateTime(2026, 1, 10), IsActive = true }
                );
        }

        // ===================== SAVE CHANGES OVERRIDE =====================

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var currentUserId = GetCurrentUserId();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntities baseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        baseEntity.CreatedAt = now;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        baseEntity.UpdatedAt = now;
                        baseEntity.UpdatedBy = currentUserId;
                    }
                }

                if (entry.Entity is AuditableEntities auditableEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditableEntity.CreatedBy = currentUserId;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private long GetCurrentUserId()
        {
            var httpContext = _httpContextAccessor?.HttpContext;

            if (httpContext == null)
                return 0;

            var user = httpContext.User;

            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                return 0;

            var claim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                return 0;

            return long.Parse(claim.Value);
        }

    }
}
