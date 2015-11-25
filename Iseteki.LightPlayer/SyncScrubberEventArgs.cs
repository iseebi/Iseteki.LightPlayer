using System;
using CoreMedia;
using AVFoundation;
using Foundation;
using System.Diagnostics;

namespace Iseteki.LightPlayer
{
	public class SyncScrubberEventArgs : EventArgs
	{
        public CMTime Time
        {
            get;
            private set;
        }

        public SyncScrubberEventArgs(CMTime time)
        {
            this.Time = time;
            
        }
	}
}

