﻿using SCPackets.Models;
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
using SCPackets.RoomOutsideUpdate;
using Server.Management.Singleton;
using Server.Models.InstanceHelpers;

namespace Server
{
    [NotMapped]
    public class RoomInstance : Room
    {
        public int AmountOfPeople => Users.Count;
        public int AmountOfAdministration => Users.Count(x => x.User.Rank > 0);


        public ITrackStrategy TrackStrategy { get; set; }
        public TrackModel CurrentTrack => Tracks[0];

        public RoomHelper ActionHelper { get; }

        public List<TrackModel> Tracks { get; set; }
        public List<ServerUserModel> Users { get; set; }


        public RoomInstance()
        {
            Tracks = new List<TrackModel>();
            Users = new List<ServerUserModel>();
            TrackStrategy = new TrackJustOnce();

            ActionHelper = new RoomHelper(Users);

            TimeLeftReached += TimeReachedZero;
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
            TrackStrategy.NextTrack(Tracks);
            if (CurrentTrack != null)
                TimeLeft = CurrentTrack.Duration;
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
                next = Tracks[1];

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
