using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpDj.Domain.Entity
{
    public class UserConnectionEntity
    {
        public int Id { get; set; }
        public UserEntity UserEntity { get; set; }
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
