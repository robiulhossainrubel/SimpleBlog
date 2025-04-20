using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ApproveController(IPostService postService) : Controller
    {
        public IActionResult Index()
        {
            var posts = postService.GetAll().Where(x => x.PostStatus != Status.Approve).OrderByDescending(x => x.CreatedAt).ToList();

            return View(posts);
        }
        public IActionResult Details(int id)
        {
            var post = postService.Get(id);

            return View(post);
        }
        [HttpPost]
        public IActionResult Details(Post post)
        {
            postService.Update(post);

            return RedirectToAction(nameof(Index));
        }
    }
}
