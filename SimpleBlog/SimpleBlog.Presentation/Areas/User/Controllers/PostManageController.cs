using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
    public class PostManageController : Controller
    {
        private readonly IPostService _postService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<PostManageController> _logger;

        public PostManageController(IPostService postService, UserManager<AppUser> userManager, ILogger<PostManageController> logger)
        {
            _postService = postService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? status)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                if (status == null)
                {
                    var posts = _postService.GetAll(x => x.AppUserId == user.Id);

                    return View(posts);
                }
                else
                {
                    ViewData["status"] = status;

                    var posts = _postService.GetAll(x => x.AppUserId == user.Id && x.PostStatus == (Status)status);

                    return View(posts);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, Internal error occure";
                _logger.LogError(ex, ex.Message);

                return View();
            }
        }
    }
}
