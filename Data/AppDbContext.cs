using Microsoft.EntityFrameworkCore;
using SNSCakeBakery_Service.Models;

namespace SNSCakeBakery_Service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Core Management DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<GoogleSyncLog> GoogleSyncLogs { get; set; }

        // Bakery Catalog DbSets
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ImageType> ImageTypes { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Apply existing configurations for Users, Orders, etc.
            ConfigureExistingEntities(modelBuilder);

            // 2. Apply identity configurations for new Bakery entities
            ConfigureBakeryEntities(modelBuilder);

            // 3. THE "QUOTE-KILLER": Force all names to UPPERCASE
            // This allows you to query in DBeaver without using double quotes.
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Set Table Name to UPPERCASE
                var tableName = entity.GetTableName()?.ToUpper();
                entity.SetTableName(tableName);

                foreach (var property in entity.GetProperties())
                {
                    // Set Column Name to UPPERCASE
                    property.SetColumnName(property.GetColumnName().ToUpper());
                }

                foreach (var key in entity.GetKeys())
                    key.SetName(key.GetName().ToUpper());

                foreach (var fk in entity.GetForeignKeys())
                    fk.SetConstraintName(fk.GetConstraintName().ToUpper());

                foreach (var index in entity.GetIndexes())
                    index.SetDatabaseName(index.GetDatabaseName().ToUpper());
            }
        }

        private void ConfigureBakeryEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductType>(entity => {
                entity.HasKey(pt => pt.TypeId);
                entity.Property(pt => pt.TypeId).UseIdentityColumn();
            });

            modelBuilder.Entity<Product>(entity => {
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.ProductId).UseIdentityColumn();
                entity.Property(p => p.BasePrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.HasOne<ProductType>()
                      .WithMany()
                      .HasForeignKey(p => p.ProductTypeId);
            });

            modelBuilder.Entity<ImageType>(entity => {
                entity.HasKey(it => it.ImageTypeId);
                entity.Property(it => it.ImageTypeId).UseIdentityColumn();
            });

            modelBuilder.Entity<ProductImage>(entity => {
                entity.HasKey(pi => pi.ImageId);
                entity.Property(pi => pi.ImageId).UseIdentityColumn();
                entity.Property(pi => pi.UploadedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne<Product>()
                      .WithMany(p => p.Images)
                      .HasForeignKey(pi => pi.ProductId);
            });
        }

        private void ConfigureExistingEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasMaxLength(36).ValueGeneratedNever();
            });

            modelBuilder.Entity<Order>(entity => {
                entity.Property(o => o.Id).HasMaxLength(36).ValueGeneratedNever();
            });

            modelBuilder.Entity<Address>(entity => {
                entity.Property(a => a.AddressId).UseIdentityColumn();
            });
        }
    }
}