using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;
using SimpleBlog.Presentation.CustomAttributes;
using SimpleBlog.Presentation.ViewModel;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
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
            var user = await _userManager.GetUserAsync(HttpContext.User);

            post.AppUserId = user.Id;

            _postService.Create(post);

            TempData["message"] = "Post Pending For Approval";

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var postVm = new PostVM
            {
                Post = _postService.Get((int)id)
            };

            var isSignIn = _signInManager.IsSignedIn(HttpContext.User);

            if (isSignIn == true)
            {
                postVm.UserId = _userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult().Id;
            }

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
        public async Task<IActionResult> React(int postId, int reactId, int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            _reactionService.React(postId, reactId, user.Id);

            if (id != null)
            {
                return LocalRedirect($"/User/Post/Details/{id}");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Rubel()
        {
            return Json(new { success = true, message = "This is a test post action." });
        }
    }
}
