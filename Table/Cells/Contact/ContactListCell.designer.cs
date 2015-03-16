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
	[Register ("ContactListCell")]
	partial class ContactListCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel emailLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel email { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem filterButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView icon { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel nameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel phone { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel phoneLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (icon != null) {
				icon.Dispose ();
				icon = null;
			}

			if (phoneLabel != null) {
				phoneLabel.Dispose ();
				phoneLabel = null;
			}

			if (emailLabel != null) {
				emailLabel.Dispose ();
				emailLabel = null;
			}

			if (nameLabel != null) {
				nameLabel.Dispose ();
				nameLabel = null;
			}

			if (filterButton != null) {
				filterButton.Dispose ();
				filterButton = null;
			}
		}
	}
}
