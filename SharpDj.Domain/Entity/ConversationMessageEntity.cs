namespace SharpDj.Domain.Entity
{
    public class ConversationMessageEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public UserEntity Author { get; set; }
    }
}
