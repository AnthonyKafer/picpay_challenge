using Microsoft.EntityFrameworkCore;
using picpay_challenge.Domain.Models.Transaction;
using picpay_challenge.Domain.Models.User;

namespace picpay_challenge.Domain.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<BaseUser> Users => Set<BaseUser>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
