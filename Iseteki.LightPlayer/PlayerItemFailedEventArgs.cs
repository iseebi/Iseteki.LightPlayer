using System;
using CoreMedia;
using AVFoundation;
using Foundation;
using System.Diagnostics;

namespace Iseteki.LightPlayer
{
	public class PlayerItemFailedEventArgs : EventArgs
	{
        public NSError Error
        {
            get;
            set;
        }

        public PlayerItemFailedEventArgs(NSError error)
        {
            Error = error;
        }
	}
}

