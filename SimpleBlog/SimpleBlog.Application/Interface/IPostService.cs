using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Application.Interface
{
    public interface IPostService
    {
        void Create(Post post);
        Post Get(int id);
        List<Post> GetAll();
    }
}
