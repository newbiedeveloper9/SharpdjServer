using System;
using System.Collections.Generic;
using System.Text;
using SharpDj.Common.DTO;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class RoomMapper : IDualMapper<RoomEntity, RoomDetailsDTO, RoomMapper.RoomMapperBag>
    {
        private readonly RoomConfigMapper _roomConfigMapper;

        public RoomMapper(RoomConfigMapper roomConfigMapper)
        {
            _roomConfigMapper = roomConfigMapper;
        }

        public RoomDetailsDTO MapToDTO(RoomEntity entity, RoomMapperBag bag=null)
        {
            return new RoomDetailsDTO()
            {
                Id = entity.Id,
                Name = entity.Name,
                ImageUrl = entity.ImagePath,
                RoomConfigDTO = _roomConfigMapper.MapToDTO(entity.ConfigEntity)
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

        public class RoomMapperBag
        {
            public UserEntity Author { get; set; }

            public RoomMapperBag(UserEntity author)
            {
                Author = author;
            }
        }
    }
}
