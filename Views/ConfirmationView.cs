
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading;

using Securecom.Messaging;
using Securecom.Messaging.Utils;
using Securecom.Messaging.Spec;
using Securecom.Messaging.Net;
using Securecom.Messaging.Entities;

using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.IO;

namespace Stext{

	public partial class ConfirmationView : UIViewController{
	

		AppDelegate appDelegate;

		private const int STATE_CONNECTING = 1;
		private const int STATE_SMS_VERIFICATION = 2;
		private const int STATE_GENERATING_KEYS = 3;
		private const int STATE_REGISTERING_WITH_SERVER = 4;
		private const int STATE_FINISHED = 5;
		private const int STATE_GO_TO_CHATVIEW = 6;
		private const int NAP_TIME = 2000;

		private int STATE = 1;


		public ConfirmationView () : base ("ConfirmationView", null){}


		public override void ViewDidAppear (bool animated){
			SetContent ();
		}


		public override void ViewWillAppear (bool animated){
			STATE = STATE_CONNECTING;
			base.ViewWillAppear (animated);
			InitProcessingView ();
		}


		private void SetCurrentState(int state){

			if (STATE == STATE_CONNECTING) {
				label1.Hidden = false;
				spinner1.Hidden = false;
			}else if(STATE == STATE_SMS_VERIFICATION){
				img1.Hidden = false;
				spinner1.Hidden = true;
				spinner2.Hidden = false;
				label2.Hidden = false;
			}else if(STATE == STATE_GENERATING_KEYS){
				spinner2.Hidden = true;
				spinner3.Hidden = false;
				img2.Hidden = false;
				label3.Hidden = false;			
			}else if(STATE == STATE_REGISTERING_WITH_SERVER){
				spinner3.Hidden = true;
				spinner4.Hidden = false;
				img3.Hidden = false;
				label4.Hidden = false;
			}else if(STATE == STATE_FINISHED){
				spinner4.Hidden = true;
				img4.Hidden = false;
			}else if(STATE == STATE_GO_TO_CHATVIEW){
				appDelegate.GoToView (appDelegate.chatListView);
			}

		}


		private void InitProcessingView(){

			processingView.Hidden = true;
			processingView.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			processingView.Layer.Opacity = 1.0f;
			processingView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 80);
			processingViewRect.Layer.CornerRadius = 5.0f;
			processingViewRect.Frame = new RectangleF (0, 0, 280, 150);
			processingViewRect.Center = new PointF (UIScreen.MainScreen.Bounds.Width / 2, UIScreen.MainScreen.Bounds.Height / 2);
			processingViewRect.Layer.Opacity = 1.0f;

			label1.Hidden = true;
			spinner1.Hidden = true;
			img1.Hidden = true;

			label2.Hidden = true;
			spinner2.Hidden = true;
			img2.Hidden = true;

			label3.Hidden = true;
			spinner3.Hidden = true;
			img3.Hidden = true;

			label4.Hidden = true;
			spinner4.Hidden = true;
			img4.Hidden = true;

		}


		private void ShowProcessingView(){
			processingView.Hidden = false;
		}


		private void HideProcessingView(){
			processingView.Hidden = true;
		}


		public override void ViewDidLoad (){

			base.ViewDidLoad ();
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

			done.TouchUpInside += (sender, e) => {
				ShowProcessingView();

				StartProcessing();
			};

		}


		private void StartProcessing(){

			SetCurrentState(STATE_CONNECTING);
         String verificationCode = confCodeInput.Text;

			InvokeInBackground (delegate {
            MessageManager manager = appDelegate.MessageManager;
				while(STATE != STATE_GO_TO_CHATVIEW){

               if (STATE == STATE_CONNECTING)
               {
                  Thread.Sleep(NAP_TIME);
               } else if (STATE == STATE_SMS_VERIFICATION)
               {

                  KeyGenerator kg = new KeyGenerator();
                  String signalingKey = kg.GenerateSignalingKey();
                  int registrationId = kg.GenerateRegistrationID();

                  //manager.VerifyAccount(verificationCode, signalingKey, true, registrationId);

               } else
					   Thread.Sleep(NAP_TIME);

					STATE++;
					InvokeOnMainThread(delegate {
						SetCurrentState(STATE);
					});
				}
			});
		}


		private void SetContent(){
			if (appDelegate.registrationView.registerMode == appDelegate.MODE_REGISTER_EMAIL) {
				NavigationItem.TitleView = StextUtil.SetTitleBarImage ("Email Verification",10,40);
			} else {
				NavigationItem.TitleView = StextUtil.SetTitleBarImage ("Phone Verification",10,40);
			}
		}



	}
}

