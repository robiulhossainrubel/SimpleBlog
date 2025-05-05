using System.Linq.Expressions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Test.ServiceTest;

public class PostServiceTest
{
    private readonly IPostRepository _postRepositoryMock;
    private readonly IPostService _sut;

    public PostServiceTest()
    {
        _postRepositoryMock = Substitute.For<IPostRepository>();
        _sut = new PostService(_postRepositoryMock);
    }

    #region Create Tests
    [Fact]
    public void Create_CallRepo_CreateSuccessfull()
    {
        // Arrange
        var postDto = new PostDTO
        {
            Title = "fkdk",
            Body = "skdmnkmnffs",
            AppUserId = 1,
        };

        // Act
        _sut.Create(postDto);

        // Assert
        _postRepositoryMock.Received(1).Create(Arg.Any<Post>());
    }

    [Fact]
    public async Task Create_ThrowException_ReThrowException()
    {
        // Arrange
        var postDto = new PostDTO
        {
            Title = "fkdk",
            Body = "skdmnkmnffs",
            AppUserId = 1,
        };

        // Act
        _postRepositoryMock.Create(Arg.Any<Post>()).Throws(new Exception());

        // Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.Create(postDto));
    }
    #endregion

    #region Get tests
    [Fact]
    public void Get_GetSinglePost_ReturnPost()
    {
        // Arrange
        const int postId = 1;

        var post = new Post
        {
            Id = 1,
            Title = "fkdk",
            Body = "skdmnkmnffs",
            AppUserId = 1,
        };
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

    #region GetAll Test
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

    #region TopPost Tests
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

    #region TopPost Tests
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

    #region Update Tests
    [Fact]
    public void Update_CallRepo_UpdateSuccessfull()
    {
        // Arrange
        const int postId = 1;

        var post = new Post
        {
            Id = 1,
            Title = "fkdk",
            Body = "skdmnkmnffs",
            AppUserId = 1,
        };
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

        var post = new Post
        {
            Id = 1,
            Title = "fkdk",
            Body = "skdmnkmnffs",
            AppUserId = 1,
        };

        _postRepositoryMock.GetPost(postId).Returns(post);
        post.PostStatus = Status.Approve;

        // Act
        _postRepositoryMock.Update(Arg.Any<Post>()).Throws(new Exception());

        // Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.Update(post));
    }
    #endregion

    public List<Post> GetDummyPosts(int n)
    {
        var posts = new List<Post>();
        for (int i = 1; i <= n; i++)
        {
            var post = new Post
            {
                Id = i,
                Title = $"fkdsawdk {i}",
                Body = $"skdmsfascvnkmnffs{i}",
                AppUserId = 1,
            };
            posts.Add(post);
        }

        return posts;
    }
}
