using SimpleBlog.Application.DTOs;

namespace SimpleBlog.Application.Interface
{
    public interface IAuthService
    {
        public Task<bool> SignInAsync(SignInDTO signInDTO);
        public Task<bool> SignUpAsync(SignUpDTO signUpDTO);
        public Task SignOutAsync();
    }
}
