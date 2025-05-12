using Microsoft.AspNetCore.Identity;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #region SignIn
        public async Task<SignInResult> SignInAsync(SignInDTO signInDto)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(signInDto.Email, signInDto.Password, signInDto.RememberMe, false);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region SignOut
        public Task SignOutAsync()
        {
            return _signInManager.SignOutAsync();
        }
        #endregion

        #region SignUp
        public async Task<IdentityResult> SignUpAsync(SignUpDTO signUpDto)
        {
            try
            {
                var user = new AppUser
                {
                    Name = signUpDto.Name,
                    Email = signUpDto.Email,
                    UserName = signUpDto.Email,
                    NormalizedUserName = signUpDto.Name,
                    Role = signUpDto.Role ?? "User"
                };

                var result = await _userManager.CreateAsync(user, signUpDto.Password);
                await _userManager.AddToRoleAsync(user, user.Role);

                if (result.Succeeded == true)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
