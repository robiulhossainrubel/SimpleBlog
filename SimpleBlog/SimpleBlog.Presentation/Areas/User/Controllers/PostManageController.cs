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

        public PostManageController(IPostService postService, UserManager<AppUser> userManager)
        {
            _postService = postService;
            _userManager = userManager;
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
                TempData["error"] = ex.Message;

                return View();
            }
        }
    }
}
