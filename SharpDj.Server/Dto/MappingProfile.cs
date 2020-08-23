using AutoMapper;
using SharpDj.Server.Entity;
using SharpDj.Server.Models;

namespace SharpDj.Server.Dto
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Room, RoomInstance>();
        }
    }
}
