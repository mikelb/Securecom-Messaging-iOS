
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

						Console.WriteLine("rkolli @ STATE_REGISTERING_WITH_SERVER");

						// Do Directory sync
						try{
							RefreshPushDirectory();
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

		public static void RefreshPushDirectory(){
			AddressBook book = new AddressBook();
			List<String> contactlist = new List<String>();
			var phoneUtil = PhoneNumberUtil.GetInstance();
			book.RequestPermission().ContinueWith(t => {
				if (!t.Result) {
					Console.WriteLine("Permission denied by user or manifest");
					return;
				}
				Console.WriteLine("rkolli >>>>> @RefreshPushDirectory, Address book count = " + book.Count());

				foreach (Contact contact in book.OrderBy(c => c.LastName)) {
					if (String.IsNullOrEmpty(contact.DisplayName))
						continue;
					foreach (Phone value in contact.Phones) {
						if (value.Number.Contains("*") || value.Number.Contains("#"))
							continue;
						Console.WriteLine("phone number: " + value.Number);
						try {
							contactlist.Add(phoneUtil.Format(phoneUtil.Parse(value.Number, "US"), PhoneNumberFormat.E164));
						} catch (Exception e) {
						}
					}
					foreach (Email value in contact.Emails) {
						contactlist.Add(value.Address);
					}

				}
				Dictionary<string,string> tokens = getDirectoryServerTokenDictionary(contactlist);
				List<String> list = new List<String>();
				foreach (string key in tokens.Keys)
					list.Add(key);
				Console.WriteLine("intersecting " + list);
				List<String> response = MessageManager.RetrieveDirectory(list);
				List<String> result = new List<String>();
				foreach (string key in response) {
					if (tokens[key] != null)
						result.Add(tokens[key]);
				}
				Console.WriteLine("rkolli >>>>> we're here 1");
				// Figure out where the SQLite database will be.
				using (var conn= new SQLite.SQLiteConnection(AppDelegate._pathToContactsDatabase))
				{
					Console.WriteLine("rkolli >>>>> we're here 2");
					foreach(String contact in result){
						Console.WriteLine("we're here, push contact = " + contact);
						var pcontact = new PushContact{Number = contact};
						conn.Insert(pcontact);
					}
				}

				Console.WriteLine("rkolli >>>>> we're here 3");

			}, TaskScheduler.Current);
		}

		public static Dictionary<string,string> getDirectoryServerTokenDictionary(List<string> e164numbers)
		{
			Dictionary<string,string> tokenDictionary = new Dictionary<string,string>(e164numbers.Count());
			foreach (String number in e164numbers) {
				try {
					String tokenWithPadding = getDirectoryServerToken(number);
					string token = tokenWithPadding.Substring(0, tokenWithPadding.Length - 2);
					tokenDictionary.Add(token, number);
				} catch (Exception e) {

				}
			}
			return tokenDictionary;
		}

		private static string getDirectoryServerToken(string e164number)
		{
			byte[] number = Encoding.Default.GetBytes(e164number);
			SHA1 sha1 = SHA1.Create();
			byte[] hash = Utils.Split(sha1.ComputeHash(number), 0, 10);
			return Encoding.ASCII.GetString(Base64.Encode(hash));
		}

		private void SetContent()
		{
			String title = (appDelegate.registrationView.registerMode == appDelegate.MODE_REGISTER_EMAIL) ? "Email Verification" : "Phone Verification";
			NavigationItem.TitleView = StextUtil.SetTitleBarImage(title, 10, 40);
			confCodeInput.Text = "";
		}
	}
}

