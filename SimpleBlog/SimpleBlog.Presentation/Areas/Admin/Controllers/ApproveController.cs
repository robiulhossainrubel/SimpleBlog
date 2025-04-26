using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ApproveController : Controller
    {
        private readonly IPostService _postService;

        public ApproveController(IPostService postService)
        {
            _postService = postService;
        }

        public IActionResult Index()
        {
            try
            {
                var posts = _postService.GetAll(x => x.PostStatus != Status.Approve).ToList();

                return View(posts);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                return View();
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var post = _postService.Get(id);

                return View(post);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                return View();
            }
        }

        [HttpPost]
        public IActionResult Details(Post post)
        {
            try
            {
                _postService.Update(post);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                return View(post);
            }
        }

        public IActionResult TopPosts()
        {
            try
            {
                var topPosts = _postService.TopPosts();

                return View(topPosts);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                return View();
            }
        }
    }
}
