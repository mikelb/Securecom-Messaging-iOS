
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace Stext{


	public partial class ChatListView : UIViewController{


		AppDelegate appDelegate;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;

		public ChatListView () : base ("ChatListView", null){}


		public override void ViewDidLoad (){
		
			base.ViewDidLoad ();
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

			ShowEditButton ();

			this.composeButton.Clicked += (sender, e) => {
				ComposeAction();
			};
		
			this.editButton.Clicked += (sender, e) => {
				ShowDoneButton();
			};

			this.doneButton.Clicked += (sender, e) => {
				ShowEditButton();
			};

			this.keysButton.Clicked += (sender, e) => {
				this.appDelegate.GoToView(this.appDelegate.myIdKeyView);
			};


			this.markAllReadButton.Clicked += (sender, e) => {
				this.appDelegate.alert.showOkAlert("Mark All Read","Marking all chat messages as read.");
			};

		}


		public void RowSelected(UITableView tableView, NSIndexPath indexPath){

			ICustomCell selectedCell = source.CellGroups[indexPath.Section].Cells[indexPath.Row];

			appDelegate.GoToView (appDelegate.chatView);

		}


		private void ShowDoneButton(){
			NavigationItem.SetLeftBarButtonItem (doneButton, false);
			NavigationItem.SetRightBarButtonItem (markAllReadButton,false);
			this.table.SetEditing (true, true);
		}


		private void ShowEditButton(){
			NavigationItem.SetLeftBarButtonItem (editButton, false);
			NavigationItem.SetRightBarButtonItem (composeButton,false);
			this.table.SetEditing (false, true);
		}

		public override void ViewWillAppear (bool animated){
			this.Title = "Chats";
			this.PopulateTable ();
		}


		public void PopulateTable(){

			List<CustomCellGroup> cellGroups = new List<CustomCellGroup> ();
			tableCellGroup = new CustomCellGroup ();
			cellGroups.Add (tableCellGroup);

			for (int x = 0; x <= 1; x++) {
				ChatCell chatCell = ChatCell.Create ();
				chatCell.SetHeader("John Doe");
				chatCell.SetSubheading("My latest message");
				chatCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				tableCellGroup.Cells.Add (chatCell);
			}

			for (int x = 0; x <= 1; x++) {
				ChatCell chatCell = ChatCell.Create ();
				chatCell.SetHeader("Jane Doe");
				chatCell.SetSubheading("Her latest message");
				chatCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				chatCell.MarkAsRead ();
				tableCellGroup.Cells.Add (chatCell);
			}

			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;

			table.Source = source;

		}


		private void ComposeAction(){

			try{

				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();

				actionSheet.AddButton ("New Chat");
				actionSheet.AddButton ("New Group");		
				actionSheet.AddButton ("Cancel");		

				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == (0)) {

					} else if (b.ButtonIndex == (1)) {
						this.appDelegate.GoToView(this.appDelegate.newGroupView);
					} 
				};
				actionSheet.ShowInView (View);
			}catch(Exception ex){
				Console.Write(ex.Message);
			}
		}


	}
}

