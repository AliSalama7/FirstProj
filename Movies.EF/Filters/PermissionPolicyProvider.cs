using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
namespace Movies.EF.Filters
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return FallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy?>(null);  // Return null if no fallback policy is required
        }


        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("Permission", StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));
                return Task.FromResult(policy.Build());
            }
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}