using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.ViewModel
{
    public class PostVM
    {
        public Post Post { get; set; }
        public Comment Comment { get; set; } = new Comment();
    }
}
