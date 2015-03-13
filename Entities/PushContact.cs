using System;

namespace Stext
{
	public class PushContact
	{
		[SQLite.PrimaryKey, SQLite.AutoIncrement]
		public int ID { get; set; }
		public string Number { get; set; }
	}
}

