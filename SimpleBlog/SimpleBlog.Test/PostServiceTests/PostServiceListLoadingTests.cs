using NSubstitute.ExceptionExtensions;
using NSubstitute;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Domain.Entities;
using System.Linq.Expressions;

namespace SimpleBlog.Test.PostServiceTests;

public class PostServiceListLoadingTests : PostServiceBaseTest
{
    #region GetAll
    [Fact]
    public void GetAll_CallRepo_ReturnAllPosts()
    {
        // Arrange
        var posts = GetDummyPosts(5);
        _postRepositoryMock.GetAllPosts(Arg.Any<Expression<Func<Post, bool>>>()).Returns(posts);

        // Act
        var result = _sut.GetAll();

        // Assert
        Assert.Equal(result.Count, posts.Count);
    }

    [Fact]
    public void GetAll_CallRepo_ThrowException()
    {
        // Act
        _postRepositoryMock.GetAllPosts(Arg.Any<Expression<Func<Post, bool>>>()).Throws(new Exception());

        // Assert
        Assert.Throws<Exception>(() => _sut.GetAll());
    }
    #endregion

    #region GetPaginate Tests
    [Fact]
    public void GetPaginate_CallRepo_ReturnTopPost()
    {
        // Arrange
        var posts = new Pagination<Post>(GetDummyPosts(5), 10, 1, 5);
        _postRepositoryMock.GetPaginate(1, 5).Returns(posts);

        // Act
        var result = _sut.GetPaginate(1, 5);

        // Assert
        Assert.Equal(posts.Items.Count, result.Items.Count);
    }

    [Fact]
    public void GetPaginate_CallRepo_ThrowException()
    {
        // Arrange
        _postRepositoryMock.GetPaginate(1, 5).Throws(new Exception());

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.GetPaginate(1, 5));
    }
    #endregion

    #region TopPost
    [Fact]
    public void TopPosts_CallRepo_ReturnTopPost()
    {
        // Arrange
        var posts = GetDummyPosts(5);
        _postRepositoryMock.TopPosts().Returns(posts);

        // Act
        var result = _sut.TopPosts();

        // Assert
        Assert.Equal(posts.Count, result.Count);
    }

    [Fact]
    public void TopPosts_CallRepo_ThrowException()
    {
        // Arrange
        _postRepositoryMock.TopPosts().Throws(new Exception());

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.TopPosts());
    }
    #endregion
}