namespace BackendBase.Core.Infrastructure.Mediator;

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        var handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");

        var method = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException("Handle method not found on handler");

        var result = await (Task<Result<TResponse>>)method.Invoke(handler, [request, cancellationToken])!;
        return result;
    }
}