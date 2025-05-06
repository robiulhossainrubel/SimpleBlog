using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Test.CommentServiceTests;

[ExcludeFromCodeCoverage]
public class CommentServiceBaseTest
{
    protected readonly CommentService _sut;
    protected readonly ICommentRepository _commentRepositoryMock;

    public CommentServiceBaseTest()
    {
        _commentRepositoryMock = Substitute.For<ICommentRepository>();
        _sut = new CommentService(_commentRepositoryMock);
    }

    public List<Comment> GetDummyComments(int n)
    {
        var comments = new List<Comment>();

        for (int i = 1; i <= n; i++)
        {
            var comment = new Comment
            {
                Id = i,
                Text = $"Comment {i}",
                AppUserId = 1,
                PostId = 1
            };
            comments.Add(comment);
        }

        return comments;
    }

    public Comment GetDummyComment()
    {
        var comment = new Comment
        {
            Id = 1,
            Text = "Comment 1",
            AppUserId = 1,
            PostId = 1
        };

        return comment;
    }

    public CommentDTO GetDummyCommentDTO()
    {
        var commentDto = new CommentDTO
        {
            Text = "Comment 1",
            AppUserId = 1,
            PostId = 1
        };

        return commentDto;
    }
}