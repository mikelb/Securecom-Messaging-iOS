
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

			appDelegate.chatView.setThreadID(0);

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

			settingsButton.Clicked += (sender, e) => {
				//this.appDelegate.GoToView(this.appDelegate.settingsView);
				SettingsAction();
			};

			search.CancelButtonClicked += (sender, e) => {
				search.Text = "";
				search.ResignFirstResponder();
				search.SetShowsCancelButton(false, true);
			};

			search.TextChanged += async (object sender, UISearchBarTextChangedEventArgs e) => {
				search.SetShowsCancelButton(true, true);
			};

		}


		private void SettingsAction(){
			try{

				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();

				actionSheet.AddButton ("Refresh Securecom Contacts");
				actionSheet.AddButton ("Re-register");
				actionSheet.AddButton ("Cancel");

				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == (0)) {
						try{
							DoContactsSync();
						}catch(Exception e){
							UIAlertView alert = new UIAlertView("Failed!", "Contact Refresh Failed!", null, "Ok");
							alert.Show();
						}

					} else if (b.ButtonIndex == (1)){
						var alert = new UIAlertView {
							Title = "Re-register?", 
							Message = "This action will delete your preivious conversations!"
						};
						alert.AddButton("Yes");
						alert.AddButton("No");
						// last button added is the 'cancel' button (index of '2')
						alert.Clicked += delegate(object a1, UIButtonEventArgs b1) {
							if (b1.ButtonIndex == (0)) {
								using (var conn = new SQLite.SQLiteConnection(AppDelegate._pathToMessagesDatabase)) {
									conn.Execute("DELETE FROM PushChatThread");
									conn.Execute("DELETE FROM PushMessage");
									conn.Commit();
									conn.Close();
								}
								using (var conn1 = new SQLite.SQLiteConnection(AppDelegate._pathToContactsDatabase)) {
									conn1.Execute("DELETE FROM PushContact");
									conn1.Commit();
									conn1.Close();
								}
								appDelegate.GoToView(appDelegate.registrationView);
							}
						};
						alert.Show ();
					} 
				};
				actionSheet.ShowInView (View);
			}catch(Exception ex){
				Console.Write(ex.Message);
			}
		}

		public void DoContactsSync(){
			AddressBook book = new AddressBook();
			List<String> contactlist = new List<String>();
			book.RequestPermission().ContinueWith(t => {
				if (!t.Result) {
					Console.WriteLine("Permission denied by user or manifest");
					return;
				}

				int counter = 0;
				int contact_count = book.Count();
				Console.WriteLine("rkolli >>>>> @RefreshPushDirectory, Address book count = " + contact_count);

				foreach (Contact contact in book.OrderBy(c => c.LastName)) {
					int idx = counter++;
					if (!String.IsNullOrEmpty(contact.DisplayName)) {
						foreach (Phone value in contact.Phones) {
							if (!value.Number.Contains("*") || !value.Number.Contains("#")) {
								var phoneUtil = PhoneNumberUtil.GetInstance();
								PhoneNumber numberObject = phoneUtil.Parse(value.Number, "US");
								var number = phoneUtil.Format(numberObject, PhoneNumberFormat.E164);
								contactlist.Add(number);
							}
						}
						foreach (Email value in contact.Emails) {
							contactlist.Add(value.Address);
						}
					}

				}
				Dictionary<string,string> tokens = ConfirmationView.getDirectoryServerTokenDictionary(contactlist);
				List<String> list = new List<String>();
				foreach (string key in tokens.Keys)
					list.Add(key);
				Console.WriteLine("intersecting " + list);
				List<String> response = MessageManager.RetrieveDirectory(list);
				List<String> result = new List<String>();
				foreach (string key in response) {
					if (tokens[key] != null)
						result.Add(tokens[key]);
				}
				Console.WriteLine("rkolli >>>>> we're here 1");
				// Figure out where the SQLite database will be.
				using (var conn= new SQLite.SQLiteConnection(AppDelegate._pathToContactsDatabase))
				{
					Console.WriteLine("rkolli >>>>> we're here 2");
					conn.CreateTable<PushContact>();

					foreach(String contact in result){
						Console.WriteLine("we're here, push contact = " + contact);
						var pcontact = new PushContact{Number = contact};
						conn.Insert(pcontact);
					}
				}

				Console.WriteLine("rkolli >>>>> we're here 3");

			}, TaskScheduler.Current);

			UIAlertView alert = new UIAlertView("Successfull!", "Contact Refresh Successfull!", null, "Ok");
			alert.Show();
		}

		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{

			ChatCell selectedCell = (ChatCell) source.CellGroups[indexPath.Section].Cells[indexPath.Row];
			appDelegate.chatView.setThreadSelected(selectedCell.GetHeader());
			using (var conn = new SQLite.SQLiteConnection(AppDelegate._pathToMessagesDatabase)) {
				conn.Execute("UPDATE PushChatThread Set Read = ? WHERE ID = ?", 0, selectedCell.GetThreadID());
				conn.Commit();
				conn.Close();
			}
			Console.WriteLine("rkolli >>>>> @RowSelected, ThreadID = "+selectedCell.GetThreadID());
			appDelegate.chatView.setThreadID(selectedCell.GetThreadID());
			appDelegate.chatView.setNumber(selectedCell.GetNumber());
			appDelegate.GoToView(appDelegate.chatView);
//			UIAlertView alert = new UIAlertView("Chat index selected", ""+indexPath.Row, null, "Ok");
//			alert.Show();

		}




		private void ShowDoneButton()
		{
			NavigationItem.SetLeftBarButtonItem(doneButton, false);
			NavigationItem.SetRightBarButtonItem(markAllReadButton, false);
			this.table.SetEditing(true, true);
		}


		private void ShowEditButton()
		{
			List<PushChatThread> pctList;
			using (var conn = new SQLite.SQLiteConnection(AppDelegate._pathToMessagesDatabase)) {
				pctList = conn.Query<PushChatThread>("select * from PushChatThread");
			}
			if(pctList.Count <= 0){
				NavigationItem.SetLeftBarButtonItem(null, false);
				NavigationItem.SetHidesBackButton(true, true);
			}else{
				NavigationItem.SetLeftBarButtonItem(editButton, false);
			}
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
			table.ReloadData();

			List<PushChatThread> pm;
			UIImage thumbnail = null;

			List<CustomCellGroup> cellGroups = new List<CustomCellGroup>();
			tableCellGroup = new CustomCellGroup();
			cellGroups.Add(tableCellGroup);

			using (var conn = new SQLite.SQLiteConnection(AppDelegate._pathToMessagesDatabase)) {
				bool headerExists = false;
				Console.WriteLine("rkolli >>>>> @PopulateTable, fetching messages");
				pm = conn.Query<PushChatThread>("SELECT * FROM PushChatThread ORDER BY TimeStamp DESC");
				int count = 0;
				foreach (PushChatThread _m in pm) {
					Console.WriteLine("Db APMessages, Number = " + _m.Number+", message id = "+_m.TimeStamp+", Message Body = "+_m.Snippet+", ID = "+_m.ID+", Read = "+_m.Read);
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
									thumbnail = c.GetThumbnail();
								}
								break;
							}
						}

						if (c.Emails.Any()) {
							foreach (Email e in c.Emails) {
								if (_m.Number.Equals(e.Address)) {
									display_name = c.DisplayName;
									thumbnail = c.GetThumbnail();
								}
								break;
							}
						}
					}


					//if (!headerExists) {
						ChatCell chatCell = ChatCell.Create();
						chatCell.SetHeader(display_name);
						chatCell.SetSubheading(_m.Snippet);
						chatCell.SetThreadID(_m.ID);
						chatCell.SetNumber(_m.Number);
						chatCell.SetAvatar(thumbnail);
					        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(_m.TimeStamp/1000).ToLocalTime();
						Console.WriteLine("rkolli >>>>> Time after format is "+epoch.ToString("HH:mm"));
						chatCell.SetLabelTime("" + epoch.ToString("HH:mm"));
						chatCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
						
					if (_m.Read != 0) {
						chatCell.BackgroundColor = UIColor.Green;
					}
//					if ((idx & 1) == 1)
//						chatCell.MarkAsRead();
						tableCellGroup.Cells.Insert(count, chatCell);
						count++;
					//}

				}
				conn.Commit();
				conn.Close();
			}
			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			source.DeleteAction = DeleteSelected;
			source.DeleteTitle = "Delete";
			table.Source = source;

			ShowEditButton();

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

		public void DeleteSelected(UITableView tableView, NSIndexPath indexPath){
			ChatCell selectedCell = (ChatCell) source.CellGroups[indexPath.Section].Cells[indexPath.Row];
			Console.WriteLine("rkolli >>>>> @RowSelected to DELETE, ThreadID = "+selectedCell.GetThreadID());
			try{
				using (var conn = new SQLite.SQLiteConnection(AppDelegate._pathToMessagesDatabase)) {
					conn.Execute("DELETE FROM PushChatThread WHERE ID = ?", selectedCell.GetThreadID());
					conn.Execute("DELETE FROM PushMessage WHERE Thread_id = ?", selectedCell.GetThreadID());
					conn.Commit();
					conn.Close();
				}
			}catch(Exception e){
				Console.WriteLine("Error while deleting thread "+e.Message);
			}
			ShowEditButton();
			UIAlertView alert = new UIAlertView("Deleted", ""+indexPath.Row, null, "Ok");
			alert.Show();
		}

		private void ComposeAction()
		{
			try {

				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet();

				actionSheet.AddButton("New Chat");
				//actionSheet.AddButton("New Group");		
				actionSheet.AddButton("Cancel");		

				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == (0)) {
						this.appDelegate.GoToView(this.appDelegate.contactListView);
					}
//					} else if (b.ButtonIndex == (1)) {
//							this.appDelegate.GoToView(this.appDelegate.newGroupView);
//						} 
				};
				actionSheet.ShowInView(View);
			} catch (Exception ex) {
				Console.Write(ex.Message);
			}
		}


	}
}

