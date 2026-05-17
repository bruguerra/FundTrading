using FundTrading.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundTrading.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Document)
                .HasMaxLength(11)
                .IsRequired();

            builder.Property(x => x.AvailableBalance)
                .HasPrecision(18, 2);

            builder.HasIndex(x => x.Document)
                .IsUnique();
        }
    }
}
