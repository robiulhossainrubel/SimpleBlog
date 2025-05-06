using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace SimpleBlog.Test.Blog.CommentServiceTests;

public class CommentServiceListLoadingTests : CommentServiceBaseTest
{
    #region GetAll
    [Fact]
    public void GetAll_RepoReturnsValidList_ReturnsAllComment()
    {
        // Arrange
        var comments = GetDummyComments(5);
        _commentRepositoryMock.GetAll(includeProperties: Arg.Any<string>()).Returns(comments);

        // Act
        var result = _sut.GetAll();

        // Assert
        Assert.Equal(comments.Count, result.Count);
    }

    [Fact]
    public void GetAll_ThrowException_ForError()
    {
        // Arrange
        _commentRepositoryMock.GetAll(includeProperties: Arg.Any<string>()).Throws(new Exception());

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.GetAll());
    }
    #endregion
}