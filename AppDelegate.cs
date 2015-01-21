using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


using Securecom.Messaging;
using Securecom.Messaging.Utils;
using Securecom.Messaging.Spec;
using Securecom.Messaging.Net;
using Securecom.Messaging.Entities;
using Securecom.Messaging.Utils;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;

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


		public  String RatchetBobLiveTest ( Securecom.Messaging.Spec.IECPublicKey ourEphemPub, Securecom.Messaging.Spec.IECPrivateKey ourEphemPriv, Securecom.Messaging.Spec.IECPublicKey theirBaseKey, Securecom.Messaging.Spec.IECPublicKey theirEphemPub,
			Securecom.Messaging.Spec.IECPublicKey ourIdPub, Securecom.Messaging.Spec.IECPrivateKey ourIdPrivate, Securecom.Messaging.Spec.IECPublicKey theirId, String emsg )
		{

			//AccountTests.SetupConfig ();
			STextConfig cfg = STextConfig.GetInstance ();


			RecipientDevice rp = new RecipientDevice(13964,-1);


			SessionRecordV2 srv = new SessionRecordV2 (cfg.MasterSecret, rp);
			srv.SessionState = new textsecure.SessionStructure ();

			//IdentityKeyUtil iku = new IdentityKeyUtil ();


			ECKeyPair ourPair = new ECKeyPair ();
			ourPair.PrivateKey = ourEphemPriv;
			ourPair.PublicKey = ourEphemPub;

			IdentityKeyPair kp = new IdentityKeyPair ();
			kp.PrivateKey = ourIdPrivate;
			IdentityKey ik = new IdentityKey ();
			ik.PublicKey = ourIdPub;
			kp.PublicKey = ik;


			IdentityKey tik = new IdentityKey ();
			tik.PublicKey = theirId;

			RatchetingSession rs = new RatchetingSession ();

			rs.InitializeSession (srv.SessionState, ourPair, theirBaseKey, ourPair, theirEphemPub, kp, tik);

			System.Console.WriteLine ("CIpher Key = [" + Convert.ToBase64String (srv.SessionState.senderChain.messageKeys [0].cipherKey) +"]");
			System.Console.WriteLine ("MAC Key = [" + Convert.ToBase64String (srv.SessionState.senderChain.messageKeys [0].macKey) +"]");


			ChainKey mck = new ChainKey ();

			mck.Index = 0;
			mck.Key = theirEphemPub.GetPublicKey();


			// new sender chain
			RootKey rk = new RootKey ();
			//rk.Key = rootk.GetPublicKey ();

			// TODO: OK ?
			rk.Key = srv.SessionState.rootKey;

			Tuple<IRootKey, IChainKey> senderTuple = rk.CreateChain (theirEphemPub, ourPair);
			IChainKey newSenderChain = senderTuple.Item2;
			IRootKey newSenderRoot = senderTuple.Item1;

			System.Console.WriteLine ("new Rook Key -" + Convert.ToBase64String (newSenderRoot.Key));
			Securecom.Messaging.ecc.PublicKey shouldbekey = new Securecom.Messaging.ecc.PublicKey (Convert.FromBase64String ("PHuMFzEbhDA3sdMq6Bp0WXvwvVJtR2tL+aFzxHs2wtY="), CurveConst.DjbType);

			mck.Key = newSenderChain.Key;

			System.Console.WriteLine ("NEW SEnder Chain key key: " + Convert.ToBase64String (newSenderChain.Key));


			byte[] shouldbechainkey = (new Securecom.Messaging.ecc.PublicKey (Convert.FromBase64String ("vQouBvdRn/2+AhtnwRSfXa2U3AgOQspIknnrKAALHnU="), CurveConst.DjbType)).GetPublicKey();

			MessageKeys mk = mck.MessageKeys;

			System.Console.WriteLine ("CIPHER: " + Convert.ToBase64String (mk.CipherKey));

			KeyGenerator kg = new KeyGenerator ();

			IBufferedCipher ciph = kg.GetAESWithCRT (mk, false);

			byte[] sdata = Convert.FromBase64String ("kbXz7w==");
			byte[] thefinalmessage = ciph.DoFinal (Convert.FromBase64String ( emsg));

			String message = Utils.FromBytes (thefinalmessage);


			System.Console.WriteLine ("MESSAGE: " + message);
			return ( message );
		}


		public void GregTest (String msg )
		{
			STextConfig cfg = STextConfig.GetInstance ();
			MasterCipher mc = new MasterCipher (cfg.MasterSecret);
			byte[] sigkey = Convert.FromBase64String (cfg.AccountAttributes.SignalingKey);

			byte[] dec = Convert.FromBase64String (msg);

			byte[] mac = new byte[20];

			Array.Copy (sigkey, sigkey.Length - 20, mac, 0, 20);

	//		byte[] body = mc.VerifyMacBody (mac, dec);


			byte[] dmsg = new byte[dec.Length-1-10];
			Array.Copy (dec, 1, dmsg,0, dec.Length-1-10);


			
			byte[] realsigkey = new byte[32];

			Array.Copy (sigkey, 0, realsigkey,0,  32);

			byte[] decoded = mc.GetDecryptedBody (realsigkey, dmsg);

			MemoryStream ms = new MemoryStream (decoded);

			textsecure.IncomingPushMessageSignal ips = ProtoBuf.Serializer.Deserialize<textsecure.IncomingPushMessageSignal> (ms);

			Securecom.Messaging.Net.PreKeyWhisperMessageBaseImpl pmu = new PreKeyWhisperMessageBaseImpl ();
			pmu.FromSerialize (ips.message);

			/* RecipientDevice rp = new RecipientDevice(pmu.RegistrationId,-1);


			KeyExchangeProcessorV2 kep = new KeyExchangeProcessorV2 ();
			kep.MasterSecret = cfg.MasterSecret;


			SessionRecordV2 srv = new SessionRecordV2 (cfg.MasterSecret, rp);
			srv.SessionState = new textsecure.SessionStructure ();
*/

			IPreKeyRecord pkr = cfg.GetPreKey (pmu.PreKeyId);

			IdentityKeyUtil iku = new IdentityKeyUtil ();

			RatchetingSession rs = new RatchetingSession ();


			System.Console.WriteLine ("<==================>");
			System.Console.WriteLine ("Our Ephem - " + Convert.ToBase64String(pkr.KeyPair.PublicKey.Serialize));
			System.Console.WriteLine ("Our Prv Ephem - " + Convert.ToBase64String(pkr.KeyPair.PrivateKey.Serialize));


			System.Console.WriteLine ("Their Base - " + Convert.ToBase64String(pmu.BaseKey.Serialize));
			System.Console.WriteLine ("Sender Ephem - " + Convert.ToBase64String(pmu.WhisperMessage.SenderEphemeral.Serialize));

			System.Console.WriteLine ("My IK- " + Convert.ToBase64String(iku.GetIdentityKeyPair (cfg.MasterSecret).PublicKey.PublicKey.Serialize));
			System.Console.WriteLine ("My ID Priv  - " + Convert.ToBase64String(iku.GetIdentityKeyPair (cfg.MasterSecret).PrivateKey.Serialize));
			System.Console.WriteLine ("Their IK - " + Convert.ToBase64String(pmu.IdentityKey.PublicKey.Serialize));

			System.Console.WriteLine ("MSG: " + Convert.ToBase64String (pmu.WhisperMessage.CipherText));
			System.Console.WriteLine ("<==================>");


			String result = RatchetBobLiveTest (pkr.KeyPair.PublicKey, 
												pkr.KeyPair.PrivateKey, pmu.BaseKey, pmu.WhisperMessage.SenderEphemeral, iku.GetIdentityKeyPair (cfg.MasterSecret).PublicKey.PublicKey, 
				iku.GetIdentityKeyPair (cfg.MasterSecret).PrivateKey, pmu.IdentityKey.PublicKey, Convert.ToBase64String(pmu.WhisperMessage.CipherText));


			UIAlertView alert = new UIAlertView("New Whisper Message: ", result, null, "Ok");
			alert.Show();


			return;



	
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

					GregTest ( payload);

                   //UIAlertView alert = new UIAlertView("New message", payload, null, "Ok");
                   //alert.Show();
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

