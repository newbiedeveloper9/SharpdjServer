namespace SharpDj.Domain.Entity
{
    public class RoomConfigEntity
    {
        public int Id { get; set; }

        public string PublicEnterMessage { get; set; }
        public string PublicLeaveMessage { get; set; }
        public string LocalEnterMessage { get; set; }
        public string LocalLeaveMessage { get; set; }
        
        public RoomEntity Room { get; set; }
        public int RoomId { get; set; }
    }
}