using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NotionV2.API.Models;

namespace NotionV2.API.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            if (isAuthenticated)
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Login == context.User.Identity.Name);
                if (user == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    var content = JsonConvert.SerializeObject(new {error = "Authentication token is obsolete."});
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(content);
                }
                else
                {
                    await _next.Invoke(context);
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}