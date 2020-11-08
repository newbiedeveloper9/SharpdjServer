using SharpDj.Common.DTO;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Interfaces;

namespace SharpDj.Domain.Mapper
{
    public interface IRoomMapper : IDualMapper<RoomEntity, RoomDetailsDTO, RoomMapperBag>
    {
        RoomDetailsDTO MapToDto(RoomEntity entity, RoomMapperBag bag = null);
    }
}