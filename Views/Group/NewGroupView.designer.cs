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
	[Register ("NewGroupView")]
	partial class NewGroupView
	{
		[Outlet]
		MonoTouch.UIKit.UITableView addContactTable { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView addContactView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton addMemberButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem cancelButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem contactCancelButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem contactDoneButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem doneButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem filterButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView groupImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField groupNameInput { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton processingCancelButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView processingRectView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel processingTextLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView processingView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISearchBar search { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView table { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (addContactTable != null) {
				addContactTable.Dispose ();
				addContactTable = null;
			}

			if (addMemberButton != null) {
				addMemberButton.Dispose ();
				addMemberButton = null;
			}

			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}

			if (contactCancelButton != null) {
				contactCancelButton.Dispose ();
				contactCancelButton = null;
			}

			if (contactDoneButton != null) {
				contactDoneButton.Dispose ();
				contactDoneButton = null;
			}

			if (doneButton != null) {
				doneButton.Dispose ();
				doneButton = null;
			}

			if (filterButton != null) {
				filterButton.Dispose ();
				filterButton = null;
			}

			if (groupImage != null) {
				groupImage.Dispose ();
				groupImage = null;
			}

			if (groupNameInput != null) {
				groupNameInput.Dispose ();
				groupNameInput = null;
			}

			if (processingCancelButton != null) {
				processingCancelButton.Dispose ();
				processingCancelButton = null;
			}

			if (processingRectView != null) {
				processingRectView.Dispose ();
				processingRectView = null;
			}

			if (processingTextLabel != null) {
				processingTextLabel.Dispose ();
				processingTextLabel = null;
			}

			if (processingView != null) {
				processingView.Dispose ();
				processingView = null;
			}

			if (search != null) {
				search.Dispose ();
				search = null;
			}

			if (table != null) {
				table.Dispose ();
				table = null;
			}

			if (addContactView != null) {
				addContactView.Dispose ();
				addContactView = null;
			}
		}
	}
}
