namespace PicPerfect.DATA
{
    using Microsoft.EntityFrameworkCore;
    using PicPerfect.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Album> Album { get; set; }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<Reports> Reports { get; set; }
        public DbSet<EditHistory> EditHistory { get; set; }
        public DbSet<AlbumImages> AlbumImages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlbumImages>()
          .HasKey(m => new { m.AlbumId, m.ImageId });
        }

    }

}
