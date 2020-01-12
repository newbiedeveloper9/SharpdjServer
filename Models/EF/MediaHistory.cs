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
        public int Id { get; set; }
        public MediaType MediaType { get; set; }
        public string Url { get; set; }
    }

    public enum MediaType
    {
        SoundCloud,
        Youtube
    }
}
