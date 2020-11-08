using System;
using Serilog;
using SharpDj.Common;
using SharpDj.Common.ListWrapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Singleton;

namespace SharpDj.Server.Application.Management
{
    public class Setup
    {
        private readonly ServerContext _context;

        public Setup(ServerContext context)
        {
            _context = context;

            try
            {
                Log.Information("Checking database health...");
                if (CheckDatabaseHealth() == false)
                    return;
                Log.Information("Initializing rooms...");
                InitializeRooms();
                Log.Information("Set up events...");
                RoomSingleton.Instance.RoomInstances.AfterAdd += RoomInstancesOnAfterAdd;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "An error occurred while trying to setup server...");
                throw;
            }
        }

        private void RoomInstancesOnAfterAdd(object sender, AfterAddEventArgs<RoomEntityInstance> e)
        {
            BufferSingleton.Instance.RoomUserListBufferManager.CreateBuffer(e.Item.Id);

            var squareRoomBuffer = BufferSingleton.Instance.SquareRoomBufferManager.GetRequest();
            squareRoomBuffer.InsertRooms.Add(e.Item.ToRoomOutsideModel());
        }

        private void InitializeRooms()
        {
            foreach (var room in _context.Rooms)
            {
                RoomSingleton.Instance.RoomInstances.Add((RoomEntityInstance)room);
                BufferSingleton.Instance.RoomUserListBufferManager.CreateBuffer(room.Id);
            }
        }

        private bool CheckDatabaseHealth()
        {
            return _context.Database.CanConnect();
        }
    }
}
