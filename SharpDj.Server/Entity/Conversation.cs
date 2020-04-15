using System.Collections.Generic;

namespace SharpDj.Server.Entity
{
    public class Conversation
    {
        public int Id { get; set; }
        public List<User> Users { get; set; }
        public List<ConversationMessage> Messages { get; set; }
        public string Title { get; set; }
    }
}
