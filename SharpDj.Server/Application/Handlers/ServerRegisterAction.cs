using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using SCPackets.Packets.Register;
using Serilog;
using SharpDj.Common;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Repository;
using SharpDj.Server.Application.Bags;
using SharpDj.Server.Management;
using SharpDj.Server.Management.HandlersAction;
using SharpDj.Server.Security;

namespace SharpDj.Server.Application.Handlers
{
    public class ServerRegisterAction : RequestHandler<RegisterRequest>
    {
        private readonly IUserRepository _userRepository;

        public ServerRegisterAction(IDictionaryConverter<IActionBag> bagsConverter, IUserRepository userRepository)
            : base(bagsConverter)
        {
            _userRepository = userRepository;
        }

        protected override async Task Action(RegisterRequest req, Connection conn, List<IActionBag> actionBags)
        {
            var validate = await Validate(req, conn);
            if (!validate)
                return;

            string salt = Scrypt.GenerateSalt();
            var user = new UserEntity()
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
            _userRepository.AddUser(user);
            await _userRepository.UnitOfWork.SaveChangesAsync();

            await conn.SendAsync<RegisterResponse>(new RegisterResponse(RegisterResult.Success, req));
            Log.Information("Success register: {@UserEntity}", user.ToString());
        }

        private async Task<bool> Validate(RegisterRequest req, Connection conn)
        {
            var validation = new DictionaryConditionsValidation<RegisterResult>();
            validation.Conditions = new Dictionary<RegisterResult, bool>()
            {
                {RegisterResult.PasswordError, (req.Password.Length < 6 || req.Password.Length > 48)},
                {RegisterResult.EmailError, !DataValidation.EmailIsValid(req.Email)},
                {RegisterResult.LoginError, !DataValidation.LengthIsValid(req.Login, 2, 32)},
                {RegisterResult.UsernameError, !DataValidation.LengthIsValid(req.Username, 2, 32)},
                {RegisterResult.AlreadyExist, _userRepository.GivenLoginOrEmailExists(req.Login, req.Email)}
            };

            var result = validation.AnyError();
            if (result != null)
            {
                Log.Information("Register validation failed. @Result", result);
                await conn.SendAsync<RegisterResponse>(new RegisterResponse((RegisterResult)result, req));
                return false;
            }

            return true;
        }
    }
}
