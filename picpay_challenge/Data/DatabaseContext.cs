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
        public DbSet<Transaction> Transactions => Set<Transaction>();

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

            modelBuilder.Entity<Transaction>()
              .HasOne(t => t.Payer)
              .WithMany()
              .HasForeignKey(t => t.PayerId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
             .HasOne(t => t.Payee)
             .WithMany()
             .HasForeignKey(t => t.PayeeId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }

}