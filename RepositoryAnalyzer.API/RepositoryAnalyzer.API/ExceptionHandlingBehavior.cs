using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RepositoryAnalyzer.Domain.Exceptions;

public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ADomainException ex)
        {
            _logger.LogWarning(ex, "Domain exception occurred: {Message}", ex.Message);

            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                context.Response.StatusCode = (int)ex.GetHttpStatusCode();
                await context.Response.WriteAsync(ex.Message);
            }

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while processing {RequestName}", typeof(TRequest).Name);
            throw;
        }
    }
}