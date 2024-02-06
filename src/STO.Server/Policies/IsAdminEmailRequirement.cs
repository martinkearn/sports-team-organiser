using Microsoft.AspNetCore.Authorization;

namespace STO.Server.Policies
{
    public class IsAdminEmailRequirement : IAuthorizationRequirement
    {
        public string AdminEmail { get; }

        public IsAdminEmailRequirement(string adminEmail)
        {
            AdminEmail = adminEmail;
        }
    }
}