using SharpDj.Common.DTO;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class RoomMapper : IRoomMapper
    {
        private readonly IRoomConfigMapper _roomConfigMapper;

        public RoomMapper(IRoomConfigMapper roomConfigMapper)
        {
            _roomConfigMapper = roomConfigMapper;
        }

        public RoomDetailsDTO MapToDto(RoomEntity entity, RoomMapperBag bag = null)
        {
            return new RoomDetailsDTO()
            {
                Id = entity.Id,
                Name = entity.Name,
                ImageUrl = entity.ImagePath,
                RoomConfigDTO = _roomConfigMapper.MapToDto(entity.ConfigEntity)
            };
        }

        public RoomEntity MapToEntity(RoomDetailsDTO dto, RoomMapperBag bag)
        {
            return new RoomEntity()
            {
                Id = dto.Id,
                Name = dto.Name,
                Author = bag.Author,
                ImagePath = dto.ImageUrl,
                ConfigEntity = _roomConfigMapper.MapToEntity(dto.RoomConfigDTO)
            };
        }
    }
}
