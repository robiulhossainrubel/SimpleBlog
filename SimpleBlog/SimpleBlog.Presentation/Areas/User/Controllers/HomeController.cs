using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostService _postService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IPostService postService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<HomeController> logger)
        {
            _postService = postService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult Index(int? pageNo)
        {
            try
            {
                int pageSize = 5;

                var posts = _postService.GetPaginate(pageNo ?? 1, pageSize);
                var isSignIn = _signInManager.IsSignedIn(HttpContext.User);

                if (isSignIn == true)
                {
                    var currentUser = _userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();
                    posts.UserId = currentUser.Id;
                }

                return View(posts);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, Internal error occure";
                _logger.LogError(ex, ex.Message);


                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
