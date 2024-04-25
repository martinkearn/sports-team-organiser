using Microsoft.AspNetCore.Authorization;

namespace STO.Wasm.Services;

/// <summary>
/// This authorisation handler will bypass all requirements
/// </summary>
public class BypassAuthService : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var requirement in context.PendingRequirements.ToList())
            context.Succeed(requirement); //Simply pass all requirements
        
        return Task.CompletedTask;
    }
}