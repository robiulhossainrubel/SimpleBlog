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

        public async Task<SignInResult> SignInAsync(SignInDTO signInDTO)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(signInDTO.Email, signInDTO.Password, signInDTO.RememberMe, false);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task SignOutAsync()
        {
            return _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> SignUpAsync(SignUpDTO signUpDTO)
        {
            try
            {
                var user = new AppUser
                {
                    Name = signUpDTO.Name,
                    Email = signUpDTO.Email,
                    UserName = signUpDTO.Email,
                    NormalizedUserName = signUpDTO.Name,
                    Role = signUpDTO.Role ?? "User"
                };

                var result = await _userManager.CreateAsync(user, signUpDTO.Password);
                await _userManager.AddToRoleAsync(user, user.Role);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return result;
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
