using FundTrading.Domain.DomainObjects;
using FundTrading.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class FundTradingContext : DbContext, IUnitOfWork
{
    public FundTradingContext(DbContextOptions<FundTradingContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FundTradingContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditing();

        return await base.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> Commit()
    {
        return SaveChangesAsync().ContinueWith(t => t.Result > 0);
    }

    private void ApplyAuditing()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.SetCreationAudit(now, "system");
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.SetUpdateAudit(now, "system");

                entry.Property(x => x.CreatedAt).IsModified = false;
                entry.Property(x => x.CreatedBy).IsModified = false;
            }
        }
    }
}