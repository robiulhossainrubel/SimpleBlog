using NSubstitute;

namespace SimpleBlog.Test.Blog.AuthServiceTests;

public class AuthServiceSignOutTests : AuthServiceBaseTest
{
    #region SignOutAsync
    [Fact]
    public async Task SignOutAsync_CallSignInManager_SignOut()
    {
        // Act
        await _sut.SignOutAsync();

        // Assert
        await _signInManager.Received(1).SignOutAsync();
    }
    #endregion
}