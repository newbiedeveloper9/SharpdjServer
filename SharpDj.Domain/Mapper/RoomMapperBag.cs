using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class RoomMapperBag
    {
        public UserEntity Author { get; }

        public RoomMapperBag(UserEntity author)
        {
            Author = author;
        }
    }
}
