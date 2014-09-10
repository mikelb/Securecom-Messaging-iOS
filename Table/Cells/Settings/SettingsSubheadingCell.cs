using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext{

	public partial class SettingsSubheadingCell : ACustomCell{

		public static readonly UINib Nib = UINib.FromName ("SettingsSubheadingCell", NSBundle.MainBundle);

		public static readonly NSString Key = new NSString ("SettingsSubheadingCell");

		public SettingsSubheadingCell (IntPtr handle) : base (handle){}

		public static SettingsSubheadingCell Create (){
			return (SettingsSubheadingCell)Nib.Instantiate (null, null) [0];
		}

		public override float getHeight(){
			return 21.0f;
		}

	}
}

