using System;
using CoreMedia;
using AVFoundation;
using Foundation;
using System.Diagnostics;

namespace Iseteki.LightPlayer
{
    public class LightPlayer : NSObject
    {
        static readonly NSString StatusKey = new NSString("status");
        static readonly NSString LoadedTimeRangesKey = new NSString("loadedTimeRanges");
        static readonly NSString PlaybackBufferEmptyKey = new NSString("playbackBufferEmpty");

        static readonly NSObject StatusContext = new NSObject();
        static readonly NSObject TimeRangesContext = new NSObject();
        static readonly NSObject PlaybackBufferEmptyContext = new NSObject();

        const int NSEC_PER_SEC = 1000000000;

        public AVPlayer Player
        {
            get;
            private set;
        }

        public AVPlayerItem PlayerItem
        {
            get;
            private set;
        }

        public event EventHandler<SyncScrubberEventArgs> SyncScrubber;
        public event EventHandler<PlayerItemReadyEventArgs> PlayerItemReady;
        public event EventHandler<PlayerItemFailedEventArgs> PlayerItemFailed;
        public event EventHandler<PlayerItemLoadTimeRangeEventArgs> PlayerItemLoadTimeRange;
        public event EventHandler PlayerBufferEmpty;

        NSUrl AssetUrl
        {
            get;
            set;
        }

        NSObject TimeObserver
        {
            get;
            set;
        }


        public CMTime CurrentTime
        {
            get { return  (Player != null) ? Player.CurrentTime : CMTime.Invalid; }
        }

        public AVPlayerItem CurrentItem
        {
            get { return  (Player != null) ? Player.CurrentItem : null; }
        }

        void CleanPreviousResources()
        {
            if (PlayerItem != null)
            {
                PlayerItem.RemoveObserver(this, StatusKey);
                PlayerItem.RemoveObserver(this, LoadedTimeRangesKey);
                PlayerItem.RemoveObserver(this, PlaybackBufferEmptyKey);
                RemoveScrubberTimer();
                PlayerItem = null;
            }
        }

        public void PrepareForUrl(NSUrl url)
        {
            AssetUrl = url;

            CleanPreviousResources();
            PlayerItem = new AVPlayerItem(url);

            PlayerItem.AddObserver(this, StatusKey, NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, StatusContext.Handle);
            PlayerItem.AddObserver(this, LoadedTimeRangesKey, NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, TimeRangesContext.Handle);
            PlayerItem.AddObserver(this, PlaybackBufferEmptyKey, NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, PlaybackBufferEmptyContext.Handle);

            if (Player != null)
            {
                Player.Dispose();
            }
            Player = new AVPlayer(PlayerItem);
        }

        public LightPlayer()
        {
        }

        public LightPlayer(NSUrl url)
        {
            /*  You cannot directly create an AVAsset instance to represent the media in an HTTP Live Stream,
                so create AVPlayerItem first, then it can generate corresponding AVAsset after downloading
             */
            PrepareForUrl(url);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.WriteLine("Disposing Iseteki.LightPlayer {0}", this);
                CleanPreviousResources();
                if (Player != null)
                {
                    Player.Dispose();
                    Player = null;
                }
            }
            base.Dispose(disposing);
        }

        public void Pause()
        {
            if (Player != null)
            {
                Player.Pause();
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (Player != null)
                {
					return Player.Rate > 0.0;
                }
                return false;
            }
        }

        public void Play()
        {
            if (Player != null)
            {
                Player.Play();
            }
        }

        public void SeekTo(double position)
        {
            if (Player == null)
            {
                return;
            }
            if (PlayerItem != Player.CurrentItem)
            {
                PlayerItem.RemoveObserver(this, StatusKey);
                PlayerItem.RemoveObserver(this, LoadedTimeRangesKey);
                PlayerItem.RemoveObserver(this, PlaybackBufferEmptyKey);
                PlayerItem = Player.CurrentItem;
                PlayerItem.AddObserver(this, StatusKey, NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, StatusContext.Handle);
                PlayerItem.AddObserver(this, LoadedTimeRangesKey, NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, TimeRangesContext.Handle);
                PlayerItem.AddObserver(this, PlaybackBufferEmptyKey, NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, PlaybackBufferEmptyContext.Handle);
            }


            var totalTime = PlayerItemDuration.Seconds;
            var scrubToTime = CMTime.FromSeconds(totalTime * position, NSEC_PER_SEC);

            PlayerItem.Seek(scrubToTime);
        }

        public CMTime PlayerItemDuration
        {
            get
            {
                if (Player != null)
                {
                    var thePlayerItem = Player.CurrentItem;
                    if (thePlayerItem.Status == AVPlayerItemStatus.ReadyToPlay)
                    {
                        return thePlayerItem.Duration;
                    }
                }
                return CMTime.Invalid;
            }
        }

        void InitScrubberTimer()
        {
            const double interval = .1f;

            if (Player == null)
            {
                return;
            }

            if (PlayerItemDuration.IsInvalid)
            {
                return;
            }

            var weakThis = new WeakReference<LightPlayer>(this);
            TimeObserver = Player.AddPeriodicTimeObserver(CMTime.FromSeconds(interval, NSEC_PER_SEC), null, time =>
                {
                    LightPlayer wThis;
                    if (weakThis.TryGetTarget(out wThis))
                    {
                        if (wThis.SyncScrubber != null)
                        {
                            wThis.SyncScrubber(wThis, new SyncScrubberEventArgs(time));
                        }
                    }
                });
        }

        void RemoveScrubberTimer()
        {
            if (Player == null)
            {
                return;
            }
            if (TimeObserver != null)
            {
                Player.RemoveTimeObserver(TimeObserver);
                TimeObserver = null;
            }
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (context == StatusContext.Handle)
            {
                var status = (AVPlayerItemStatus)((NSNumber)change.ObjectForKey(NSObject.ChangeNewKey)).Int64Value;
                switch (status)
                {
                /* Indicates that the status of the player is not yet known because
                     it has not tried to load new media resources for playback */
                    case AVPlayerItemStatus.Unknown:
                        {
                            Debug.WriteLine("status unknown");
                        }
                        break;

                    case AVPlayerItemStatus.ReadyToPlay:
                        {
                            Debug.WriteLine("status ready");
                            // invoke the block provided by the UI module

                            if (PlayerItemReady != null)
                            {
                                var thePlayerItem = (AVPlayerItem)ofObject;
                                PlayerItemReady(this, new PlayerItemReadyEventArgs(thePlayerItem.Status));
                            }
                        }
                        break;

                    case AVPlayerItemStatus.Failed:
                        {
                            Debug.WriteLine("status failed {0}", PlayerItem.Error);
                            if (PlayerItemFailed != null)
                            {
                                PlayerItemFailed(this, new PlayerItemFailedEventArgs(PlayerItem.Error));
                            }
                        }
                        break;
                }
            }
            else if (context == TimeRangesContext.Handle)
            {
                if (PlayerItemLoadTimeRange != null)
                {
                    var thePlayerItem = (AVPlayerItem)ofObject;
                    var times = thePlayerItem.LoadedTimeRanges;

                    // there is only ever one NSValue in the array
                    // could get empty array for the first time,
                    // happened for some remote sites

                    if (times != null && times.Length > 0)
                    {
                        var value = times[0];
                        var range = value.CMTimeRangeValue;
                        var start = range.Start.Seconds;
                        var duration = range.Duration.Seconds;

                        PlayerItemLoadTimeRange(this, new PlayerItemLoadTimeRangeEventArgs(start, duration));
                    }
                }
            }
            else if (context == PlaybackBufferEmptyContext.Handle)
            {
                if (PlayerBufferEmpty != null)
                {
                    PlayerBufferEmpty(this, EventArgs.Empty);
                }
            }
            else
            {
                base.ObserveValue(keyPath, ofObject, change, context);
            }
        }
    }
}

