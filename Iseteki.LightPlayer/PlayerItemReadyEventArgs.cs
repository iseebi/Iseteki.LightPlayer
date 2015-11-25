using System;
using CoreMedia;
using AVFoundation;
using Foundation;
using System.Diagnostics;

namespace Iseteki.LightPlayer
{
    public class PlayerItemReadyEventArgs : EventArgs
    {
        public AVPlayerItemStatus Status
        {
            get;
            private set;
        }

        public PlayerItemReadyEventArgs(AVPlayerItemStatus status)
        {
            Status = status;
        }
    }

}

