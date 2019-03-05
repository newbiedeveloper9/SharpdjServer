using System;
using Communication.Shared.Data;
using Server.Models;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            /*var server = new ServerManagment.Server();
            server.Start();*/
            var userAuth = new UserAuth()
            {
                Hash = "Hash",
                Login = "Login",
                Salt = "Salt",
            };
                

            var user = new User()
            {
                Email = "asd@gmail.com",
                Rank = Rank.Admin,
                Username = "Crisey",
                UserAuth = userAuth,
            };
           var context = new ServerContext();
            context.Users.Add(user);
            context.SaveChanges();
            foreach (var contextUser in context.Users)
            {
                Console.WriteLine(contextUser.Username);
                Console.WriteLine(contextUser.UserAuth.Id);
            }
            foreach (var contextUserAuth in context.UserAuths)
            {
                Console.WriteLine(contextUserAuth.Hash);
                Console.WriteLine(contextUserAuth.Id);
                Console.WriteLine("---------------");
            }
            context.SaveChanges();

            Console.ReadLine();
        }
    }
}