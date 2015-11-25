// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MoviePlayerSample
{
	[Register ("PlayerViewController")]
	partial class PlayerViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton DoneButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIActivityIndicatorView LoadingIndicator { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView OverlayView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		Iseteki.LightPlayer.PlayerLayerView PlayerView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton StateButton { get; set; }

		[Action ("DoneButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void DoneButton_TouchUpInside (UIButton sender);

		[Action ("StateButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void StateButton_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (DoneButton != null) {
				DoneButton.Dispose ();
				DoneButton = null;
			}
			if (LoadingIndicator != null) {
				LoadingIndicator.Dispose ();
				LoadingIndicator = null;
			}
			if (OnTapGestureRecognizer != null) {
				OnTapGestureRecognizer.Dispose ();
				OnTapGestureRecognizer = null;
			}
			if (OverlayView != null) {
				OverlayView.Dispose ();
				OverlayView = null;
			}
			if (PlayerView != null) {
				PlayerView.Dispose ();
				PlayerView = null;
			}
			if (StateButton != null) {
				StateButton.Dispose ();
				StateButton = null;
			}
		}
	}
}
