using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace picpay_challenge.Domain.Models.User
{
    internal class UserConfig : IEntityTypeConfiguration<BaseUser>
    {
        public void Configure(EntityTypeBuilder<BaseUser> modelBuilder)
        {

            modelBuilder
                    .HasIndex(u => u.Email)
                    .IsUnique();

            modelBuilder
                    .HasIndex(u => u.CPF)
                    .IsUnique();

            modelBuilder
               .Property(u => u.Role)
               .HasConversion<string>();

            modelBuilder
                .HasIndex(u => u.CNPJ)
                .IsUnique();

        }

    }
}
