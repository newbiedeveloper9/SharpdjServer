﻿using Network;
using SCPackets.Models;
using SCPackets.Packets.Login;
using Serilog;
using SharpDj.Common.Handlers.Base;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Common.Security;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Mapper;
using SharpDj.Domain.Repository;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Singleton;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpDj.Server.Application.Commands.Handlers.Authentication
{
    public class ServerLoginAction : IAction<LoginRequest>
    {
        private readonly IRoomMapper _roomMapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;

        public ServerLoginAction(IRoomMapper roomMapper, IUserRepository userRepository,
            IRoomRepository roomRepository)
        {
            _roomMapper = roomMapper;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        //check if user is logged in already
        public async Task Handle(LoginRequest req, Connection conn, IList<IActionBag> actionBags)
        {

            var userEntity = await _userRepository.GetUserByLoginOrEmailAsync(req.Login, req.Login);
            if (await Validate(req, conn, userEntity) == false)
            {
                return;
            }

            AddUserConnectionToServer(userEntity, conn);

            var response = new LoginResponse(LoginResult.Success, req)
            {
                Data = FillData(userEntity)
            };

            if (req.RememberMe)
            {
                var authKey = await GenerateNewAuthKey(userEntity.UserAuthEntity);
                response.AuthenticationKey = authKey;
            }

            conn.Send(response);
            Log.Information("Success login: {@UserEntity}", userEntity.ToString());
            
        }

        //todo: move to special validation class for each handler
        private async Task<bool> Validate(LoginRequest req, Connection conn, UserEntity userEntity)
        {
            if (userEntity != null)
            {
                var hashedPass = Scrypt.Hash(req.Password, userEntity?.UserAuthEntity?.Salt);
                if (hashedPass == userEntity.UserAuthEntity?.Hash)
                {
                    return true;
                }
            }

            Log.Information("Login validation failed. {@Result}", LoginResult.CredentialsError);
            conn.Send(new LoginResponse(LoginResult.CredentialsError, req));
            return false;
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