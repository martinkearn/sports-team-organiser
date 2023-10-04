using Microsoft.AspNetCore.Authorization;

namespace STO.Policies
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