using System.Reflection;
using BackendBase.Domain.Abstract;
using BackendBase.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendBase.Infrastructure.Data;
public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IDomainEventDispatcher? dispatcher) : DbContext(options)
{
    private readonly IDomainEventDispatcher? _dispatcher = dispatcher;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // ignore events if no dispatcher provided
        if (_dispatcher == null)
        {
            return result;
        }

        // dispatch events only if save was successful
        var entitiesWithEvents = ChangeTracker.Entries<DomainEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToArray();

        await _dispatcher.DispatchAndClearEvents(entitiesWithEvents).ConfigureAwait(false);

        return result;
    }
}
