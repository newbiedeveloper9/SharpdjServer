using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class MediaHistory
    {
        [Key, Column("MediaHistoryId")]
        public int Id { get; set; }

        public Room Room { get; set; }
    }
}
