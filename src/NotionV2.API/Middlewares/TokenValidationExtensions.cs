using Microsoft.AspNetCore.Builder;

namespace NotionV2.API.Middlewares
{
    public static class TokenValidationExtensions
    {
        public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidationMiddleware>();
        }
    }
}