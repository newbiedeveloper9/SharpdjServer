using System;
using SCPackets;
using SharpDj.Common;
using SharpDj.Domain.Mapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Application
{
    public class Setup
    {
        private readonly ServerContext _context;

        public Setup(ServerContext context)
        {
            _context = context;

            try
            {
                Log.Information("Initializing rooms...");
                InitializeRooms();
                RoomSingleton.Instance.RoomInstances.AfterAdd += RoomAfterCreationNewRoom;
                
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace, "An error occurred during setup");
            }
        }

        private void RoomAfterCreationNewRoom(object sender, ListWrapper<RoomInstance>.AfterAddEventArgs e)
        {
            BufferSingleton.Instance.RoomUserListBufferManager.CreateBuffer(e.Item.Id);

            var squareRoomBuffer = BufferSingleton.Instance.SquareRoomBufferManager.GetRequest();
            squareRoomBuffer.InsertRooms.Add(e.Item.ToRoomOutsideModel());
        }

        private void InitializeRooms()
        {
            foreach (var room in _context.Rooms)
            {
                RoomSingleton.Instance.RoomInstances.Add((RoomInstance)room);
                BufferSingleton.Instance.RoomUserListBufferManager.CreateBuffer(room.Id);
            }
        }
    }
}
