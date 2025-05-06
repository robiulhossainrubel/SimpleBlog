using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Test.CommentServiceTests;

public class CommentServiceOperationalTests : CommentServiceBaseTest
{
    #region Create
    [Fact]
    public async Task Create_CallsRepository_CreateSuccessfull()
    {
        // Arrange
        var commentDto = GetDummyCommentDTO();

        // Act
        await _sut.Create(commentDto);

        // Assert
        await _commentRepositoryMock.Received(1).Create(Arg.Any<Comment>());
    }

    [Fact]
    public async Task Create_CallsRepository_ReturnException()
    {
        // Arrange
        var commentDto = GetDummyCommentDTO();

        _commentRepositoryMock.Create(Arg.Any<Comment>()).ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.Create(commentDto));
    }
    #endregion
}