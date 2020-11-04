﻿using System;
using System.Collections.Generic;
using System.Text;
using SharpDj.Common.DTO;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class RoomConfigMapper : IDualMapper<RoomConfigEntity, RoomConfigDTO>
    {
        public RoomConfigDTO MapToDTO(RoomConfigEntity roomConfigEntity)
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