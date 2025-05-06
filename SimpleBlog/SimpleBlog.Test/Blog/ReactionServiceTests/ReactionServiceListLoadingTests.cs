using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace SimpleBlog.Test.Blog.ReactionServiceTests;

public class ReactionServiceListLoadingTests : ReactionServiceBaseTest
{
    #region GetAll
    [Fact]
    public void GetAll_RepoReturnValidList_ReturnsAllReaction()
    {
        // Arrange
        var reactions = GetDummyReactions(5);
        _reactionRepositoryMock.GetAll(includeProperties: Arg.Any<string>()).Returns(reactions);

        // Act
        var result = _sut.GetAll();

        // Assert
        Assert.Equal(reactions.Count, result.Count);
    }

    [Fact]
    public void GetAll_ThrowException_ReThrowException()
    {
        // Arrange
        _reactionRepositoryMock.GetAll(includeProperties: Arg.Any<string>()).Throws(new Exception());

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.GetAll());
    }
    #endregion
}