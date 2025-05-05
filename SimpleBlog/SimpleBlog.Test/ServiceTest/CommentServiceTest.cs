using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Services;

namespace SimpleBlog.Test.ServiceTest
{
    public class CommentServiceTest
    {
        private readonly CommentService _sut;
        private readonly ICommentRepository _commentRepositoryMock;
        public CommentServiceTest()
        {
            _commentRepositoryMock = Substitute.For<ICommentRepository>();
            _sut = new CommentService(_commentRepositoryMock);
        }

        #region GetAll Tests
        [Fact]
        public void GetAll_RepoReturnsValidList_ReturnsAllComment()
        {
            // Arrange
            var comments = new List<Comment>
            {
                new Comment
                {
                    Id = 1,
                    Text = "Comment 1",
                    AppUserId = 1,
                    PostId = 1
                },
                new Comment
                {
                    Id = 2,
                    Text = "Comment 2",
                    AppUserId = 1,
                    PostId = 1
                },
                new Comment
                {
                    Id = 3,
                    Text = "Comment 3",
                    AppUserId = 1,
                    PostId = 1
                }
            };
            _commentRepositoryMock.GetAll(includeProperties: Arg.Any<string>()).Returns(comments);

            // Act
            var result = _sut.GetAll();

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void GetAll_ThrowException_ForError()
        {
            // Arrange
            _commentRepositoryMock.GetAll(includeProperties: Arg.Any<string>()).Throws(new Exception());

            // Act & Assert
            Assert.Throws<Exception>(() => _sut.GetAll());
        }
        #endregion

        #region Create Tests
        [Fact]
        public async Task Create_CallsRepository_CreateSuccessfull()
        {
            // Arrange
            var comment = new Comment
            {
                //Id = 1,
                Text = "Comment 1",
                AppUserId = 1,
                PostId = 1
            };
            var commentDto = new CommentDTO
            {
                Text = "Comment 1",
                AppUserId = 1,
                PostId = 1
            };

            // Act
            await _sut.Create(commentDto);

            // Assert
            //await _commentRepositoryMock.Create(comment);
            await _commentRepositoryMock.Received(1).Create(Arg.Any<Comment>());
        }
        [Fact]
        public async Task Create_CallsRepository_ReturnException()
        {
            // Arrange
            var commentDto = new CommentDTO
            {
                Text = "Comment 1",
                AppUserId = 1,
                PostId = 1
            };

            _commentRepositoryMock.Create(Arg.Any<Comment>()).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _sut.Create(commentDto));
        }
        #endregion
    }
}