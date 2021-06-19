using Microsoft.EntityFrameworkCore;
using Serilog;
using SharpDj.Common.ListWrapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Singleton;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SharpDj.Server.Application
{
    public class Setup
    {
        private readonly ServerContext _context;

        public Setup(ServerContext context)
        {
            _context = context;
        }

        public async Task Start()
        {
            try
            {
                Log.Information("Checking database health...");
                await EnsureDatabaseCreated()
                    .ConfigureAwait(false);
                await DatabaseHealthCommand()
                    .ConfigureAwait(false);

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

        private async Task EnsureDatabaseCreated()
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Log.Information("Applying {@Count} migrations...", pendingMigrations.Count());
                await _context.Database.MigrateAsync()
                    .ConfigureAwait(false);
            }
        }

        private async Task DatabaseHealthCommand()
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1")
                .ConfigureAwait(false);
        }
    }
}
