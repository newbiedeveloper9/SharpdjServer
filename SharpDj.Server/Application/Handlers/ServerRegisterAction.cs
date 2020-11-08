using Network;
using SCPackets.Packets.Register;
using Serilog;
using SharpDj.Common;
using SharpDj.Domain.Factory;
using SharpDj.Domain.Repository;
using SharpDj.Server.Application.Bags;
using SharpDj.Server.Management;
using SharpDj.Server.Management.HandlersAction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpDj.Server.Application.Handlers
{
    public class ServerRegisterAction : RequestHandler<RegisterRequest>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFactory _userFactory;

        public ServerRegisterAction(IDictionaryConverter<IActionBag> bagsConverter, IUserRepository userRepository, IUserFactory userFactory)
            : base(bagsConverter)
        {
            _userRepository = userRepository;
            _userFactory = userFactory;
        }

        protected override async Task Action(RegisterRequest req, Connection conn, List<IActionBag> actionBags)
        {
            var validate = await Validate(req, conn);
            if (!validate)
                return;

            var newUser = _userFactory.GetUserEntity(req);
            _userRepository.AddUser(newUser);
            await _userRepository.UnitOfWork.SaveChangesAsync();

            await conn.SendAsync<RegisterResponse>(new RegisterResponse(RegisterResult.Success, req));
            Log.Information("Success register: {@UserEntity}", newUser.ToString());
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
