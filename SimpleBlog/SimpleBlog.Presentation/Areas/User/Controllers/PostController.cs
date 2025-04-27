using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Presentation.ViewModel;

namespace SimpleBlog.Presentation.Areas.User.Controllers
{
    [Authorize(Policy = "CheckBlockUser")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IReactionService _reactionService;
        private readonly ICommentService _commentService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, IReactionService reactionService, ICommentService commentService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<PostController> logger)
        {
            _postService = postService;
            _reactionService = reactionService;
            _commentService = commentService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostDTO post)
        {
            try
            {
                if (ModelState.IsValid == true)
                {
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    post.AppUserId = currentUser.Id;
                    _postService.Create(post);

                    TempData["message"] = "Post Pending For Approval";

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(post);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, Internal error occure";
                _logger.LogError(ex, ex.Message);

                return View();
            }

        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            try
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
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, Internal error occure";
                _logger.LogError(ex, ex.Message);

                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Details(PostVM postVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var comment = postVM.Comment;
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                    comment.AppUserId = currentUser.Id;
                    _commentService.Create(comment);

                    postVM.Post = _postService.Get(postVM.Comment.PostId);

                    return View(postVM);
                }
                else
                {
                    postVM.Post = _postService.Get(postVM.Comment.PostId);

                    return View(postVM);
                }
            }
            catch (Exception ex)
            {
                postVM.Post = _postService.Get(postVM.Comment.PostId);

                TempData["error"] = "Something went wrong, Internal error occure";
                _logger.LogError(ex, ex.Message);

                return View(postVM);
            }
        }

        public async Task<IActionResult> React(int postId, int reactId, string url)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                _reactionService.React(postId, reactId, currentUser.Id);

                return LocalRedirect(url);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, Internal error occure";
                _logger.LogError(ex, ex.Message);

                return LocalRedirect(url);
            }
        }
    }
}
