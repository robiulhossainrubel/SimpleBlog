using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.Areas.Auth.Controllers
{
    public class AuthController(IAuthService authService, RoleManager<AppUserRole> roleManager) : Controller
    {
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
                var result = await authService.SignInAsync(signInDTO);

                if (result == true)
                {
                    return LocalRedirect(url);
                }
            }

            return View(signInDTO);
        }
        [HttpPost]
        public IActionResult SignOut(string returnUrl = null)
        {
            authService.SignOutAsync().GetAwaiter().GetResult();

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
            ViewBag.Roles = roleManager.Roles.Select(x => new SelectListItem { Text = x.Name, Value = x.Name });

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDTO signUpDTO)
        {
            ViewBag.Roles = roleManager.Roles.Select(x => new SelectListItem { Text = x.Name, Value = x.Name });
            var url = Url.Content(signUpDTO.ReturnUrl ?? "/");

            if (ModelState.IsValid)
            {
                bool result = await authService.SignUpAsync(signUpDTO);

                if (result == true)
                {
                    return LocalRedirect("/");
                }
            }

            return View();
        }
    }
}
