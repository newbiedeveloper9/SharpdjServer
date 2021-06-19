using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using SCPackets.Packets.Register;
using Serilog;
using SharpDj.Common;
using SharpDj.Common.Handlers;
using SharpDj.Common.Handlers.Base;
using SharpDj.Common.Handlers.Dictionaries;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Domain.Factory;
using SharpDj.Domain.Repository;
using SharpDj.Server.Application.Handlers.Helpers;
using SharpDj.Server.Application.Management;

namespace SharpDj.Server.Application.Commands.Handlers.Authentication
{
    public class ServerRegisterAction : AbstractHandler,
        IAction<RegisterRequest>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFactory _userFactory;

        public ServerRegisterAction(IUserRepository userRepository, IUserFactory userFactory,
            IDictionaryConverter<IActionBag> bagConverter)
            : base(bagConverter)
        {
            _userRepository = userRepository;
            _userFactory = userFactory;
        }

        IHandler IAction<RegisterRequest>.BuildPipeline()
        {
            var include = new BasicIncludeHandler(BagConverter);
            var blockLoggedUser = new BlockLoggedUserHandler(BagConverter);

            include.SetNext(blockLoggedUser);

            return include;
        }

        public async Task ProcessRequest(RegisterRequest req, Connection conn, IList<IActionBag> actionBags)
        {
            if (await isValid(req, conn) == false)
            {
                return;
            }

            var newUser = _userFactory.CreateUserEntity(req);
            _userRepository.AddUser(newUser);
            await _userRepository.UnitOfWork.SaveChangesAsync();

            conn.Send(new RegisterResponse(RegisterResult.Success, req));

            Log.Information("Success register: {@UserEntity}", newUser.ToString());

            await base.Handle(req, actionBags)
                .ConfigureAwait(false);
        }

        private async Task<bool> isValid(RegisterRequest req, Connection conn)
        {
            var validation = new DictionaryConditionsValidation<RegisterResult>
            {
                Conditions = new Dictionary<RegisterResult, bool>()
                {
                    {RegisterResult.PasswordError, (req.Password.Length < 6 || req.Password.Length > 48)},
                    {RegisterResult.EmailError, !DataValidation.EmailIsValid(req.Email)},
                    {RegisterResult.LoginError, !DataValidation.LengthIsValid(req.Login, 2, 32)},
                    {RegisterResult.UsernameError, !DataValidation.LengthIsValid(req.Username, 2, 32)},
                    {RegisterResult.AlreadyExist, await _userRepository.GivenLoginOrEmailExistsAsync(req.Login, req.Email)}
                }
            };


            var result = validation.AnyError();
            if (result != null)
            {
                return false;
            }

            return true;
        }
    }
}