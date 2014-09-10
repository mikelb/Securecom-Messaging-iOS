using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext{

	public partial class SettingsHeader : ACustomCell{

		public static readonly UINib Nib = UINib.FromName ("SettingsHeader", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("SettingsHeader");

		public SettingsHeader (IntPtr handle) : base (handle){}

		public static SettingsHeader Create (){
			return (SettingsHeader)Nib.Instantiate (null, null) [0];
		}

		public override float getHeight(){
			return 21.0f;
		}

		public void SetSubheader(string subheader){
			this.headerLabel.Hidden = true;
			this.subheaderLabel.Hidden = false;
			this.subheaderLabel.Text = subheader;
		}

		public void SetHeader(string header){
			this.subheaderLabel.Hidden = true;
			this.headerLabel.Hidden = false;
			this.headerLabel.Text = header;
		}

	}
}

