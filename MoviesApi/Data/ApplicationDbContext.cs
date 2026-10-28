
using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){}

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Genre)
                .WithMany()
                .HasForeignKey(m => m.GenreId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}