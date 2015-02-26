using System;
using System.IO;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Org.BouncyCastle.Crypto;

using Securecom.Messaging;
using Securecom.Messaging.ecc;
using Securecom.Messaging.Entities;
using Securecom.Messaging.Net;
using Securecom.Messaging.Spec;
using Securecom.Messaging.Utils;
using System.Collections.Generic;

namespace Stext
{

	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public UINavigationController rootNavigationController;
		public int MODE_REGISTER_PHONE = 0;
		public int MODE_REGISTER_EMAIL = 1;

		public Alert alert;

		public String DeviceToken;
		private static string documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		public static string _pathToMessagesDatabase = Path.Combine(documents, "Messages.db");
		public static string _pathToContactsDatabase = Path.Combine(documents, "Securecom_Contacts.db");


#region VIEWS

		public RegistrationView registrationView;
		public VerificationView verificationView;
		public ConfirmationView confirmationView;
		public ChatListView chatListView;
		public ChatView chatView;
		public ContactListView contactListView;
		public ContactDetailView contactDetailView;
		public NewGroupView newGroupView;
		public MyIdKeyView myIdKeyView;
		public ContactKeysView contactKeysView;
		public SettingsView settingsView;

#endregion

		private STextConfig config;

		public void CreateMessageManager(String phoneNumber)
		{
			config.MobileNumber = phoneNumber;
			config.Password = "3sApmcX4px5tp2b9dPH46lMI";
			config.SaveConfig();

			//System.Security.Cryptography.X509Certificates.X509Certificate certificate = new System.Security.Cryptography.X509Certificates.X509Certificate("signing-ca-1.crt");
			//_manager = new MessageManager(new Uri(config.ServerUrl), phoneNumber, null, certificate);
		}

		//private MessageManager _manager;

		//public MessageManager MessageManager {
		//	get{ return _manager; }
		//}

		public override UIWindow Window { get; set; }

		private NSDictionary userSelectedNotification = null;

		public override void FinishedLaunching(UIApplication application)
		{
			UIAlertView _alert = new UIAlertView("@FinishedLaunching", "HERE", null, "Ok");
			_alert.Show();

			Window = new UIWindow(UIScreen.MainScreen.Bounds);
			rootNavigationController = new UINavigationController();
			alert = new Alert();
			InitViews();

			SetNavigationProperties();
			STextConfig.Storage = new FileStorage();
			config = STextConfig.GetInstance();
			GoToView(GetLaunchView());
			Window.RootViewController = rootNavigationController;
			Window.MakeKeyAndVisible();

			if (UIApplication.SharedApplication.RespondsToSelector(new MonoTouch.ObjCRuntime.Selector("registerUserNotificationSettings:"))) {
				UIUserNotificationSettings settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
				UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications();
			} else {
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound);
			}
		}

		public void SetNavigationProperties()
		{
		}

		private UIViewController GetLaunchView()
		{
			//return registrationView;
			if (!config.registered)
				return registrationView;
			return chatListView;
		}

		private void RefreshChatListView(){
			chatListView.PopulateTable();
			chatView.refreshChat();

		}

		public void GoToView(UIViewController view)
		{
			try {
				if (this.rootNavigationController != null) {
					Boolean PopView = false;
					var controllers = this.rootNavigationController.ViewControllers;
					foreach (var item in controllers) {
						if (item.Equals(view)) {
							rootNavigationController.PopToViewController(view, true);
							PopView = true;
							break;
						}
					}
					if (!PopView) {
						rootNavigationController.PushViewController(view, true);
					}
				}
			} catch (Exception e) {
				WriteDebugOutput(e.Message);
			}
		}

		public void WriteDebugOutput(String outputString)
		{
			Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - " + outputString);
		}


		public void InitViews()
		{
			registrationView = new RegistrationView();
			verificationView = new VerificationView();
			confirmationView = new ConfirmationView();
			chatListView = new ChatListView();
			chatView = new ChatView();
			contactListView = new ContactListView();
			contactDetailView = new ContactDetailView();
			newGroupView = new NewGroupView();
			myIdKeyView = new MyIdKeyView();
			contactKeysView = new ContactKeysView();
			settingsView = new SettingsView();

			// Create and Initialize Apple push message DB
			// Figure out where the SQLite database will be.
			using (var conn= new SQLite.SQLiteConnection(_pathToMessagesDatabase))
			{
				Console.WriteLine("rkolli >>>>> Connect to Messages DB, if doesn't exist! create PushChatThread, PushMessage tables");
				conn.CreateTable<PushChatThread>();
				conn.CreateTable<PushMessage>();
			}
		}

		public override void OnResignActivation(UIApplication application) {}
		public override void DidEnterBackground(UIApplication application) {}
		public override void WillEnterForeground(UIApplication application) {}
		public override void WillTerminate(UIApplication application) {}

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			DeviceToken = new NSString(MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(
				new MonoTouch.ObjCRuntime.Class("NSString").Handle,
				new MonoTouch.ObjCRuntime.Selector("stringWithFormat:").Handle,
				new NSString("%@").Handle,
				deviceToken.Handle)).ToString();

			Console.WriteLine("RegisteredForRemoteNotifications where deviceToken=" + DeviceToken);
		}

		/// <Docs>Reference to the UIApplication that invoked this delegate method.</Docs>
		/// <summary>
		/// Indicates that the application received a remote notification.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="userInfo">User info.</param>
		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			if (userInfo != null) {
				ProcessNotification(userInfo, false);
			}
		}


		public  String RatchetBobLiveTest(PublicKey ourEphemPub, PrivateKey ourEphemPriv, PublicKey theirBaseKey, PublicKey theirEphemPub,
		                                  PublicKey ourIdPub, PrivateKey ourIdPrivate, PublicKey theirId, String emsg)
		{
			//AccountTests.SetupConfig ();
			STextConfig cfg = STextConfig.GetInstance();
			RecipientDevice rp = new RecipientDevice(13964, -1);

			SessionRecordV2 srv = new SessionRecordV2(cfg.MasterSecret, rp);
			srv.SessionState = new textsecure.SessionStructure();

			//IdentityKeyUtil iku = new IdentityKeyUtil ();

			KeyPair ourPair = new KeyPair(ourEphemPub, ourEphemPriv);

			Securecom.Messaging.Net.IdentityKey ik = new Securecom.Messaging.Net.IdentityKey(ourIdPub);
			Securecom.Messaging.Net.IdentityKeyPair kp = new Securecom.Messaging.Net.IdentityKeyPair(ik, ourIdPrivate);
			Securecom.Messaging.Net.IdentityKey tik = new Securecom.Messaging.Net.IdentityKey(theirId);

			RatchetingSession.InitializeSession(srv.SessionState, ourPair, theirBaseKey, ourPair, theirEphemPub, kp, tik);

			System.Console.WriteLine("CIpher Key = [" + Convert.ToBase64String(srv.SessionState.senderChain.messageKeys[0].cipherKey) + "]");
			System.Console.WriteLine("MAC Key = [" + Convert.ToBase64String(srv.SessionState.senderChain.messageKeys[0].macKey) + "]");

			ChainKey mck = new ChainKey();

			mck.Index = 0;
			mck.Key = theirEphemPub.Key();

			// new sender chain
			RootKey rk = new RootKey();
			//rk.Key = rootk.GetPublicKey ();

			// TODO: OK ?
			rk.Key = srv.SessionState.rootKey;

			Tuple<RootKey, ChainKey> senderTuple = rk.CreateChain(theirEphemPub, ourPair);
			ChainKey newSenderChain = senderTuple.Item2;
			RootKey newSenderRoot = senderTuple.Item1;

			System.Console.WriteLine("new Rook Key -" + Convert.ToBase64String(newSenderRoot.Key));
			Securecom.Messaging.ecc.PublicKey shouldbekey = new Securecom.Messaging.ecc.PublicKey(Convert.FromBase64String("PHuMFzEbhDA3sdMq6Bp0WXvwvVJtR2tL+aFzxHs2wtY="), CurveConst.DjbType);

			mck.Key = newSenderChain.Key;

			System.Console.WriteLine("NEW SEnder Chain key key: " + Convert.ToBase64String(newSenderChain.Key));

			byte[] shouldbechainkey = (new Securecom.Messaging.ecc.PublicKey(Convert.FromBase64String("vQouBvdRn/2+AhtnwRSfXa2U3AgOQspIknnrKAALHnU="), CurveConst.DjbType)).Key();

			MessageKeys mk = mck.MessageKeys;

			System.Console.WriteLine("CIPHER: " + Convert.ToBase64String(mk.CipherKey));

			IBufferedCipher ciph = KeyGenerator.GetAESWithCRT(mk, false);

			byte[] sdata = Convert.FromBase64String("kbXz7w==");
			byte[] thefinalmessage = ciph.DoFinal(Convert.FromBase64String(emsg));

			String message = Utils.FromBytes(thefinalmessage);

			System.Console.WriteLine("MESSAGE: " + message);
			return (message);
		}

		public void GregTest(String msg)
		{
			STextConfig cfg = STextConfig.GetInstance();
			byte[] sigkey = Convert.FromBase64String(cfg.AccountAttributes.SignalingKey);

			byte[] dec = Convert.FromBase64String(msg);

			byte[] mac = new byte[20];

			Array.Copy(sigkey, sigkey.Length - 20, mac, 0, 20);

			//		byte[] body = mc.VerifyMacBody (mac, dec);

			byte[] dmsg = new byte[dec.Length - 1 - 10];
			Array.Copy(dec, 1, dmsg, 0, dec.Length - 1 - 10);


			
			byte[] realsigkey = new byte[32];
			Array.Copy(sigkey, 0, realsigkey, 0, 32);

			byte[] decoded = MasterCipher.GetDecryptedBody(realsigkey, dmsg);
			MemoryStream ms = new MemoryStream(decoded);

			textsecure.IncomingPushMessageSignal ips = ProtoBuf.Serializer.Deserialize<textsecure.IncomingPushMessageSignal>(ms);

			Securecom.Messaging.Net.PreKeyWhisperMessageBaseImpl pmu = PreKeyWhisperMessageBaseImpl.FromSerialize(ips.message);

			/*
			RecipientDevice rp = new RecipientDevice(pmu.RegistrationId,-1);
			KeyExchangeProcessorV2 kep = new KeyExchangeProcessorV2 ();
			kep.MasterSecret = cfg.MasterSecret;
			SessionRecordV2 srv = new SessionRecordV2 (cfg.MasterSecret, rp);
			srv.SessionState = new textsecure.SessionStructure ();
			*/

			PreKeyRecord pkr = cfg.GetPreKey(pmu.PreKeyId);

			IdentityKeyUtil iku = new IdentityKeyUtil();

			System.Console.WriteLine("<==================>");
			System.Console.WriteLine("Our Ephem - " + Convert.ToBase64String(pkr.KeyPair.PublicKey.Serialize));
			System.Console.WriteLine("Our Prv Ephem - " + Convert.ToBase64String(pkr.KeyPair.PrivateKey.Serialize));
			System.Console.WriteLine("Their Base - " + Convert.ToBase64String(pmu.BaseKey.Serialize));
			System.Console.WriteLine("Sender Ephem - " + Convert.ToBase64String(pmu.WhisperMessage.SenderEphemeral.Serialize));

			System.Console.WriteLine("My IK- " + Convert.ToBase64String(iku.GetIdentityKeyPair(cfg.MasterSecret).PublicKey.PublicKey.Serialize));
			System.Console.WriteLine("My ID Priv  - " + Convert.ToBase64String(iku.GetIdentityKeyPair(cfg.MasterSecret).PrivateKey.Serialize));
			System.Console.WriteLine("Their IK - " + Convert.ToBase64String(pmu.IdentityKey.PublicKey.Serialize));

			System.Console.WriteLine("MSG: " + Convert.ToBase64String(pmu.WhisperMessage.CipherText));
			System.Console.WriteLine("<==================>");

			String result = RatchetBobLiveTest(pkr.KeyPair.PublicKey, 
				                pkr.KeyPair.PrivateKey, pmu.BaseKey, pmu.WhisperMessage.SenderEphemeral, iku.GetIdentityKeyPair(cfg.MasterSecret).PublicKey.PublicKey, 
				                iku.GetIdentityKeyPair(cfg.MasterSecret).PrivateKey, pmu.IdentityKey.PublicKey, Convert.ToBase64String(pmu.WhisperMessage.CipherText));
				
			UIAlertView alert = new UIAlertView("New Whisper Message: ", result, null, "Ok");
			alert.Show();
		}
			
		public static String ProcessIncomingMessage(String msg)
		{
			String result = Utils.FromBytes(MessageManager.ProcessIncomingMessage(msg));
			Console.WriteLine("result is " + result);
			return result;
		}

		/// <summary>
		/// Processes the notification.
		/// </summary>
		/// <param name="options">Options.</param>
		/// <param name="fromFinishedLaunching">If set to <c>true</c> from finished launching.</param>
		public void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			if (!options.ContainsKey(new NSString("aps")))
				return;

			UIApplicationState state = UIApplication.SharedApplication.ApplicationState;
			NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;
           
			//String alert = (aps[new NSString("alert")] as NSString).ToString();
			//String payload = (aps[new NSString("m")] as NSString).ToString();
			if (options.ContainsKey(new NSString("m"))) {
				NSString m = options.ObjectForKey(new NSString("m")) as NSString;
				try { //do something with the message
					String payload = m.ToString();
					String msg = ProcessIncomingMessage(payload);
					updateChatThread(payload, msg);
					//GregTest ( payload);
					//UIAlertView alert = new UIAlertView("New message", payload, null, "Ok");
					//alert.Show();
				} catch (Exception e) {
					Console.WriteLine("exception: " + e.Message);
					Console.WriteLine("stack trace: " + e.StackTrace);
				}
			} else {
				switch (state) {
				case UIApplicationState.Background:
					break;
				case UIApplicationState.Active:
					if (!fromFinishedLaunching) {
						//do something with the alert
					}
					break;
				case UIApplicationState.Inactive:
					break;
				}
			}
		}

		private void updateChatThread(string payload, string msg){
			const string sender_pattern = "\"sender\":\"";
			const string messageid_pattern = "\"messageId\":";
			int a = payload.IndexOf(sender_pattern) + sender_pattern.Length;
			int b = payload.IndexOf("\"", a);
			string sender = payload.Substring(a, b - a);

			int x = payload.IndexOf(messageid_pattern) + messageid_pattern.Length;
			int y = payload.IndexOf(",", x);
			string messageid = payload.Substring(x, y - x);
			Console.WriteLine("rkolli >>>>> @ProcessNotification, sender = "+sender+", messageid = "+messageid+", message body = "+msg);
			// Figure out where the SQLite database will be.
			using (var conn= new SQLite.SQLiteConnection(_pathToMessagesDatabase))
			{
				int present_thread_id = 0;
				// Check if there is an existing thread for this sender
				List<PushChatThread> pctList = conn.Query<PushChatThread>("select * from PushChatThread");

				if (pctList != null && pctList.Count > 0) {
					foreach (PushChatThread pct in pctList) {
						Console.WriteLine("rkolli >>>>> @updateChatThread"+", Count = " + pctList.Count);
						Console.WriteLine("rkolli >>>>> @updateChatThread"+", Number = " + pct.Number + ", Sender = " + sender + ", ID = " + pct.ID);
						if (pct.Number.Equals(sender)) {
							present_thread_id = pct.ID;
							Console.WriteLine("rkolli >>>>> @updateChatThread, updaing chat row, present_thread_id = " + present_thread_id);
							conn.Execute("UPDATE PushChatThread Set Snippet = ?, TimeStamp = ?, Message_count = ?, Read = ?, Type = ? WHERE ID = ?", msg, messageid, 1, 1, "Push", present_thread_id);
							Console.WriteLine("rkolli >>>>> @updateChatThread, update successful");
							conn.Commit();
							break;
						}
					}
				}

				if (present_thread_id == 0) {
					var pct_val = new PushChatThread{Number = sender, Recipient_id = 0, TimeStamp = Convert.ToInt64(messageid), Message_count = 1, Snippet = msg, Read = 1, Type = "Push"};
					conn.Insert(pct_val);
					present_thread_id = pct_val.ID;
					//conn.Execute("UPDATE PushChatThread Set Recipient_id = ? WHERE Number = ?", present_thread_id, sender);
					Console.WriteLine("rkolli >>>>> @updateChatThread, inserting new chat row, present_thread_id = " +present_thread_id);
				}
				Console.WriteLine("rkolli >>>>> inserting message into the DB");

				var pmessage = new PushMessage{Thread_id = present_thread_id, Number = sender, TimeStamp = CurrentTimeMillis(), TimeStamp_Sent = Convert.ToInt64(messageid), Read = 1, Message = msg, Service = "Push"};
				conn.Insert(pmessage);
				conn.Commit();
				conn.Close();
			}
			RefreshChatListView();
			UIAlertView alert = new UIAlertView("New Whisper Message ", msg, null, "Ok");
			alert.Show();
		}

		public static long CurrentTimeMillis()
		{
			return (long) (DateTime.UtcNow -  new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		}
	}
}
