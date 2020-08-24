namespace SharpDj.Domain.Entity
{
    public class ConversationMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public User Author { get; set; }
    }
}
