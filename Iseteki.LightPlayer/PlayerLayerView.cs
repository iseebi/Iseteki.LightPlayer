using System;
using UIKit;
using Foundation;
using ObjCRuntime;
using AVFoundation;

namespace Iseteki.LightPlayer
{
    [Register("IsetekiMoviePlayerLayerView")]
    public class PlayerLayerView : UIView
    {
        [Export("layerClass")]
        public static Class LayerClass()
        {
            return new Class(typeof(AVPlayerLayer));
        }

        public PlayerLayerView (IntPtr handle) : base (handle)
        {
        }

        AVPlayerLayer PlayerLayer
        {
            get { return (AVPlayerLayer)Layer; }
        }

        public AVPlayer Player
        {
            get { return PlayerLayer.Player; }
            set { PlayerLayer.Player = value; }
        }
    }
}

