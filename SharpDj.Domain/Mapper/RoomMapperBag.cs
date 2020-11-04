using SharpDj.Common.DTO;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class RoomMapperBag
    {
        public UserEntity Author { get; set; }

        public RoomMapperBag(UserEntity author)
        {
            Author = author;
        }
    }
}
