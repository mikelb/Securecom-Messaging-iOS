using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext{


	public partial class SettingsCell : ACustomCell{
	
		public static readonly UINib Nib = UINib.FromName ("SettingsCell", NSBundle.MainBundle);

		public static readonly NSString Key = new NSString ("SettingsCell");

		public SettingsCell (IntPtr handle) : base (handle){}

		public static SettingsCell Create (){
			return (SettingsCell)Nib.Instantiate (null, null) [0];
		}

		public override float getHeight(){
			return 59.0f;
		}

	}
}

