
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

using Xamarin.Contacts;
using System.Threading.Tasks;
using System.Linq;
using PhoneNumbers;
using Securecom.Messaging;
using Securecom.Messaging.Net;

namespace Stext
{


	public partial class ChatListView : UIViewController
	{


		AppDelegate appDelegate;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;
		private List<PushChatThread> pct;

		public ChatListView()
			: base("ChatListView", null)
		{
		}

		public override void ViewDidLoad()
		{
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
				PopulateTable();
			};

			search.TextChanged += async (object sender, UISearchBarTextChangedEventArgs e) => {
				search.SetShowsCancelButton(true, true);
				String searchText = e.SearchText.ToLower();
				if (e.SearchText.Equals("")) {
					PopulateTable();
					return;
				}
				List<CustomCellGroup> mCellGroups = new List<CustomCellGroup>();
				CustomCellGroup mTableCellGroup = new CustomCellGroup();
				int count = 0;
				foreach (PushChatThread thread in pct) {
					if (String.IsNullOrEmpty(thread.Number))
						continue;
					if (!thread.Number.ToLower().Contains(searchText) && !thread.DisplayName.ToLower().Contains(searchText))
						continue;
					
					ChatCell chatCell = ChatCell.Create();
					chatCell.SetHeader(thread.DisplayName + " (" + thread.Number + ")");
					chatCell.SetSubheading(thread.Snippet);
					chatCell.SetThreadID(thread.ID);
					chatCell.SetNumber(thread.Number);
					chatCell.SetAvatar(null);
					DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(thread.TimeStamp / 1000).ToLocalTime();
					chatCell.SetLabelTime(epoch.ToString("HH:mm"));
					chatCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					mTableCellGroup.Cells.Insert(count, chatCell);
					count++;
				}
					
				mTableCellGroup.Name = (count == 0) ? "No Results" : "Search Results";
				mCellGroups.Add(mTableCellGroup);

				source = new CustomCellTableSource(mCellGroups);
				source.RowSelectedAction = RowSelected;
				source.DeleteAction = DeleteSelected;
				source.DeleteTitle = "Delete";
				table.Source = source;
				table.ReloadData();
			};

		}


		private void SettingsAction()
		{
			try {
				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet();

				actionSheet.AddButton("Refresh Securecom Contacts");
				actionSheet.AddButton("Re-register");
				actionSheet.AddButton("Cancel");

				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == (0)) {
						try {
							LoadingOverlay loadingOverlay;

							// Determine the correct size to start the overlay (depending on device orientation)
							var bounds = UIScreen.MainScreen.Bounds; // portrait bounds
							if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
								bounds.Size = new SizeF(bounds.Size.Height, bounds.Size.Width);
							}
							// show the loading overlay on the UI thread using the correct orientation sizing
							loadingOverlay = new LoadingOverlay(bounds);
							this.View.Add(loadingOverlay);

							Task taskA = Task.Factory.StartNew(StextUtil.RefreshPushDirectory);
							taskA.Wait();
							loadingOverlay.Hide();

						} catch (Exception e) {
							UIAlertView alert = new UIAlertView("Failed!", "Contact Refresh Failed!", null, "Ok");
							alert.Show();
						}

					} else if (b.ButtonIndex == (1)) {
							var alert = new UIAlertView {
								Title = "Re-register?", 
								Message = "This action will delete your preivious conversations!"
							};
							alert.AddButton("Yes");
							alert.AddButton("No");
							// last button added is the 'cancel' button (index of '2')
							alert.Clicked += delegate(object a1, UIButtonEventArgs b1) {
								if (b1.ButtonIndex == (0)) {
									using (var conn = new SQLite.SQLiteConnection(AppDelegate._dbPath)) {
										conn.Execute("DELETE FROM PushContact");
										conn.Execute("DELETE FROM PushChatThread");
										conn.Execute("DELETE FROM PushMessage");
										conn.Commit();
										conn.Close();
									}
									Session.ClearSessions();
									appDelegate.GoToView(appDelegate.registrationView);
								}
							};
							alert.Show();
						} 
				};
				actionSheet.ShowInView(View);
			} catch (Exception ex) {
				Console.Write(ex.Message);
			}
		}

		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{

			ChatCell selectedCell = (ChatCell)source.CellGroups[indexPath.Section].Cells[indexPath.Row];
			appDelegate.chatView.setThreadSelected(selectedCell.GetHeader());
			using (var conn = new SQLite.SQLiteConnection(AppDelegate._dbPath)) {
				conn.Execute("UPDATE PushChatThread Set Read = ? WHERE ID = ?", 0, selectedCell.GetThreadID());
				conn.Commit();
				conn.Close();
			}
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
			using (var conn = new SQLite.SQLiteConnection(AppDelegate._dbPath)) {
				pctList = conn.Query<PushChatThread>("select * from PushChatThread");
			}
			if (pctList.Count <= 0) {
				NavigationItem.SetLeftBarButtonItem(null, false);
				NavigationItem.SetHidesBackButton(true, true);
			} else {
				NavigationItem.SetLeftBarButtonItem(editButton, false);
			}
			NavigationItem.SetRightBarButtonItem(composeButton, false);
			this.table.SetEditing(false, true);
		}

		public override void ViewWillAppear(bool animated)
		{
			this.Title = "Chats";
			this.PopulateTable();
		}
			
		public void PopulateTable()
		{
			table.ReloadData();
			UIImage thumbnail = null;
			List<CustomCellGroup> cellGroups = new List<CustomCellGroup>();
			tableCellGroup = new CustomCellGroup();
			cellGroups.Add(tableCellGroup);
			PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
			AddressBook book = new AddressBook();
			var conn = new SQLite.SQLiteConnection(AppDelegate._dbPath);
			pct = conn.Query<PushChatThread>("SELECT * FROM PushChatThread ORDER BY TimeStamp DESC");
			conn.Close();
			int count = 0;

			foreach (PushChatThread thread in pct) {
				String display_name = thread.DisplayName;

				ChatCell chatCell = ChatCell.Create();
				chatCell.SetHeader(display_name + " (" + thread.Number + ")");
				chatCell.SetSubheading(thread.Snippet);
				chatCell.SetThreadID(thread.ID);
				chatCell.SetNumber(thread.Number);
				chatCell.SetAvatar(thumbnail);
				DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(thread.TimeStamp / 1000).ToLocalTime();
				chatCell.SetLabelTime("" + epoch.ToString("HH:mm"));
				chatCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					
				if (thread.Read != 0)
					chatCell.BackgroundColor = UIColor.Green;
				tableCellGroup.Cells.Insert(count, chatCell);
				count++;

			}
			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			source.DeleteAction = DeleteSelected;
			source.DeleteTitle = "Delete";
			table.Source = source;

			ShowEditButton();

		}

		public void DeleteSelected(UITableView tableView, NSIndexPath indexPath)
		{
			ChatCell selectedCell = (ChatCell)source.CellGroups[indexPath.Section].Cells[indexPath.Row];
			try {
				using (var conn = new SQLite.SQLiteConnection(AppDelegate._dbPath)) {
					conn.Execute("DELETE FROM PushChatThread WHERE ID = ?", selectedCell.GetThreadID());
					conn.Execute("DELETE FROM PushMessage WHERE Thread_id = ?", selectedCell.GetThreadID());
					conn.Commit();
					conn.Close();
				}
			} catch (Exception e) {
				Console.WriteLine("Error while deleting thread " + e.Message);
			}
			ShowEditButton();
//			UIAlertView alert = new UIAlertView("Deleted Conversation with", ""+selectedCell.GetNumber(), null, "Ok");
//			alert.Show();
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

