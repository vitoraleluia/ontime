using MediatR;

using Microsoft.Extensions.Logging;

namespace OnTime.Application.Features;

public abstract class BaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<BaseHandler<TRequest, TResponse>> logger;

    public BaseHandler(ILogger<BaseHandler<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await HandleSafe(request, cancellationToken);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            this.logger.LogError(ex, "Error while handling {Request}. Got this error: {ExceptionMessage}", requestName,
                ex.Message);

            throw;
        }
    }

    protected abstract Task<TResponse> HandleSafe(TRequest request, CancellationToken cancellationToken);
}