using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public DbSet<ProductsAttribute> ProductsAttribute { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

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

            modelBuilder.Entity<Permissions>(b =>
            {
                b.HasKey(p => p.PermissionId);
                b.Property(p => p.PermissionId)
                 .ValueGeneratedOnAdd();        // <-- auto-gen on INSERT
                                                // optionally, for SQL Server:
                                                // .UseIdentityColumn(seed: 1, increment: 1);

                b.Property(p => p.PermissionName)
                 .IsRequired()
                 .HasMaxLength(255);
                b.Property(p => p.PermissionDescription)
                 .IsRequired()
                 .HasMaxLength(255); 
            });

            modelBuilder.Entity<ProductsAttribute>(pa =>
            {
                // PK + GUID default
                pa.HasKey(x => x.ProductsAttributeId);
                pa.Property(x => x.ProductsAttributeId)
                  .HasDefaultValueSql("NEWID()");

                // Name
                pa.Property(x => x.Name)
                  .IsRequired()
                  .HasMaxLength(200);

                // Audit field defaults
                pa.Property(x => x.CreatedAt)
                  .HasDefaultValueSql("SYSUTCDATETIME()")
                  .ValueGeneratedOnAdd();

                // Soft-delete filter
                pa.HasQueryFilter(x => !x.IsDeleted);

                // FKs back to Users for CreatedBy / UpdatedBy
                pa.HasOne<Users>()
                  .WithMany()
                  .HasForeignKey(x => x.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

                pa.HasOne<Users>()
                  .WithMany()
                  .HasForeignKey(x => x.UpdatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Category>(cat =>
            {
                // PK + GUID default
                cat.HasKey(x => x.CategoryId);
                cat.Property(x => x.CategoryId)
                   .HasDefaultValueSql("NEWID()");

                // Name & IsActive
                cat.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(200);

                cat.Property(x => x.IsActive)
                   .HasDefaultValue(true);

                // Self-reference (Parent ↔ Subcategories)
                cat.HasOne(x => x.ParentCategory)
                   .WithMany(x => x.Subcategories)
                   .HasForeignKey(x => x.ParentCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

                // Audit defaults
                cat.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .ValueGeneratedOnAdd();

                // Soft-delete filter
                cat.HasQueryFilter(x => !x.IsDeleted);

                // CreatedBy/UpdatedBy → Users
                cat.HasOne<Users>()
                   .WithMany()
                   .HasForeignKey(x => x.CreatedBy)
                   .OnDelete(DeleteBehavior.Restrict);

                cat.HasOne<Users>()
                   .WithMany()
                   .HasForeignKey(x => x.UpdatedBy)
                   .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Product>(prod =>
            {
                // PK + GUID default
                prod.HasKey(x => x.ProductId);
                prod.Property(x => x.ProductId)
                    .HasDefaultValueSql("NEWID()");

                // Required fields
                prod.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                prod.Property(x => x.SKU)
                    .IsRequired()
                    .HasMaxLength(100);

                // Optional description
                prod.Property(x => x.Description)
                    .HasMaxLength(2000);

                // Price precision
                prod.Property(x => x.Price)
                    .HasColumnType("decimal(18,2)");

                // Active flag default
                prod.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                // Audit defaults
                prod.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("SYSUTCDATETIME()")
                    .ValueGeneratedOnAdd();

                // Soft-delete filter
                prod.HasQueryFilter(x => !x.IsDeleted);

                // Audit FKs → Users
                prod.HasOne<Users>()
                    .WithMany()
                    .HasForeignKey(x => x.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                prod.HasOne<Users>()
                    .WithMany()
                    .HasForeignKey(x => x.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Product>()
         .HasMany(p => p.Categories)
         .WithMany()
         .UsingEntity<Dictionary<string, object>>(
             "ProductCategories",
             j => j.HasOne<Category>()
                   .WithMany()
                   .HasForeignKey("CategoryId")
                   .HasConstraintName("FK_ProductCategories_Category")
                   .OnDelete(DeleteBehavior.Cascade),
             j => j.HasOne<Product>()
                   .WithMany()
                   .HasForeignKey("ProductId")
                   .HasConstraintName("FK_ProductCategories_Product")
                   .OnDelete(DeleteBehavior.Cascade),
             j =>
             {
                 j.HasKey("ProductId", "CategoryId");
                 j.ToTable("ProductCategories"); 
             });


            modelBuilder.Entity<ProductImage>(img =>
            {
                // PK + GUID default
                img.HasKey(x => x.ImageId);
                img.Property(x => x.ImageId)
                   .HasDefaultValueSql("NEWID()");

                // Required Url
                img.Property(x => x.Url)
                   .IsRequired()
                   .HasMaxLength(1000);

                // SortOrder default
                img.Property(x => x.SortOrder)
                   .HasDefaultValue(0);

                // AltText optional
                img.Property(x => x.AltText)
                   .HasMaxLength(500);

                // FK to Product
                img.HasOne(x => x.Product)
                   .WithMany(p => p.ProductImages)
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

                // Audit defaults
                img.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()")
                   .ValueGeneratedOnAdd();

                // Soft-delete filter
                img.HasQueryFilter(x => !x.IsDeleted);

                // CreatedBy/UpdatedBy → Users
                img.HasOne<Users>()
                   .WithMany()
                   .HasForeignKey(x => x.CreatedBy)
                   .OnDelete(DeleteBehavior.Restrict);

                img.HasOne<Users>()
                   .WithMany()
                   .HasForeignKey(x => x.UpdatedBy)
                   .OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<ProductAttribute>(pa =>
            {
                // composite primary key
                pa.HasKey(x => new { x.ProductId, x.ProductsAttributeId });

                // FK → Product
                pa.HasOne(x => x.Product)
                  .WithMany(p => p.ProductAttributes)
                  .HasForeignKey(x => x.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

                // FK → ProductsAttribute
                pa.HasOne(x => x.ProductsAttribute)
                  .WithMany(a => a.ProductAttributes)
                  .HasForeignKey(x => x.ProductsAttributeId)
                  .OnDelete(DeleteBehavior.Cascade);

                // Value
                pa.Property(x => x.Value)
                  .IsRequired()
                  .HasMaxLength(500);

                // Audit defaults & soft-delete
                pa.Property(x => x.CreatedAt)
                  .HasDefaultValueSql("SYSUTCDATETIME()")
                  .ValueGeneratedOnAdd();
                pa.HasQueryFilter(x => !x.IsDeleted);

                // Audit FKs → Users
                pa.HasOne<Users>()
                  .WithMany()
                  .HasForeignKey(x => x.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
                pa.HasOne<Users>()
                  .WithMany()
                  .HasForeignKey(x => x.UpdatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<Review>(rv =>
            {
                // PK + GUID default
                rv.HasKey(x => x.ReviewId);
                rv.Property(x => x.ReviewId)
                  .HasDefaultValueSql("NEWID()");

                // Rating, Title, Body
                rv.Property(x => x.Rating)
                  .IsRequired();
                rv.Property(x => x.Title)
                  .HasMaxLength(200);
                rv.Property(x => x.Body)
                  .IsRequired()
                  .HasMaxLength(2000);

                // FK → Product
                rv.HasOne(x => x.Product)
                  .WithMany(p => p.Reviews)
                  .HasForeignKey(x => x.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

                // FK → User
                rv.HasOne(x => x.User)
                  .WithMany() // or .WithMany(u => u.Reviews) if you add collection
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

                // Self-ref: Parent ↔ Replies
                rv.HasOne(x => x.ParentReview)
                  .WithMany(x => x.Replies)
                  .HasForeignKey(x => x.ParentReviewId)
                  .OnDelete(DeleteBehavior.Restrict);

                // Audit defaults & soft-delete
                rv.Property(x => x.CreatedAt)
                  .HasDefaultValueSql("SYSUTCDATETIME()")
                  .ValueGeneratedOnAdd();
                rv.HasQueryFilter(x => !x.IsDeleted);

                // Audit FKs → Users
                rv.HasOne<Users>()
                  .WithMany()
                  .HasForeignKey(x => x.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
                rv.HasOne<Users>()
                  .WithMany()
                  .HasForeignKey(x => x.UpdatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            // ------- Cart -------
            modelBuilder.Entity<Cart>(c =>
            {
                // PK + GUID default
                c.HasKey(x => x.CartId);
                c.Property(x => x.CartId)
                 .HasDefaultValueSql("NEWID()");

                // FK → Users
                c.HasOne(x => x.User)
                 .WithMany()  // or .WithMany(u => u.Carts) if you add a nav on Users
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                // CartItems nav
                c.HasMany(x => x.CartItems)
                 .WithOne(ci => ci.Cart)
                 .HasForeignKey(ci => ci.CartId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Audit + soft‐delete
                c.Property(x => x.CreatedAt)
                 .HasDefaultValueSql("SYSUTCDATETIME()")
                 .ValueGeneratedOnAdd();
                c.HasQueryFilter(x => !x.IsDeleted);

                c.HasOne<Users>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
                c.HasOne<Users>().WithMany().HasForeignKey(x => x.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
            });

            // ------- CartItem -------
            modelBuilder.Entity<CartItem>(ci =>
            {
                // PK + GUID default
                ci.HasKey(x => x.CartItemId);
                ci.Property(x => x.CartItemId)
                  .HasDefaultValueSql("NEWID()");

                // FKs → Cart & Product
                ci.HasOne(x => x.Cart)
                  .WithMany(c => c.CartItems)
                  .HasForeignKey(x => x.CartId)
                  .OnDelete(DeleteBehavior.Cascade);

                ci.HasOne(x => x.Product)
                  .WithMany()  // or .WithMany(p => p.CartItems) if you add nav on Product
                  .HasForeignKey(x => x.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

                // Quantity
                ci.Property(x => x.Quantity)
                  .IsRequired();

                // Audit + soft‐delete
                ci.Property(x => x.CreatedAt)
                  .HasDefaultValueSql("SYSUTCDATETIME()")
                  .ValueGeneratedOnAdd();
                ci.HasQueryFilter(x => !x.IsDeleted);

                ci.HasOne<Users>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
                ci.HasOne<Users>().WithMany().HasForeignKey(x => x.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
            });


            // ------- Order -------
            modelBuilder.Entity<Order>(o =>
            {
                // PK + GUID default
                o.HasKey(x => x.OrderId);
                o.Property(x => x.OrderId)
                 .HasDefaultValueSql("NEWID()");

                // FK → Users
                o.HasOne(x => x.User)
                 .WithMany()  // or .WithMany(u => u.Orders) if you add nav on Users
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                // OrderItems nav
                o.HasMany(x => x.OrderItems)
                 .WithOne(oi => oi.Order)
                 .HasForeignKey(oi => oi.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Properties
                o.Property(x => x.TotalAmount)
                 .IsRequired()
                 .HasColumnType("decimal(18,2)");
                o.Property(x => x.Status)
                 .IsRequired()
                 .HasMaxLength(50);
                o.Property(x => x.PlacedAt)
                 .HasDefaultValueSql("SYSUTCDATETIME()");

                // Audit + soft-delete
                o.Property(x => x.CreatedAt)
                 .HasDefaultValueSql("SYSUTCDATETIME()")
                 .ValueGeneratedOnAdd();
                o.HasQueryFilter(x => !x.IsDeleted);

                o.HasOne<Users>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
                o.HasOne<Users>().WithMany().HasForeignKey(x => x.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
            });

            // ------- OrderItem -------
            modelBuilder.Entity<OrderItem>(oi =>
            {
                // PK + GUID default
                oi.HasKey(x => x.OrderItemId);
                oi.Property(x => x.OrderItemId)
                  .HasDefaultValueSql("NEWID()");

                // FKs → Order & Product
                oi.HasOne(x => x.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(x => x.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

                oi.HasOne(x => x.Product)
                  .WithMany()  // or .WithMany(p => p.OrderItems) if you add nav on Product
                  .HasForeignKey(x => x.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

                // Quantity & UnitPrice
                oi.Property(x => x.Quantity)
                  .IsRequired();
                oi.Property(x => x.UnitPrice)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

                // Audit + soft-delete
                oi.Property(x => x.CreatedAt)
                  .HasDefaultValueSql("SYSUTCDATETIME()")
                  .ValueGeneratedOnAdd();
                oi.HasQueryFilter(x => !x.IsDeleted);

                oi.HasOne<Users>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
                oi.HasOne<Users>().WithMany().HasForeignKey(x => x.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
            });

        }

    }
}
