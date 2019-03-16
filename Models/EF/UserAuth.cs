using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class UserAuth
    {
        [Key, Column("UserAuthId")]
        public int Id { get; set; }
        public string Login { get; set; }

        public string Hash { get; set; }
        public string Salt { get; set; }
        
    }
}