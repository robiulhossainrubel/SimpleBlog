using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Presentation.ViewModel;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
    [Authorize(Policy = "CheckUser")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IReactionService _reactionService;
        private readonly ICommentService _commentService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public PostController(IPostService postService, IReactionService reactionService, ICommentService commentService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _postService = postService;
            _reactionService = reactionService;
            _commentService = commentService;
            _userManager = userManager;
            _signInManager = signInManager;
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
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            post.AppUserId = currentUser.Id;

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

            var isSignIn = _signInManager.IsSignedIn(HttpContext.User);

            if (isSignIn == true)
            {
                var currentUser = _userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();
                postVm.UserId = currentUser.Id;
            }

            return View(postVm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(PostVM postVM)
        {
            var comment = postVM.Comment;
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            comment.AppUserId = currentUser.Id;
            _commentService.Create(comment);

            postVM.Post = _postService.Get(postVM.Comment.PostId);

            return View(postVM);
        }

        [Authorize]
        public async Task<IActionResult> React(int postId, int reactId, int? id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            _reactionService.React(postId, reactId, currentUser.Id);

            if (id != null)
            {
                return LocalRedirect($"/User/Post/Details/{id}");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
