using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Presentation.ViewModel;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
    public class PostController(IPostService postService, ILikeDisLikeService likeDisLikeService, ICommentService commentService, UserManager<AppUser> userManager) : Controller
    {
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Post post)
        {
            post.LikeDisLikeId = likeDisLikeService.Create(new LikeDisLike());

            var userId = userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult().Id;
            post.AppUserId = userId;
            post.Status = Status.Pending;
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
        public IActionResult Comment()
        {
            return View();
        }
        public IActionResult Like(int id)
        {
            var ld = likeDisLikeService.Get(id);
            ld.Like++;
            likeDisLikeService.Update(ld);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult DisLike(int id)
        {
            var ld = likeDisLikeService.Get(id);
            ld.DisLike++;
            likeDisLikeService.Update(ld);
            return RedirectToAction("Index", "Home");
        }
    }
}
