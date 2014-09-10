using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace Stext{


	public class ChatBubbleCell : ACustomCell{

		public static NSString KeyLeft = new NSString ("BubbleElementLeft");
		public static NSString KeyRight = new NSString ("BubbleElementRight");
		public static UIImage bleft, bright, left, right, icon; 
		public static UIFont infoFont = UIFont.SystemFontOfSize(12);

		UIView view;
		UIView imageView;
		UIView undeliveredView;
		UIView imgView;
		UILabel label;
		UILabel senderLabel;
		UIFont font;
		UIButton undelivered;
		bool isLeft;

		static ChatBubbleCell (){
			bright = UIImage.FromFile ("Images/Chat/chat-bubble-right@2x.png");
			bleft = UIImage.FromFile ("Images/Chat/chat-bubble-left@2x.png");
			left = bleft.CreateResizableImage (new UIEdgeInsets (10, 16, 18, 26));
			right = bright.CreateResizableImage (new UIEdgeInsets (11, 11, 17, 18));
		}


		public ChatBubbleCell (bool isLeft) : base (UITableViewCellStyle.Default, isLeft ? KeyLeft : KeyRight){

			var rect = new RectangleF (0, 0, 1, 1);
			this.isLeft = isLeft;
			view = new UIView (rect);

			imageView = new UIImageView (isLeft ? left : right);
			view.AddSubview (imageView);

			font = UIFont.PreferredBody;
			infoFont = UIFont.PreferredCaption1;

			label = new UILabel (rect) {
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0,
				Font = font,
				TextColor = (isLeft ? UIColor.Black : UIColor.White),
				BackgroundColor = UIColor.Clear
			};

			view.AddSubview (label);

			senderLabel = new UILabel (rect) {
				Lines = 1,
				TextAlignment = (isLeft ? UITextAlignment.Left : UITextAlignment.Right),
				Font = infoFont,
				TextColor = UIColor.Gray,
				BackgroundColor = UIColor.Clear

			};

			view.AddSubview (senderLabel);

			icon = UIImage.FromFile ("Images/icons/lock_30.png");
			var imgRect = new RectangleF (0, 0, 15, 15);
			imgView = new UIImageView(icon);

			view.AddSubview (imgView);

			undelivered = new UIButton (UIButtonType.DetailDisclosure);
			undelivered.TintColor = UIColor.Red;
			undelivered.Hidden = true;
			view.AddSubview (undelivered);

			ContentView.Add (view);
			canDelete = true;

		}


		public void SetAsSent(){
			this.imageView.Alpha = 1.0f;
		}


		public void SetAsPending(){
			this.imageView.Alpha = 0.5f;
		}

		public override void LayoutSubviews (){

			base.LayoutSubviews ();

			var frame = ContentView.Frame;
			var textSize = GetSizeForText (this, label.Text, true);
			var frameSize = textSize + BubblePadding;
			imageView.Frame = new RectangleF (new PointF (isLeft ? 10 : frame.Width-frameSize.Width-10, frame.Y + HeaderPadding), frameSize);
			view.SetNeedsDisplay ();
			frame = imageView.Frame;
			label.Frame = new RectangleF (new PointF (frame.X + (isLeft ? 16 : 8), frame.Y + 6), textSize);

			var senderTextSize = GetSizeForText (this, senderLabel.Text, false);
			senderLabel.Frame = new RectangleF (new PointF((isLeft ? ContentView.Frame.X + 22 : ContentView.Frame.Width - (senderTextSize.Width + 25)), frame.Y + frameSize.Height - 6), senderTextSize);

			SizeF imgSize = new SizeF(25, 25);
			int sOffset = 18;

			if (senderLabel.Text.Length == 7) {
				sOffset = 22;
			}

			undelivered.Frame = new RectangleF(
				new PointF((isLeft ? 290 : 5), 
					frame.Y + frameSize.Height - 25), 
					imgSize);

			imgSize = new SizeF(10, 13);
			sOffset = 18;

			if (senderLabel.Text.Length == 7) {
				sOffset = 22;
			}

			imgView.Frame = new RectangleF(
				new PointF((isLeft ? ContentView.Frame.X + 12 : ContentView.Frame.Width - (senderTextSize.Width + sOffset) + 4), 
				frame.Y + frameSize.Height - 2), 
				imgSize);

		}

		static internal SizeF BubblePadding = new SizeF (22, 16);
		static internal float HeaderPadding = 8.0f;


		SizeF GetSizeForText (UIView tv, string text, bool splitLine){
			NSString nsText = new NSString (text);
			var textRect = nsText.GetBoundingRect (new SizeF (tv.Bounds.Width * .9f - 10 - 22, 99999), (splitLine ? NSStringDrawingOptions.UsesLineFragmentOrigin : 0), new UIStringAttributes { Font = font }, new NSStringDrawingContext ());
			return new SizeF (textRect.Width, textRect.Height);
		}

		public void SetAsUndelivered(){
			this.undelivered.Hidden = false;
		}

		public void Update (string text, string sender){
			label.Text = text;
			senderLabel.Text = sender;
			SetNeedsLayout ();
		}

		public override float getHeight(){
			return GetSizeForText (this, label.Text, true).Height + BubblePadding.Height + GetSizeForText(this, senderLabel.Text, false).Height + HeaderPadding;
		}

	}
}

