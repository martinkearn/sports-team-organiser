using Microsoft.AspNetCore.Authorization;

namespace STO.Wasm.Policies
{
    public class IsAdminEmailRequirement(string adminEmail) : IAuthorizationRequirement
    {
        public string AdminEmail { get; } = adminEmail;
    }
}