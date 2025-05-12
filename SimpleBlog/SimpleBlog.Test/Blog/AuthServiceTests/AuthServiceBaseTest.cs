using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Test.Blog.AuthServiceTests;

public class AuthServiceBaseTest : IDisposable
{
    protected readonly SignInManager<AppUser> _signInManager;
    protected readonly UserManager<AppUser> _userManager;
    protected readonly IAuthService _sut;

    public AuthServiceBaseTest()
    {
        var userStore = Substitute.For<IUserStore<AppUser>>();
        _userManager = Substitute.For<UserManager<AppUser>>(
            userStore, null, null, null, null, null, null, null, null);

        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        var userClaimsPrincipalFactory = Substitute.For<IUserClaimsPrincipalFactory<AppUser>>();
        _signInManager = Substitute.For<SignInManager<AppUser>>(
            _userManager, contextAccessor, userClaimsPrincipalFactory, null, null, null, null);

        _sut = new AuthService(_signInManager, _userManager);
    }

    #region DummyData Helper
    public AppUser GetDummyAppUser()
    {
        var user = new AppUser
        {
            Id = 1,
            Name = "Rubel",
            UserName = "admin@gmail.com",
            NormalizedUserName = "ADMIN@GMAIL.COM",
            Email = "admin@gmail.com",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            PasswordHash = "AQAAAAIAAYagAAAAEAgVBuw3JzTD6AQDB8K2ccKAXHzgHtvSx0tzTXPAQxTNSQYGhDpbLBaWDtBmysHQwA==",
            SecurityStamp = "MMX5U3PRP3W55CNKW2QFP5SDUVJYBJMK",
            ConcurrencyStamp = "5e68237c-a085-4568-8ac1-90f0d8c465fa",
            Role = "Admin",
        };

        return user;
    }

    public SignInDTO GetDummySignInDTO()
    {
        var signInDto = new SignInDTO
        {
            Email = "admin@gmail.com",
            Password = "@Admin123",
            RememberMe = false
        };

        return signInDto;
    }

    public SignUpDTO GetDummySignUpDTO()
    {
        var signUpDto = new SignUpDTO
        {
            Name = "Rubel",
            Email = "admin@gmail.com",
            Password = "@Admin123",
            ConfirmPassword = "@Admin123",
            Role = "Admin"
        };

        return signUpDto;
    }
    #endregion

    public void Dispose()
    {
        _userManager?.Dispose();
    }
}