using Network;
using SCPackets.Models;
using SCPackets.Packets.Login;
using Serilog;
using SharpDj.Common.Security;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Mapper;
using SharpDj.Domain.Repository;
using SharpDj.Server.Application.Dictionaries;
using SharpDj.Server.Application.Dictionaries.Bags;
using SharpDj.Server.Application.Management;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Singleton;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDj.Server.Application.Handlers.Base;

namespace SharpDj.Server.Application.Handlers.Authentication
{
    public class ServerLoginAction : RequestHandler<LoginRequest>
    {
        private readonly IRoomMapper _roomMapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;

        public ServerLoginAction(IDictionaryConverter<IActionBag> bagConverter,
            IRoomMapper roomMapper,
            IUserRepository userRepository, 
            IRoomRepository roomRepository)
            : base(bagConverter)
        {
            _roomMapper = roomMapper;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        //check if user is logged in already
        protected override async Task Action(LoginRequest req, Connection conn, List<IActionBag> actionBags)
        {
            var userEntity = await _userRepository.GetUserByLoginOrEmailAsync(req.Login, req.Login);
            await Validate(req, conn, userEntity);

            AddUserConnectionToServer(userEntity, conn);

            var response = new LoginResponse(LoginResult.Success, req);
            response.Data = FillData(userEntity);

            if (req.RememberMe)
            {
                var authKey = await GenerateNewAuthKey(userEntity.UserAuthEntity);
                response.AuthenticationKey = authKey;
            }

            await conn.SendAsync<LoginResponse>(response);
            Log.Information("Success login: {@UserEntity}", userEntity.ToString());
        }

        //todo: move to special validation class for each handler
        private async Task Validate(LoginRequest req, Connection conn, UserEntity userEntity)
        {
            var validation = new DictionaryConditionsValidation<LoginResult>();

            var hashedPass = Scrypt.Hash(req.Password, userEntity?.UserAuthEntity?.Salt);

            validation.Conditions = new Dictionary<LoginResult, bool>()
            {
                { LoginResult.Error, userEntity == null }, // TODO may help crackers, cause of different keys, the whole Validation system is to refactor
                { LoginResult.CredentialsError, userEntity?.UserAuthEntity?.Hash != hashedPass },
            };

            var result = validation.AnyError();
            if (result != null)
            {
                Log.Information("Register validation failed. @Result", result);
                await conn.SendAsync<LoginResponse>(new LoginResponse((LoginResult)result, req));
            }
        }

        //todo: remove it soon
        private void AddUserConnectionToServer(UserEntity user, Connection conn)
        {
            var userIsActive = ClientSingleton.Instance.Users.FirstOrDefault(x => x.UserEntity.Id == user.Id);
            if (userIsActive != null)
            {
                ClientSingleton.Instance.Users
                    .FirstOrDefault(x => x.UserEntity.Id == user.Id)
                    .Connections.Add(conn);
            }
            else
            {
                ClientSingleton.Instance.Users.Add(new ServerUserModel(user, conn));
            }
        }

        private PreviewLogin FillData(UserEntity userEntity)
        {
            var data = new PreviewLogin();

            foreach (var roomModel in RoomSingleton.Instance.RoomInstances.ToReadonlyList())
            {
                data.RoomOutsideModelList.Add(roomModel.ToRoomOutsideModel());
            }

            var userRooms = _roomRepository.GetRoomByCreatorId(userEntity.Id);
            foreach (var room in userRooms)
            {
                var roomDto = _roomMapper.MapToDto(room);
                data.UserRoomList.Add(roomDto);
            }

            data.User = userEntity.ToUserClient();
            return data;
        }

        //todo: will be removed with new JWT auth system
        private async Task<string> GenerateNewAuthKey(UserAuthEntity userAuthEntity)
        {
            var authKey = Scrypt.GenerateSalt();
            userAuthEntity.AuthenticationKey = authKey;
            userAuthEntity.AuthenticationExpiration = DateTime.Now.AddDays(30);
            await _userRepository.UnitOfWork.SaveChangesAsync();

            return authKey;
        }
    }
}