using System;

namespace Stext
{
	public class PushMessage
	{
		[SQLite.PrimaryKey, SQLite.AutoIncrement]
		public int ID { get; set; }
		public int Thread_id { get; set; }
		public string Number { get; set; }
		public long TimeStamp { get; set; }
		public long TimeStamp_Sent { get; set; }
		public int Read {get; set; }
		public string Message{ get; set; }
		public bool Status{ get; set; }
		public string Service{ get; set; }
	}
}

