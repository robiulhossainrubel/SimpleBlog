using Microsoft.AspNetCore.Identity;

namespace SimpleBlog.Domain.Entities
{
    public class AppUserRole : IdentityRole<int>
    {
        public AppUserRole() { }
    }
}
