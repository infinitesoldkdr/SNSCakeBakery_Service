using Microsoft.EntityFrameworkCore;
using SNSCakeBakery_Service.Models;

namespace SNSCakeBakery_Service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<GoogleSyncLog> GoogleSyncLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================================
            // FIX: Configure User entity for string Primary Key
            // ==========================================================
            modelBuilder.Entity<User>(entity =>
            {
                // 1. Configure PK: Set max length for MySQL VARCHAR, prevent auto-increment.
                entity.Property(u => u.Id)
                      .HasMaxLength(36) 
                      .ValueGeneratedNever(); 

                // User Email unique constraint
                entity.HasIndex(u => u.Email)
                      .IsUnique();
            });

            // ==========================================================
            // FIX: Configure Order entity for string Primary Key & FK
            // ==========================================================
            modelBuilder.Entity<Order>(entity =>
            {
                // 1. Configure PK: Set max length for MySQL VARCHAR, prevent auto-increment.
                entity.Property(o => o.Id)
                      .HasMaxLength(36)
                      .ValueGeneratedNever(); 

                // 2. Configure Foreign Key (FK) to ensure it matches the User.Id length
                entity.Property(o => o.UserId)
                      .HasMaxLength(36);

                // Order → User relationship
                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}