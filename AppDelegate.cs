using System;
using System.Collections.Generic;
using System.IO;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Securecom.Messaging;
using PhoneNumbers;
using Securecom.Messaging.Net;

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
		public static string _dbPath = Path.Combine(documents, "Messages.db");

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
			config.Save();

			//System.Security.Cryptography.X509Certificates.X509Certificate certificate = new System.Security.Cryptography.X509Certificates.X509Certificate("signing-ca-1.crt");
			//_manager = new MessageManager(new Uri(config.ServerUrl), phoneNumber, null, certificate);
		}

		public override UIWindow Window { get; set; }

		public static String GetCountryCode()
		{
			return "US"; //"CH"; //"IN";
		}

		public override void FinishedLaunching(UIApplication application)
		{
			Window = new UIWindow(UIScreen.MainScreen.Bounds);
			rootNavigationController = new UINavigationController();
			alert = new Alert();
			InitViews();

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

		private UIViewController GetLaunchView()
		{
			if (config.Registered)
				return chatListView;
			return registrationView;
		}

		private void RefreshChatListView()
		{
			chatListView.PopulateTable();
			chatView.refreshChat();

		}

		public void GoToView(UIViewController view)
		{
			if (this.rootNavigationController == null)
				return;
			try {
				bool popView = false;
				foreach (var item in rootNavigationController.ViewControllers) {
					if (item.Equals(view)) {
						rootNavigationController.PopToViewController(view, true);
						popView = true;
						break;
					}
				}
				if (!popView)
					rootNavigationController.PushViewController(view, true);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
		}

		private void InitViews()
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
			using (var conn = new SQLite.SQLiteConnection(_dbPath)) {
				conn.CreateTable<PushChatThread>();
				conn.CreateTable<PushMessage>();
				conn.CreateTable<PushContact>();
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
			if (config.Registered) {
				Console.WriteLine("re-registering apn id with the server");
				String dt = DeviceToken;
				dt = dt.Replace("<", String.Empty).Replace(">", String.Empty).Replace(" ", String.Empty);
				MessageManager.RegisterApnId(dt);
			}

		}

		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			if (userInfo != null) {
				ProcessNotification(userInfo, false);
			}
		}


		private void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
		{

			if (!options.ContainsKey(new NSString("aps")))
				return;

			UIApplicationState state = UIApplication.SharedApplication.ApplicationState;
           
			if (options.ContainsKey(new NSString("m"))) {
				NSString m = options.ObjectForKey(new NSString("m")) as NSString;
				try { //do something with the message
					IncomingMessage message = MessageManager.ReadPushMessage(m.ToString());
					if (message.Receipt)
						alert.showAlert("Receipt", "got receipt for " + message.MessageId);
					else
						updateChatThread(message, MessageManager.ProcessIncomingMessage(message));
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

		private void updateChatThread(IncomingMessage message, string msg)
		{
			// Figure out where the SQLite database will be.
			var conn = new SQLite.SQLiteConnection(_dbPath);
			String number = message.Sender;
			// Check if there is an existing thread for this sender

			PushChatThread thread = conn.FindWithQuery<PushChatThread>("select * from PushChatThread where Number = ?", number);
			if (thread != null) {
				conn.Execute("UPDATE PushChatThread Set Snippet = ?, TimeStamp = ?, Message_count = ?, Read = ?, Type = ? WHERE ID = ?", msg, message.MessageId, 1, 1, "Push", thread.ID);
				conn.Commit();
			} else {
				PushContact contact = conn.FindWithQuery<PushContact>("select * from PushContact where Number = ?", number);
				thread = new PushChatThread {
					DisplayName = contact.Name,
					Number = number,
					Recipient_id = 0,
					TimeStamp = Convert.ToInt64(message.MessageId),
					Message_count = 1,
					Snippet = msg,
					Read = 1,
					Type = "Push"
				};
				conn.Insert(thread);
				//conn.Execute("UPDATE PushChatThread Set Recipient_id = ? WHERE Number = ?", present_thread_id, sender);
			}

			var pmessage = new PushMessage {
				Thread_id = thread.ID,
				Number = number,
				TimeStamp = CurrentTimeMillis(),
				TimeStamp_Sent = Convert.ToInt64(message.MessageId),
				Read = 1,
				Message = msg,
				Status = true,
				Service = "Push"
			};
			conn.Insert(pmessage);
			conn.Commit();
			conn.Close();
			RefreshChatListView();
		}

		public static long CurrentTimeMillis()
		{
			return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		}
	}
}
