/*using System;
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
using SharpDj.Server.Application.Commands.Extensions;
using SharpDj.Server.Application.Management;

namespace SharpDj.Server.Application.Commands.Handlers.Authentication
{
    public class ServerRegisterAction : AbstractHandler,
        IPacketRegister<RegisterRequest>
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

        public IHandler BuildPipeline =>
            new BasicIncludeHandler(BagConverter).SetNext(
                new BlockLoggedUserHandler(BagConverter));

        public async Task ProcessRequest(RegisterRequest req, Connection conn, IList<IActionBag> actionBags)
        {
            if (await Validate(req, conn) == false)
            {
                await base.Handle(req, actionBags)
                    .ConfigureAwait(false);
            }

            var newUser = _userFactory.CreateUserEntity(req);
            _userRepository.AddUser(newUser);
            await _userRepository.UnitOfWork.SaveChangesAsync();

            conn.Send(new RegisterResponse(RegisterResult.Success, req));

            Log.Information("Success register: {@UserEntity}", newUser.ToString());

            await base.Handle(req, actionBags)
                .ConfigureAwait(false);
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
                {RegisterResult.AlreadyExist, await _userRepository.GivenLoginOrEmailExistsAsync(req.Login, req.Email)}
            };


            var result = validation.AnyError();
            if (result != null)
            {
                Log.Information("Register validation failed. {@Result}", result);
                conn.Send(new RegisterResponse((RegisterResult)result, req));
                return false;
            }

            return true;
        }
    }
}
*/