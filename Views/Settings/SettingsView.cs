using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace Stext{

	public partial class SettingsView : UIViewController{

		AppDelegate appDelegate;
		private float btnCornerRadius = 5.0f;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;

		public SettingsView () : base ("SettingsView", null){}

		public override void DidReceiveMemoryWarning (){
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad (){
			this.Title = "Settings";
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			base.ViewDidLoad ();
		}

		public override void ViewWillAppear (bool animated){
			this.PopulateTable ();
		}

		public void PopulateTable(){

			List<CustomCellGroup> cellGroups = new List<CustomCellGroup> ();
			tableCellGroup = new CustomCellGroup ();
			cellGroups.Add (tableCellGroup);

			SettingsCell cell;
			SettingsHeader hCell;

			hCell = SettingsHeader.Create ();
			hCell.SetHeader("REGISTRATION");
			tableCellGroup.Cells.Add (hCell);

			cell = SettingsCell.Create ();
			cell.sectionDetail.Text = "Turn this off to unregister your account";
			tableCellGroup.Cells.Add (cell);

			hCell = SettingsHeader.Create ();
			hCell.SetHeader("NOTIFICATIONS");
			tableCellGroup.Cells.Add (hCell);

			hCell = SettingsHeader.Create ();
			hCell.SetSubheader("Notifications");
			tableCellGroup.Cells.Add (hCell);

			cell = SettingsCell.Create ();
			cell.sectionDetail.Text = "Display message notifications in the status bar";
			tableCellGroup.Cells.Add (cell);

			hCell = SettingsHeader.Create ();
			hCell.SetSubheader("In-thread notifications");
			tableCellGroup.Cells.Add (hCell);

			cell = SettingsCell.Create ();
			cell.sectionDetail.Text = "Display message notifications in status bar";
			tableCellGroup.Cells.Add (cell);

			hCell = SettingsHeader.Create ();
			hCell.SetHeader("INPUT SETTINGS");
			tableCellGroup.Cells.Add (hCell);

			hCell = SettingsHeader.Create ();
			hCell.SetSubheader("Enter sends");
			tableCellGroup.Cells.Add (hCell);

			cell = SettingsCell.Create ();
			cell.sectionDetail.Text = "Pressing the enter key will send text messages";
			tableCellGroup.Cells.Add (cell);

//			hCell = SettingsHeader.Create ();
//			hCell.SetHeader("APPEARANCE");
//			tableCellGroup.Cells.Add (hCell);
//
//			hCell = SettingsHeader.Create ();
//			hCell.SetSubheader("Theme");
//			tableCellGroup.Cells.Add (hCell);
//
//			hCell = SettingsHeader.Create ();
//			hCell.SetSubheader("Language");
//			tableCellGroup.Cells.Add (hCell);

			hCell = SettingsHeader.Create ();
			hCell.SetHeader("STORAGE");
			tableCellGroup.Cells.Add (hCell);

			hCell = SettingsHeader.Create ();
			hCell.SetSubheader("Delete old messages");
			tableCellGroup.Cells.Add (hCell);

			cell = SettingsCell.Create ();
			cell.sectionDetail.Text = "Automatically delete older messages once a conversation thread reaches a certain length";
			tableCellGroup.Cells.Add (cell);

//			hCell = SettingsHeader.Create ();
//			hCell.SetSubheader("Conversation limit length");
//			tableCellGroup.Cells.Add (hCell);
//
//			hCell = SettingsHeader.Create ();
//			hCell.SetSubheader("Trim all threads now");
//			tableCellGroup.Cells.Add (hCell);

			hCell = SettingsHeader.Create ();
			hCell.SetHeader("ADVANCED");
			tableCellGroup.Cells.Add (hCell);

			hCell = SettingsHeader.Create ();
			hCell.SetSubheader("Complete key exchanges");
			tableCellGroup.Cells.Add (hCell);

			cell = SettingsCell.Create ();
			cell.sectionDetail.Text = "Automatically complete key exchanges for new session or for existing sessions with the same identity key";
			tableCellGroup.Cells.Add (cell);

			hCell = SettingsHeader.Create ();
			hCell.SetSubheader("Screen security");
			tableCellGroup.Cells.Add (hCell);

			cell = SettingsCell.Create ();
			cell.sectionDetail.Text = "Disable screen security to allow screen shots";
			tableCellGroup.Cells.Add (cell);

			hCell = SettingsHeader.Create ();
			hCell.SetSubheader("Refresh Push Directory");
			tableCellGroup.Cells.Add (hCell);

			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;

		}


		public void RowSelected(UITableView tableView, NSIndexPath indexPath){
			//SettingsCell selectedCell = (SettingsCell)source.CellGroups[indexPath.Section].Cells[indexPath.Row];
		}


	}
}

