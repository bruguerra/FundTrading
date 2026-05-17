using FundTrading.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundTrading.Data.Configurations
{
    public class CustomerFundPositionConfiguration : IEntityTypeConfiguration<CustomerFundPosition>
    {
        public void Configure(EntityTypeBuilder<CustomerFundPosition> builder)
        {
            builder.ToTable("CustomerFundPositions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ShareQuantity)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Positions)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InvestmentFund)
                .WithMany(x => x.Positions)
                .HasForeignKey(x => x.InvestmentFundId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.CustomerId,
                x.InvestmentFundId
            }).IsUnique();
        }
    }
}
