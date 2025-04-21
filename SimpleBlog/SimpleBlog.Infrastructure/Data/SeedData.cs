using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Data
{
    public class SeedData : ISeedData
    {
        private readonly BlogDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppUserRole> _roleManager;
        public SeedData(BlogDbContext context, UserManager<AppUser> userManager, RoleManager<AppUserRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (!_roleManager.RoleExistsAsync("User").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new AppUserRole("User")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new AppUserRole("Admin")).GetAwaiter().GetResult();

                var admin = new AppUser
                {
                    Name = "Admin",
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Role = "Admin"
                };

                _userManager.CreateAsync(admin, "@Admin123").GetAwaiter().GetResult();

                var user = _context.Users.FirstOrDefault(u => u.Email == "admin@gmail.com");
                _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            }
            return;
        }
    }
}
