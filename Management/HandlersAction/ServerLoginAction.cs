﻿using Network;
using SCPackets.LoginPacket;
using Server.Models;
using Server.Security;
using System;
using System.Data.Entity;
using System.Linq;

namespace Server.Management.HandlersAction
{
    public class ServerLoginAction
    {
        private readonly ServerContext _context;

        public ServerLoginAction(ServerContext context)
        {
            _context = context;
        }

        public void Request(LoginRequest req, Connection conn)
        {
            try
            {
                User user = _context.Users.Include(x=>x.UserAuth).
                    FirstOrDefault(x => (x.UserAuth.Login.Equals(req.Login)) || (x.Email.Equals(req.Login)));
                if (user == null)
                {
                    ConnectionExtension.SendPacket(conn, new LoginResponse(Result.Error, req), this);
                    return;
                }

                string hashedPass = Scrypt.Hash(req.Password, user.UserAuth.Salt);
                if (user.UserAuth.Hash.Equals(hashedPass))
                {
                    ClientSingleton.Instance.Users.Add(user);
                    ConnectionExtension.SendPacket(conn, new LoginResponse(Result.Success, req), this);
                    Console.WriteLine("Login: {0}", user);
                }
                else
                {
                    ConnectionExtension.SendPacket(conn, new LoginResponse(Result.Error, req), this);
                }
            }
            catch (Exception e)
            {
                ConnectionExtension.SendPacket(conn, new LoginResponse(Result.Error, req), this);
                Console.WriteLine(e.Message);
            }
        }
    }
}