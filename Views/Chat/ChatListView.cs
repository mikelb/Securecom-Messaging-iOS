
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

namespace Stext{


	public partial class ChatListView : UIViewController{


		AppDelegate appDelegate;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;

		public ChatListView () : base ("ChatListView", null){}


		public override void ViewDidLoad (){
		
			base.ViewDidLoad ();
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

			ShowEditButton ();

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
				this.appDelegate.alert.showOkAlert("Mark All Read","Marking all chat messages as read.");
			};

		}


		public void RowSelected(UITableView tableView, NSIndexPath indexPath){

			ICustomCell selectedCell = source.CellGroups[indexPath.Section].Cells[indexPath.Row];

			appDelegate.GoToView (appDelegate.chatView);

		}


		private void ShowDoneButton(){
			NavigationItem.SetLeftBarButtonItem (doneButton, false);
			NavigationItem.SetRightBarButtonItem (markAllReadButton,false);
			this.table.SetEditing (true, true);
		}


		private void ShowEditButton(){
			NavigationItem.SetLeftBarButtonItem (editButton, false);
			NavigationItem.SetRightBarButtonItem (composeButton,false);
			this.table.SetEditing (false, true);
		}

		public override void ViewWillAppear (bool animated){
			this.Title = "Chats";
			this.PopulateTable ();
		}


		public void PopulateTable(){

            List<CustomCellGroup> cellGroups = new List<CustomCellGroup>();
            tableCellGroup = new CustomCellGroup();
            cellGroups.Add(tableCellGroup);

            AddressBook book = new AddressBook();
	    List<String> contactlist = new List<String>();
            book.RequestPermission().ContinueWith(t =>
            {
                if (!t.Result)
                {
                    Console.WriteLine("Permission denied by user or manifest");
                    return;
                }

                int counter = 0;
		int contact_count = book.Count();
			Console.WriteLine("Address book count = "+contact_count);

	                foreach (Contact contact in book.OrderBy(c => c.LastName))
	                {
			    int idx = counter++;
			    if(!String.IsNullOrEmpty(contact.DisplayName)){
				foreach (Phone value in contact.Phones)
				{
					if(!value.Number.Contains("*") || !value.Number.Contains("#"))
				        {
						var phoneUtil = PhoneNumberUtil.GetInstance();
						PhoneNumber numberObject = phoneUtil.Parse(value.Number, "US");
						var number = phoneUtil.Format(numberObject, PhoneNumberFormat.E164);
						contactlist.Add(number);
					}
				}
				foreach (Email value in contact.Emails)
				{
					contactlist.Add(value.Address);
				}
			    }
	                    ChatCell chatCell = ChatCell.Create();
	                    chatCell.SetHeader(contact.DisplayName);
	                    //chatCell.SetSubheading("My latest message");
	                    chatCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
	                    if (idx % 2 == 1)
	                        chatCell.MarkAsRead();
	                    tableCellGroup.Cells.Add(chatCell);
	                }
			Dictionary<string,string> tokens = getDirectoryServerTokenDictionary(contactlist);
			

                source = new CustomCellTableSource(cellGroups);
                source.RowSelectedAction = RowSelected;

                table.Source = source;

            }, TaskScheduler.FromCurrentSynchronizationContext());



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

		private Dictionary<string,string> getDirectoryServerTokenDictionary(List<string> e164numbers){
			Dictionary<string,string> tokenDictionary = new Dictionary<string,string>(e164numbers.Count());
			foreach (String number in e164numbers)
			{
				try{
					String tokenWithPadding = getDirectoryServerToken(number);
					string token = tokenWithPadding.Substring(0, tokenWithPadding.Length - 2);
					tokenDictionary.Add(token, number);
				}
				catch(Exception e) {

				}
			}
			return tokenDictionary;
		}

		private string getDirectoryServerToken(string e164number){
			byte[] number = System.Text.Encoding.Default.GetBytes(e164number);
			SHA1 sha1 = SHA1.Create();
			byte[] hash = sha1.ComputeHash(number);
			byte[] hash_10 = new byte[10];
			Array.Copy(hash, hash_10, 10);

			return Encoding.ASCII.GetString(Base64.Encode(hash_10));
		}

		private void ComposeAction(){

			try{

				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();

				actionSheet.AddButton ("New Chat");
				actionSheet.AddButton ("New Group");		
				actionSheet.AddButton ("Cancel");		

				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == (0)) {

					} else if (b.ButtonIndex == (1)) {
						this.appDelegate.GoToView(this.appDelegate.newGroupView);
					} 
				};
				actionSheet.ShowInView (View);
			}catch(Exception ex){
				Console.Write(ex.Message);
			}
		}


	}
}

