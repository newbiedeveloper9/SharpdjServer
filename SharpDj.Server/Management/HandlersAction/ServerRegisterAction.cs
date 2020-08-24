﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets;
using SCPackets.Packets.Register;
using SharpDj.Common;
using SharpDj.Domain.Entity;
using SharpDj.Infrastructure;
using SharpDj.Server.Security;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    class ServerRegisterAction : ActionAbstract<RegisterRequest>
    {
        private readonly ServerContext _context;

        public ServerRegisterAction(ServerContext context)
        {
            _context = context;
        }

        public override async Task Action(RegisterRequest req, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);

            try
            {
                if (!Validate(req, ext))
                    return;

                string salt = Scrypt.GenerateSalt();
                var user = new User()
                {
                    Email = req.Email,
                    UserAuth = new UserAuth()
                    {
                        Salt = salt,
                        Hash = Scrypt.Hash(req.Password, salt),
                        Login = req.Login,
                    },
                    Username = req.Username
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                ext.SendPacket(new RegisterResponse(RegisterResult.Success, req));
                Log.Information("Success register: {@User}", user.ToString());
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ext.SendPacket(new RegisterResponse(RegisterResult.Error, req));
            }
        }

        private bool Validate(RegisterRequest req, ConnectionExtension ext)
        {
            var validation = new DictionaryConditionsValidation<RegisterResult>();
            validation.Conditions = new Dictionary<RegisterResult, bool>()
            {
                {RegisterResult.PasswordError, (req.Password.Length < 6 || req.Password.Length > 48)},
                {RegisterResult.EmailError, !DataValidation.EmailIsValid(req.Email)},
                {RegisterResult.LoginError, !DataValidation.LengthIsValid(req.Login, 2, 32)},
                {RegisterResult.UsernameError, !DataValidation.LengthIsValid(req.Username, 2, 32)},
                {RegisterResult.AlreadyExist, AccountExist(req.Login, req.Email)}
            };

            var result = validation.AnyError();
            if (result != null)
            {
                Log.Information("Register validation failed. @Result", result);
                ext.SendPacket(new RegisterResponse((RegisterResult)result, req));
                return false;
            }

            return true;
        }

        private bool AccountExist(string login, string email)
        {
            return _context.Users.Include(x => x.UserAuth)
                        .Any(x => x.UserAuth.Login.Equals(login)) ||
                    _context.Users.Any(x => x.Email.Equals(email));
        }

    }
}
