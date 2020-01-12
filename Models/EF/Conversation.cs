using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public List<User> Users { get; set; }
        public List<ConversationMessage> Messages { get; set; }
        public string Title { get; set; }
    }
}
