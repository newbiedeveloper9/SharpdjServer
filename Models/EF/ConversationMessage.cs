using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ConversationMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public User Author { get; set; }
    }
}
