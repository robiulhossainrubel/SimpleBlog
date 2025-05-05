using Microsoft.AspNetCore.Identity;
using SimpleBlog.Application.DTOs;

namespace SimpleBlog.Application.Interface
{
    public interface IAuthService
    {
        public Task<SignInResult> SignInAsync(SignInDTO signInDto);
        public Task<IdentityResult> SignUpAsync(SignUpDTO signUpDto);
        public Task SignOutAsync();
    }
}
