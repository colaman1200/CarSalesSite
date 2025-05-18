using Microsoft.EntityFrameworkCore;

namespace CarSalesSite.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CarHistory> CarHistory { get; set; }
        public DbSet<Favourites> Favourites { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Brand> Brands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Favourites>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favourites)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Favourites>()
                .HasOne(f => f.Car)
                .WithMany() // Или WithMany(c => c.Favourites), если есть навигация в Car
                .HasForeignKey(f => f.CarId);
        }


    }



}