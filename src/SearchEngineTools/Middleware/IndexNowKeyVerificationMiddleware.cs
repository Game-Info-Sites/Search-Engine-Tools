using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SearchEngineTools.Services;

namespace SearchEngineTools.Middleware
{
    public class IndexNowKeyVerificationMiddleware(
        RequestDelegate next,
        ILogger<IndexNowKeyVerificationMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context, IIndexNowKeyService indexNowKeyService)
        {
            var requestedKey = GetRequestedKey(context.Request.Path);
            if (requestedKey is null)
            {
                await next(context);
                return;
            }

            var domain = context.Request.Host.Host;

            if (await indexNowKeyService.KeyBelongsToDomainAsync(domain, requestedKey, context.RequestAborted))
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(requestedKey, context.RequestAborted);
                return;
            }

            logger.LogDebug("IndexNow key verification failed for domain {Domain}", domain);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }

        private static string? GetRequestedKey(PathString path)
        {
            var value = path.Value;
            if (string.IsNullOrWhiteSpace(value) || !value.StartsWith('/') || !value.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var fileName = value[1..];
            if (fileName.Contains('/'))
            {
                return null;
            }

            var key = fileName[..^4];
            if (key.Length != 32)
            {
                return null;
            }

            return key.All(Uri.IsHexDigit) ? key : null;
        }
    }
}
