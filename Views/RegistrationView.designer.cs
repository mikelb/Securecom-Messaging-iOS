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
	[Register ("RegistrationView")]
	partial class RegistrationView
	{
		[Outlet]
		MonoTouch.UIKit.UIButton Continue { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton Edit { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView EmailRegisterView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton EmChooseOptionButton { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UITextField EmEmailInput { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton EmRegister { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton EmSkip { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton PhChooseOptionButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView PhoneRegisterView { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UITextField PhPhoneNumberInput { get; private set; }

		[Outlet]
		MonoTouch.UIKit.UIButton PhRegister { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel processingBottomLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel processingDeviceLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView processingLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView processingRectView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView processingSpinner { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel processingTopLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView processingView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Continue != null) {
				Continue.Dispose ();
				Continue = null;
			}

			if (Edit != null) {
				Edit.Dispose ();
				Edit = null;
			}

			if (EmailRegisterView != null) {
				EmailRegisterView.Dispose ();
				EmailRegisterView = null;
			}

			if (EmChooseOptionButton != null) {
				EmChooseOptionButton.Dispose ();
				EmChooseOptionButton = null;
			}

			if (EmEmailInput != null) {
				EmEmailInput.Dispose ();
				EmEmailInput = null;
			}

			if (EmRegister != null) {
				EmRegister.Dispose ();
				EmRegister = null;
			}

			if (EmSkip != null) {
				EmSkip.Dispose ();
				EmSkip = null;
			}

			if (PhChooseOptionButton != null) {
				PhChooseOptionButton.Dispose ();
				PhChooseOptionButton = null;
			}

			if (PhoneRegisterView != null) {
				PhoneRegisterView.Dispose ();
				PhoneRegisterView = null;
			}

			if (PhPhoneNumberInput != null) {
				PhPhoneNumberInput.Dispose ();
				PhPhoneNumberInput = null;
			}

			if (PhRegister != null) {
				PhRegister.Dispose ();
				PhRegister = null;
			}

			if (processingLabel != null) {
				processingLabel.Dispose ();
				processingLabel = null;
			}

			if (processingRectView != null) {
				processingRectView.Dispose ();
				processingRectView = null;
			}

			if (processingSpinner != null) {
				processingSpinner.Dispose ();
				processingSpinner = null;
			}

			if (processingView != null) {
				processingView.Dispose ();
				processingView = null;
			}

			if (processingTopLabel != null) {
				processingTopLabel.Dispose ();
				processingTopLabel = null;
			}

			if (processingDeviceLabel != null) {
				processingDeviceLabel.Dispose ();
				processingDeviceLabel = null;
			}

			if (processingBottomLabel != null) {
				processingBottomLabel.Dispose ();
				processingBottomLabel = null;
			}
		}
	}
}
