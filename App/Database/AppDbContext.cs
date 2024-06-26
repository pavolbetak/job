using App.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace App.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {

        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Image> Images{ get; set; }
        public DbSet<Site> Sites{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Article>().HasKey(x => x.Id);
            modelBuilder.Entity<Article>().HasIndex(x => x.Title);

            modelBuilder.Entity<Author>().HasKey(x => x.Id);
            modelBuilder.Entity<Author>().HasIndex(x => x.Name).IsUnique();

            modelBuilder.Entity<Image>().HasKey(x => x.Id);
            
            modelBuilder.Entity<Site>().HasKey(x => x.Id);

            modelBuilder.Entity<Article>()
                .HasMany(a => a.Authors)
                .WithMany(a => a.Articles);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Site)
                .WithMany(s => s.Articles)
                .HasForeignKey("SiteId");

            modelBuilder.Entity<Author>()
                .HasOne(a => a.Image)
                .WithOne(i => i.Author)
                .HasForeignKey<Image>("ImageId");
        }
    }
}
