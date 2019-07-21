using System;
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
        public string AuthenticationKey { get; set; }

        [DataType(DataType.Date)]
        public DateTime AuthenticationExpiration { get; set; }
        
    }

    public static class UserAuthHelper
    {
        public static void ClearAuthKey(this UserAuth auth, ServerContext _context)
        {
            auth.AuthenticationKey = string.Empty;
            auth.AuthenticationExpiration = DateTime.Now.AddYears(-10);
            _context.SaveChanges();
        }
    }
}