using CartoonWorld.Models;
using Microsoft.EntityFrameworkCore;

namespace CartoonWorld.Data
{
    public class CartoonWorldDbContext : DbContext
    {
        public CartoonWorldDbContext(DbContextOptions<CartoonWorldDbContext> options) : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MovieUser> MovieUsers { get; set; }
        //Zonder deze lijnen zouden de tabelnamen in het meervoud zijn
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().ToTable("Movie");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<MovieUser>().ToTable("MovieUser");

        }

    }
}
