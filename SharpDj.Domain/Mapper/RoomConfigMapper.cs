using SharpDj.Common.DTO;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class RoomConfigMapper : IRoomConfigMapper
    {
        public RoomConfigDTO MapToDto(RoomConfigEntity roomConfigEntity)
        {
            return new RoomConfigDTO()
            {
                PublicEnterMessage = roomConfigEntity.PublicEnterMessage,
                PublicLeaveMessage = roomConfigEntity.PublicLeaveMessage,
                LocalEnterMessage = roomConfigEntity.LocalEnterMessage,
                LocalLeaveMessage = roomConfigEntity.LocalLeaveMessage
            };
        }

        public RoomConfigEntity MapToEntity(RoomConfigDTO roomConfigDTO)
        {
            return new RoomConfigEntity()
            {
                PublicEnterMessage = roomConfigDTO.PublicEnterMessage,
                PublicLeaveMessage = roomConfigDTO.PublicLeaveMessage,
                LocalEnterMessage = roomConfigDTO.LocalEnterMessage,
                LocalLeaveMessage = roomConfigDTO.LocalLeaveMessage
            };
        }
    }
}
