using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Test.Blog.ReactionServiceTests;

[ExcludeFromCodeCoverage]
public class ReactionServiceBaseTest
{
    protected readonly IReactionRepository _reactionRepositoryMock;
    protected readonly IReactionService _sut;

    public ReactionServiceBaseTest()
    {
        _reactionRepositoryMock = Substitute.For<IReactionRepository>();
        _sut = new ReactionService(_reactionRepositoryMock);
    }

    public List<Reaction> GetDummyReactions(int n)
    {
        var reactions = new List<Reaction>();

        for (int i = 1; i <= n; i++)
        {
            var reaction = new Reaction
            {
                Id = i,
                ReactType = ReactionType.Like,
                AppUserId = 1,
                PostId = 1
            };
            reactions.Add(reaction);
        }

        return reactions;
    }

    public Reaction GetDummyReaction(int id)
    {
        var reaction = new Reaction
        {
            Id = id,
            ReactType = ReactionType.Like,
            AppUserId = 1,
            PostId = 1
        };

        return reaction;
    }
}