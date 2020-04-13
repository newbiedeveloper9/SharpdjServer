using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using SCPackets;
using SCPackets.AuthKeyLogin;
using SCPackets.ConnectToRoom;
using SCPackets.CreateRoom;
using SCPackets.Disconnect;
using SCPackets.LoginPacket;
using SCPackets.PullPostsInRoom;
using SCPackets.RegisterPacket;
using SCPackets.SendRoomChatMessage;
using SCPackets.UpdateRoomData;
using SharpDj.Server.Management.HandlersAction;
using SharpDj.Server.Management.Singleton;
using SharpDj.Server.Models;
using SharpDj.Server.Models.EF;
using Log = Serilog.Log;


namespace SharpDj.Server.Management
{
    public class ServerPacketsToHandleList
    {
        private readonly ServerContext _context;
        private readonly List<IHandlerModel> _handlers = new List<IHandlerModel>();

        public ServerPacketsToHandleList(ServerContext context)
        {
            this._context = context;
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                //var user = _context.Users.FirstOrDefault(); //Will create entire EF structure at the beginning 
                //user = null;

                InitializeRooms();
                RoomSingleton.Instance.RoomInstances.AfterAdd += RoomAfterCreationNewRoom;

                var login = new ServerLoginAction(_context);
                var register = new ServerRegisterAction(_context);
                var createRoom = new ServerCreateRoomAction(_context);
                var updateRoom = new ServerUpdateRoomAction(_context);
                var disconnect = new ServerDisconnectAction(_context);
                var sendRoomChatMessage = new ServerSendRoomChatMessageAction(_context);
                var connectToRoom = new ServerConnectToRoomAction(_context);
                var authKeyLogin = new ServerAuthKeyLoginAction(_context);
                var pullPostsInRoom = new ServerPullPostsInRoomAction(_context);

                _handlers.Add(new HandlerModel<LoginRequest>(login.Action));
                _handlers.Add(new HandlerModel<RegisterRequest>(register.Action));
                _handlers.Add(new HandlerModel<CreateRoomRequest>(createRoom.Action));
                _handlers.Add(new HandlerModel<UpdateRoomDataRequest>(updateRoom.Action));
                _handlers.Add(new HandlerModel<DisconnectRequest>(disconnect.Action));
                _handlers.Add(new HandlerModel<SendRoomChatMessageRequest>(sendRoomChatMessage.Action));
                _handlers.Add(new HandlerModel<ConnectToRoomRequest>(connectToRoom.Action));
                _handlers.Add(new HandlerModel<AuthKeyLoginRequest>(authKeyLogin.Action));
                _handlers.Add(new HandlerModel<PullPostsInRoomRequest>(pullPostsInRoom.Action));
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
        }

        public void RegisterPackets(Connection conn)
        {
            _handlers.ForEach(x => x.RegisterPacket(conn));
        }

        private void RoomAfterCreationNewRoom(object sender, ListWrapper<RoomInstance>.AfterAddEventArgs e)
        {
            BufferSingleton.Instance.RoomUserListBufferManager.CreateBuffer(e.Item.Id);

            var squareRoomBuffer = BufferSingleton.Instance.SquareRoomBufferManager.GetRequest();
            squareRoomBuffer.InsertRooms.Add(e.Item.ToRoomOutsideModel());
        }

        private void InitializeRooms()
        {
            foreach (var room in _context.Rooms)
            {
                RoomSingleton.Instance.RoomInstances.Add(room.ToRoomInstance());
                BufferSingleton.Instance.RoomUserListBufferManager.CreateBuffer(room.Id);
            }
        }
    }
}
