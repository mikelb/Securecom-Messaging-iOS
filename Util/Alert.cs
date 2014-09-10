using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace Stext{

	public class Alert{
	
		public Alert (){}


		public void showErrorAlert(string msg){
			showOkAlert("Error", msg);	
		}

		public void showOkAlert (string msg, string subMsg){
			var alert = new UIAlertView (msg, subMsg , null, "OK");
			alert.Show ();
		}

		public void showAlert (string msg, string subMsg){
			var alert = new UIAlertView (msg, subMsg , null, "Cancel", "OK");
			alert.Show ();
		}

		public UIAlertView _showOkAlert (string msg, string subMsg){
			var alert = new UIAlertView (msg, subMsg , null, "Cancel", "OK");
			return alert;
		}

		public UIAlertView showOkCancelAlert (string msg, string subMsg){
			var alert = new UIAlertView (msg, subMsg , null, "OK", "Cancel");
			return alert;
		}

		public UIAlertView showCustomCancelAlert (string msg, string subMsg, string buttonTitle){
			var alert = new UIAlertView (msg, subMsg , null, buttonTitle, "Cancel");
			return alert;
		}


		public UIAlertView showInputAlert (string msg,string subMsg){
			var alert = new UIAlertView (msg, subMsg, null, "OK","Cancel");
			alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
			return alert;
		}

		public UIAlertView showQuickAlert(string whichAlertNumber){
			var alert = new UIAlertView ("", "The " + whichAlertNumber + " situation in your Situations list appears here so you can send its alert from the home screen with one tap.", null, "Setup","Cancel");
			//alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
			return alert;
		}

		public class LoadingView : UIAlertView{
			
			private UIActivityIndicatorView _activityView;
			public void Show(string title){
				Title = title;
				_activityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
				_activityView.Frame = new RectangleF(150, 150, 30, 30);
				_activityView.StartAnimating();
				AddSubview(_activityView);
				Show();
				// Spinner - add after Show() or we have no Bounds.
				/* _activityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
				_activityView.Frame = new RectangleF((Bounds.Width / 2) - 15, Bounds.Height - 50, 30, 30);
				_activityView.StartAnimating();
				AddSubview(_activityView); */
			}
			public void Hide(){
				DismissWithClickedButtonIndex(0, true);
			}
		}
	
	}
}

