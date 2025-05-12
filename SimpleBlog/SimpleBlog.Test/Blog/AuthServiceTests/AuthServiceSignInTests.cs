using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace SimpleBlog.Test.Blog.AuthServiceTests;

public class AuthServiceSignInTests : AuthServiceBaseTest
{
    #region SignInAsync
    [Fact]
    public async Task SignInAsync_CallSignInManager_SignInSuccessful()
    {
        // Arrange
        var singInDto = GetDummySignInDTO();
        var expectedResult = SignInResult.Success;
        _signInManager.PasswordSignInAsync(singInDto.Email, singInDto.Password, singInDto.RememberMe, false).Returns(expectedResult);

        // Act
        var result = await _sut.SignInAsync(singInDto);

        // Assert
        Assert.Equal(result, expectedResult);
    }

    [Fact]
    public async Task SignInAsync_ThrowException_ReThrowException()
    {
        // Arrange
        var singInDto = GetDummySignInDTO();
        _signInManager.PasswordSignInAsync(singInDto.Email, singInDto.Password, singInDto.RememberMe, false).Throws(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.SignInAsync(singInDto));
    }
    #endregion
}