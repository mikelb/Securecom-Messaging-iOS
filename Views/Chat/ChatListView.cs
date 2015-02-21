
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

using Xamarin.Contacts;
using System.Threading.Tasks;
using System.Linq;
using PhoneNumbers;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Utilities.Encoders;
using Securecom.Messaging;
using System.IO;
using MonoTouch.AddressBook;

namespace Stext
{


	public partial class ChatListView : UIViewController
	{


		AppDelegate appDelegate;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;

		public ChatListView()
			: base("ChatListView", null)
		{
		}


		public override void ViewDidLoad()
		{
			Console.WriteLine("rkolli >>>>> @HERE1");
			base.ViewDidLoad();
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

			ShowEditButton();

			this.composeButton.Clicked += (sender, e) => {
				ComposeAction();
			};
		
			this.editButton.Clicked += (sender, e) => {
				ShowDoneButton();
			};

			this.doneButton.Clicked += (sender, e) => {
				ShowEditButton();
			};

			this.keysButton.Clicked += (sender, e) => {
				this.appDelegate.GoToView(this.appDelegate.myIdKeyView);
			};


			this.markAllReadButton.Clicked += (sender, e) => {
				this.appDelegate.alert.showOkAlert("Mark All Read", "Marking all chat messages as read.");
			};

		}


		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{

			ICustomCell selectedCell = source.CellGroups[indexPath.Section].Cells[indexPath.Row];

			appDelegate.GoToView(appDelegate.chatView);
			UIAlertView alert = new UIAlertView("Chat index selected", ""+indexPath.Row, null, "Ok");
			alert.Show();

		}


		private void ShowDoneButton()
		{
			NavigationItem.SetLeftBarButtonItem(doneButton, false);
			NavigationItem.SetRightBarButtonItem(markAllReadButton, false);
			this.table.SetEditing(true, true);
		}


		private void ShowEditButton()
		{
			NavigationItem.SetLeftBarButtonItem(editButton, false);
			NavigationItem.SetRightBarButtonItem(composeButton, false);
			this.table.SetEditing(false, true);
		}

		public override void ViewWillAppear(bool animated)
		{
			Console.WriteLine("rkolli >>>>> @HERE2");
			this.Title = "Chats";
			this.PopulateTable();
		}

		public void PopulateTable()
		{
			List<PushChatThread> pm;

			List<CustomCellGroup> cellGroups = new List<CustomCellGroup>();
			tableCellGroup = new CustomCellGroup();
			cellGroups.Add(tableCellGroup);

			using (var conn = new SQLite.SQLiteConnection(AppDelegate._pathToMessagesDatabase)) {
				bool headerExists = false;
				Console.WriteLine("rkolli >>>>> @PopulateTable, fetching messages");
				pm = conn.Query<PushChatThread>("SELECT * FROM PushChatThread ORDER BY TimeStamp DESC");
				int count = 0;
				foreach (PushChatThread _m in pm) {
					Console.WriteLine("Db APMessages, Number = " + _m.Number+", message id = "+_m.TimeStamp+", Message Body = "+_m.Snippet+", ID = "+_m.ID);
//					List<ICustomCell> temp = tableCellGroup.Cells;
//					foreach (ChatCell icc in temp) {
//						Console.WriteLine("rkolli >>>>> Header = "+icc.GetHeader()+", message = "+_m.Snippet);
//							if (icc.GetHeader().Equals(_m.Number)) {
//								headerExists = true;
//								ChatCell chatCell = ChatCell.Create();
//								chatCell.SetHeader(_m.Number);
//								chatCell.SetSubheading(_m.Snippet);
//								DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(_m.TimeStamp / 1000).ToLocalTime();
//								Console.WriteLine("rkolli >>>>> Time after format is " + epoch.ToString("HH:mm"));
//								chatCell.SetLabelTime("" + epoch.ToString("HH:mm"));
//								chatCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
//								tableCellGroup.Cells.RemoveAt(_m.ID - 1);
//								tableCellGroup.Cells.Insert(_m.ID - 1, chatCell); 
//								break;
//							}
//						}
					//}

					string display_name = _m.Number;

					AddressBook book = new AddressBook();

					foreach (Contact c in book) {
						if (c.Phones.Any()) {
							foreach (Phone p in c.Phones) {
								if (_m.Number.Equals(p.Number)) {
									display_name = c.DisplayName;
								}
								break;
							}
						}

						if (c.Emails.Any()) {
							foreach (Email e in c.Emails) {
								if (_m.Number.Equals(e.Address)) {
									display_name = c.DisplayName;
								}
								break;
							}
						}
					}


					//if (!headerExists) {
						ChatCell chatCell = ChatCell.Create();
						chatCell.SetHeader(display_name);
						chatCell.SetSubheading(_m.Snippet);
						DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(_m.TimeStamp/1000).ToLocalTime();
						Console.WriteLine("rkolli >>>>> Time after format is "+epoch.ToString("HH:mm"));
						chatCell.SetLabelTime("" + epoch.ToString("HH:mm"));
						chatCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
//					if ((idx & 1) == 1)
//						chatCell.MarkAsRead();
						tableCellGroup.Cells.Insert(count, chatCell);
						count++;
					//}

				}
			}
			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;

			/*
			for (int x = 0; x <= 1; x++) {
				ChatCell chatCell = ChatCell.Create ();
				chatCell.SetHeader("John Doe");
				chatCell.SetSubheading("My latest message");
				chatCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				tableCellGroup.Cells.Add (chatCell);
			}

			for (int x = 0; x <= 1; x++) {
				ChatCell chatCell = ChatCell.Create ();
				chatCell.SetHeader("Jane Doe");
				chatCell.SetSubheading("Her latest message");
				chatCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				chatCell.MarkAsRead ();
				tableCellGroup.Cells.Add (chatCell);
			}*/

			
		}

		private void ComposeAction()
		{
			try {

				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet();

				actionSheet.AddButton("New Chat");
				actionSheet.AddButton("New Group");		
				actionSheet.AddButton("Cancel");		

				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == (0)) {
						this.appDelegate.GoToView(this.appDelegate.contactListView);
					} else if (b.ButtonIndex == (1)) {
							this.appDelegate.GoToView(this.appDelegate.newGroupView);
						} 
				};
				actionSheet.ShowInView(View);
			} catch (Exception ex) {
				Console.Write(ex.Message);
			}
		}


	}
}

