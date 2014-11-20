using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


using Securecom.Messaging;
using Securecom.Messaging.Utils;
using Securecom.Messaging.Spec;
using Securecom.Messaging.Net;
using Securecom.Messaging.Entities;

using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.IO;

namespace Stext
{

    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {

        public UINavigationController rootNavigationController;
        public int MODE_REGISTER_PHONE = 0;
        public int MODE_REGISTER_EMAIL = 1;

        public Alert alert;

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

      public void CreateMessageManager(String phoneNumber)
      {
         FileStorage fs = new FileStorage ();
         STextConfig.Storage = fs;

         STextConfig cfg = STextConfig.GetInstance ();

         cfg.MobileNumber = phoneNumber;
         cfg.Password = "3sApmcX4px5tp2b9dPH46lMI";
         cfg.ServerUrl = PushServerUrl;

         cfg.SaveConfig ();

         X509Certificate certificate = new X509Certificate("signing-ca-1.crt");
         _manager = new MessageManager(new Uri(Stext.AppDelegate.PushServerUrl), phoneNumber, null, certificate);
      }
      private MessageManager _manager;

      public MessageManager MessageManager
      {
         get{ return _manager; }
      }

        public override UIWindow Window
        {
            get;
            set;
        }

        private NSDictionary userSelectedNotification = null;
        
        public static readonly string PushServerUrl =  "https://stext1.ftlnetworks.com:4443";
        
        public override void FinishedLaunching(UIApplication application)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            rootNavigationController = new UINavigationController();
            alert = new Alert();
            InitViews();
            SetNavigationProperties();
            GoToView(GetLaunchView());
            Window.RootViewController = rootNavigationController;
            Window.MakeKeyAndVisible();



            if (UIApplication.SharedApplication.RespondsToSelector(new MonoTouch.ObjCRuntime.Selector("registerUserNotificationSettings:")))
            {
                UIUserNotificationSettings settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound);
            }
        }

        public void SetNavigationProperties() { }


        private UIViewController GetLaunchView()
        {

           return registrationView;
           /*
           ApplicationPreferences preferences = new ApplicationPreferences();
           if (preferences.LocalNumber == null)
            return registrationView;
           return chatListView;
           return  chatView;*/
        }

       public void GoToView(UIViewController view)
        {

            try
            {
                if (this.rootNavigationController != null)
                {
                    Boolean PopView = false;
                    var controllers = this.rootNavigationController.ViewControllers;

                    foreach (var item in controllers)
                    {

                        if (item.Equals(view))
                        {

                            rootNavigationController.PopToViewController(view, true);
                            PopView = true;
                            break;
                        }
                    }

                    if (!PopView)
                    {
                        rootNavigationController.PushViewController(view, true);
                    }
                }
            }
            catch (Exception e)
            {
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
        }

        public override void OnResignActivation(UIApplication application)
        {
        }

        public override void DidEnterBackground(UIApplication application)
        {
        }

        public override void WillEnterForeground(UIApplication application)
        {
        }

        public override void WillTerminate(UIApplication application)
        {
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            DeviceToken = new NSString(MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(
               new MonoTouch.ObjCRuntime.Class("NSString").Handle,
               new MonoTouch.ObjCRuntime.Selector("stringWithFormat:").Handle,
               new NSString("%@").Handle,
               deviceToken.Handle)).ToString();

            Console.WriteLine("RegisteredForRemoteNotifications where deviceToken=" + DeviceToken);
        }

        public String DeviceToken;

        /// <Docs>Reference to the UIApplication that invoked this delegate method.</Docs>
        /// <summary>
        /// Indicates that the application received a remote notification.
        /// </summary>
        /// <param name="application">Application.</param>
        /// <param name="userInfo">User info.</param>
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            if (userInfo != null)
            {
                ProcessNotification(userInfo, false);
            }
        }

        /// <summary>
        /// Processes the notification.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="fromFinishedLaunching">If set to <c>true</c> from finished launching.</param>
        public void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        {
            UIApplicationState state = UIApplication.SharedApplication.ApplicationState;

            if (!options.ContainsKey(new NSString("aps")))
            {
                return;
            }

            NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;
            
           
            //String alert = (aps[new NSString("alert")] as NSString).ToString();
            //String payload = (aps[new NSString("m")] as NSString).ToString();
            if (options.ContainsKey(new NSString("m")))
            {
               NSString m = options.ObjectForKey(new NSString("m")) as NSString;
                try //do something with the message
                {
                   String payload = m.ToString();
                   UIAlertView alert = new UIAlertView("New message", payload, null, "Ok");
                   alert.Show();
                }
                catch (Exception e)
                {
                }
            }
            else
            {
                switch (state)
                {
                    case UIApplicationState.Background:
                        break;
                    case UIApplicationState.Active:
                        if (!fromFinishedLaunching)
                        {
                            //do something with the alert
                        }
                        break;
                    case UIApplicationState.Inactive:
                        break;
                }
            }
        }
    }
}

