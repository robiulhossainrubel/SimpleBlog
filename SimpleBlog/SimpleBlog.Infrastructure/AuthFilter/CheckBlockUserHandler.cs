using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.DI.AuthFilter
{
    public class CheckBlockUserHandler(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : AuthorizationHandler<CheckBlockUser>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckBlockUser requirement)
        {
            var user = userManager.GetUserAsync(context.User).GetAwaiter().GetResult();

            if (user != null)
            {
                if (user.LockoutEnd != null)
                {
                    signInManager.SignOutAsync();

                    return Task.CompletedTask;
                }
                else
                {
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
