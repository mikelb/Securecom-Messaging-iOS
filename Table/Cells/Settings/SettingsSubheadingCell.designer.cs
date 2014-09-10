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
	[Register ("SettingsSubheadingCell")]
	partial class SettingsSubheadingCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel textLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (textLabel != null) {
				textLabel.Dispose ();
				textLabel = null;
			}
		}
	}
}
