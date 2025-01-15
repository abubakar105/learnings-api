using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Learnings.Infrastrcuture.ApplicationDbContext
{
    public class LearningDbContext : IdentityDbContext<Users>
    {
        public LearningDbContext(DbContextOptions<LearningDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }  // Corrected plural form

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();
            //modelBuilder.Entity<Users>(entity => {
            //    entity.HasKey(k => k.UsrId);
            //    entity.HasIndex(u => u.Email).IsUnique();
            //});
            modelBuilder.Entity<RolePermissions>()
               .HasKey(rp => new { rp.RoleId, rp.PermissionId });  // Composite key for RolePermissions

            // Configure the relationship between RolePermissions and Role
            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Roles)  // RolePermissions has one Role
                .WithMany()               // No collection property in IdentityRole for RolePermissions
                .HasForeignKey(rp => rp.RoleId); // Foreign key to IdentityRole

            // Configure the relationship between RolePermissions and Permissions
            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Permission)  // RolePermissions has one Permission
                .WithMany()                    // No collection property in Permissions for RolePermissions
                .HasForeignKey(rp => rp.PermissionId); // Foreign key to Permissions
        }

    }
}
