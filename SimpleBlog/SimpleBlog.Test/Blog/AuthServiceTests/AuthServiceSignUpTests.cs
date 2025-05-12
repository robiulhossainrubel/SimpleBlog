using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Test.Blog.AuthServiceTests;

public class AuthServiceSignUpTests : AuthServiceBaseTest
{
    #region SignUpAsync
    [Fact]
    public async Task SignUpAsync_CallUserManager_CreateUser()
    {
        // Arrange
        var signUpDto = GetDummySignUpDTO();
        var user = GetDummyAppUser();
        var expectedResult = IdentityResult.Success;
        _userManager.CreateAsync(Arg.Any<AppUser>(), Arg.Any<string>()).Returns(expectedResult);

        // Act
        var result = await _sut.SignUpAsync(signUpDto);

        // Assert
        Assert.Equal(expectedResult, result);
        await _userManager.Received(1).AddToRoleAsync(Arg.Any<AppUser>(), Arg.Any<string>());
        await _signInManager.Received(1).SignInAsync(Arg.Any<AppUser>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task SignUpAsync_ThrowException_ReThrowException()
    {
        // Arrange
        var signUpDto = GetDummySignUpDTO();
        _userManager.CreateAsync(Arg.Any<AppUser>(), Arg.Any<string>()).Throws(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.SignUpAsync(signUpDto));
    }
    #endregion
}