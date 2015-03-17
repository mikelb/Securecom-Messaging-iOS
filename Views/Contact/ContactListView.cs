
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

namespace Stext{

	public partial class ContactListView : UIViewController{


		AppDelegate appDelegate;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;
		private List<CustomCellGroup> cellGroups;
		private AddressBook book = new AddressBook();
		private static bool isOnlySecurecomContacts = true;

		#region entity fields
		public string name;
		public string mobile;
		public string email;
		#endregion

		public ContactListView () : base ("ContactListView", null){	}


		public override void ViewWillAppear (bool animated){
			this.Title = "Select Contact";
			try{
				this.PopulateTable ("Securecom users");
			}catch(Exception e){
				Console.WriteLine("Exception Thrown At "+e.ToString());
			}
		}


		public override void DidReceiveMemoryWarning (){
			base.DidReceiveMemoryWarning ();
		}


		private void AddTableRefresh(){
			UIRefreshControl refreshControl = new UIRefreshControl ();
			refreshControl.ValueChanged += delegate(object sender, EventArgs e) {
				PerformUpdate(refreshControl.EndRefreshing);
			};
			table.AddSubview (refreshControl);
		}


		public void PerformUpdate(Action OnUpdateComplete){
			InvokeInBackground(delegate {
				try {
					Thread.Sleep(2000);
				}
				catch (Exception ex) {
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


		public override void ViewDidLoad (){

			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

			NavigationItem.SetRightBarButtonItem (filterButton, false);
			NavigationItem.SetLeftBarButtonItem(cancelButton, false);

			AddTableRefresh ();
			ButtonActions();

			base.ViewDidLoad ();
			search.CancelButtonClicked += (sender, e) => {
				search.Text = "";
				search.ResignFirstResponder();
				search.SetShowsCancelButton(false, true);
			};
			search.TextChanged += async (object sender, UISearchBarTextChangedEventArgs e) => {
				search.SetShowsCancelButton(true, true);
				string searchText = e.SearchText.ToLower();
				Console.WriteLine("Search text = " + e.SearchText);
				if(!e.SearchText.Equals("")){
					List<CustomCellGroup> mCellGroups = new List<CustomCellGroup>();
					CustomCellGroup mTableCellGroup = new CustomCellGroup { Name = "No Results" };

					foreach (Contact contact in book.OrderBy(c => c.DisplayName)) {
						bool found = false;
						if (!String.IsNullOrEmpty(contact.DisplayName)) {
							//Decide contact group based on the first character
							String group = contact.DisplayName.Substring(0, 1).ToUpper();
							if(contact.DisplayName.ToLower().Contains(searchText)){
								found = true;
							}else if(contact.Phones.Any() || contact.Emails.Any()){
								foreach (Phone p in contact.Phones) {
									String temp = p.Number;
									temp = temp.Replace("(", "");
									temp = temp.Replace(")", "");
									temp = temp.Replace("-", "");
									temp = temp.Replace(" ", "");
									found |= temp.Contains(searchText);
								}

								foreach (Email email in contact.Emails) {
									found |= email.Address.ToLower().Contains(searchText);
								}
							}

							if(found)
							{
								mTableCellGroup.Name = "Search Results";
								Console.WriteLine("rkolli >>>>> Adding Contacts, group = " + group);
								Console.WriteLine("rkolli >>>>> Search result, Contact display name = " + contact.DisplayName);
								ContactListCell cell = ContactListCell.Create();
								cell.SetName(contact.DisplayName);
								foreach (Phone p in contact.Phones) {
									Console.WriteLine("rkolli >>>>> Search result, Contact Number = " + p.Number);
									cell.SetPhone(p.Number);
									break;
								}
								foreach (Email email in contact.Emails) {
									Console.WriteLine("rkolli >>>>> Search result, Contact Email = " + email.Address);
									cell.SetEmail(email.Address);
									break;
								}

								mTableCellGroup.Cells.Add (cell);
							}
						}
					}

					mCellGroups.Add(mTableCellGroup);

					source = new CustomCellTableSource(mCellGroups);
					source.RowSelectedAction = RowSelected;
					table.Source = source;
				}else {
					PopulateTableWithExistingContactsData();
				}
				AddTableRefresh ();
//				foreach (Contact contact in book.OrderBy(c => c.DisplayName)) {
//					if (!String.IsNullOrEmpty(contact.DisplayName)) {
//						//Decide contact group based on the first character
//						String group = contact.DisplayName.Substring(0, 1).ToUpper();
//						Console.WriteLine("rkolli >>>>> Adding Contacts, group = " + group);
//						bool name_exists = false;
//						if(contact.DisplayName.Contains(e.SearchText) || contact.Phones.First(Phone => Phone.Number.Contains(e.SearchText)).Number != null || 
//												contact.Emails.First(Email => Email.Address.Contains(e.SearchText)).Address != null){
//							foreach (CustomCellGroup ccg in cellGroups) {
//								if (ccg.Name.Equals(group)) {
//									name_exists = true;
//									tableCellGroup = ccg;
//								}
//							}
//							ContactListCell cell = ContactListCell.Create();
//							cell.SetName(contact.DisplayName);
//							if (contact.Phones.Any()) {
//								foreach (Phone p in contact.Phones) {
//									cell.SetPhone(p.Number);
//									break;
//								}
//							}
//							if (contact.Emails.Any()) {
//								foreach (Email email in contact.Emails) {
//									cell.SetEmail(email.Address);
//									break;
//								}
//							}
//							if (!name_exists) {
//								tableCellGroup = new CustomCellGroup {
//									Name = group
//								};
//								cellGroups.Add(tableCellGroup);
//							}
//							tableCellGroup.Cells.Add(cell);
//						}
//					}
//				}
			};
		}


		private void ButtonActions(){

			filterButton.Clicked += (sender, e) => FilterAction();

			cancelButton.Clicked += (sender, e) => appDelegate.GoToView(appDelegate.chatListView);

		}

		private void FilterAction(){
			UIActionSheet actionSheet;
			actionSheet = new UIActionSheet();

			if (!isOnlySecurecomContacts) {
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
			} else {
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
			}
		}

		private void AddContactsData(string filter){
			List<String> contactlist = new List<String>();
			List<PushContact> pc;
			book.RequestPermission().ContinueWith(t => {
				if (!t.Result) {
					Console.WriteLine("Permission denied by user or manifest");
					return;
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());

			// Figure out where the SQLite database will be.
			Console.WriteLine("rkolli >>>>> Reading Contacts");

			using (var conn = new SQLite.SQLiteConnection(AppDelegate._pathToContactsDatabase)) {
				pc = conn.Query<PushContact>("select * from PushContact");

				// TODO: Remove this
				foreach (PushContact _c in pc) {
					Console.WriteLine("Db Contacts = " + _c.Number);
				}
			}
		
			foreach (Contact contact in book.OrderBy(c => c.DisplayName)) {
				if (!String.IsNullOrEmpty(contact.DisplayName)) {
					//Decide contact group based on the first character
					String group = contact.DisplayName.Substring(0, 1).ToUpper();
					bool name_exists = false;
					isOnlySecurecomContacts = (filter == "Securecom users") ? true : false;

					//Console.WriteLine("rkolli >>>>> Adding Contacts, group = " + group+", isOnlySecurecomContacts = "+isOnlySecurecomContacts);

					foreach (CustomCellGroup ccg in cellGroups) {
						if (ccg.Name.Equals(group)) {
							name_exists = true;
							tableCellGroup = ccg;
						}
					}

					ContactListCell cell = ContactListCell.Create();
					cell.SetName(contact.DisplayName);
					cell.mobile = null;
					cell._email = null;

					cell.SetEmail(null);
					cell.SetPhone(null);

					if (contact.Phones.Any()) {
						foreach (Phone p in contact.Phones) {
							if (isOnlySecurecomContacts) {
								foreach (PushContact push in pc) {
									String temp = p.Number;
									temp = temp.Replace("(", string.Empty);
									temp = temp.Replace(")", string.Empty);
									temp = temp.Replace("-", string.Empty);
									temp = Regex.Replace(temp, @"\s", "");
									if (push.Number.Contains(temp)) {
										cell.SetPhone(p.Number);
										cell.registeredState = ContactListCell.STATE_REGISTERED;
										break;
									}
								}
							} else {
								cell.SetPhone(p.Number);
								cell.registeredState = ContactListCell.STATE_PENDING;
							}
							break;
						}
					}

					if (contact.Emails.Any()) {
						foreach (Email e in contact.Emails) {
							if (isOnlySecurecomContacts) {
								foreach (PushContact push in pc) {
									if (push.Number == e.Address) {
										cell.SetEmail(e.Address);
										cell.registeredState = ContactListCell.STATE_REGISTERED;
										break;
									}
								}
							} else {
								cell.SetPhone(e.Address);
								cell.registeredState = ContactListCell.STATE_PENDING;
							}
							break;
						}
					}

					if (!name_exists) {
						if (cell._email != null || cell.mobile != null) {
							tableCellGroup = new CustomCellGroup { Name = group };
							cellGroups.Add(tableCellGroup);
						} else
							continue;
					}

					if (cell._email != null || cell.mobile != null) {
						tableCellGroup.Cells.Add(cell);
					}
				}
			}

		}

		public void PopulateTable(string filter){
			cellGroups = new List<CustomCellGroup> ();
			AddContactsData (filter);
			//AddTestData();
			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;
			AddTableRefresh ();
		}

		public void PopulateTableWithExistingContactsData(){
			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;
		}


		private void ShowPendingContactOptions(){
			try{
				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();
				if (!string.IsNullOrEmpty(this.mobile))
					actionSheet.AddButton(this.mobile);
				if (!string.IsNullOrEmpty(this.email))
					actionSheet.AddButton (this.email);
				actionSheet.AddButton ("Cancel");
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					using (var conn= new SQLite.SQLiteConnection(AppDelegate._pathToMessagesDatabase))
					{
						string recipient = "";
						int present_thread_id = 0;

						if (actionSheet.ButtonTitle(b.ButtonIndex) == this.mobile) {
							recipient = this.mobile;
						} else if(actionSheet.ButtonTitle(b.ButtonIndex) == this.email){
							recipient = this.email;
						}

						if(actionSheet.ButtonTitle(b.ButtonIndex) != "Cancel"){ 
							List<PushChatThread> pctList = conn.Query<PushChatThread>("select * from PushChatThread");
							foreach (PushChatThread pct in pctList) {
								if (pct.Number.Equals(recipient)) {
									present_thread_id = pct.ID;
								}
							}
							if(present_thread_id == 0){
								var pct_val = new PushChatThread{Number = recipient, Recipient_id = 0, TimeStamp = AppDelegate.CurrentTimeMillis(), Message_count = 0, Snippet = "", Read = 0, Type = "PushLocal"};
								conn.Insert(pct_val);
								conn.Commit();
								conn.Close();
								present_thread_id = pct_val.ID;
							}
							appDelegate.chatView.setThreadID(present_thread_id);
							appDelegate.chatView.setNumber(recipient);
							appDelegate.chatView.setThreadSelected(this.name+" ("+recipient+")");
							appDelegate.GoToView(appDelegate.chatView);
						}
					}
				};
				actionSheet.ShowInView (View);
			}catch(Exception ex){
				Console.Write(ex.Message);
			}
		}


		public void RowSelected(UITableView tableView, NSIndexPath indexPath){

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

