namespace SimpleBlog.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
