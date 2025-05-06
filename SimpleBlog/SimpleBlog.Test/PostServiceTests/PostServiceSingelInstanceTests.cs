using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace SimpleBlog.Test.PostServiceTests;

public class PostServiceSingelInstanceTests : PostServiceBaseTest
{
    #region Get
    [Fact]
    public void Get_GetSinglePost_ReturnPost()
    {
        // Arrange
        const int postId = 1;

        var post = GetDummyPost(postId);
        _postRepositoryMock.GetPost(postId).Returns(post);

        // Act
        var result = _sut.Get(postId);

        // Assert
        Assert.Equal(result.Title, post.Title);
        Assert.Equal(result.Body, post.Body);
    }

    [Fact]
    public void Get_ThrowException_ReThrowException()
    {
        // Arrange
        const int postId = 1;

        _postRepositoryMock.GetPost(Arg.Any<int>()).Throws(new Exception());

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.Get(postId));
    }
    #endregion
}