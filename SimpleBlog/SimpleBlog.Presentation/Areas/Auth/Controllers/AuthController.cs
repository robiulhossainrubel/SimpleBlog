using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;

namespace SimpleBlog.Presentation.Areas.Auth.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInDTO signInDTO)
        {
            try
            {
                var url = Url.Content(signInDTO.ReturnUrl ?? "~/");

                if (ModelState.IsValid == true)
                {
                    var result = await _authService.SignInAsync(signInDTO);

                    if (result.Succeeded == true)
                    {
                        return LocalRedirect(url);
                    }

                    if (result.IsLockedOut == true)
                    {
                        TempData["error"] = "You are Blocked";

                        return View(signInDTO);
                    }
                }

                TempData["error"] = "Sign In Failed";

                return View(signInDTO);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, Internal error occure";
                _logger.LogError(ex, ex.Message);

                return View(signInDTO);
            }
        }

        public async Task<IActionResult> SignOutAsync(string returnUrl = null)
        {
            await _authService.SignOutAsync();

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(SignIn));
            }
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDTO signUpDTO)
        {
            try
            {
                var url = Url.Content(signUpDTO.ReturnUrl ?? "/");

                if (ModelState.IsValid == true)
                {
                    var result = await _authService.SignUpAsync(signUpDTO);

                    if (result.Succeeded == true)
                    {
                        return LocalRedirect("/");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                return View(signUpDTO);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, Internal error occure";
                _logger.LogError(ex, ex.Message);

                return View(signUpDTO);
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
