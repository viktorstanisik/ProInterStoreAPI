using Microsoft.EntityFrameworkCore;
using ProInterStore_Domain.EntityModels;

namespace ProInterStore_Domain
{
    public class ProInterStoreDbContext : DbContext
    {
        public ProInterStoreDbContext(DbContextOptions<ProInterStoreDbContext> options) : base(options)
        {
        }

        public DbSet<StoreItem> StoreItems { get; set; }
        public DbSet<StoreItemAttribute> StoreItemAttribute { get; set; }
        public DbSet<AuditInfo> AuditInfo { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LoggedUserInfo> LoggedUsersInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureStoreItem(modelBuilder);
            ConfigureStoreItemAttribute(modelBuilder);
            ConfigureAuditInfo(modelBuilder);
            ConfigureUser(modelBuilder);
            ConfigureLoggedUserInfo(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureStoreItem(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreItem>()
               .HasKey(si => si.Id);

            modelBuilder.Entity<StoreItem>()
                .Property(si => si.ItemCode)
                .IsRequired();

            modelBuilder.Entity<StoreItem>()
                .Property(si => si.Name)
                .IsRequired();

            modelBuilder.Entity<StoreItem>()
                .Property(si => si.ProductGroupId)
                .IsRequired();

            modelBuilder.Entity<StoreItem>()
                .Property(si => si.UnitOfMeasure)
                .IsRequired();

            modelBuilder.Entity<StoreItem>()
                .Property(si => si.isDeleted)
                .IsRequired();

            modelBuilder.Entity<StoreItem>()
                .HasMany(si => si.Attributes)
                .WithOne(attribute => attribute.StoreItem)
                .HasForeignKey(attribute => attribute.StoreItemId);
        }

        private static void ConfigureStoreItemAttribute(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreItemAttribute>()
                        .HasKey(sia => sia.Id);

            modelBuilder.Entity<StoreItemAttribute>()
                .Property(sia => sia.AttributeColor)
                .HasMaxLength(50);

            modelBuilder.Entity<StoreItemAttribute>()
                .Property(sia => sia.AttributeHeight)
                .IsRequired();

            modelBuilder.Entity<StoreItemAttribute>()
                .Property(sia => sia.AttributeWidth)
                .IsRequired();

            modelBuilder.Entity<StoreItemAttribute>()
                .Property(sia => sia.AttributeWeight)
                .IsRequired();

            modelBuilder.Entity<StoreItemAttribute>()
                .Property(sia => sia.StoreItemId)
                .IsRequired();

            modelBuilder.Entity<StoreItemAttribute>()
                .HasOne(sia => sia.StoreItem)
                .WithMany(si => si.Attributes)
                .HasForeignKey(sia => sia.StoreItemId);
        }

        private static void ConfigureAuditInfo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditInfo>()
                        .HasKey(ai => ai.Id);
        }

        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNumber)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();
        }

        private static void ConfigureLoggedUserInfo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoggedUserInfo>()
                        .HasKey(u => u.Id);

            modelBuilder.Entity<LoggedUserInfo>()
                    .Property(u => u.UserId)
                    .IsRequired();

            modelBuilder.Entity<LoggedUserInfo>()
                    .Property(u => u.LastLogin)
                    .IsRequired(false);

            modelBuilder.Entity<LoggedUserInfo>()
                    .Property(u => u.LoginStatus)
                    .IsRequired();
        }
    }
}

