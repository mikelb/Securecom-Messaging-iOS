
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

namespace Stext{

	public partial class ContactListView : UIViewController{


		AppDelegate appDelegate;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;
		private List<CustomCellGroup> cellGroups;
		private AddressBook book = new AddressBook();

		#region entity fields
		public string name;
		public string mobile;
		public string email;
		#endregion

		public ContactListView () : base ("ContactListView", null){	}


		public override void ViewWillAppear (bool animated){
			this.Title = "Select Contact";
			this.PopulateTable ();
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
					PopulateTable();
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

			search.TextChanged += async (object sender, UISearchBarTextChangedEventArgs e) => {
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
			try{
				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();
				actionSheet.AddButton ("Stext users");
				actionSheet.AddButton ("Non-Stext users");
				actionSheet.AddButton ("Show All");
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == (0)) {
					} else {
					} 
				};
				actionSheet.ShowInView (View);
			}catch(Exception ex){
				Console.Write(ex.Message);
			}
		}

		private void AddContactsData(){
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
				foreach (PushContact _c in pc) {
					Console.WriteLine("Db Contacts = " + _c.Number);
				}
			}

			foreach (Contact contact         in book.OrderBy(c => c.DisplayName)) {
				if (!String.IsNullOrEmpty(contact.DisplayName)) {
					//Decide contact group based on the first character
					String group = contact.DisplayName.Substring(0, 1).ToUpper();
					Console.WriteLine("rkolli >>>>> Adding Contacts, group = "+group);
					bool name_exists = false;
					foreach (CustomCellGroup ccg in cellGroups) {
						if (ccg.Name.Equals(group)) {
							name_exists = true;
							tableCellGroup = ccg;
						}
					}

					if (!name_exists) {
						tableCellGroup = new CustomCellGroup { Name = group };
						cellGroups.Add(tableCellGroup);
					} 

					ContactListCell cell = ContactListCell.Create ();
					cell.SetName(contact.DisplayName);
					if (contact.Phones.Any()) {
						foreach (Phone p in contact.Phones) {
							cell.SetPhone(p.Number);
							break;
						}
					}

					if (contact.Emails.Any()) {
						foreach (Email e in contact.Emails) {
							cell.SetEmail(e.Address);
							break;
						}
					}
					tableCellGroup.Cells.Add (cell);
				}
			}
		}

		public void PopulateTable(){
			cellGroups = new List<CustomCellGroup> ();
			AddContactsData ();
			//AddTestData();
			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			table.Source = source;
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
				actionSheet.AddButton ("Call " + this.mobile);
				actionSheet.AddButton ("SMS " + this.mobile);
				actionSheet.AddButton ("Email " + this.email);
				actionSheet.AddButton ("Edit " + this.name);
				actionSheet.AddButton ("Cancel");
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {

					if (b.ButtonIndex == (0)) {
						StextUtil.OpenDialerWithNumber(this.mobile);
					} else if (b.ButtonIndex == (1)) {
						string[] recipeints = new string[] { this.mobile }; 
						MFMessageComposeViewController smsView = StextUtil.CreateSimpleSMSView (recipeints, "");
					}else if (b.ButtonIndex == (2)) {
						StextUtil.OpenEmail(this.email,null , null);
					}else if (b.ButtonIndex == (3)) {
						this.appDelegate.GoToView(this.appDelegate.contactDetailView);
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
			this.email = selectedCell.email;
			this.mobile = selectedCell.mobile;

			if (selectedCell.registeredState == ContactListCell.STATE_REGISTERED) {
				this.appDelegate.GoToView (this.appDelegate.chatView);
			}else if (selectedCell.registeredState == ContactListCell.STATE_PENDING) {
				ShowPendingContactOptions ();
				//this.appDelegate.GoToView (this.appDelegate.contactDetailView);
			}else {
				ShowPendingContactOptions ();
				//this.appDelegate.GoToView (this.appDelegate.contactDetailView);
			}

			//this.appDelegate.alert.showOkAlert("Contact State",selectedCell.registeredState.ToString());
		}



	}
}

