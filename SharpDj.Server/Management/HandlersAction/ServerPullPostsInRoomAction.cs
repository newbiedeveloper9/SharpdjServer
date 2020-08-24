﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Network;
using SCPackets.Packets.PullRoomChat;
using SharpDj.Domain.Mapper;
using SharpDj.Infrastructure;
using SharpDj.Server.Singleton;
using Log = Serilog.Log;

namespace SharpDj.Server.Management.HandlersAction
{
    public class ServerPullPostsInRoomAction : ActionAbstract<PullRoomChatRequest>
    {
        private readonly ServerContext _context;
        private readonly ChatMessageMapperService _chatMessageMapperService;

        public ServerPullPostsInRoomAction(ServerContext context, ChatMessageMapperService chatMessageMapperService)
        {
            _context = context;
            _chatMessageMapperService = chatMessageMapperService;
        }

        public override async Task Action(PullRoomChatRequest chatRequest, Connection conn)
        {
            var ext = new ConnectionExtension(conn, this);
            try
            {
                var active = ConnectionExtension.GetClient(conn);
                if (ext.SendRequestOrIsNull(active)) return;

                var roomInstance = RoomSingleton.Instance.RoomInstances
                    .GetList()
                    .FirstOrDefault(x => x.Id == active.ActiveRoom.RoomId);
                if (roomInstance == null)
                {
                    Log.Information("Room with given id doesn't exist");
                    conn.Send(new PullRoomChatResponse(PullRoomChatResult.Error));
                    return;
                }

                var room = _context.Rooms
                    .Include(x => x.Posts)
                    .FirstOrDefaultAsync(x => x.Id == chatRequest.RoomId)
                    .Result;
                if (room == null)
                {
                    Log.Error("Room with id{@RoomId} not found.", chatRequest.RoomId);
                    conn.Send(new PullRoomChatResponse(PullRoomChatResult.Error));
                    return;
                }

                //Get 50 newer posts
                var postsServer = room.Posts
                    .Reverse()
                    .Skip(chatRequest.MessageCount)
                    .Take(50)
                    .Reverse();

                var roomChatPosts = postsServer.ToList();
                if (!roomChatPosts.Any())
                {
                    Log.Information("There is no more posts in roomDetails");
                    conn.Send(new PullRoomChatResponse(PullRoomChatResult.EOF));
                    return;
                }

                var response = new PullRoomChatResponse(PullRoomChatResult.Success);
                response.Posts = roomChatPosts
                    .Select(x => _chatMessageMapperService.MapToDTO(x))
                    .ToList();

                conn.Send(response);
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                conn.Send(new PullRoomChatResponse(PullRoomChatResult.Error));
            }
        }
    }
}