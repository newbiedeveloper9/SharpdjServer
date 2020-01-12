using SCPackets.Models;
using Server.Management;
using Server.Management.Strategy;
using Server.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Network;
using NLog;
using SCPackets;
using Server.Management.Singleton;
using Server.Models.InstanceHelpers;

namespace Server
{
    [NotMapped]
    public class RoomInstance : Room
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public int AmountOfPeople => Users.Count;
        public int AmountOfAdministration => Users.GetList().Count(x => x.User.Rank > 0);

        public ITrackStrategy TrackStrategy { get; set; }
        public TrackModel CurrentTrack => Tracks.GetList().ElementAtOrDefault(0);

        public RoomHelper ActionHelper { get; }
        public ListWrapper<TrackModel> Tracks { get; set; }
        public ListWrapper<ServerUserModel> Users { get; set; }


        public RoomInstance()
        {
            Tracks = new ListWrapper<TrackModel>();
            Users = new ListWrapper<ServerUserModel>();
            TrackStrategy = new TrackJustOnce();
            ActionHelper = new RoomHelper(Users.GetList());

            TimeLeftReached += TimeReachedZero;
            Users.AfterUpdate += UsersOnAfterUpdate;
        }

        /// <summary>
        /// <para>It's called only after room data is updated</para>
        /// <para>Using buffer system</para>
        /// </summary>
        public void SendUpdateRequest()
        {
            var squareBuffer = BufferSingleton.Instance.SquareRoomBufferManager.GetRequest();
            squareBuffer.UpdatedRooms.Add(ToRoomOutsideModel());
        }


        private void UsersOnAfterUpdate(object sender, ListWrapper<ServerUserModel>.UpdateEventArgs e)
        {
            var clientUser = e.Item.User.ToUserClient();
            var buffer = BufferSingleton.Instance.RoomUserListBufferManager.GetByRoomId(Id);
            if (buffer == null)
                Logger.Debug($"ROOM ID: [{Id}]| Buffer cannot find room by id");

            if (e.State == ListWrapper<ServerUserModel>.UpdateEventArgs.UpdateState.Remove)
                buffer.RequestPacket.RemoveUsers.Add(clientUser);
            else
                buffer.RequestPacket.InsertUsers.Add(clientUser);

            SendUpdateRequest();
        }

        private int _timeLeft;
        public int TimeLeft
        {
            get => _timeLeft;
            set
            {
                _timeLeft = value;
                if (value == 0)
                    OnTimeLeftReached(EventArgs.Empty);
            }
        }

        public void PlayMedia()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                TimeLeft--;
            });
        }

        private void TimeReachedZero(object sender, EventArgs e)
        {
            TrackStrategy.NextTrack(Tracks.__nothing__);
            if (CurrentTrack != null)
                TimeLeft = CurrentTrack.Duration;

            BufferSingleton.Instance.SquareRoomBufferManager.GetRequest().UpdatedRooms.Add(ToRoomOutsideModel());
        }

        #region Events
        public event EventHandler TimeLeftReached;

        protected virtual void OnTimeLeftReached(EventArgs e)
        {
            EventHandler handler = TimeLeftReached;
            handler?.Invoke(this, e);
        }
        #endregion Events

        public RoomOutsideModel ToRoomOutsideModel()
        {
            var next = new TrackModel();
            var current = new TrackModel();
            var previous = new TrackModel();

            if (Tracks.Count > 1)
                next = Tracks.GetList().ElementAtOrDefault(1);

            return new RoomOutsideModel()
            {
                Id = Id,
                Name = Name,
                ImagePath = ImagePath,
                AmountOfPeople = AmountOfPeople,
                AmountOfAdministration = AmountOfAdministration,
                NextTrack = next,
                CurrentTrack = current,
                PreviousTrack = previous
            };
        }
    }
}
