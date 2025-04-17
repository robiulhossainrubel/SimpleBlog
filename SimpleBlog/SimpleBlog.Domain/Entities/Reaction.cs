namespace SimpleBlog.Domain.Entities
{
    public class Reaction
    {
        public int Id { get; set; }
        public ReactionType ReactType { get; set; }
        public int AppUserId { get; set; }
        public int PostId { get; set; }
    }
}
