using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostService _postService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public HomeController(IPostService postService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _postService = postService;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index(int? pageNo)
        {
            int pageSize = 5;

            var posts = _postService.GetPaginate(pageNo ?? 1, pageSize);
            var isSignIn = _signInManager.IsSignedIn(HttpContext.User);

            if (isSignIn == true)
            {
                posts.UserId = _userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult().Id;
            }

            //var posts = _postService.GetAll().Where(x => x.PostStatus == Status.Approve).OrderBy(x => x.CreatedAt).ToList();

            //return View(PagingList<Post>.CreateAsync(posts.AsQueryable<Post>(), pageNo ?? 1, pageSize));
            return View(posts);
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
