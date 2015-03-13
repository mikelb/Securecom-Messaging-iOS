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
	[Register ("ChatView")]
	partial class ChatView
	{
		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem accessoryCreateButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView accessoryTextView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar accessoryToolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton call { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem cancel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView chatView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem deleteAllButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar editToolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem fakePhotoButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel headerLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField inputFakeMessage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem lockButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem photoButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView table { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField textMessage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolbarFakeMessage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISegmentedControl topButtons { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (accessoryCreateButton != null) {
				accessoryCreateButton.Dispose ();
				accessoryCreateButton = null;
			}

			if (accessoryTextView != null) {
				accessoryTextView.Dispose ();
				accessoryTextView = null;
			}

			if (accessoryToolbar != null) {
				accessoryToolbar.Dispose ();
				accessoryToolbar = null;
			}

			if (call != null) {
				call.Dispose ();
				call = null;
			}

			if (cancel != null) {
				cancel.Dispose ();
				cancel = null;
			}

			if (chatView != null) {
				chatView.Dispose ();
				chatView = null;
			}

//			if (edit != null) {
//				edit.Dispose ();
//				edit = null;
//			}

			if (editToolbar != null) {
				editToolbar.Dispose ();
				editToolbar = null;
			}

			if (fakePhotoButton != null) {
				fakePhotoButton.Dispose ();
				fakePhotoButton = null;
			}

			if (headerLabel != null) {
				headerLabel.Dispose ();
				headerLabel = null;
			}

//			if (info != null) {
//				info.Dispose ();
//				info = null;
//			}

			if (inputFakeMessage != null) {
				inputFakeMessage.Dispose ();
				inputFakeMessage = null;
			}

			if (lockButton != null) {
				lockButton.Dispose ();
				lockButton = null;
			}

			if (photoButton != null) {
				photoButton.Dispose ();
				photoButton = null;
			}

			if (table != null) {
				table.Dispose ();
				table = null;
			}

			if (textMessage != null) {
				textMessage.Dispose ();
				textMessage = null;
			}

			if (toolbarFakeMessage != null) {
				toolbarFakeMessage.Dispose ();
				toolbarFakeMessage = null;
			}

			if (topButtons != null) {
				topButtons.Dispose ();
				topButtons = null;
			}

			if (deleteAllButton != null) {
				deleteAllButton.Dispose ();
				deleteAllButton = null;
			}
		}
	}
}
