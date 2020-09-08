using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpDj.Domain.Entity
{
    public class LogEntity
    {
        public int Id { get; set; }
        public LogType Type { get; set; }
        public UserEntity UserEntity { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public ConnectionType ConnectionType { get; set; }
    }

    public enum LogType
    {
        Register,
        Login,
        JoinedRoom,
    }
}
