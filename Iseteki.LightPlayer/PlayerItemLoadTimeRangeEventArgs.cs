using System;
using CoreMedia;
using AVFoundation;
using Foundation;
using System.Diagnostics;

namespace Iseteki.LightPlayer
{
    public class PlayerItemLoadTimeRangeEventArgs : EventArgs
    {
        public double Start
        {
            get; 
            private set;
        }

        public double Duration
        {
            get;
            private set;
        }

        public PlayerItemLoadTimeRangeEventArgs(double start, double duration)
        {
            Start = start;
            Duration = duration;
        }
    }
}

