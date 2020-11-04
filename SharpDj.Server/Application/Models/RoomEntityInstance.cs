using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpDj.Common;
using SharpDj.Common.Enums;
using SharpDj.Domain.Entity;
using SharpDj.Server.Management.Strategy;
using SharpDj.Server.Singleton;
using SharpDj.Common.DTO;
using Log = Serilog.Log;

namespace SharpDj.Server.Models
{
    [NotMapped]
    public class RoomEntityInstance : RoomEntity
    {
        public int AmountOfPeople => Users.Count;
        public int AmountOfAdministration => Users.GetList().Count(x => x.UserEntity.Rank > 0);

        public ITrackStrategy TrackStrategy { get; set; }
        public TrackDTO CurrentTrack => Tracks.GetList().ElementAtOrDefault(0);

        public RoomHelper ActionHelper { get; }
        public ListWrapper<TrackDTO> Tracks { get; set; }
        public ListWrapper<ServerUserModel> Users { get; set; }


        public RoomEntityInstance()
        {
            Tracks = new ListWrapper<TrackDTO>();
            Users = new ListWrapper<ServerUserModel>();
            TrackStrategy = new TrackJustOnce();
            ActionHelper = new RoomHelper(Users);

            TimeLeftReached += TimeReachedZero;
            Users.AfterUpdate += UsersOnAfterUpdate;
        }

        /// <summary>
        /// <para>It's called only after roomDetails data is updated</para>
        /// <para>Using buffer system</para>
        /// </summary>
        public void SendUpdateRequest()
        {
            var squareBuffer = BufferSingleton.Instance.SquareRoomBufferManager.GetRequest();
            squareBuffer.UpdatedRooms.Add(ToRoomOutsideModel());
        }

        private void UsersOnAfterUpdate(object sender, ListWrapper<ServerUserModel>.UpdateEventArgs e)
        {
            var clientUser = e.Item.UserEntity.ToUserClient();
            var buffer = BufferSingleton.Instance.RoomUserListBufferManager.GetByRoomId(Id);
            if (buffer == null)
                Log.Debug($"ROOM ID: [{Id}]| Buffer cannot find roomDetails by id");

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
            TrackStrategy.NextTrack(Tracks.Wrapper);
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

        public PreviewRoomDTO ToRoomOutsideModel()
        {
            var next = new TrackDTO();
            var current = new TrackDTO();
            var previous = new TrackDTO();

            if (Tracks.Count > 1)
                next = Tracks.GetList().ElementAtOrDefault(1);

            return new PreviewRoomDTO()
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
