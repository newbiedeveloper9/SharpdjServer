using SCPackets.Packets.Register;
using SharpDj.Common.Security;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Factory
{
    public class UserFactory : IUserFactory
    {
        public UserEntity GetUserEntity(RegisterRequest req)
        {
            string salt = Scrypt.GenerateSalt();
            return new UserEntity()
            {
                Email = req.Email,
                UserAuthEntity = new UserAuthEntity()
                {
                    Salt = salt,
                    Hash = Scrypt.Hash(req.Password, salt),
                    Login = req.Login,
                },
                Username = req.Username
            };
        }
    }
}