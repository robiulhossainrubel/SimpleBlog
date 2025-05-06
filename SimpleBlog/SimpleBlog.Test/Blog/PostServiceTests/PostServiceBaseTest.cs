using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Test.Blog.PostServiceTests;

[ExcludeFromCodeCoverage]
public class PostServiceBaseTest
{
    protected readonly IPostRepository _postRepositoryMock;
    protected readonly IPostService _sut;

    public PostServiceBaseTest()
    {
        _postRepositoryMock = Substitute.For<IPostRepository>();
        _sut = new PostService(_postRepositoryMock);
    }

    #region DummyData Helper
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

    public Post GetDummyPost(int id)
    {
        var post = new Post
        {
            Id = id,
            Title = "fkdk",
            Body = "skdmnkmnffs",
            AppUserId = 1,
        };

        return post;
    }

    public PostDTO GetDummyPostDTO()
    {
        var postDto = new PostDTO
        {
            Title = "fkdk",
            Body = "skdmnkmnffs",
            AppUserId = 1,
        };

        return postDto;
    }
    #endregion
}