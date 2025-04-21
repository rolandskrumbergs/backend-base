using BackendBase.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BackendBase.Infrastructure.Data.Interceptors;
public sealed class SoftDeleteInterceptor(IUserContext userContext) : SaveChangesInterceptor
{
    private readonly IUserContext _userContext = userContext;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context is null)
        {
            return base.SavingChangesAsync(
                eventData, result, cancellationToken);
        }

        IEnumerable<EntityEntry<ISoftDeletable>> entries =
            eventData
                .Context
                .ChangeTracker
                .Entries<ISoftDeletable>()
                .Where(e => e.State == EntityState.Deleted);

        var currentUserAccountId = _userContext.AccountId;

        foreach (EntityEntry<ISoftDeletable> softDeletable in entries)
        {
            softDeletable.State = EntityState.Modified;
            softDeletable.Entity.IsDeleted = true;
            softDeletable.Entity.DeletedAt = DateTimeOffset.UtcNow;
            softDeletable.Entity.DeletedBy = currentUserAccountId;

            // We need to do the following as EF automatically marks owned types as null
            // if deletion is happening. If entity is referencing owned type, then
            // it should be marked as unchanged
            foreach (var reference in softDeletable.References)
            {
                if (reference.TargetEntry == null)
                {
                    continue;
                }

                var entityType = reference.TargetEntry.Entity.GetType();

                // If entity is owned, then we need to leave target entry as unchanged
                var attributes = entityType.GetCustomAttributes(typeof(OwnedAttribute), true);

                if (attributes is not null)
                {
                    reference.TargetEntry.State = EntityState.Unchanged;
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}