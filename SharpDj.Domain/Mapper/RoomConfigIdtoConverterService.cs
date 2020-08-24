using System;
using System.Collections.Generic;
using System.Text;
using SharpDj.Common.DTO;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class RoomConfigIdtoConverterService : IDualMapper<RoomConfig, RoomConfigDTO>
    {
        public RoomConfigDTO MapToDTO(RoomConfig roomConfig)
        {
            return new RoomConfigDTO()
            {
                PublicEnterMessage = roomConfig.PublicEnterMessage,
                PublicLeaveMessage = roomConfig.PublicLeaveMessage,
                LocalEnterMessage = roomConfig.LocalEnterMessage,
                LocalLeaveMessage = roomConfig.LocalLeaveMessage
            };
        }

        public RoomConfig MapToEntity(RoomConfigDTO roomConfigDTO)
        {
            return new RoomConfig()
            {
                PublicEnterMessage = roomConfigDTO.PublicEnterMessage,
                PublicLeaveMessage = roomConfigDTO.PublicLeaveMessage,
                LocalEnterMessage = roomConfigDTO.LocalEnterMessage,
                LocalLeaveMessage = roomConfigDTO.LocalLeaveMessage
            };
        }
    }
}
