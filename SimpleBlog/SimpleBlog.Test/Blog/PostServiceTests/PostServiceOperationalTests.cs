using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Test.Blog.PostServiceTests;

public class PostServiceOperationalTests : PostServiceBaseTest
{
    #region Create
    [Fact]
    public async Task Create_CallRepo_CreateSuccessfull()
    {
        // Arrange
        var postDto = GetDummyPostDTO();

        // Act
        await _sut.Create(postDto);

        // Assert
        await _postRepositoryMock.Received(1).Create(Arg.Any<Post>());
    }

    [Fact]
    public async Task Create_ThrowException_ReThrowException()
    {
        // Arrange
        var postDto = GetDummyPostDTO();
        _postRepositoryMock.Create(Arg.Any<Post>()).Throws(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.Create(postDto));
    }
    #endregion

    #region Update
    [Fact]
    public void Update_CallRepo_UpdateSuccessfull()
    {
        // Arrange
        const int postId = 1;

        var post = GetDummyPost(postId);
        _postRepositoryMock.GetPost(postId).Returns(post);
        post.PostStatus = Status.Approve;

        // Act
        _sut.Update(post);

        // Assert
        Assert.Equal(Status.Approve, post.PostStatus);
        _postRepositoryMock.Received(1).Update(Arg.Any<Post>());
    }

    [Fact]
    public async Task Update_ThrowException_ReThrowException()
    {
        // Arrange
        const int postId = 1;

        var post = GetDummyPost(1);

        _postRepositoryMock.GetPost(postId).Returns(post);
        post.PostStatus = Status.Approve;

        // Act
        _postRepositoryMock.Update(Arg.Any<Post>()).Throws(new Exception());

        // Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.Update(post));
    }
    #endregion
}