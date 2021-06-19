using SCPackets.Packets.Register;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Factory
{
    public interface IUserFactory
    {
        UserEntity CreateUserEntity(RegisterRequest req);
    }
}