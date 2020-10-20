using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
                Log.Information("Checking database health...");
                if (CheckDatabaseHealth() == false)
                    return;
                Log.Information("Initializing rooms...");
                InitializeRooms();
                Log.Information("Set up events...");
                RoomSingleton.Instance.RoomInstances.AfterAdd += RoomAfterCreationNewRoom;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "An error occurred while trying to setup server...");
                throw;
            }
        }

        private void RoomAfterCreationNewRoom(object sender, ListWrapper<RoomEntityInstance>.AfterAddEventArgs e)
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
