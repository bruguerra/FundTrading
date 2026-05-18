using FundTrading.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundTrading.Data.Configurations
{
    public class FundOrderConfiguration : IEntityTypeConfiguration<FundOrder>
    {
        public void Configure(EntityTypeBuilder<FundOrder> builder)
        {
            builder.ToTable("FundOrders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OperationType)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.ShareQuantity)
                .IsRequired();

            builder.Property(x => x.SharePrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.RejectionReason)
                .HasMaxLength(500);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.InvestmentFund)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.InvestmentFundId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
