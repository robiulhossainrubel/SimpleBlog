using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
    public class HomeController(IPostService postService) : Controller
    {
        private int page = 5;
        public IActionResult Index(int? pages)
        {

            var posts = postService.GetAll().Where(x => x.Status == Status.Approve).Take(pages ?? 5).OrderBy(x => x.CreatedAt).ToList();
            page += 5;
            TempData["page"] = page;
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
