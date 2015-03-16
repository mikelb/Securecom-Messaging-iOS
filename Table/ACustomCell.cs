using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Stext
{
	public abstract class ACustomCell : UITableViewCell, ICustomCell
	{
		public CustomCellTableSource tableSource { get; set; }
		public NSIndexPath indexPath { get; set; }
		public bool canMove{get; set;}
		public bool shouldIndent{get; set;}
		public bool canDelete {get; set;}
	

		protected void ReloadTable()
		{
			if (tableSource != null) {
				tableSource.ReloadTable ();
			}
		}

		public ACustomCell(UITableViewCellStyle style, NSString identifier) : base(style, identifier)
		{
			canMove = true;
			shouldIndent = true;
			canDelete = true;
		}
	
		public ACustomCell (IntPtr handle) : base (handle){
			canMove = true;
			shouldIndent = true;
			canDelete = true;
		}

		public abstract float getHeight();

	
		public bool AllowsDelete() {
			return canDelete;
		}
	
	}
}

