using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Presentation.ViewModel;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
    public class PostController(IPostService postService, IReactionService reactionService, ICommentService commentService, UserManager<AppUser> userManager) : Controller
    {
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            post.AppUserId = user.Id;

            postService.Create(post);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Details(int id)
        {
            var postVm = new PostVM
            {
                Post = postService.Get(id)
            };

            return View(postVm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(PostVM postVM)
        {
            var comment = postVM.Comment;
            var user = await userManager.GetUserAsync(HttpContext.User);

            comment.AppUserId = user.Id;
            commentService.Create(comment);

            postVM.Post = postService.Get(postVM.Comment.PostId);

            return View(postVM);
        }

        [Authorize]
        public IActionResult React(int postId, int reactId)
        {
            var userId = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult().Id;

            reactionService.React(postId, reactId, userId);

            return RedirectToAction("Index", "Home");
        }
    }
}
