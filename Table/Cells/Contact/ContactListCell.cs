using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace Stext{


	public partial class ContactListCell : ACustomCell{


		public static readonly UINib Nib = UINib.FromName ("ContactListCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("ContactListCell");
		public ContactListCell (IntPtr handle) : base (handle){}
		public float height = 100.0f;

		public const int STATE_NOT_REGISTERED = 0;
		public const int STATE_REGISTERED = 2;
		public const int STATE_PENDING = 1;
		public int registeredState = STATE_NOT_REGISTERED;

		public string name;
		public string mobile;
		public string _email;

		public static ContactListCell Create (){
			return (ContactListCell)Nib.Instantiate (null, null) [0];
		}

		public void SetName(string name){
			this.name = name;
			this.nameLabel.Text = name;
		}

		public void SetEmail(string value){
			if (value == null) {
				this.email.Enabled = false;
				return;
			}
			this._email = value;
			this.email.Enabled = true;
			this.emailLabel.Text = value;
		}

		public void SetPhone(string value){
			if (value == null) {
				this.phone.Enabled = false;
				return;
			}
			this.mobile = value;
			this.phone.Enabled = true;
			this.phoneLabel.Text = value;
		}

		public override float getHeight(){
			return height;
		}

		public void SetIconChecked(){
			registeredState = STATE_REGISTERED;
			icon.Image = UIImage.FromFile ("Images/Contact/greenlock.png");
		}

		public void SetIconLocked(){
			registeredState = STATE_PENDING;
			icon.Image = UIImage.FromFile ("Images/Contact/lock.png");
		}



	}
}

