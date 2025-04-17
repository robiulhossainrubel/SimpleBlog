using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Presentation.Areas.Admin.Controllers
{
    public class ApproveController(IPostService postService) : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var posts = postService.GetAll().Where(x => x.Status != Status.Approve).OrderByDescending(x => x.CreatedAt).ToList();
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
