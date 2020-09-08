using System;
using System.Collections.Generic;
using System.Text;
using SharpDj.Common.DTO;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class RoomMapperService : IDualMapper<RoomEntity, RoomDetailsDTO, RoomMapperService.RoomMapperBag>
    {
        private readonly RoomConfigIdtoConverterService _roomConfigIdtoConverterService;

        public RoomMapperService(RoomConfigIdtoConverterService roomConfigIdtoConverterService)
        {
            _roomConfigIdtoConverterService = roomConfigIdtoConverterService;
        }

        public RoomDetailsDTO MapToDTO(RoomEntity entity, RoomMapperBag bag=null)
        {
            return new RoomDetailsDTO()
            {
                Id = entity.Id,
                Name = entity.Name,
                ImageUrl = entity.ImagePath,
                RoomConfigDTO = _roomConfigIdtoConverterService.MapToDTO(entity.ConfigEntity)
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
                ConfigEntity = _roomConfigIdtoConverterService.MapToEntity(dto.RoomConfigDTO)
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
