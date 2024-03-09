using Microsoft.AspNetCore.Authorization;

namespace STO.Wasm.Policies
{
    public class IsAdminEmailHandler : AuthorizationHandler<IsAdminEmailRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminEmailRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "emails"))
            {
                return Task.CompletedTask;
            }
            
            var emailAddress = context?.User.FindFirst(c => c.Type == "emails")?.Value;
            
            if (emailAddress == requirement.AdminEmail)
            {
                context?.Succeed(requirement);
            }
            
            return Task.CompletedTask;
        }
    }
}