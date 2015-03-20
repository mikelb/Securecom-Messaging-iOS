using System;

namespace Stext
{
	public class PushContact
	{
		[SQLite.PrimaryKey, SQLite.AutoIncrement]
		public int ID { get; set; }
		[SQLite.Unique]
		public string  Number { get; set; }
		public string  Name { get; set; }
	}
}

