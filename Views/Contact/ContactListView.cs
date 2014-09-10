
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Threading;
using MonoTouch.MessageUI;

namespace Stext{

	public partial class ContactListView : UIViewController{


		AppDelegate appDelegate;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;
		private List<CustomCellGroup> cellGroups;

		#region entity fields
		public string name;
		public string mobile;
		public string email;
		#endregion

		public ContactListView () : base ("ContactListView", null){	}


		public override void ViewWillAppear (bool animated){
			this.Title = "Select Contact";
			this.PopulateTable ();
		}


		public override void DidReceiveMemoryWarning (){
			base.DidReceiveMemoryWarning ();
		}


		private void AddTableRefresh(){
			UIRefreshControl refreshControl = new UIRefreshControl ();
			refreshControl.ValueChanged += delegate(object sender, EventArgs e) {
				PerformUpdate(refreshControl.EndRefreshing);
			};
			table.AddSubview (refreshControl);
		}


		public void PerformUpdate(Action OnUpdateComplete){
			InvokeInBackground(delegate {
				try {
					Thread.Sleep(2000);
				}
				catch (Exception ex) {
					InvokeOnMainThread(delegate {
						appDelegate.alert.showErrorAlert(ex.Message);
						if (OnUpdateComplete != null)
							OnUpdateComplete();
					});
				}
				InvokeOnMainThread(delegate {
					PopulateTable();
					if (OnUpdateComplete != null)
						OnUpdateComplete();
				});
			});
		}


		public override void ViewDidLoad (){

			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

			NavigationItem.SetRightBarButtonItem (filterButton, false);
			NavigationItem.SetLeftBarButtonItem(cancelButton, false);

			AddTableRefresh ();
			ButtonActions();

			base.ViewDidLoad ();
		}


		private void ButtonActions(){

			filterButton.Clicked += (sender, e) => {
				FilterAction();
			};

		}

		private void FilterAction(){
			try{
				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();
				actionSheet.AddButton ("Stext users");
				actionSheet.AddButton ("Non-Stext users");
				actionSheet.AddButton ("Show All");
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


		private void AddTestData(){

			tableCellGroup = new CustomCellGroup { Name = "A" };
			cellGroups.Add (tableCellGroup);

			ContactListCell cell = ContactListCell.Create ();
			cell.SetName("Johny Appleseed");
			cell.SetEmail("jonny.appleseed@gmail.com");
			cell.SetPhone("+233665588");
			tableCellGroup.Cells.Add (cell);

			tableCellGroup = new CustomCellGroup { Name = "D" };
			cellGroups.Add (tableCellGroup);

			cell = ContactListCell.Create ();
			cell.SetName("John Doe");
			cell.SetEmail("jon.doe@gmail.com");
			cell.SetPhone("+123456789");
			cell.SetIconChecked ();
			tableCellGroup.Cells.Add (cell);

			tableCellGroup = new CustomCellGroup { Name = "S" };
			cellGroups.Add (tableCellGroup);

			cell = ContactListCell.Create ();
			cell.SetName("John Smith");
			cell.SetEmail("jon.smith@gmail.com");
			cell.SetPhone("+6502258866");
			cell.SetIconLocked ();
			tableCellGroup.Cells.Add (cell);

			tableCellGroup = new CustomCellGroup { Name = "V" };
			cellGroups.Add (tableCellGroup);

			cell = ContactListCell.Create ();
			cell.SetName("Juan Valdez");
			cell.SetEmail("jvaldez@msn.com");
			cell.SetPhone("+5588996655");
			cell.SetIconLocked ();
			tableCellGroup.Cells.Add (cell);

		}


		public void PopulateTable(){
			cellGroups = new List<CustomCellGroup> ();
			AddTestData ();
			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;
		}


		private void ShowPendingContactOptions(){
			try{
				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();
				actionSheet.AddButton ("Call " + this.mobile);
				actionSheet.AddButton ("SMS " + this.mobile);
				actionSheet.AddButton ("Email " + this.email);
				actionSheet.AddButton ("Edit " + this.name);
				actionSheet.AddButton ("Cancel");
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {

					if (b.ButtonIndex == (0)) {
						StextUtil.OpenDialerWithNumber(this.mobile);
					} else if (b.ButtonIndex == (1)) {
						string[] recipeints = new string[] { this.mobile }; 
						MFMessageComposeViewController smsView = StextUtil.CreateSimpleSMSView (recipeints, "");
					}else if (b.ButtonIndex == (2)) {
						StextUtil.OpenEmail(this.email,null , null);
					}else if (b.ButtonIndex == (3)) {
						this.appDelegate.GoToView(this.appDelegate.contactDetailView);
					} 

				};
				actionSheet.ShowInView (View);
			}catch(Exception ex){
				Console.Write(ex.Message);
			}
		}


		public void RowSelected(UITableView tableView, NSIndexPath indexPath){

			ContactListCell selectedCell = (ContactListCell)source.CellGroups[indexPath.Section].Cells[indexPath.Row];

			this.name = selectedCell.name;
			this.email = selectedCell.email;
			this.mobile = selectedCell.mobile;

			if (selectedCell.registeredState == ContactListCell.STATE_REGISTERED) {
				this.appDelegate.GoToView (this.appDelegate.chatView);
			}else if (selectedCell.registeredState == ContactListCell.STATE_PENDING) {
				ShowPendingContactOptions ();
				//this.appDelegate.GoToView (this.appDelegate.contactDetailView);
			}else {
				ShowPendingContactOptions ();
				//this.appDelegate.GoToView (this.appDelegate.contactDetailView);
			}

			//this.appDelegate.alert.showOkAlert("Contact State",selectedCell.registeredState.ToString());
		}



	}
}

