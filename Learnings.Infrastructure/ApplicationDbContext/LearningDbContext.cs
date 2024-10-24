using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Learnings.Infrastrcuture.ApplicationDbContext
{
    public class LearningDbContext : IdentityDbContext<Users>
    {
        public LearningDbContext(DbContextOptions <LearningDbContext> options): base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
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
        }

    }
}
