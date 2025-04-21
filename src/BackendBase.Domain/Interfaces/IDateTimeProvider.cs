namespace BackendBase.Domain.Interfaces;
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}