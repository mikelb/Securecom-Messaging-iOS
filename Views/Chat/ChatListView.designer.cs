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
	[Register ("ChatListView")]
	partial class ChatListView
	{
		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem chatsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem composeButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem doneButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem editButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem keysButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem markAllReadButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISearchBar search { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem settingsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView table { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (chatsButton != null) {
				chatsButton.Dispose ();
				chatsButton = null;
			}

			if (composeButton != null) {
				composeButton.Dispose ();
				composeButton = null;
			}

			if (doneButton != null) {
				doneButton.Dispose ();
				doneButton = null;
			}

			if (editButton != null) {
				editButton.Dispose ();
				editButton = null;
			}

			if (keysButton != null) {
				keysButton.Dispose ();
				keysButton = null;
			}

			if (search != null) {
				search.Dispose ();
				search = null;
			}

			if (settingsButton != null) {
				settingsButton.Dispose ();
				settingsButton = null;
			}

			if (table != null) {
				table.Dispose ();
				table = null;
			}

			if (markAllReadButton != null) {
				markAllReadButton.Dispose ();
				markAllReadButton = null;
			}
		}
	}
}
