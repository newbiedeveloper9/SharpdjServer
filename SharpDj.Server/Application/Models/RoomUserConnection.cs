namespace SharpDj.Server.Models
{
    public class RoomUserConnection
    {
        public RoomUserConnection(int roomId)
        {
            RoomId = roomId;
        }

        public int RoomId { get; set; }
    }
}