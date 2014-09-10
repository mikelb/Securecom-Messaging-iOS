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
	[Register ("ContactDetailView")]
	partial class ContactDetailView
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView contactPictureImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField emailInput { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel headerLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton inviteButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton killKeyboardButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField mobileInput { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField nameInput { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton processingCancelButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel processingLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView processingRectView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView processingView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (contactPictureImage != null) {
				contactPictureImage.Dispose ();
				contactPictureImage = null;
			}

			if (emailInput != null) {
				emailInput.Dispose ();
				emailInput = null;
			}

			if (headerLabel != null) {
				headerLabel.Dispose ();
				headerLabel = null;
			}

			if (inviteButton != null) {
				inviteButton.Dispose ();
				inviteButton = null;
			}

			if (mobileInput != null) {
				mobileInput.Dispose ();
				mobileInput = null;
			}

			if (nameInput != null) {
				nameInput.Dispose ();
				nameInput = null;
			}

			if (processingCancelButton != null) {
				processingCancelButton.Dispose ();
				processingCancelButton = null;
			}

			if (processingLabel != null) {
				processingLabel.Dispose ();
				processingLabel = null;
			}

			if (processingRectView != null) {
				processingRectView.Dispose ();
				processingRectView = null;
			}

			if (processingView != null) {
				processingView.Dispose ();
				processingView = null;
			}

			if (killKeyboardButton != null) {
				killKeyboardButton.Dispose ();
				killKeyboardButton = null;
			}
		}
	}
}
