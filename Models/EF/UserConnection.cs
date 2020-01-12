using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class UserConnection
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public ConnectionType ConnectionType { get; set; } = ConnectionType.Desktop;
    }

    public enum ConnectionType
    {
        Desktop,
        Web,
        Mobile
    }
}
