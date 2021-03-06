﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Repository;
using SharpDj.Domain.SeedWork;

namespace SharpDj.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ServerContext _serverContext;
        public IUnitOfWork UnitOfWork => _serverContext;

        public RoomRepository(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        public async Task<RoomEntity> GetRoomByIdAsync(int roomId)
        {
            return await _serverContext.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == roomId);
        }

        public IEnumerable<RoomEntity> GetRoomByCreatorId(long userId)
        {
            return _serverContext.Rooms
                .AsNoTracking()
                .Include(x => x.Config)
                .Where(x => x.Author.Id == userId);
        }

        public async Task UpdateRoom(RoomEntity room)
        {
            var entity = await _serverContext.Rooms.FindAsync(room.Id);
            _serverContext.Entry(entity).CurrentValues.SetValues(room);
        }

        public bool AnyRoomContainsName(string name)
        {
            return _serverContext.Rooms.Any(x => x.Name == name);
        }
    }
}