using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;

namespace SimpleBlog.Presentation.Areas.Admin.Controllers
{
    public class UserManagerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly BlogDbContext _context;

        public UserManagerController(UserManager<AppUser> userManager, BlogDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var users = _context.Users.Where(u => u.Id != currentUser.Id).ToList();

            return View(users);
        }

        public async Task<IActionResult> BlockUnBlockAsync(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
                {
                    user.LockoutEnd = null;
                }
                else
                {
                    user.LockoutEnd = DateTime.Now.AddYears(100);
                }
            }

            _context.Users.Update(user);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
