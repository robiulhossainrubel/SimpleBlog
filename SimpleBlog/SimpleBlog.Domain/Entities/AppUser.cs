using Microsoft.AspNetCore.Identity;

namespace SimpleBlog.Domain.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
