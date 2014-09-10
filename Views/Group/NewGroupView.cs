using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace Stext{


	public partial class NewGroupView : UIViewController{


		AppDelegate appDelegate;

		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;

		private CustomCellGroup acTableCellGroup;
		private CustomCellTableSource acSource;

		private UIBarButtonItem backButton = null;

		public NewGroupView () : base ("NewGroupView", null){}


		private void InitProcessingView(){
			processingView.Hidden = true;
			processingView.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			processingView.Layer.Opacity = 1.0f;
			processingView.BackgroundColor = UIColor.FromRGBA (0, 0, 0, 80);
			processingRectView.Layer.CornerRadius = 5.0f;
			processingRectView.Frame = new RectangleF (0, 0, 280, 150);
			processingRectView.Center = new PointF (UIScreen.MainScreen.Bounds.Width / 2, UIScreen.MainScreen.Bounds.Height / 3);
			processingRectView.Layer.Opacity = 1.0f;
		}


		public override void ViewWillAppear (bool animated){
			InitProcessingView ();
			PopulateTable ();
			PopulateAddContactTable ();
		}


		private void AddTestData(){
			for (int x = 0; x <= 10; x++) {
				GroupMemberCell cell = GroupMemberCell.Create ();
				cell.SetName ("George Lund");
				cell.removeButton.TouchUpInside += (sender, e) => {
					this.appDelegate.alert.showOkAlert("Remove Contact: " + cell.TextLabel.Text ,"");
				};
				tableCellGroup.Cells.Add (cell);
			}
		}


		private void AddAcTestData(List<CustomCellGroup> cellGroups){

			acTableCellGroup = new CustomCellGroup{ Name = "A" };
			cellGroups.Add (acTableCellGroup);

			GroupMemberCell cell = GroupMemberCell.Create ();
			cell.SetName ("Jonny Appleseed");
			cell.removeButton.Hidden = true;
			acTableCellGroup.Cells.Add (cell);

			acTableCellGroup = new CustomCellGroup{ Name = "B" };
			cellGroups.Add (acTableCellGroup);

			cell = GroupMemberCell.Create ();
			cell.SetName ("Jen Bowman");
			cell.removeButton.Hidden = true;
			acTableCellGroup.Cells.Add (cell);

			acTableCellGroup = new CustomCellGroup{ Name = "C" };
			cellGroups.Add (acTableCellGroup);

			cell = GroupMemberCell.Create ();
			cell.SetName ("Jackie Chan");
			cell.removeButton.Hidden = true;
			acTableCellGroup.Cells.Add (cell);

			acTableCellGroup = new CustomCellGroup{ Name = "D" };
			cellGroups.Add (acTableCellGroup);

			cell = GroupMemberCell.Create ();
			cell.SetName ("Matt Duke");
			cell.removeButton.Hidden = true;
			acTableCellGroup.Cells.Add (cell);

			acTableCellGroup = new CustomCellGroup{ Name = "F" };
			cellGroups.Add (acTableCellGroup);

			cell = GroupMemberCell.Create ();
			cell.SetName ("Jeff Franco");
			cell.removeButton.Hidden = true;
			acTableCellGroup.Cells.Add (cell);

			acTableCellGroup = new CustomCellGroup{ Name = "S" };
			cellGroups.Add (acTableCellGroup);

			cell = GroupMemberCell.Create ();
			cell.SetName ("David Smith");
			cell.removeButton.Hidden = true;
			acTableCellGroup.Cells.Add (cell);

			acTableCellGroup = new CustomCellGroup{ Name = "T" };
			cellGroups.Add (acTableCellGroup);

			cell = GroupMemberCell.Create ();
			cell.SetName ("Ryan Tillman");
			cell.removeButton.Hidden = true;
			acTableCellGroup.Cells.Add (cell);

		}


		public void PopulateAddContactTable(){
			List<CustomCellGroup> cellGroups = new List<CustomCellGroup> ();
			AddAcTestData (cellGroups);
			acSource = new CustomCellTableSource(cellGroups);
			acSource.RowSelectedAction = ContactRowSelected;
			addContactTable.Source = acSource;
		}


		public void PopulateTable(){
			List<CustomCellGroup> cellGroups = new List<CustomCellGroup> ();
			tableCellGroup = new CustomCellGroup ();
			cellGroups.Add (tableCellGroup);
			AddTestData ();
			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;
		}


		public void ContactRowSelected(UITableView tableView, NSIndexPath indexPath){
			//GroupMemberCell selectedCell = (GroupMemberCell)source.CellGroups[indexPath.Section].Cells[indexPath.Row];
			//this.appDelegate.alert.showOkAlert("Selected Contact: " + selectedCell.TextLabel.Text,"");
		}


		public void RowSelected(UITableView tableView, NSIndexPath indexPath){
			//GroupMemberCell selectedCell = (GroupMemberCell)source.CellGroups[indexPath.Section].Cells[indexPath.Row];
			//this.appDelegate.alert.showOkAlert("Selected Contact: " + selectedCell.TextLabel.Text,"");
		}


		private void DismissKeyboard (){
			this.search.ResignFirstResponder ();
			this.View.EndEditing(true);
		}


		public override void ViewWillDisappear (bool animated){
			this.addContactView.Hidden = true;
		}


		private void SetupUiElements(){

			this.Title = "New Group";
			groupImage.Layer.CornerRadius = 25.0f;
			backButton = NavigationController.NavigationBar.BackItem.BackBarButtonItem;
			//NavigationItem.SetLeftBarButtonItem (cancelButton,false);
			NavigationItem.SetRightBarButtonItem (doneButton,false);
						
			filterButton.Clicked += (sender, e) => {
				appDelegate.alert.showOkAlert("Filter Contacts","");
			};

			addMemberButton.TouchUpInside += (sender, e) => {
				this.addContactView.Hidden = false;
				//NavigationItem.SetLeftBarButtonItem (null,false);
				this.Title = "Add contact";
				NavigationItem.SetLeftBarButtonItem (null,false);
				NavigationItem.SetRightBarButtonItem (filterButton,false);
				DismissKeyboard();
			};

			contactCancelButton.Clicked += (sender, e) => {
				this.addContactView.Hidden = true;
				//NavigationItem.SetLeftBarButtonItem (cancelButton,false);
				this.Title = "New Group";
				NavigationItem.SetLeftBarButtonItem (backButton,false);
				NavigationItem.SetRightBarButtonItem (doneButton,false);
				DismissKeyboard();
			};

		}


		public override void ViewDidLoad (){
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			SetupUiElements ();
			base.ViewDidLoad ();
		}


	}
}

