
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext{

	public partial class ChatCell : ACustomCell{

		public static readonly UINib Nib = UINib.FromName ("ChatCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("ChatCell");
		public ChatCell (IntPtr handle) : base (handle){}

		public static ChatCell Create (){
			return (ChatCell)Nib.Instantiate (null, null) [0];
		}
	
		public void SetHeader(string header){
			this.labelHeader.Text = header;
			this.RoundAvatar ();
		}

		public void SetSubheading(string subHeading){
			this.labelSubheader.Text = subHeading;
			this.labelSubheaderRead.Text = subHeading;
		}

		public override float getHeight(){
			return 44.0f;
		}

		public void RoundAvatar(){
			this.avatar.Layer.MasksToBounds = true;
			this.avatar.Layer.CornerRadius = 20.0f;
		}

		public void MarkAsRead(){
			this.labelSubheader.Hidden = true;
			this.labelSubheaderRead.Hidden = false;
		}


	}
}

