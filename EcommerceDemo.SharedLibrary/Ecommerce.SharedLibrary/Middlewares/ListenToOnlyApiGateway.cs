using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.SharedLibrary.Middlewares
{
    public class ListenToOnlyApiGateway(RequestDelegate next, IConfiguration config)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Skip middleware for gRPC requests
            if (context.Request.ContentType?.StartsWith("application/grpc") == true)
            {
                await next(context);
                return;
            }

            var secret = context.Request.Headers["X-Internal-Gateway-Token"];

            if (string.IsNullOrEmpty(secret) || secret != config["Authentication:Key"])
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access denied. Only API Gateway is allowed.");
                return;
            }
            await next(context);
        }
    }
}
