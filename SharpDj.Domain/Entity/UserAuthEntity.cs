using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpDj.Domain.Entity
{
    public class UserAuthEntity
    {
        public ulong Id { get; set; }
        public string Login { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public string AuthenticationKey { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DataType(DataType.Date)]
        public DateTime? AuthenticationExpiration { get; set; }
        
    }
}