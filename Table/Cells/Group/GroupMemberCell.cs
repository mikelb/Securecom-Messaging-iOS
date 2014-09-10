using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext{

	public partial class GroupMemberCell : ACustomCell{

		public static readonly UINib Nib = UINib.FromName ("GroupMemberCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("GroupMemberCell");

		public GroupMemberCell (IntPtr handle) : base (handle){}

		public static GroupMemberCell Create (){
			return (GroupMemberCell)Nib.Instantiate (null, null) [0];
		}

		public override float getHeight(){
			return 44.0f;
		}

		public void SetName(string name){
			this.rightImage.Image = UIImage.FromFile ("Images/icons/lock@2x.png");
			this.leftImage.Layer.CornerRadius = 12.0f;
			this.textLabel.Text = name;
		}

	}
}

