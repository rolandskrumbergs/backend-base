namespace BackendBase.Core.Infrastructure.Mediator;

public interface IMediator
{
    Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}