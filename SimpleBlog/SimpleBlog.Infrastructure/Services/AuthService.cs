using Microsoft.AspNetCore.Identity;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Services
{
    public class AuthService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : IAuthService
    {
        public async Task<bool> SignInAsync(SignInDTO signInDTO)
        {
            var result = await signInManager.PasswordSignInAsync(signInDTO.Email, signInDTO.Password, signInDTO.RememberMe, false);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }

        public Task SignOutAsync()
        {
            return signInManager.SignOutAsync();
        }

        public async Task<bool> SignUpAsync(SignUpDTO signUpDTO)
        {
            var user = new AppUser
            {
                Email = signUpDTO.Email,
                UserName = signUpDTO.Email,
                NormalizedUserName = signUpDTO.Name,
                Role = signUpDTO.Role ?? "User"
            };

            var result = await userManager.CreateAsync(user, signUpDTO.Password);
            await userManager.AddToRoleAsync(user, user.Role);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return true;
            }
            return false;
        }
    }
}
