
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Threading;
using MonoTouch.MessageUI;
using Xamarin.Contacts;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PhoneNumbers;

namespace Stext
{

	public partial class ContactListView : UIViewController
	{


		AppDelegate appDelegate;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;
		//private Directory<String, CustomCellGroup> cellgroups;
		private List<CustomCellGroup> cellGroups;
		private static bool SecurecomContactsOnly = true;
		private string filter;

#region entity fields

		public string name;
		public string mobile;
		public string email;

#endregion

		public ContactListView()
			: base("ContactListView", null)
		{
		}


		public override void ViewWillAppear(bool animated)
		{
			this.Title = "Select Contact";
			try {
				this.PopulateTable("Securecom users");
			} catch (Exception e) {
				Console.WriteLine("Exception Thrown At " + e.ToString());
			}
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}


		private void AddTableRefresh()
		{
			UIRefreshControl refreshControl = new UIRefreshControl();
			refreshControl.ValueChanged += delegate(object sender, EventArgs e) {
				PerformUpdate(refreshControl.EndRefreshing);
			};
			table.AddSubview(refreshControl);
		}


		public void PerformUpdate(Action OnUpdateComplete)
		{
			InvokeInBackground(delegate {
				try {
					Thread.Sleep(2000);
				} catch (Exception ex) {
					InvokeOnMainThread(delegate {
						appDelegate.alert.showErrorAlert(ex.Message);
						if (OnUpdateComplete != null)
							OnUpdateComplete();
					});
				}
				InvokeOnMainThread(delegate {
					PopulateTable("Securecom users");
					if (OnUpdateComplete != null)
						OnUpdateComplete();
				});
			});
		}


		public override void ViewDidLoad()
		{

			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

			NavigationItem.SetRightBarButtonItem(filterButton, false);
			NavigationItem.SetLeftBarButtonItem(cancelButton, false);

			AddTableRefresh();
			ButtonActions();

			base.ViewDidLoad();
			search.CancelButtonClicked += (sender, e) => {
				search.Text = "";
				search.ResignFirstResponder();
				search.SetShowsCancelButton(false, true);
				PopulateTableWithExistingContactsData();
				AddTableRefresh();
			};
			search.TextChanged += async (object sender, UISearchBarTextChangedEventArgs e) => {
				search.SetShowsCancelButton(true, true);
				string searchText = e.SearchText.ToLower();
				Console.WriteLine("Search text = " + e.SearchText);
				if (!e.SearchText.Equals("")) {
					if (filter == "Securecom users") {
						new Thread(new System.Threading.ThreadStart(() => InvokeOnMainThread(() => ProcessSearchOnSecurecomContacts(searchText)))).Start();
					} else if (filter == "Show All") {
							ProcessSearchOnAllContacts(searchText);
						}

				} else {
					PopulateTableWithExistingContactsData();
				}
				AddTableRefresh();
			};
		}

		private void ProcessSearchOnSecurecomContacts(string searchText)
		{
			List<CustomCellGroup> mCellGroups = new List<CustomCellGroup>();
			List<PushContact> pc;
			CustomCellGroup mTableCellGroup = new CustomCellGroup { Name = "No Results" };
			using (var conn = new SQLite.SQLiteConnection(AppDelegate._dbPath)) {
				pc = conn.Query<PushContact>("select * from PushContact");
			}
			foreach (PushContact _c in pc) {
				if (String.IsNullOrEmpty(_c.Name))
					continue;
				if (_c.Name.ToLower().Contains(searchText) || _c.Number.Contains(searchText)) {
					mTableCellGroup.Name = "Search Results";
					ContactListCell cell = ContactListCell.Create();
					cell.SetName(_c.Name);
					if (_c.Number.Contains("@")) {
						cell.SetEmail(_c.Number);
					} else {
						cell.SetPhone(_c.Number);
					}
					mTableCellGroup.Cells.Add(cell);
				}
			}
			mCellGroups.Add(mTableCellGroup);
			source = new CustomCellTableSource(mCellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;
		}

		private void ProcessSearchOnAllContacts(string searchText)
		{
			List<CustomCellGroup> mCellGroups = new List<CustomCellGroup>();
			CustomCellGroup mTableCellGroup = new CustomCellGroup();
			AddressBook book = new AddressBook();
			foreach (Contact contact in book.OrderBy(c => c.DisplayName)) {
				if (String.IsNullOrEmpty(contact.DisplayName))
					continue;
				bool found = false;
				//Decide contact group based on the first character
				String group = contact.DisplayName.Substring(0, 1).ToUpper();
				if (contact.DisplayName.ToLower().Contains(searchText)) {
					found = true;
				} else if (contact.Phones.Any()) {
					foreach (Phone p in contact.Phones) {
						String temp = p.Number
							.Replace("(", string.Empty)
							.Replace(")", string.Empty)
							.Replace("-", string.Empty)
							.Replace(" ", string.Empty);
						found |= temp.Contains(searchText);
					}
				} else if (contact.Emails.Any()) {
					foreach (Email e in contact.Emails) {
						found |= e.Address.ToLower().Contains(searchText);
					}
				}
				if (!found)
					continue;
				ContactListCell cell = ContactListCell.Create();
				cell.SetName(contact.DisplayName);
				foreach (Phone p in contact.Phones) {
					cell.SetPhone(p.Number);
					break;
				}
				foreach (Email e in contact.Emails) {
					cell.SetEmail(e.Address);
					break;
				}
				mTableCellGroup.Cells.Add(cell);
			}
			mTableCellGroup.Name = mTableCellGroup.Cells.Count == 0 ? "No Results" : "Search Results";
			mCellGroups.Add(mTableCellGroup);
			source = new CustomCellTableSource(mCellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;
		}

		private void ButtonActions()
		{

			filterButton.Clicked += (sender, e) => FilterAction();

			cancelButton.Clicked += (sender, e) => appDelegate.GoToView(appDelegate.chatListView);

		}

		private void FilterAction()
		{
			UIActionSheet actionSheet;
			actionSheet = new UIActionSheet();

			if (SecurecomContactsOnly) {
				SecurecomContactsOnly = false;
				try {
					actionSheet.AddButton("Show All");
					actionSheet.AddButton("Cancel");
					actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
						if (b.ButtonIndex == (0)) {
							PopulateTable("Show All");
						} 
					};
					actionSheet.ShowInView(View);
				} catch (Exception ex) {
					Console.Write(ex.Message);
				}
			} else {
				SecurecomContactsOnly = true;
				try {
					actionSheet.AddButton("Securecom users");
					actionSheet.AddButton("Cancel");
					actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
						if (b.ButtonIndex == (0)) {
							PopulateTable("Securecom users");
						} 
					};
					actionSheet.ShowInView(View);
				} catch (Exception ex) {
					Console.Write(ex.Message);
				}
			}
		}

		private void AddContactsData(String contactFilter)
		{

			// Figure out where the SQLite database will be.
			bool showAll = contactFilter != "Securecom users";
			var conn = new SQLite.SQLiteConnection(AppDelegate._dbPath);
			List<PushContact> pc = conn.Query<PushContact>("select * from PushContact");
			conn.Close();
			List<String> registeredContacts = new List<String>();
			List<String> groups = new List<String>();
			foreach (PushContact c in pc)
				registeredContacts.Add(c.Number);
			var phoneUtil = PhoneNumberUtil.GetInstance();
			if (!showAll) {
				Dictionary<String, List<String>> map = new Dictionary<String, List<String>>();
				foreach (PushContact c in pc) {
					String n = c.Name ?? c.Number;
					if (!map.ContainsKey(n))
						map[n] = new List<String>();
					map[n].Add(c.Number);
				}
				foreach (KeyValuePair<String, List<String>> entry in map.OrderBy(c => c.Key)) {
					String group = entry.Key.Substring(0, 1).ToUpper();
					bool newGroup = !groups.Contains(group);
					foreach (CustomCellGroup ccg in cellGroups) {
						if (ccg.Name.Equals(group)) {
							newGroup = false;
							tableCellGroup = ccg;
						}
					}
					ContactListCell cell = ContactListCell.Create();
					cell.SetName(entry.Key);
					foreach (String number in entry.Value) {
						if (number.Contains("@"))
							cell.SetEmail(number);
						else
							cell.SetPhone(number);
					}
					if (newGroup) {
						tableCellGroup = new CustomCellGroup { Name = group };
						cellGroups.Add(tableCellGroup);
					}
					tableCellGroup.Cells.Add(cell);
				}
				return;
			}

			AddressBook book = new AddressBook();
			book.RequestPermission().ContinueWith(t => {
				if (!t.Result) {
					Console.WriteLine("Permission denied by user or manifest");
					return;
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());

			foreach (Contact contact in book.OrderBy(c => c.DisplayName)) {
				if (!showAll && registeredContacts.Count == 0)
					break;
				if (String.IsNullOrEmpty(contact.DisplayName))
					continue;
				String group = contact.DisplayName.Substring(0, 1).ToUpper();
				bool newGroup = !groups.Contains(group);
				foreach (CustomCellGroup ccg in cellGroups) {
					if (ccg.Name.Equals(group)) {
						newGroup = false;
						tableCellGroup = ccg;
					}
				}

				ContactListCell cell = ContactListCell.Create();
				cell.SetName(contact.DisplayName);
				cell.SetEmail(null);
				cell.SetPhone(null);

				if (contact.Phones.Any()) {
					foreach (Phone p in contact.Phones) {
						if (showAll) {
							cell.SetPhone(p.Number);
							cell.registeredState = ContactListCell.STATE_PENDING;
							break;
						}
						if (p.Number.Contains("*") || p.Number.Contains("#"))
							continue;
						String number;
						try {
							number = phoneUtil.Format(phoneUtil.Parse(p.Number, AppDelegate.GetCountryCode()), PhoneNumberFormat.E164);
						} catch (Exception e) {
							continue;
						}
						if (!registeredContacts.Contains(number))
							continue;
						registeredContacts.Remove(number);
						cell.SetPhone(p.Number);
						cell.registeredState = ContactListCell.STATE_REGISTERED;
						//conn.Execute("UPDATE PushContact Set Name = ? WHERE Number = ?", contact.DisplayName, number);
						break;
					}
				}
				if (contact.Emails.Any()) {
					foreach (Email e in contact.Emails) {
						if (showAll) {
							cell.SetEmail(e.Address);
							cell.registeredState = ContactListCell.STATE_PENDING;
							break;
						}
						if (!registeredContacts.Contains(e.Address))
							continue;						
						registeredContacts.Remove(e.Address);
						cell.SetEmail(e.Address);
						cell.registeredState = ContactListCell.STATE_REGISTERED;
						//conn.Execute("UPDATE PushContact Set Name = ? WHERE Number = ?", contact.DisplayName, e.Address);
						break;
					}
				}
				if (cell._email == null && cell.mobile == null)
					continue;
				if (newGroup) {
					tableCellGroup = new CustomCellGroup { Name = group };
					cellGroups.Add(tableCellGroup);
				}
				tableCellGroup.Cells.Add(cell);
			}
			//conn.Close();

		}

		public void PopulateTable(string afilter)
		{
			this.filter = afilter;
			cellGroups = new List<CustomCellGroup>();
			AddContactsData(afilter);
			//AddTestData();
			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;
			AddTableRefresh();
		}

		public void PopulateTableWithExistingContactsData()
		{
			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;
		}


		private void ShowPendingContactOptions()
		{
			try {
				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet();
				if (!string.IsNullOrEmpty(this.mobile))
					actionSheet.AddButton(this.mobile);
				if (!string.IsNullOrEmpty(this.email))
					actionSheet.AddButton(this.email);
				actionSheet.AddButton("Cancel");
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					using (var conn = new SQLite.SQLiteConnection(AppDelegate._dbPath)) {
						string recipient = "";
						//int present_thread_id = 0;

						if (actionSheet.ButtonTitle(b.ButtonIndex) == this.mobile) {
							try {
								var phoneUtil = PhoneNumberUtil.GetInstance();
								recipient = phoneUtil.Format(phoneUtil.Parse(this.mobile, AppDelegate.GetCountryCode()), PhoneNumberFormat.E164);
							} catch (Exception e) {
								recipient = this.mobile;
							}
						} else if (actionSheet.ButtonTitle(b.ButtonIndex) == this.email) {
							recipient = this.email;
						}

						if (actionSheet.ButtonTitle(b.ButtonIndex) != "Cancel") { 
							/*
							List<PushChatThread> pctList = conn.Query<PushChatThread>("select * from PushChatThread");

							foreach (PushChatThread pct in pctList) {
								if (pct.Number.Equals(recipient)) {
									present_thread_id = pct.ID;
									break;
								}
							}
							*/
							PushChatThread thread = conn.FindWithQuery<PushChatThread>("select * from PushChatThread where Number = ?", recipient);
							//if (present_thread_id == 0) {
							if (thread == null) {
								PushContact contact = conn.FindWithQuery<PushContact>("select * from PushContact where Number = ?", recipient);
								thread = new PushChatThread {
									DisplayName = contact.Name,
									Number = recipient,
									Recipient_id = 0,
									TimeStamp = AppDelegate.CurrentTimeMillis(),
									Message_count = 0,
									Snippet = "",
									Read = 0,
									Type = "PushLocal"
								};
								conn.Insert(thread);
								conn.Commit();
								conn.Close();
								//present_thread_id = pct_val.ID;
							}

							//appDelegate.chatView.setThreadID(present_thread_id);
							appDelegate.chatView.setThreadID(thread.ID);
							appDelegate.chatView.setNumber(recipient);
							appDelegate.chatView.setThreadSelected(thread.DisplayName + " (" + recipient + ")");
							appDelegate.GoToView(appDelegate.chatView);
						}
					}
				};
				actionSheet.ShowInView(View);
			} catch (Exception ex) {
				Console.Write(ex.Message);
			}
		}


		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{

			ContactListCell selectedCell = (ContactListCell)source.CellGroups[indexPath.Section].Cells[indexPath.Row];

			this.name = selectedCell.name;
			this.email = selectedCell._email;
			this.mobile = selectedCell.mobile;

			ShowPendingContactOptions();
//			if (selectedCell.registeredState == ContactListCell.STATE_REGISTERED) {
//				ShowPendingContactOptions();
//			} else if (selectedCell.registeredState == ContactListCell.STATE_PENDING) {
//				ShowPendingContactOptions();
//			}

			//this.appDelegate.alert.showOkAlert("Contact State",selectedCell.registeredState.ToString());
		}



	}
}

