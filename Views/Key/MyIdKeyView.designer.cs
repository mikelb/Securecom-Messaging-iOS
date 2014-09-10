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
	[Register ("MyIdKeyView")]
	partial class MyIdKeyView
	{
		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem chatsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel contactGeneratedLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView contactKey { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton contactKeysButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView contactKeysView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView contactKeyView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView contactLockImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch contactTrustSwitch { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem keysButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel myGeneratedLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton myIdKeyButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView myIdKeyView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView myKey { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem scanButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem settingsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView table { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (contactGeneratedLabel != null) {
				contactGeneratedLabel.Dispose ();
				contactGeneratedLabel = null;
			}

			if (contactKey != null) {
				contactKey.Dispose ();
				contactKey = null;
			}

			if (contactKeysButton != null) {
				contactKeysButton.Dispose ();
				contactKeysButton = null;
			}

			if (contactKeysView != null) {
				contactKeysView.Dispose ();
				contactKeysView = null;
			}

			if (contactKeyView != null) {
				contactKeyView.Dispose ();
				contactKeyView = null;
			}

			if (contactLockImage != null) {
				contactLockImage.Dispose ();
				contactLockImage = null;
			}

			if (contactTrustSwitch != null) {
				contactTrustSwitch.Dispose ();
				contactTrustSwitch = null;
			}

			if (myGeneratedLabel != null) {
				myGeneratedLabel.Dispose ();
				myGeneratedLabel = null;
			}

			if (myIdKeyButton != null) {
				myIdKeyButton.Dispose ();
				myIdKeyButton = null;
			}

			if (myIdKeyView != null) {
				myIdKeyView.Dispose ();
				myIdKeyView = null;
			}

			if (myKey != null) {
				myKey.Dispose ();
				myKey = null;
			}

			if (scanButton != null) {
				scanButton.Dispose ();
				scanButton = null;
			}

			if (table != null) {
				table.Dispose ();
				table = null;
			}

			if (chatsButton != null) {
				chatsButton.Dispose ();
				chatsButton = null;
			}

			if (keysButton != null) {
				keysButton.Dispose ();
				keysButton = null;
			}

			if (settingsButton != null) {
				settingsButton.Dispose ();
				settingsButton = null;
			}
		}
	}
}
