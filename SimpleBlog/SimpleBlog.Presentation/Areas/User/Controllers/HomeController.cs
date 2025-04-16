using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
    public class HomeController(IPostService postService) : Controller
    {
        private static int p = 0;
        public IActionResult Index()
        {

            var posts = postService.GetAll().Skip(p).Take(5).ToList();
            p = p + 5;

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
