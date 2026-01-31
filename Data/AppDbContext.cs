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
            // User Configuration (Oracle Optimized)
            // ==========================================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users"); // Ensure table name is exactly as expected

                entity.HasKey(u => u.Id);
                
                entity.Property(u => u.Id)
                      .HasMaxLength(36)
                      .ValueGeneratedNever(); 

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(u => u.FirebaseUid)
                      .IsRequired()
                      .HasMaxLength(128);

                // Unique Constraints
                entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("UX_User_Email");
                entity.HasIndex(u => u.FirebaseUid).IsUnique().HasDatabaseName("UX_User_FirebaseUid");
            });

            // ==========================================================
            // Order Configuration
            // ==========================================================
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                entity.Property(o => o.Id)
                      .HasMaxLength(36)
                      .ValueGeneratedNever(); 

                entity.Property(o => o.UserId)
                      .IsRequired()
                      .HasMaxLength(36);

                // Relationship: Order -> User
                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_Orders_Users");
            });

            // ==========================================================
            // Address Configuration
            // ==========================================================
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Addresses");
                
                // Addresses usually use an Integer Identity in Oracle
                entity.Property(a => a.AddressId)
                      .ValueGeneratedOnAdd(); 
            });
        }
    }
}