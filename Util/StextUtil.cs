using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.MessageUI;
using MonoTouch.Foundation;
using System.Collections.Generic;
using PhoneNumbers;
using Xamarin.Contacts;
using System.Linq;
using Securecom.Messaging;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using Securecom.Messaging.Utils;
using Org.BouncyCastle.Utilities.Encoders;

namespace Stext
{

	public static class StextUtil
	{


		public static Alert alert;


		public static void writeDebugOutput(String outputString)
		{
			Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - " + outputString);
		}


		public static void OpenEmail(string EmailAddress, string subject, string body)
		{
			try {
				string args = "";

				if (EmailAddress != null) {

					if (subject != null) {
						args += "&subject=" + subject;
					}

					if (subject != null) {
						args += "&body=" + body;
					}

					NSUrl url = new NSUrl("mailto:" + EmailAddress + args);

					if (!UIApplication.SharedApplication.OpenUrl(url)) {
						writeDebugOutput("Scheme 'mailto:' is not supported on this device");
					}
				}
			} catch (Exception ex) {
				writeDebugOutput(ex.StackTrace);
			}
		}


		public static void OpenAppSettings(string app)
		{
			try {
				if (app != null) {
					NSUrl url = new NSUrl("prefs://" + app);
					if (!UIApplication.SharedApplication.OpenUrl(url)) {
						writeDebugOutput("Scheme 'prefs:' is not supported on this device");
					}
				}
			} catch (Exception ex) {
				writeDebugOutput(ex.StackTrace);
			}
		}



		public static void OpenDialerWithNumber(string DialNumber)
		{
			try {
				if (DialNumber != null) {
					NSUrl url = new NSUrl("telprompt:" + DialNumber);
					if (!UIApplication.SharedApplication.OpenUrl(url)) {
						writeDebugOutput("Scheme 'tel:' is not supported on this device");
					}
				}
			} catch (Exception ex) {
				writeDebugOutput(ex.StackTrace);
			}
		}


		public static MFMessageComposeViewController CreateSimpleSMSView(string[] recipients, string body)
		{
			MFMessageComposeViewController smsView = new MFMessageComposeViewController();
			smsView.Recipients = recipients;
			if (body != null) {
				smsView.Body = body;
			}
			smsView.MessageComposeDelegate = new SmsSimpleDelegate();
			return smsView;
		}


		public class SmsSimpleDelegate : MFMessageComposeViewControllerDelegate
		{
			public override void Finished(MFMessageComposeViewController controller, MessageComposeResult result)
			{
				controller.DismissModalViewControllerAnimated(true);
			}
		}


		public static UIView SetTitleBarImage(string title, int imgX, int titleX)
		{

			UIView navView;
			UIImageView navImage;
			UILabel navTitle;
			UIImageView navImageView;

			navView = new UIView(new RectangleF(0, 0, 300, 40));	
			navTitle = new UILabel(new RectangleF(titleX, 0, 300, 40));
			navTitle.Text = title;
			navTitle.TextColor = UIColor.Black;
			navImage = new UIImageView(new RectangleF(imgX, 8, 25, 25));					
			navImage.Image = UIImage.FromFile("Images/Titlebar/chat.png");
			navImageView = new UIImageView(navImage.Image);
			navImageView.Layer.CornerRadius = 5.0f;
			navImageView.Layer.MasksToBounds = true;
			navView.AddSubview(navTitle);
			navView.AddSubview(navImage);

			return navView;
		}

		// resize the image to be contained within a maximum width and height, keeping aspect ratio
		public static UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1)
				return sourceImage;
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

		public static void RefreshPushDirectory()
		{
			Console.WriteLine("starting contacts sync");
			AddressBook book = new AddressBook();
			PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
			book.RequestPermission().ContinueWith(t => {
				if (!t.Result) {
					Console.WriteLine("Permission denied by user or manifest");
					return;
				}
				long now = Utils.CurrentTimeMillis();
				Dictionary<String, String> map = new Dictionary<String, String>();
				int i = 0, j = 0, k = 0;
				foreach (Contact contact in book) {
					if (String.IsNullOrEmpty(contact.DisplayName))
						continue;
					foreach (Phone phone in contact.Phones) {
						j++;
						if (phone.Number.Contains("*") || phone.Number.Contains("#"))
							continue;
						try {
							String number = phoneUtil.Format(phoneUtil.Parse(phone.Number, AppDelegate.GetCountryCode()), PhoneNumberFormat.E164);
							if (!map.ContainsKey(number))
								map.Add(number, contact.DisplayName);
						} catch (Exception e) {
							Console.WriteLine("Exception parsing/formatting '" + phone.Number + "': " + e.Message);
						}
					}
					foreach (Email email in contact.Emails) {
						k++;
						if (!map.ContainsKey(email.Address))
							map.Add(email.Address, contact.DisplayName);
					}
					i++;
				}
				Console.WriteLine(i + " contacts in address book with " + j + " phone numbers and " + k + " email addresses");
				Dictionary<String, String> tokens = hashNumbers(map.Keys.ToList());
				List<String> response = MessageManager.RetrieveDirectory(tokens.Keys.ToList());
				Console.WriteLine("found " + response.Count + " securecom users");
				using (var conn = new SQLite.SQLiteConnection(AppDelegate._dbPath)) {
					conn.BeginTransaction();
					conn.Execute("DELETE FROM PushContact");
					foreach (String key in response) {
						String number = tokens[key];
						if (number == null) // is this needed?
							continue;
						conn.Insert(new PushContact { Number = number, Name = map[number] });
					}
					conn.Commit();

				}
				Console.WriteLine("contacts sync finished, took " + ((Utils.CurrentTimeMillis() - now) / 1000.0) + " seconds");
			}, TaskScheduler.Current);
		}

		private static Dictionary<String, String> hashNumbers(List<String> numbers)
		{
			SHA1 sha1 = SHA1.Create();
			Dictionary<String, String> result = new Dictionary<String, String>(numbers.Count());
			foreach (String number in numbers) {
				try {
					byte[] hash = Utils.Split(sha1.ComputeHash(Encoding.Default.GetBytes(number)), 0, 10);
					result.Add(Encoding.ASCII.GetString(Base64.Encode(hash)).Substring(0, 14), number);
				} catch (Exception e) {}
			}
			return result;
		}

		public static void UpdateThreadNames()
		{
			var conn = new SQLite.SQLiteConnection(AppDelegate._dbPath);
			List<PushChatThread> pct = conn.Query<PushChatThread>("SELECT * FROM PushChatThread where DisplayName is null or DisplayName = ''");
			PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
			AddressBook book = new AddressBook();
			foreach (PushChatThread thread in pct) {
				String display_name = null;
				foreach (Contact c in book) {
					if (thread.Number.Contains("@")) {
						if (!c.Emails.Any())
							continue;
						foreach (Email e in c.Emails) {
							if (thread.Number.Equals(e.Address)) {
								display_name = c.DisplayName;
								break;
							}
						}
					} else {
						if (!c.Phones.Any())
							continue;
						foreach (Phone p in c.Phones) {
							if (p.Number.Contains("*") || p.Number.Contains("#"))
								continue;
							try {
								String number = phoneUtil.Format(phoneUtil.Parse(p.Number, AppDelegate.GetCountryCode()), PhoneNumberFormat.E164);
								if (thread.Number.Equals(number)) {
									display_name = c.DisplayName;
									break;
								}
							} catch (Exception e) {
								Console.WriteLine("Exception parsing/formatting '" + p.Number + "': " + e.Message);
							}
						}
					}
					if (display_name != null) {
						conn.Execute("UPDATE PushChatThread Set DisplayName = ? WHERE ID = ?", display_name, thread.ID);
						break;
					}
				}
			}
			conn.Commit();
			conn.Close();
		}
	}
}

