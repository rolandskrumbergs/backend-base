namespace BackendBase.Domain.Interfaces;

// Apply this marker interface only to aggregate root entities
// Repositories will only work with aggregate roots, not their children
#pragma warning disable CA1040 // Avoid empty interfaces
public interface IAggregateRoot { }
#pragma warning restore CA1040 // Avoid empty interfaces
