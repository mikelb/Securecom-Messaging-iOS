using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext
{
	public class CustomCellTableSource : UITableViewSource
	{
		public AppDelegate appDelegate;

		public UITableView table { get; set; }

		public List<CustomCellGroup> CellGroups {
			get { return cellGroups; }
			set { cellGroups = value; }
		}

		public Boolean editable = true;
		public Boolean moveable = false;

		public RowSelectedDelegate RowSelectedAction { get; set; }

		public DeleteActionDelegate DeleteAction { get; set; }

		public string DeleteTitle { get; set; }

		protected List<CustomCellGroup> cellGroups = new List<CustomCellGroup>();

		public CustomCellTableSource(List<CustomCellGroup> cellGroupList)
		{
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			this.CellGroups = cellGroupList;
		}

#region -= data binding/display methods =-

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override int NumberOfSections(UITableView tableView)
		{
			return cellGroups.Count;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override int RowsInSection(UITableView tableview, int section)
		{
			return cellGroups[section].Cells.Count;
		}

		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
		public override string TitleForHeader(UITableView tableView, int section)
		{
			return cellGroups[section].Name;
		}

		/// <summary>
		/// Called by the TableView to retrieve the footer text for the particular section(group)
		/// </summary>
		public override string TitleForFooter(UITableView tableView, int section)
		{
			return cellGroups[section].Footer;
		}

#endregion

#region -= user interaction methods =-

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow(indexPath, true);
			if (RowSelectedAction != null) {
				RowSelectedAction(tableView, indexPath);
			}
		}

		public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
		{
			//Console.WriteLine("Row " + indexPath.Row.ToString() + " deselected");	
		}

		public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
		{
			//Console.WriteLine("Accessory for Section, " + indexPath.Section.ToString() + " and Row, " + indexPath.Row.ToString() + " tapped");
		}

#endregion



		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{

			ICustomCell cell = cellGroups[indexPath.Section].Cells[indexPath.Row];

			cell.tableSource = this;
			return (UITableViewCell)cell;
		}


		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			ICustomCell cell = cellGroups[indexPath.Section].Cells[indexPath.Row];

			return cell.getHeight();
		}

		public void ReloadTable()
		{
			InvokeOnMainThread(delegate() {
				if (table != null) {
					table.ReloadData();
				}
			});
		}

#region -= editing methods =-

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			ICustomCell cell = cellGroups[indexPath.Section].Cells[indexPath.Row];
			return cell.canDelete;
		}

		public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
		{
			//ICustomCell cell = cellGroups[indexPath.Section].Cells[indexPath.Row];
			//return cell.canMove;
			return false;
		}

		/// <summary>
		/// called by the table view when a row is moved.
		/// </summary>
		public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			var item = CellGroups[sourceIndexPath.Section].Cells[sourceIndexPath.Row];
			CellGroups[sourceIndexPath.Section].Cells.RemoveAt(sourceIndexPath.Row);
			CellGroups[destinationIndexPath.Section].Cells.Insert(destinationIndexPath.Row, item);
			//appDelegate.movedRows = true;

		}

		/// <summary>
		/// Called manually when the table goes into edit mode
		/// </summary>
		public void WillBeginTableEditing(UITableView tableView)
		{
			//---- start animations
			tableView.BeginUpdates();

			//---- insert a new row in the table
			tableView.InsertRows(new NSIndexPath[] { NSIndexPath.FromRowSection(tableView.NumberOfRowsInSection(0), 0) }, UITableViewRowAnimation.Fade);
		
			//---- end animations
			tableView.EndUpdates();
		}

		public void DidFinishTableEditing(UITableView tableView)
		{

		}

		public override bool ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
		{

			ICustomCell cell = CellGroups[indexPath.Section].Cells[indexPath.Row];

			return cell.shouldIndent;

		}



		public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
		{
			ICustomCell cell = CellGroups[indexPath.Section].Cells[indexPath.Row];
			return (cell.AllowsDelete()) ? UITableViewCellEditingStyle.Delete : UITableViewCellEditingStyle.None;
		}

		public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
		{
			return DeleteTitle;
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			switch (editingStyle) {
			case UITableViewCellEditingStyle.Delete:
				if (DeleteAction != null) {
					try {
						ICustomCell cell = CellGroups[indexPath.Section].Cells[indexPath.Row];
						//appDelegate.deleteRows = true;
						//appDelegate.deleteRowIndex = indexPath.Row;
						//appDelegate.deletedRow = cell;
						DeleteAction(tableView, indexPath);
						CellGroups[indexPath.Section].Cells.RemoveAt(indexPath.Row);
						tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);	
					} catch (Exception ex) {
						System.Console.Write(ex.StackTrace);
					}					
				}
				break;
			}
		}

		public override NSIndexPath CustomizeMoveTarget(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/guides/ios/application_fundamentals/delegates,_protocols,_and_events
			//throw new NotImplementedException ();
			if (sourceIndexPath.Section != proposedIndexPath.Section) {
				int row = 0;
				if (sourceIndexPath.Section < proposedIndexPath.Section) {
					row = tableView.NumberOfRowsInSection(sourceIndexPath.Section) - 1;
				}
				return NSIndexPath.FromRowSection(row, sourceIndexPath.Section);
			} else {
				return proposedIndexPath;
			}
		}

#endregion
	}

	public delegate void RowSelectedDelegate(UITableView tableView, NSIndexPath indexPath);
	public delegate void DeleteActionDelegate(UITableView tableView, NSIndexPath indexPath);
}

