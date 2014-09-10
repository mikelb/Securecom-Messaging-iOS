using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.MessageUI;
using MonoTouch.Foundation;

namespace Stext{

	public static class StextUtil{


		public static Alert alert;


		public static void writeDebugOutput(String outputString){
			Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")  + " - " + outputString);
		}


		public static void OpenEmail(string EmailAddress, string subject , string body){
			try {
				string args = "";

				if(EmailAddress != null){

					if(subject != null){
						args += "&subject=" + subject;
					}

					if(subject != null){
						args += "&body=" + body;
					}

					NSUrl url = new NSUrl ("mailto:" + EmailAddress + args);

					if (!UIApplication.SharedApplication.OpenUrl (url)) {
						writeDebugOutput ("Scheme 'mailto:' is not supported on this device");
					}
				}
			} catch (Exception ex) {
				writeDebugOutput(ex.StackTrace);
			}
		}


		public static void OpenAppSettings(string app){
			try {
				if(app != null){
					NSUrl url = new NSUrl ("prefs://" + app);
					if (!UIApplication.SharedApplication.OpenUrl (url)) {
						writeDebugOutput ("Scheme 'prefs:' is not supported on this device");
					}
				}
			} catch (Exception ex) {
				writeDebugOutput(ex.StackTrace);
			}
		}



		public static void OpenDialerWithNumber(string DialNumber){
			try {
				if(DialNumber != null){
					NSUrl url = new NSUrl ("telprompt:" + DialNumber);
					if (!UIApplication.SharedApplication.OpenUrl (url)) {
						writeDebugOutput ("Scheme 'tel:' is not supported on this device");
					}
				}
			} catch (Exception ex) {
				writeDebugOutput(ex.StackTrace);
			}
		}


		public static MFMessageComposeViewController CreateSimpleSMSView(string[] recipients, string body){
			MFMessageComposeViewController smsView = new MFMessageComposeViewController();
			smsView.Recipients = recipients;
			if (body != null) {
				smsView.Body = body;
			}
			smsView.MessageComposeDelegate = new SmsSimpleDelegate();
			return smsView;
		}


		public class SmsSimpleDelegate : MFMessageComposeViewControllerDelegate{
			public override void Finished (MFMessageComposeViewController controller, MessageComposeResult result){
				controller.DismissModalViewControllerAnimated (true);
			}
		}


		public static UIView SetTitleBarImage(string title, int imgX, int titleX){

			UIView navView;
			UIImageView navImage;
			UILabel navTitle;
			UIImageView navImageView;

			navView = new UIView (new RectangleF (0, 0, 300, 40));	
			navTitle = new UILabel (new RectangleF (titleX, 0, 300, 40));
			navTitle.Text = title;
			navTitle.TextColor = UIColor.Black;
			navImage = new UIImageView (new RectangleF (imgX, 8, 25, 25));					
			navImage.Image = UIImage.FromFile ("Images/Titlebar/chat.png");
			navImageView = new UIImageView (navImage.Image);
			navImageView.Layer.CornerRadius = 5.0f;
			navImageView.Layer.MasksToBounds = true;
			navView.AddSubview (navTitle);
			navView.AddSubview (navImage);

			return navView;
		}


	}
}

