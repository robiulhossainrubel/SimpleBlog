using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;

namespace SimpleBlog.Presentation.Areas.Auth.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
            var url = Url.Content(signInDTO.ReturnUrl ?? "~/");

            if (ModelState.IsValid)
            {
                var result = await _authService.SignInAsync(signInDTO);

                if (result == true)
                {
                    return LocalRedirect(url);
                }
            }

            TempData["error"] = "Sign In Failed";

            return View(signInDTO);
        }
        public IActionResult SignOut(string returnUrl = null)
        {
            _authService.SignOutAsync().GetAwaiter().GetResult();

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
            var url = Url.Content(signUpDTO.ReturnUrl ?? "/");

            if (ModelState.IsValid)
            {
                bool result = await _authService.SignUpAsync(signUpDTO);

                if (result == true)
                {
                    return LocalRedirect("/");
                }
            }

            TempData["error"] = "Sign Up Failed";

            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
