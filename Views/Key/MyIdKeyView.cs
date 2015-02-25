using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace Stext{


	public partial class MyIdKeyView : UIViewController{


		AppDelegate appDelegate;
		private float btnCornerRadius = 5.0f;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;


		public MyIdKeyView () : base ("MyIdKeyView", null){}


		public override void ViewDidLoad (){
			this.Title = "My ID Key";
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			NavigationItem.SetRightBarButtonItem (scanButton,false);
			this.SetupButtons ();
			base.ViewDidLoad ();
		}


		private void ScanAction(){
			try{
				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();
				actionSheet.AddButton ("Scan to compare");
				actionSheet.AddButton ("Get Scanned to compare");		
				actionSheet.AddButton ("Cancel");	
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == (0)) {

					} else {

					} 
				};
				actionSheet.ShowInView (View);
			}catch(Exception ex){
				Console.Write(ex.Message);
			}
		}


		public override void ViewWillAppear (bool animated){
			this.PopulateTable ();
		}


		public void PopulateTable(){

			List<CustomCellGroup> cellGroups = new List<CustomCellGroup> ();
			tableCellGroup = new CustomCellGroup ();
			cellGroups.Add (tableCellGroup);

			for (int x = 0; x <= 8; x++) {
				ContactKeysCell chatCell = ContactKeysCell.Create ();
				chatCell.SetHeader("John Doe (" + x + ")");
				chatCell.SetSubHeader("XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX XX");
				tableCellGroup.Cells.Add (chatCell);
			}

			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;

		}

		public void RowSelected(UITableView tableView, NSIndexPath indexPath){
			ContactKeysCell selectedCell = (ContactKeysCell)source.CellGroups[indexPath.Section].Cells[indexPath.Row];
			this.Title = selectedCell.headerLabel.Text;
			//this.contactKey.Text = selectedCell.subHeaderLabel.Text;
			ShowView (contactKeyView);
		}



		private void ShowView(UIView view){
			myIdKeyView.Hidden = true;
			contactKeysView.Hidden = true;
			contactKeyView.Hidden = true;
			view.Hidden = false;
		}


		private void SetupButtons(){

			myIdKeyButton.Layer.CornerRadius = btnCornerRadius;
			contactKeysButton.Layer.CornerRadius = btnCornerRadius;

			scanButton.Clicked += (sender, e) => {
				ScanAction();
			};

			contactKeysButton.TouchUpInside += (sender, e) => {
				ShowView(contactKeysView);
			};

			myIdKeyButton.TouchUpInside += (sender, e) => {
				ShowView(myIdKeyView);
			};

			keysButton.Enabled = false;

			chatsButton.Clicked += (sender, e) => {
				this.appDelegate.GoToView(this.appDelegate.chatListView);
			};

			settingsButton.Clicked += (sender, e) => {
				this.appDelegate.GoToView(this.appDelegate.settingsView);
			};

		}

	}
}

