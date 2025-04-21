using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Presentation.ViewModel;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IReactionService _reactionService;
        private readonly ICommentService _commentService;
        private readonly UserManager<AppUser> _userManager;
        public PostController(IPostService postService, IReactionService reactionService, ICommentService commentService, UserManager<AppUser> userManager)
        {
            _postService = postService;
            _reactionService = reactionService;
            _commentService = commentService;
            _userManager = userManager;
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            post.AppUserId = user.Id;

            _postService.Create(post);

            TempData["message"] = "Post Pending For Approval";

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Details(int id)
        {
            var postVm = new PostVM
            {
                Post = _postService.Get(id)
            };

            return View(postVm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(PostVM postVM)
        {
            var comment = postVM.Comment;
            var user = await _userManager.GetUserAsync(HttpContext.User);

            comment.AppUserId = user.Id;
            _commentService.Create(comment);

            postVM.Post = _postService.Get(postVM.Comment.PostId);

            return View(postVM);
        }

        [Authorize]
        public IActionResult React(int postId, int reactId)
        {
            var userId = _userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult().Id;

            _reactionService.React(postId, reactId, userId);

            return RedirectToAction("Index", "Home");
        }
    }
}
