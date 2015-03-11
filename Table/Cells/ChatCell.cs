
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext{

	public partial class ChatCell : ACustomCell{

		public static readonly UINib Nib = UINib.FromName ("ChatCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("ChatCell");
		public ChatCell (IntPtr handle) : base (handle){}
		private int ThreadID;
		private String Number;

		public static ChatCell Create (){
			return (ChatCell)Nib.Instantiate (null, null) [0];
		}
	
		public void SetHeader(string header){
			this.labelHeader.Text = header;
		}

		public void SetAvatar(UIImage value){
			this.RoundAvatar (value);
		}

		public string GetHeader(){
			return this.labelHeader.Text;
		}

		public void SetThreadID(int value){
			this.ThreadID = value;
		}

		public int GetThreadID(){
			return this.ThreadID;
		}

		public void SetNumber(string value){
			this.Number = value;
		}

		public string GetNumber(){
			return this.Number;
		}

		public void SetSubheading(string subHeading){
			this.labelSubheader.Text = subHeading;
			this.labelSubheaderRead.Text = subHeading;
		}

		public void SetLabelTime(string time){
			this.labelTime.Text = time;
		}

		public override float getHeight(){
			return 64.0f;
		}

		public void RoundAvatar(UIImage value){
			this.avatar.Layer.MasksToBounds = true;
			this.avatar.Layer.CornerRadius = 20.0f;
			this.avatar.Image = value;
		}

		public void MarkAsRead(){
			this.labelSubheader.Hidden = true;
			this.labelSubheaderRead.Hidden = false;
		}


	}
}

