using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.EF
{
    public class RoleClaim
    {
        [Key, Column("RoleClaimId")]
        public int Id { get; set; }
        public Claim Type { get; set; }
        public ServerRole Role { get; set; }
    }
}
