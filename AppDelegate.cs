using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext{

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate{

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


		public override UIWindow Window {
			get;
			set;
		}

		public override void FinishedLaunching (UIApplication application){
			Window = new UIWindow (UIScreen.MainScreen.Bounds);
			rootNavigationController = new UINavigationController ();
			alert = new Alert ();
			InitViews();
			SetNavigationProperties ();
			GoToView(GetLaunchView());
			Window.RootViewController = rootNavigationController;
			Window.MakeKeyAndVisible();

		}

		public void SetNavigationProperties(){}

		private UIViewController GetLaunchView (){
			return registrationView;
			//return  chatView;
		}



		public void GoToView(UIViewController view){	

			try{	
				if (this.rootNavigationController != null) {
					Boolean PopView = false;
					var controllers = this.rootNavigationController.ViewControllers;

					foreach (var item in controllers) {

						if (item.Equals(view)) {

							rootNavigationController.PopToViewController(view,true);
							PopView = true;
							break;
						}		
					}

					if(!PopView){
						rootNavigationController.PushViewController(view,true);
					}
				}
			}catch(Exception e){
				WriteDebugOutput(e.Message);
			}

		}

		public void WriteDebugOutput(String outputString){
			Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")  + " - " + outputString);
		}


		public void InitViews(){
			registrationView = new RegistrationView ();
			verificationView = new VerificationView ();
			confirmationView = new ConfirmationView ();
			chatListView = new ChatListView ();
			chatView = new ChatView ();
			contactListView = new ContactListView ();
			contactDetailView = new ContactDetailView ();
			newGroupView = new NewGroupView ();
			myIdKeyView = new MyIdKeyView ();
			contactKeysView = new ContactKeysView ();
			settingsView = new SettingsView ();
		}

		public override void OnResignActivation (UIApplication application)
		{
		}

		public override void DidEnterBackground (UIApplication application)
		{
		}

		public override void WillEnterForeground (UIApplication application)
		{
		}

		public override void WillTerminate (UIApplication application)
		{
		}

	}
}

