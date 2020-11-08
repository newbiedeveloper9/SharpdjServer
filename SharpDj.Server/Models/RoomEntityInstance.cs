using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using SharpDj.Common;
using SharpDj.Common.DTO;
using SharpDj.Common.ListWrapper;
using SharpDj.Domain.Entity;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;

namespace SharpDj.Server.Application.Models
{
    [NotMapped]
    public class RoomEntityInstance : RoomEntity
    {
        public int AmountOfPeople => Users.Count();
        public int AmountOfAdministration => Users.Count(x => x.UserEntity.Rank > 0);

        public TrackDTO CurrentTrack => Tracks.FirstOrDefault();

        public RoomHelper TemporaryRoomHelper { get; } //remove it
        public ListWrapper<TrackDTO> Tracks { get; }
        public ListWrapper<ServerUserModel> Users { get; }


        public RoomEntityInstance()
        {
            Tracks = new ListWrapper<TrackDTO>();
            Users = new ListWrapper<ServerUserModel>();
            TemporaryRoomHelper = new RoomHelper(Users);

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

        private void UsersOnAfterUpdate(object sender, UpdateEventArgs<ServerUserModel> e)
        {
            var clientUser = e.Item.UserEntity.ToUserClient();
            var buffer = BufferSingleton.Instance.RoomUserListBufferManager.GetByRoomId(Id);
            if (buffer == null)
            {
                Log.Debug($"ROOM ID: [@RoomId]| Buffer cannot find roomDetails by id", Id);
            }

            if (e.State == UpdateEventArgs<ServerUserModel>.UpdateState.Remove)
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
            //TrackStrategy.NextTrack(Tracks.Wrapper); xd
            if (CurrentTrack != null)
            {
                TimeLeft = CurrentTrack.Duration;
            }

            BufferSingleton.Instance.SquareRoomBufferManager.GetRequest().UpdatedRooms.Add(ToRoomOutsideModel());
        }

        public PreviewRoomDTO ToRoomOutsideModel()
        {
            var next = new TrackDTO();
            var current = new TrackDTO();
            var previous = new TrackDTO();

            if (Tracks.Count() > 1)
                next = Tracks.FirstOrDefault();

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

        #region Events
        public event EventHandler TimeLeftReached;

        protected virtual void OnTimeLeftReached(EventArgs e)
        {
            var handler = TimeLeftReached;
            handler?.Invoke(this, e);
        }
        #endregion Events
    }
}
