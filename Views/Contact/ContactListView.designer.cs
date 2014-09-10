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
	[Register ("ContactListView")]
	partial class ContactListView
	{
		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem cancelButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem filterButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISearchBar search { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView table { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (filterButton != null) {
				filterButton.Dispose ();
				filterButton = null;
			}

			if (search != null) {
				search.Dispose ();
				search = null;
			}

			if (table != null) {
				table.Dispose ();
				table = null;
			}

			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}
		}
	}
}
