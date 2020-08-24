using System;
using System.Collections.Generic;
using System.Text;
using SharpDj.Common.DTO;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class RoomMapperService : IDualMapper<Room, RoomDetailsDTO, RoomMapperService.RoomMapperBag>
    {
        private readonly RoomConfigIdtoConverterService _roomConfigIdtoConverterService;

        public RoomMapperService(RoomConfigIdtoConverterService roomConfigIdtoConverterService)
        {
            _roomConfigIdtoConverterService = roomConfigIdtoConverterService;
        }

        public RoomDetailsDTO MapToDTO(Room entity, RoomMapperBag bag=null)
        {
            return new RoomDetailsDTO()
            {
                Id = entity.Id,
                Name = entity.Name,
                ImageUrl = entity.ImagePath,
                RoomConfigDTO = _roomConfigIdtoConverterService.MapToDTO(entity.Config)
            };
        }

        public Room MapToEntity(RoomDetailsDTO dto, RoomMapperBag bag)
        {
            return new Room()
            {
                Id = dto.Id,
                Name = dto.Name,
                Author = bag.Author,
                ImagePath = dto.ImageUrl,
                Config = _roomConfigIdtoConverterService.MapToEntity(dto.RoomConfigDTO)
            };
        }

        public class RoomMapperBag
        {
            public User Author { get; set; }

            public RoomMapperBag(User author)
            {
                Author = author;
            }
        }
    }
}
