using Microsoft.AspNetCore.Authorization;
namespace Movies.EF.Filters
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        public PermissionAuthorizationHandler()
        {
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
                return Task.CompletedTask;

            var hasPermission = context.User.Claims.Any(c =>
                c.Type == "Permission" &&
                c.Value == requirement.Permission &&
                c.Issuer == "LOCAL AUTHORITY");

            if (hasPermission)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }

    }
}