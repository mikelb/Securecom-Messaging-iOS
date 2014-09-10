// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Stext
{
	[Register ("VerificationView")]
	partial class VerificationView
	{
		[Outlet]
		MonoTouch.UIKit.UILabel bottomLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton confirmationCodeButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel deviceLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton resendButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel topLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView verificationLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (confirmationCodeButton != null) {
				confirmationCodeButton.Dispose ();
				confirmationCodeButton = null;
			}

			if (resendButton != null) {
				resendButton.Dispose ();
				resendButton = null;
			}

			if (verificationLabel != null) {
				verificationLabel.Dispose ();
				verificationLabel = null;
			}

			if (topLabel != null) {
				topLabel.Dispose ();
				topLabel = null;
			}

			if (bottomLabel != null) {
				bottomLabel.Dispose ();
				bottomLabel = null;
			}

			if (deviceLabel != null) {
				deviceLabel.Dispose ();
				deviceLabel = null;
			}
		}
	}
}
