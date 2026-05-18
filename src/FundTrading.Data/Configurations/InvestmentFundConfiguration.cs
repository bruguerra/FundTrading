using FundTrading.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundTrading.Data.Configurations
{
    public class InvestmentFundConfiguration : IEntityTypeConfiguration<InvestmentFund>
    {
        public void Configure(EntityTypeBuilder<InvestmentFund> builder)
        {
            builder.ToTable("InvestmentFunds");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.CutoffTime)
                .IsRequired();

            builder.Property(x => x.SharePrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.MinimumContributionAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.MinimumRemainingBalance)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.CapacityLimit)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.CurrentCapacity)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
