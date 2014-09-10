using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext{

	public partial class ContactKeysCell : ACustomCell{

		public static readonly UINib Nib = UINib.FromName ("ContactKeysCell", NSBundle.MainBundle);

		public static readonly NSString Key = new NSString ("ContactKeysCell");

		public ContactKeysCell (IntPtr handle) : base (handle){}

		public static ContactKeysCell Create (){
			return (ContactKeysCell)Nib.Instantiate (null, null) [0];
		}

		public override float getHeight(){
			return 44.0f;
		}

		public void SetHeader(string header){
			this.headerLabel.Text = header;
		}

		public void SetSubHeader(string subheader){
			this.subHeaderLabel.Text = subheader;
		}

	}
}

