using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Presentation.ViewModel
{
    public class PostVM
    {
        public int? UserId { get; set; }
        public Post Post { get; set; }
        public Comment Comment { get; set; } = new Comment();
    }
}
