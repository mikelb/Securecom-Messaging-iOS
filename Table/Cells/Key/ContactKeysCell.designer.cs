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
	[Register ("ContactKeysCell")]
	partial class ContactKeysCell
	{
		[Outlet]
		public MonoTouch.UIKit.UILabel headerLabel { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UIImageView icon { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UILabel subHeaderLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (icon != null) {
				icon.Dispose ();
				icon = null;
			}

			if (headerLabel != null) {
				headerLabel.Dispose ();
				headerLabel = null;
			}

			if (subHeaderLabel != null) {
				subHeaderLabel.Dispose ();
				subHeaderLabel = null;
			}
		}
	}
}
