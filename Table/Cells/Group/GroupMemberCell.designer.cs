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
	[Register ("GroupMemberCell")]
	partial class GroupMemberCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView leftImage { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UIButton removeButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView rightImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel textLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (leftImage != null) {
				leftImage.Dispose ();
				leftImage = null;
			}

			if (rightImage != null) {
				rightImage.Dispose ();
				rightImage = null;
			}

			if (textLabel != null) {
				textLabel.Dispose ();
				textLabel = null;
			}

			if (removeButton != null) {
				removeButton.Dispose ();
				removeButton = null;
			}
		}
	}
}
