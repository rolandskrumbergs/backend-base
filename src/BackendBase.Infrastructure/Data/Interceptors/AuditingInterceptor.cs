using BackendBase.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BackendBase.Infrastructure.Data.Interceptors;
public sealed class AuditingInterceptor(IUserContext userContext) : SaveChangesInterceptor
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

        var currentUserAccountId = _userContext.AccountId;

        IEnumerable<EntityEntry<IAuditable>> addedAuditableEntries =
            eventData
                .Context
                .ChangeTracker
                .Entries<IAuditable>()
                .Where(e => e.State == EntityState.Added);

        foreach (var auditableEntry in addedAuditableEntries)
        {
            auditableEntry.Entity.CreatedAt = DateTimeOffset.UtcNow;
            auditableEntry.Entity.CreatedBy = currentUserAccountId;
        }

        IEnumerable<EntityEntry<IAuditable>> modifiedAuditableEntries =
            eventData
                .Context
                .ChangeTracker
                .Entries<IAuditable>()
                .Where(e => e.State == EntityState.Modified);

        foreach (var auditableEntry in modifiedAuditableEntries)
        {
            auditableEntry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            auditableEntry.Entity.UpdatedBy = currentUserAccountId;
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
