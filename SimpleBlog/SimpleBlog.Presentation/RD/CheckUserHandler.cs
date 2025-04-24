using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Presentation.RD
{
    public class CheckUserHandler(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, BlogDbContext db) : AuthorizationHandler<CheckUser>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckUser requirement)
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
