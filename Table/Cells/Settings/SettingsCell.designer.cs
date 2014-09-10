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
	[Register ("SettingsCell")]
	partial class SettingsCell
	{
		[Outlet]
		public MonoTouch.UIKit.UISwitch sectionButton { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UITextView sectionDetail { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UILabel sectionLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (sectionLabel != null) {
				sectionLabel.Dispose ();
				sectionLabel = null;
			}

			if (sectionDetail != null) {
				sectionDetail.Dispose ();
				sectionDetail = null;
			}

			if (sectionButton != null) {
				sectionButton.Dispose ();
				sectionButton = null;
			}
		}
	}
}
