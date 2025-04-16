namespace SimpleBlog.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
        public int LikeDisLikeId { get; set; }
        public LikeDisLike LikeDisLike { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<Comment> Comment { get; set; }
    }
}
