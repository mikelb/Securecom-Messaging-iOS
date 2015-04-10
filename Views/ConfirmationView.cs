
using System;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using MonoTouch.UIKit;
using Org.BouncyCastle.Utilities.Encoders;
using PhoneNumbers;
using System.Threading.Tasks;
using System.Net;
using Securecom.Messaging;
using Securecom.Messaging.Net;
using Xamarin.Contacts;
using Securecom.Messaging.Utils;

namespace Stext
{

	public partial class ConfirmationView : UIViewController
	{
	

		AppDelegate appDelegate;

		private const int STATE_CONNECTING = 1;
		private const int STATE_SMS_VERIFICATION = 2;
		private const int STATE_GENERATING_KEYS = 3;
		private const int STATE_REGISTERING_WITH_SERVER = 4;
		private const int STATE_FINISHED = 5;
		private const int STATE_GO_TO_CHATVIEW = 6;
		private const int NAP_TIME = 2000;

		private int STATE = 1;

		public ConfirmationView()
			: base("ConfirmationView", null)
		{

		}

		public override void ViewDidAppear(bool animated)
		{
			SetContent();
		}

		public override void ViewWillAppear(bool animated)
		{
			STATE = STATE_CONNECTING;
			base.ViewWillAppear(animated);
			InitProcessingView();
		}
			
		private void SetCurrentState(int state)
		{
			switch (state) {
			case STATE_CONNECTING:
				label1.Hidden = false;
				spinner1.Hidden = false;
				break;
			case STATE_SMS_VERIFICATION:
				img1.Hidden = false;
				spinner1.Hidden = true;
				spinner2.Hidden = false;
				label2.Hidden = false;
				break;
			case STATE_GENERATING_KEYS:
				spinner2.Hidden = true;
				spinner3.Hidden = false;
				img2.Hidden = false;
				label3.Hidden = false;
				break;
			case STATE_REGISTERING_WITH_SERVER:
				spinner3.Hidden = true;
				spinner4.Hidden = false;
				img3.Hidden = false;
				label4.Hidden = false;
				break;
			case STATE_FINISHED:
				spinner4.Hidden = true;
				img4.Hidden = false;
				STextConfig cfg = STextConfig.GetInstance();
				cfg.Registered = true;
				cfg.Save();
				break;
			case STATE_GO_TO_CHATVIEW:
				appDelegate.GoToView(appDelegate.chatListView);
				break;
			}
		}
			
		private void InitProcessingView()
		{
			processingView.Hidden = true;
			processingView.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			processingView.Layer.Opacity = 1.0f;
			processingView.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 80);
			processingViewRect.Layer.CornerRadius = 5.0f;
			processingViewRect.Frame = new RectangleF(0, 0, 280, 150);
			processingViewRect.Center = new PointF(UIScreen.MainScreen.Bounds.Width / 2, UIScreen.MainScreen.Bounds.Height / 2);
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

		private void ShowProcessingView()
		{
			processingView.Hidden = false;
		}

		private void HideProcessingView()
		{
			processingView.Hidden = true;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			confCodeInput.Selected = true;

			done.TouchUpInside += (sender, e) => {
				confCodeInput.ShouldReturn += (textField) => { 
					textField.ResignFirstResponder();
					return true;
				};
				ShowProcessingView();
				StartProcessing();
			};
		}

		private void StartProcessing()
		{
			SetCurrentState(STATE_CONNECTING);
			String verificationCode = confCodeInput.Text;
			bool quitloop = false;

			InvokeInBackground(delegate {
				while (STATE != STATE_GO_TO_CHATVIEW) {
					switch (STATE) {
					default:
					case STATE_CONNECTING:
						Thread.Sleep(NAP_TIME);
						break;
					case STATE_SMS_VERIFICATION:
						STextConfig cfg = STextConfig.GetInstance();
						String signalingKey = cfg.AccountAttributes.SignalingKey;
						int registrationId = cfg.AccountAttributes.RegistrationId;
						try{
							MessageManager.VerifyAccount(verificationCode, signalingKey, true, registrationId);
						}catch(WebException we){
							InvokeOnMainThread(delegate {
								UIAlertView alert = new UIAlertView("Error!", "Invalid verification code!", null, "Ok");
								alert.Show();
								appDelegate.GoToView(appDelegate.verificationView);
								quitloop = true;
							});
						}
						break;
					case STATE_GENERATING_KEYS:
						//MasterSecretUtil masterSecretUtil = new MasterSecretUtil();
						//MasterSecret masterSecret = masterSecretUtil.GenerateMasterSecret("the passphase"); //TODO: make this into a user input
						//TODO: also need to generate asymmetricMasterSecrect as in the Java implementation.
						//IdentityKeyUtil identityKeyUtil = new IdentityKeyUtil();
						//identityKeyUtil.GenerateIdentityKeys(masterSecret);
						STextConfig st = STextConfig.GetInstance();
						MessageManager.RegisterPreKeys(st.IdentityKey, st.LastResortKey, st.Keys);
						break;
					case STATE_REGISTERING_WITH_SERVER:
						//set up for push notification
						String dt = appDelegate.DeviceToken;
						dt = dt.Replace("<", String.Empty).Replace(">", String.Empty).Replace(" ", String.Empty);
						MessageManager.RegisterApnId(dt);
						InvokeOnMainThread(delegate {
							ApplicationPreferences preference = new ApplicationPreferences();
							preference.LocalNumber = this.appDelegate.registrationView.PhPhoneNumberInput.Text;
						});

						// Do Directory sync
						try{
							StextUtil.RefreshPushDirectory();
						}catch(Exception e){
							Console.WriteLine("Exception Thrown At "+e.ToString());
						}
						break;
					}
					if(quitloop){
						break;
					}
					STATE++;
					InvokeOnMainThread(delegate {
						SetCurrentState(STATE);
					});
				}
			});
		}

		private void SetContent()
		{
			String title = (appDelegate.registrationView.registerMode == appDelegate.MODE_REGISTER_EMAIL) ? "Email Verification" : "Phone Verification";
			NavigationItem.TitleView = StextUtil.SetTitleBarImage(title, 10, 40);
			confCodeInput.Text = "";
		}
	}
}

