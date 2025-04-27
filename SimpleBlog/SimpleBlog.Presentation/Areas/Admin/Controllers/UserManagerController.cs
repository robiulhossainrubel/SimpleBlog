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
        private readonly ILogger<UserManagerController> _logger;

        public UserManagerController(UserManager<AppUser> userManager, BlogDbContext context, ILogger<UserManagerController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                var users = _context.Users.Where(u => u.Id != currentUser.Id).ToList();

                return View(users);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, Internal error occure";
                _logger.LogError(ex, ex.Message);

                return View();
            }
        }

        public IActionResult BlockUnBlock(int id)
        {
            try
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
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, Internal error occure";
                _logger.LogError(ex, ex.Message);

                return RedirectToAction(nameof(Index)); ;
            }
        }
    }
}
