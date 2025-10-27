using Microsoft.EntityFrameworkCore;
using PicPayChallenge.Models;

namespace PicPayChallenge
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BaseUser> Users => Set<BaseUser>();
        public DbSet<User> RegularUsers => Set<User>();
        public DbSet<StoreKeeper> StoreKeepers => Set<StoreKeeper>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BaseUser>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<StoreKeeper>("StoreKeeper");
            modelBuilder.Entity<BaseUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<BaseUser>()
                .HasIndex(u => u.CPF)
                .IsUnique();
            modelBuilder.Entity<StoreKeeper>()
                .HasIndex(s => s.CNPJ)
                .IsUnique();
        }
    }

}