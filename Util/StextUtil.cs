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

		// resize the image to be contained within a maximum width and height, keeping aspect ratio
		public static UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1) return sourceImage;
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext(new SizeF(width, height));
			sourceImage.Draw(new RectangleF(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}

		// resize the image (without trying to maintain aspect ratio)
		public static UIImage ResizeImage(UIImage sourceImage, float width, float height)
		{
			UIGraphics.BeginImageContext(new SizeF(width, height));
			sourceImage.Draw(new RectangleF(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}

		// crop the image, without resizing
		private static UIImage CropImage(UIImage sourceImage, int crop_x, int crop_y, int width, int height)
		{
			var imgSize = sourceImage.Size;
			UIGraphics.BeginImageContext(new SizeF(width, height));
			var context = UIGraphics.GetCurrentContext();
			var clippedRect = new RectangleF(0, 0, width, height);
			context.ClipToRect(clippedRect);
			var drawRect = new RectangleF(-crop_x, -crop_y, imgSize.Width, imgSize.Height);
			sourceImage.Draw(drawRect);
			var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return modifiedImage;
		}

	}
}

