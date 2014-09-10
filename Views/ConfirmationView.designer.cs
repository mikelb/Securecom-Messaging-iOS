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
	[Register ("ConfirmationView")]
	partial class ConfirmationView
	{
		[Outlet]
		MonoTouch.UIKit.UILabel bottomLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField confCodeInput { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton done { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView img1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView img2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView img3 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView img4 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel label1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel label2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel label3 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel label4 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView processingView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView processingViewRect { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView spinner1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView spinner2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView spinner3 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView spinner4 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel topLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (processingView != null) {
				processingView.Dispose ();
				processingView = null;
			}

			if (processingViewRect != null) {
				processingViewRect.Dispose ();
				processingViewRect = null;
			}

			if (topLabel != null) {
				topLabel.Dispose ();
				topLabel = null;
			}

			if (bottomLabel != null) {
				bottomLabel.Dispose ();
				bottomLabel = null;
			}

			if (img1 != null) {
				img1.Dispose ();
				img1 = null;
			}

			if (img2 != null) {
				img2.Dispose ();
				img2 = null;
			}

			if (img3 != null) {
				img3.Dispose ();
				img3 = null;
			}

			if (img4 != null) {
				img4.Dispose ();
				img4 = null;
			}

			if (label1 != null) {
				label1.Dispose ();
				label1 = null;
			}

			if (label2 != null) {
				label2.Dispose ();
				label2 = null;
			}

			if (label3 != null) {
				label3.Dispose ();
				label3 = null;
			}

			if (label4 != null) {
				label4.Dispose ();
				label4 = null;
			}

			if (spinner1 != null) {
				spinner1.Dispose ();
				spinner1 = null;
			}

			if (spinner2 != null) {
				spinner2.Dispose ();
				spinner2 = null;
			}

			if (spinner3 != null) {
				spinner3.Dispose ();
				spinner3 = null;
			}

			if (spinner4 != null) {
				spinner4.Dispose ();
				spinner4 = null;
			}

			if (confCodeInput != null) {
				confCodeInput.Dispose ();
				confCodeInput = null;
			}

			if (done != null) {
				done.Dispose ();
				done = null;
			}
		}
	}
}
