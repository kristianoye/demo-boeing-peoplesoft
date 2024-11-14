using demo_boeing_peoplesoft.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace demo_boeing_peoplesoft.Data
{
    public class BoeingDbContext: DbContext
    {

        public BoeingDbContext(IConfiguration config)
        {
            _config = config;
        }

        #region Fields and Properties

        private readonly IConfiguration _config;

        public DbSet<BlogCategory> BlogCategories { get; set; }

        public DbSet<BlogEntry> BlogEntries { get; set; }

        public DbSet<BlogEntryCategory> BlogEntryCategories { get; set; }

        public DbSet<MediaFile> MediaFiles { get; set; }

        public DbSet<UserModel> Users { get; set; }

        #endregion

        #region Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_config != null)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("BlogDatabase"));
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogEntryCategory>()
                .HasKey(bc => new { bc.BlogCategoryId, bc.BlogEntryId });

            modelBuilder.Entity<BlogEntryCategory>()
                .HasOne(bc => bc.BlogEntry)
                .WithMany(be => be.BlogEntryCategories)
                .HasForeignKey(b => b.BlogEntryId);

            modelBuilder.Entity<BlogEntryCategory>()
                .HasOne(bc => bc.BlogCategory)
                .WithMany(be => be.BlogEntryCategories)
                .HasForeignKey(b => b.BlogCategoryId);

            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}
