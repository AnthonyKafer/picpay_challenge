using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace picpay_challenge.Domain.Models.Transaction
{
    internal class TransactionConfig : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> modelBuilder)
        {
            modelBuilder
             .HasOne(t => t.Payer)
             .WithMany()
             .HasForeignKey(t => t.PayerId)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(t => t.Payee)
                .WithMany()
                .HasForeignKey(t => t.PayeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Property(u => u.Status)
               .HasConversion<string>();
        }
    }
}
