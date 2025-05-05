using System.Linq.Expressions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Test.ServiceTest
{
    public class ReactionServiceTest
    {
        private readonly IReactionRepository _reactionRepositoryMock;
        private readonly IReactionService _sut;
        public ReactionServiceTest()
        {
            _reactionRepositoryMock = Substitute.For<IReactionRepository>();
            _sut = new ReactionService(_reactionRepositoryMock);
        }

        #region GetAll Tests
        [Fact]
        public void GetAll_RepoReturnValidList_ReturnsAllReaction()
        {
            // Arrange
            var reactions = new List<Reaction>
            {
                new Reaction
                {
                    Id = 1,
                    ReactType = ReactionType.Like,
                    AppUserId = 1,
                    PostId = 1
                },
                new Reaction
                {
                    Id = 2,
                    ReactType = ReactionType.DisLike,
                    AppUserId = 2,
                    PostId = 1
                },
                new Reaction
                {
                    Id = 3,
                    ReactType = ReactionType.Like,
                    AppUserId = 3,
                    PostId = 1
                }
            };
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

        #region React Tests
        [Fact]
        public async Task React_Create_WhenNoReactionExist()
        {
            // Arrange
            const int postId = 1;
            const int userId = 1;
            const int reactId = (int)ReactionType.Like;

            var existingReaction = new Reaction
            {
                PostId = 1,
                AppUserId = 1,
                ReactType = ReactionType.Like
            };
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
            var existingReaction = new Reaction
            {
                PostId = 1,
                AppUserId = 1,
                ReactType = ReactionType.Like
            };
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
            var existingReaction = new Reaction
            {
                PostId = 1,
                AppUserId = 1,
                ReactType = ReactionType.Like
            };
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
            var existingReaction = new Reaction
            {
                PostId = 1,
                AppUserId = 1,
                ReactType = ReactionType.Like
            };
            _reactionRepositoryMock.Get(Arg.Any<Expression<Func<Reaction, bool>>>()).Returns(existingReaction);

            _reactionRepositoryMock.Update(Arg.Any<Reaction>()).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _sut.React(1, 5, 1));
        }

        [Fact]
        public async Task React_ThrowsException_ReThrowExceptionInDelete()
        {
            // Arrange
            var existingReaction = new Reaction
            {
                PostId = 1,
                AppUserId = 1,
                ReactType = ReactionType.Like
            };
            _reactionRepositoryMock.Get(Arg.Any<Expression<Func<Reaction, bool>>>()).Returns(existingReaction);

            _reactionRepositoryMock.Delete(Arg.Any<Reaction>()).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _sut.React(1, 10, 1));
        }
        #endregion
    }
}