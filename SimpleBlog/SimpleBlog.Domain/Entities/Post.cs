namespace SimpleBlog.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Status PostStatus { get; set; } = Status.Pending;
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<Comment> Comment { get; set; }
        public List<Reaction> Reaction { get; set; }
    }
}
