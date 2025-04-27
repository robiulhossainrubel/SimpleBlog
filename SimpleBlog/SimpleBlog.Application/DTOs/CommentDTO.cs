namespace SimpleBlog.Application.DTOs
{
    public class CommentDTO
    {
        public int PostId { get; set; }
        public string Text { get; set; }
        public int AppUserId { get; set; }
    }
}
