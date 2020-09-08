using System.Collections.Generic;

namespace SharpDj.Domain.Entity
{
    public class ConversationEntity
    {
        public int Id { get; set; }
        public ICollection<UserEntity> Users { get; set; }
        public ICollection<ConversationMessageEntity> Messages { get; set; }
        public string Title { get; set; }
    }
}
