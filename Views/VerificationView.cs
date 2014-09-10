
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext{


	public partial class VerificationView : UIViewController{

		private AppDelegate appDelegate;
		private string labelText;


		public VerificationView () : base ("VerificationView", null){}


		public override void ViewDidLoad (){

			base.ViewDidLoad ();
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

			this.confirmationCodeButton.TouchUpInside += (sender, e) => {
				this.appDelegate.GoToView(this.appDelegate.confirmationView);
			};

			this.resendButton.TouchUpInside += (sender, e) => {
				this.appDelegate.alert.showOkAlert("Resend Verification","");
			};

		}


		public override void ViewDidAppear (bool animated){
			base.ViewDidAppear (animated);
		}


		public override void ViewWillAppear (bool animated){
			SetContent ();
			base.ViewWillAppear (animated);
		}


		private void SetContent(){
			if (appDelegate.registrationView.registerMode == appDelegate.MODE_REGISTER_EMAIL) {
				NavigationItem.TitleView = StextUtil.SetTitleBarImage ("Email Verification",10,40);
				this.topLabel.Text = "An email has been sent to:";
				this.deviceLabel.Text = this.appDelegate.registrationView.EmEmailInput.Text;
				this.bottomLabel.Text = "Please tap the link in the email to complete your registration";
				this.resendButton.SetTitle("Resend Verification Email", UIControlState.Normal);
			} else {
				NavigationItem.TitleView = StextUtil.SetTitleBarImage ("Phone Verification",10,40);		
				this.topLabel.Text = "An SMS message has been sent to:";
				this.deviceLabel.Text = this.appDelegate.registrationView.PhPhoneNumberInput.Text;
				this.bottomLabel.Text = "Please tap the link in the message to complete your registration";
				this.resendButton.SetTitle("Resend Verification SMS",UIControlState.Normal);
			}
		}


	


	}
}

