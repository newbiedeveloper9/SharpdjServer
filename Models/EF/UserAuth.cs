using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class UserAuth
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public string AuthenticationKey { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DataType(DataType.Date)]
        public DateTime? AuthenticationExpiration { get; set; }
        
    }

    public static class UserAuthHelper
    {
        public static void ClearAuthKey(this UserAuth auth, ServerContext context)
        {
            auth.AuthenticationKey = string.Empty;
            auth.AuthenticationExpiration = DateTime.Now.AddYears(-20);
            context.SaveChanges();
        }
    }
}