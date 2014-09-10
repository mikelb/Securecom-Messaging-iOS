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
	[Register ("SettingsHeader")]
	partial class SettingsHeader
	{
		[Outlet]
		public MonoTouch.UIKit.UILabel headerLabel { get; private set; }

		[Outlet]
		MonoTouch.UIKit.UILabel subheaderLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (headerLabel != null) {
				headerLabel.Dispose ();
				headerLabel = null;
			}

			if (subheaderLabel != null) {
				subheaderLabel.Dispose ();
				subheaderLabel = null;
			}
		}
	}
}
