using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpDj.Domain.Entity
{
    public class UserAuditEntity
    {
        public long Id { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public ConnectionType ConnectionType { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public UserEntity User { get; set; }
        public long UserId { get; set; }
    }

    public enum ConnectionType
    {
        Desktop,
        Web,
        Mobile
    }
}
