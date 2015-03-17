using System;
using System.Collections.Generic;
using System.IO;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Securecom.Messaging;
using Securecom.Messaging.Entities;
using Securecom.Messaging.Utils;
using System.Text.RegularExpressions;

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
			Console.WriteLine("rkolli >>>>> @FinishedLaunching");
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
			//MessageManager.SendMessage(MessageManager.PrepareOutgoingMessage("hello again", "a@b.com"));
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

			using (var conn = new SQLite.SQLiteConnection(AppDelegate._pathToContactsDatabase)) {
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
		}

		/// <Docs>Reference to the UIApplication that invoked this delegate method.</Docs>
		/// <summary>
		/// Indicates that the application received a remote notification.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="userInfo">User info.</param>
		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			Console.WriteLine("rkolli >>>>> @ReceivedRemoteNotification");
			if (userInfo != null) {
				ProcessNotification(userInfo, false);
			}
		}

		public static String ProcessIncomingMessage(String msg)
		{
			String result = MessageManager.ProcessIncomingMessage(msg);
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
			Console.WriteLine("rkolli >>>>> @ProcessNotification");

			if (!options.ContainsKey(new NSString("aps")))
				return;

			UIApplicationState state = UIApplication.SharedApplication.ApplicationState;
			NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;
           
			if (options.ContainsKey(new NSString("m"))) {
				NSString m = options.ObjectForKey(new NSString("m")) as NSString;
				try { //do something with the message
					String payload = m.ToString();
					String msg = ProcessIncomingMessage(payload);
					updateChatThread(payload, msg);
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
			Console.WriteLine("rkolli >>>>> @updateChatThread, payload = "+payload);
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
						String temp = pct.Number;
						temp = temp.Replace("(", string.Empty);
						temp = temp.Replace(")", string.Empty);
						temp = temp.Replace("-", string.Empty);
						temp = Regex.Replace(temp, @"\s", "");
						if (temp.Equals(sender)) {
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

				var pmessage = new PushMessage{Thread_id = present_thread_id, Number = sender, TimeStamp = CurrentTimeMillis(), TimeStamp_Sent = Convert.ToInt64(messageid), Read = 1, Message = msg, Status = true, Service = "Push"};
				conn.Insert(pmessage);
				conn.Commit();
				conn.Close();
			}
			RefreshChatListView();
//			UIAlertView alert = new UIAlertView("New Securecom Message ", msg, null, "Ok");
//			alert.Show();
		}

		public static long CurrentTimeMillis()
		{
			return (long) (DateTime.UtcNow -  new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		}
	}
}
