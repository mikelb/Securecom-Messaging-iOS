using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading;
using MonoTouch.MessageUI;


namespace Stext{


	public partial class ContactDetailView : UIViewController{

		AppDelegate appDelegate;
		private NSNotificationCenter center = null;

		public ContactDetailView () : base ("ContactDetailView", null){}


		private void SetupContact(){
			if(appDelegate.contactListView.name != null){
				this.Title = appDelegate.contactListView.name;
				this.nameInput.Text = appDelegate.contactListView.name;
				this.mobileInput.Text = appDelegate.contactListView.mobile;
				this.emailInput.Text = appDelegate.contactListView.email;
			}
		}


		private void InitProcessingView(){
			processingView.Hidden = true;
			processingView.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			processingView.Layer.Opacity = 1.0f;
			processingView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 80);
			processingRectView.Layer.CornerRadius = 5.0f;
			processingRectView.Frame = new RectangleF (0, 0, 280, 150);
			processingRectView.Center = new PointF (UIScreen.MainScreen.Bounds.Width / 2, UIScreen.MainScreen.Bounds.Height / 3);
			processingRectView.Layer.Opacity = 1.0f;
		}


		private void UpdateContact(){

			processingView.Hidden = false;

			InvokeInBackground(delegate {
				try {
					Thread.Sleep(2000);
				}
				catch (Exception ex) {
					InvokeOnMainThread(delegate {
						processingView.Hidden = true;
					});
				}
				InvokeOnMainThread(delegate {
					this.appDelegate.contactListView.name = nameInput.Text;
					this.appDelegate.contactListView.mobile = mobileInput.Text;
					this.appDelegate.contactListView.email = emailInput.Text;
					processingView.Hidden = true;
				});
			});
		}


		private void DismissKeyboard (){
			this.View.EndEditing(true);
		}


		private void ShowInviteOptions(){
			try{
				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();
				actionSheet.AddButton ("SMS " + this.mobileInput.Text);
				actionSheet.AddButton ("Email " + this.emailInput.Text);
				actionSheet.AddButton ("Cancel");
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {

					if (b.ButtonIndex == (0)) {
						string[] recipeints = new string[] { this.mobileInput.Text }; 
						MFMessageComposeViewController smsView = StextUtil.CreateSimpleSMSView (recipeints,"You have been invited to Stext");
					}else if (b.ButtonIndex == (1)) {
						StextUtil.OpenEmail(this.emailInput.Text,"You have been invited to Stext","You have been invited to Stext. Instructions for registering...");
					}

				};
				actionSheet.ShowInView (View);
			}catch(Exception ex){
				Console.Write(ex.Message);
			}
		}



		private void SetupButtons(){

			this.center = NSNotificationCenter.DefaultCenter;

			center.AddObserver(
				UIKeyboard.WillHideNotification, (notify) => { 
					try{
						var keyboardBounds = (NSValue)notify.UserInfo.ObjectForKey(UIKeyboard.BoundsUserInfoKey);
						var keyboardSize = keyboardBounds.RectangleFValue;
						if((nameInput.Text != this.appDelegate.contactListView.name) ||
							(emailInput.Text != this.appDelegate.contactListView.email) ||
							(mobileInput.Text != this.appDelegate.contactListView.mobile)){
							SetEditing(false,false);
							UpdateContact();
						}
					}catch(Exception ex){
						Console.Write(ex.Message);
					}
				}
			);	


			this.killKeyboardButton.AllTouchEvents += (sender, e) => {
				DismissKeyboard();
			};

			this.emailInput.EditingDidEnd += (sender, e) => {
				ResignFirstResponder();
				DismissKeyboard();
			};

			this.inviteButton.TouchUpInside += (sender, e) => {
				ShowInviteOptions();
			};

			this.nameInput.Ended += (sender, e) => {
//				if((nameInput.Text != this.appDelegate.contactListView.name) ||
//					(emailInput.Text != this.appDelegate.contactListView.email) ||
//					(mobileInput.Text != this.appDelegate.contactListView.mobile)){
//					SetEditing(false,false);
//					UpdateContact();
//				}
			};

			this.processingCancelButton.TouchUpInside += (sender, e) => {
				this.processingView.Hidden = true;
			};

		}


		public override void ViewWillDisappear (bool animated){
			base.ViewWillDisappear (animated);
		}


		public override void ViewWillAppear (bool animated){
			this.SetupContact ();
			this.InitProcessingView ();
			base.ViewWillAppear (animated);
		}


		public override void ViewDidLoad (){
			SetupButtons ();
			this.Title = "Contact Detail";
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			this.contactPictureImage.Layer.CornerRadius = 32.0f;
			base.ViewDidLoad ();
		}





	}
}

