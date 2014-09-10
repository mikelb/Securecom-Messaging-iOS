using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace Stext
{
	public class CustomCellGroup
	{
		public CustomCellGroup ()
		{
		}

		public string Name { get; set;}

		public string Footer {get; set;}

		public List<ICustomCell> Cells
		{
			get { return cells; }
			set { cells = value; }
		}
		protected List<ICustomCell> cells = new List<ICustomCell> ();
	}
}

