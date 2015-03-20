using System;

namespace Stext
{
	public class PushChatThread
	{
		[SQLite.PrimaryKey, SQLite.AutoIncrement]
		public int ID { get; set; }
		[SQLite.Unique]
		public string Number {get; set; }
		public int Recipient_id { get; set; }
		public long TimeStamp { get; set; }
		public int Message_count {get; set; }
		public string Snippet {get; set; }
		public int Read {get; set; }
		public string Type{ get; set; }
		public string DisplayName{ get; set; }
	}
}

