using BackendBase.Domain.Interfaces;

namespace BackendBase.Infrastructure.Identity;
public sealed class UserContext : IUserContext
{
    public Guid AccountId => throw new NotImplementedException();
}