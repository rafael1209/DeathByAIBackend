using System.Security.Claims;
using System.Security.Principal;
using DeathByAIBackend.Interfaces;
using DeathByAIBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DeathByAIBackend.Middlewares;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthMiddlewareAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var dbContext = serviceProvider.GetRequiredService<IUserRepository>();
        return new AuthMiddlewareFilter(httpContextAccessor, dbContext);
    }

    private class AuthMiddlewareFilter(IHttpContextAccessor httpContextAccessor, IUserRepository dbContext)
        : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var request = httpContextAccessor.HttpContext!.Request;
                var userData = await IsUserAuthorizedByTokenFromHeaderAsync(request);

                if (userData != null)
                {
                    httpContextAccessor.HttpContext.Items["@me"] = userData;
                    request.HttpContext.User.AddIdentity(
                        new ClaimsIdentity(new GenericIdentity(userData.Id.ToString())));

                    await next();
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private async Task<User?> IsUserAuthorizedByTokenFromHeaderAsync(HttpRequest request)
        {
            request.Headers.TryGetValue("Authorization", out var token);
            var apiKey = token.FirstOrDefault();

            if (string.IsNullOrEmpty(apiKey)) return null;

            var user = await dbContext.TryGetUserByAuthToken(apiKey);

            return user;
        }
    }
}