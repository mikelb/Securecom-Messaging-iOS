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
	[Register ("ChatCell")]
	partial class ChatCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView avatar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel labelHeader { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel labelSubheader { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel labelSubheaderRead { get; set; }


		[Outlet]
		MonoTouch.UIKit.UILabel labelTime { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (avatar != null) {
				avatar.Dispose ();
				avatar = null;
			}

			if (labelHeader != null) {
				labelHeader.Dispose ();
				labelHeader = null;
			}

			if (labelSubheader != null) {
				labelSubheader.Dispose ();
				labelSubheader = null;
			}

			if (labelTime != null) {
				labelTime.Dispose ();
				labelTime = null;
			}

			if (labelSubheaderRead != null) {
				labelSubheaderRead.Dispose ();
				labelSubheaderRead = null;
			}

			if (labelSubheaderRead != null) {
				labelSubheaderRead.Dispose ();
				labelSubheaderRead = null;
			}
		}
	}
}
