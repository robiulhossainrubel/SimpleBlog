using System.Linq.Expressions;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repository;

        public PostService(IPostRepository repository)
        {
            _repository = repository;
        }

        public async Task Create(PostDTO postDTO)
        {
            try
            {
                var post = new Post
                {
                    Title = postDTO.Title,
                    Body = postDTO.Body,
                    AppUserId = postDTO.AppUserId
                };
                await _repository.Create(post);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Post Get(int id)
        {
            try
            {
                var post = _repository.GetPost(id);

                return post;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Post> GetAll(Expression<Func<Post, bool>>? expression = null)
        {
            try
            {
                var posts = _repository.GetAllPosts(expression);

                return posts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Pagination<Post> GetPaginate(int pageIndex, int pageSize)
        {
            try
            {
                var posts = _repository.GetPaginate(pageIndex, pageSize);

                return posts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Post> TopPosts()
        {
            try
            {
                var posts = _repository.TopPosts();

                return posts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Update(Post post)
        {
            try
            {
                await _repository.Update(post);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
