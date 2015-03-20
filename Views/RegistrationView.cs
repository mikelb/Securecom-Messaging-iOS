
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

using Securecom.Messaging;
using Securecom.Messaging.Utils;
using Securecom.Messaging.Spec;
using Securecom.Messaging.Net;
using Securecom.Messaging.Entities;

using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.IO;
using System.Net;

namespace Stext
{

	public partial class RegistrationView : UIViewController
	{

		AppDelegate appDelegate;
		public int registerMode;


		public RegistrationView()
			: base("RegistrationView", null)
		{
		}

		public override void ViewWillAppear(bool animated)
		{
			processingView.Hidden = true;
			//processingView.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			processingView.Layer.Opacity = 1.0f;
			processingView.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 80);
			processingRectView.Layer.CornerRadius = 5.0f;
			//processingRectView.Frame = new RectangleF (0, 0, 280, 150);
			processingRectView.Center = new PointF(UIScreen.MainScreen.Bounds.Width / 2, UIScreen.MainScreen.Bounds.Height / 2);
			processingRectView.Layer.Opacity = 1.0f;
		}


		private void PickRegisterOption()
		{
			try {
				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet();

				actionSheet.AddButton("Phone");
				actionSheet.AddButton("Email");		

				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == (0)) {
						EmailRegisterView.Hidden = true;
						PhoneRegisterView.Hidden = false;
						SetEditing(false, true);
						this.registerMode = this.appDelegate.MODE_REGISTER_PHONE;
					} else {
						EmailRegisterView.Hidden = false;
						PhoneRegisterView.Hidden = true;
						this.registerMode = this.appDelegate.MODE_REGISTER_EMAIL;
						SetEditing(false, true);
					} 
				};
				actionSheet.ShowInView(View);
			} catch (Exception ex) {
				Console.Write(ex.Message);
			}
		}

		private void ConfirmPhoneRegistration()
		{
			this.processingTopLabel.Text = "We will now verify that the following number is associated with this device:";
			this.processingDeviceLabel.Text = PhPhoneNumberInput.Text;
			this.processingBottomLabel.Text = "Is this number correct, or would you like to edit it before continuing?";
			this.processingView.Hidden = false;
		}


		private void ConfirmEmailRegistration()
		{
			this.processingTopLabel.Text = "We will now verify that the following email address is associated with this device:";
			this.processingDeviceLabel.Text = EmEmailInput.Text;
			this.processingBottomLabel.Text = "Is this email address correct, or would you like to edit it before continuing?";
			this.processingView.Hidden = false;
		}


		public override void ViewDidAppear(bool animated)
		{
			NavigationItem.TitleView = StextUtil.SetTitleBarImage("Connect with Stext", 35, 65);
		}

		public override void ViewDidLayoutSubviews(){
			this.PhoneRegisterView.ContentSize = new SizeF(320, 700);
			this.EmailRegisterView.ContentSize = new SizeF(320, 700);
		}
		public override void ViewDidLoad()
		{
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			this.registerMode = this.appDelegate.MODE_REGISTER_PHONE;
			this.EmailRegisterView.ContentSize = new SizeF(320, 700);
			this.PhoneRegisterView.ContentSize = new SizeF(320, 700);
			base.ViewDidLoad();

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);
	
			this.Continue.TouchUpInside += (sender, e) => {
				appDelegate.GoToView(appDelegate.verificationView);
			};

			this.PhChooseOptionButton.TouchUpInside += (sender, e) => {
				PickRegisterOption();
			};

			this.EmChooseOptionButton.TouchUpInside += (sender, e) => {
				PickRegisterOption();
			};

			this.PhRegister.TouchUpInside += (sender, e) => {
				ConfirmPhoneRegistration();
			};

			this.EmRegister.TouchUpInside += (sender, e) => {
				ConfirmEmailRegistration();
			};

			this.Edit.TouchUpInside += (sender, e) => {
				this.processingView.Hidden = true;
				this.PhPhoneNumberInput.BecomeFirstResponder();
			};

			this.EmEmailInput.ShouldReturn += (textField) => {
				textField.ResignFirstResponder();
				return true;
			};

			this.PhPhoneNumberInput.ShouldReturn += (textField) => { 
				textField.ResignFirstResponder();
				return true;
			};

			this.Continue.TouchUpInside += (sender, e) => {
				String phoneNumber = PhPhoneNumberInput.Text;
				bool isEmail = false;
				phoneNumber = (registerMode == 0) ? PhPhoneNumberInput.Text : EmEmailInput.Text;
				isEmail = (registerMode != 0);
				appDelegate.CreateMessageManager(phoneNumber);
				try{
					MessageManager.CreateAccount(phoneNumber, isEmail);
				}catch(WebException we){
					appDelegate.GoToView(appDelegate.registrationView);
					UIAlertView alert = new UIAlertView("Error!", "Please enter a valid phone number!", null, "Ok");
					alert.Show();
				}
			};
		}

	}
}

