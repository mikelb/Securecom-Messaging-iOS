using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Stext
{
	public interface ICustomCell
	{
		CustomCellTableSource tableSource { get; set; }
		NSIndexPath indexPath { get; set; }

		bool canMove{get; set;}
		bool shouldIndent{get; set;}
		bool canDelete {get; set;}

		float getHeight();

		bool AllowsDelete();
	}
}

