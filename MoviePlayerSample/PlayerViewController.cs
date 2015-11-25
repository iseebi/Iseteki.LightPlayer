using Foundation;
using System;
using UIKit;
using Iseteki.LightPlayer;

namespace MoviePlayerSample
{
    partial class PlayerViewController : UIViewController, IUIGestureRecognizerDelegate
    {
        LightPlayer Player { get; set; }

        bool IsOverlayVisible { get; set; }

        bool IsOverlayAnimating { get; set; }

        UITapGestureRecognizer OnTapGestureRecognizer { get; set; }

        public PlayerViewController(IntPtr handle)
            : base(handle)
        {
        }

        #region View Lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            IsOverlayVisible = true;
            Player = new LightPlayer();
            OnTapGestureRecognizer = new UITapGestureRecognizer(SwitchOverlay);
            OnTapGestureRecognizer.WeakDelegate = this;
            View.AddGestureRecognizer(OnTapGestureRecognizer);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Player.PlayerItemReady += HandlePlayerReady;
            Player.PlayerBufferEmpty += HandlePlayerBufferEmpty;
            Player.PlayerItemFailed += HandlePlayerItemFailed;
            Player.PrepareForUrl(NSUrl.FromString("https://devimages.apple.com.edgekey.net/streaming/examples/bipbop_16x9/bipbop_16x9_variant.m3u8"));
            PlayerView.Player = Player.Player;
        }

        public override void ViewDidAppear(bool animated)
        {
            SetNeedsStatusBarAppearanceUpdate();
            base.ViewDidAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            Player.PlayerItemReady -= HandlePlayerReady;
            PlayerView.Player = null;
            Player.Dispose();
            Player = null;
            base.ViewDidDisappear(animated);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Player != null)
                {
                    Player.Dispose();
                    Player = null;
                }
                ReleaseDesignerOutlets();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Navigation

        void ButtonStateToPlay()
        {
            StateButton.SetTitle("Pause", UIControlState.Normal);
        }

        void ButtonStateToPause()
        {
            StateButton.SetTitle("Play", UIControlState.Normal);
        }

        void Close()
        {
            DismissViewController(true, null);
        }

        void SwitchOverlay()
        {
            UpdateOverlayState(!IsOverlayVisible);
        }

        void UpdateOverlayState(bool toState)
        {
            if (IsOverlayAnimating)
            {
                return;
            }
            if (toState == IsOverlayVisible)
            {
                return;
            }
            IsOverlayVisible = toState;
            IsOverlayAnimating = true;
            UIView.Animate(0.2, () =>
                {
                    OverlayView.Alpha = (nfloat)(toState ? 1.0 : 0.0);
                }, () =>
                {
                    IsOverlayAnimating = false;
                });

        }

        #endregion

        #region Appearance

        public override bool PrefersStatusBarHidden()
        {
            return !IsOverlayVisible;
        }

        #endregion

        #region Player Events

        void HandlePlayerReady(object sender, PlayerItemReadyEventArgs e)
        {
            if (e.Status == AVFoundation.AVPlayerItemStatus.ReadyToPlay)
            {
                Player.Play();
                ButtonStateToPlay();
                LoadingIndicator.StopAnimating();
                UpdateOverlayState(false);
            }
        }

        void HandlePlayerBufferEmpty(object sender, EventArgs e)
        {
            LoadingIndicator.StartAnimating();
            ButtonStateToPause();
        }

        void HandlePlayerItemFailed(object sender, PlayerItemFailedEventArgs e)
        {
            var ac = UIAlertController.Create("Error", e.Error.LocalizedDescription, UIAlertControllerStyle.Alert);
            ac.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, a => Close()));
            PresentViewController(ac, true, null);
        }

        #endregion

        #region Control Events

        partial void StateButton_TouchUpInside(UIButton sender)
        {
            if (Player.Player.Status == AVFoundation.AVPlayerStatus.ReadyToPlay)
            {
                if (Player.IsPlaying)
                {
                    Player.Pause();
                    ButtonStateToPause();
                }
                else
                {
                    Player.Play();
                    ButtonStateToPlay();
                }
            }
        }

        partial void DoneButton_TouchUpInside(UIButton sender)
        {
            Close();
        }

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
        {
            if ((StateButton != null) && touch.View.IsDescendantOfView(StateButton))
            {
                return false;
            }
            if ((DoneButton != null) && touch.View.IsDescendantOfView(DoneButton))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}
