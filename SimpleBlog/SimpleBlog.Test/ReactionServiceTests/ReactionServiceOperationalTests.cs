using NSubstitute;
using SimpleBlog.Domain.Entities;
using System.Linq.Expressions;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace SimpleBlog.Test.ReactionServiceTests;

public class ReactionServiceOperationalTests : ReactionServiceBaseTest
{
    #region React
    [Fact]
    public async Task React_Create_WhenNoReactionExist()
    {
        // Arrange
        const int postId = 1;
        const int userId = 1;
        const int reactId = (int)ReactionType.Like;

        var existingReaction = GetDummyReaction(1);
        _reactionRepositoryMock.Get(Arg.Any<Expression<Func<Reaction, bool>>>()).ReturnsNull();

        // Act
        await _sut.React(postId, reactId, userId);

        // Assert
        await _reactionRepositoryMock.Received(1).Create(Arg.Any<Reaction>());
    }

    [Fact]
    public async Task React_Delete_WhenSameReactionClick()
    {
        // Arrange
        var existingReaction = GetDummyReaction(1);
        _reactionRepositoryMock.Get(Arg.Any<Expression<Func<Reaction, bool>>>()).Returns(existingReaction);

        // Act
        await _sut.React(1, (int)ReactionType.Like, 1);

        // Assert
        await _reactionRepositoryMock.Received(1).Delete(existingReaction);
    }

    [Fact]
    public async Task React_Update_WhenDifferentReactionClick()
    {
        // Arrange
        var existingReaction = GetDummyReaction(1);
        _reactionRepositoryMock.Get(Arg.Any<Expression<Func<Reaction, bool>>>()).Returns(existingReaction);

        // Act
        await _sut.React(1, (int)ReactionType.DisLike, 1);

        // Assert
        Assert.Equal(ReactionType.DisLike, existingReaction.ReactType);
        await _reactionRepositoryMock.Received(1).Update(existingReaction);
    }

    [Fact]
    public async Task React_ThrowsException_ReThrowExceptionInCreate()
    {
        // Arrange
        _reactionRepositoryMock.Create(Arg.Any<Reaction>()).ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.React(1, 5, 1));
    }

    [Fact]
    public async Task React_ThrowsException_ReThrowExceptionInUpdate()
    {
        // Arrange
        var existingReaction = GetDummyReaction(1);
        _reactionRepositoryMock.Get(Arg.Any<Expression<Func<Reaction, bool>>>()).Returns(existingReaction);

        _reactionRepositoryMock.Update(Arg.Any<Reaction>()).ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.React(1, 5, 1));
    }

    [Fact]
    public async Task React_ThrowsException_ReThrowExceptionInDelete()
    {
        // Arrange
        var existingReaction = GetDummyReaction(1);
        _reactionRepositoryMock.Get(Arg.Any<Expression<Func<Reaction, bool>>>()).Returns(existingReaction);

        _reactionRepositoryMock.Delete(Arg.Any<Reaction>()).ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.React(1, 10, 1));
    }
    #endregion
}