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
        [HttpPost]
        public IActionResult Create(Post post)
        {
            var userId = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult().Id;
            post.AppUserId = userId;
            post.PostStatus = Status.Pending;
            post.CreatedAt = DateTime.Now;


            postService.Create(post);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Details(int id)
        {
            var postVm = new PostVM
            {
                Post = postService.Get(id),
                Comment = new Comment()
            };

            return View(postVm);
        }
        [HttpPost]
        public IActionResult Details(PostVM postVM)
        {
            var comment = postVM.Comment;
            comment.Time = DateTime.Now;
            comment.AppUserId = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult().Id;
            commentService.Create(comment);

            postVM.Post = postService.Get(postVM.Comment.PostId);
            postVM.Comment = new Comment();
            return View(postVM);
        }
        public IActionResult React(int postId, int reactId)
        {
            var userId = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult().Id;
            var reaction = reactionService.GetByPostIdAndUserId(postId, userId);

            if (reaction == null)
            {
                var react = new Reaction
                {
                    ReactType = (ReactionType)reactId,
                    PostId = postId,
                    AppUserId = userId
                };

                reactionService.Create(react);
            }
            else
            {
                if (reaction.ReactType == (ReactionType)reactId)
                {
                    reactionService.Delete(postId, userId);
                }
                else
                {
                    reaction.ReactType = (ReactionType)reactId;
                    reactionService.Update(reaction);
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
