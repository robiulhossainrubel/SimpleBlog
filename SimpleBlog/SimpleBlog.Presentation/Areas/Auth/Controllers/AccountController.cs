using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.Areas.Auth.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChangePasswordAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var result = await _userManager.ChangePasswordAsync(currentUser, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

                if (result.Succeeded == false)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View();
                }

                await _signInManager.RefreshSignInAsync(currentUser);

                TempData["message"] = "Password Changed Successfully";

                return LocalRedirect("/");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                return View();
            }
        }
    }
}
