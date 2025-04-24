using Microsoft.AspNetCore.Identity;
using SimpleBlog.Application.DTOs;

namespace SimpleBlog.Application.Interface
{
    public interface IAuthService
    {
        public Task<SignInResult> SignInAsync(SignInDTO signInDTO);
        public Task<IdentityResult> SignUpAsync(SignUpDTO signUpDTO);
        public Task SignOutAsync();
    }
}
