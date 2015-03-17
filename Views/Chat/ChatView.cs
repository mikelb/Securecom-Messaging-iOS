
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Dialog;
using MonoTouch.CoreAnimation;
using Securecom.Messaging;
using System.Threading;

namespace Stext{

	public partial class ChatView : UIViewController, IncomingPushListener{


		AppDelegate appDelegate;
		private CustomCellGroup tableCellGroup;
		private CustomCellTableSource source;
		private NSNotificationCenter center = null;
		private UIBarButtonItem backButton = null;

		private List<string> messages;
		private List<long> message_ids;
		private List<string> timeStamps;
		private List<bool> isdelivered;
		private List<Boolean> isLeft;
		private float btnCornerRadius = 5.0f;

		#region ExpandableTextView VARS
		protected bool isKeyboardVisible = false;
		protected RootElement root;
		protected bool prevIsNewline = false;
		protected string prevAccessoryTextViewText = "";
		protected float prevAccessoryTextViewHeight;
		protected bool debug = true;
		protected float keyboardHeight;
		protected int maxInputHeight = 300;
		protected int lineHeight = 17;
		protected int txtViewLength = 245;
		protected int txtViewHeight = 33;
		protected int txtViewX = 16;
		protected int txtViewY = 0;
		#endregion

		public static string ThreadSelected;
		public static string Number;
		public static int ThreadID;
		public static float TableRowHeight = 0;
		protected UIImagePickerController imagePicker;
		private UIImage thumbnail;


		public ChatView () : base ("ChatView", null){
		}


		private void LoadTable(ChatBubbleCell _cell){

			List<CustomCellGroup> cellGroups = new List<CustomCellGroup> ();
			tableCellGroup = new CustomCellGroup ();
			cellGroups.Add (tableCellGroup);

			ChatBubbleCell cell;
			Console.WriteLine("rkolli >>>>> @LoadTable"+", messages.Count = "+messages.Count);
			for(int x = 0 ; x < messages.Count ; x++){
				cell = new ChatBubbleCell(isLeft[x], false, isdelivered[x]);
				cell.Update (messages[x], timeStamps[x]);
				cell.SetMessageID(message_ids[x]);
				if (!isdelivered[x]) {
					cell.SetAsUndelivered();
				}
				tableCellGroup.Cells.Add (cell);
			}

			if(_cell != null){
				tableCellGroup.Cells.Add (_cell);
			}

//			//Pending
//			cell = new ChatBubbleCell(false);
//			cell.Update ("This is a pending cell", "8:00 PM");
//			cell.SetAsPending ();
//			tableCellGroup.Cells.Add (cell);
//
//			//Undelivered
//			cell = new ChatBubbleCell(true);
//			cell.Update ("This is a undelivered cell", "8:05 PM");
//			cell.SetAsUndelivered ();
//			tableCellGroup.Cells.Add (cell);

			SetTableSize();

			source = new CustomCellTableSource(cellGroups);
			source.RowSelectedAction = RowSelected;
			source.DeleteAction = DeleteSelected;
			source.DeleteTitle = "Delete";
			table.Source = source;
		}


		public void DeleteSelected(UITableView tableView, NSIndexPath indexPath){
			ChatBubbleCell selectedCell = (ChatBubbleCell) source.CellGroups[indexPath.Section].Cells[indexPath.Row];
			Console.WriteLine("rkolli >>>>> ChatView @DeleteSelected to DELETE, Message ID = "+selectedCell.getMessageID());
			try{
				lock(this){
					 using (var conn = new SQLite.SQLiteConnection(AppDelegate._pathToMessagesDatabase)) {
						conn.Execute("DELETE FROM PushMessage WHERE TimeStamp = ?", selectedCell.getMessageID());
						conn.Commit();
						conn.Close();
						ViewWillAppear (true);
					}
				}
			}catch(Exception e){
				Console.WriteLine("Error while deleting thread "+e.Message);
			}
			UIAlertView alert = new UIAlertView("Deleted", ""+indexPath.Row, null, "Ok");
			alert.Show();
		}

		public void RowSelected(UITableView tableView, NSIndexPath indexPath){

			ChatBubbleCell selectedCell = (ChatBubbleCell) source.CellGroups[indexPath.Section].Cells[indexPath.Row];
		}

		public void refreshChat(){
			if (ThreadID != 0) {
				table.ReloadData();
				AddDataToConversation();
				LoadTable(null);
			}
		}

		private void InitExpandableTextView(){

			accessoryTextView.Text = " ";
			accessoryCreateButton.TintColor = UIColor.FromRGB (0, 158, 228);
			accessoryCreateButton.Enabled = false;

			prevIsNewline = false;

			RectangleF textViewRect = new RectangleF(txtViewX, txtViewY, txtViewLength, txtViewHeight);
			accessoryTextView.Frame = textViewRect;

			UIEdgeInsets contentInsets = new UIEdgeInsets (0.0f, 0.0f, 0.0f, 0.0f);
			accessoryTextView.ContentInset = contentInsets;

			SizeF size = accessoryTextView.ContentSize;
			size.Height = 33;

			RectangleF acTlbrRect = accessoryToolbar.Frame;
			acTlbrRect.Height = size.Height;
			accessoryToolbar.Frame = acTlbrRect;

			prevAccessoryTextViewHeight = accessoryTextView.ContentSize.Height;

		}


		private void AddExpandableAccessoryView(){

			inputFakeMessage.InputAccessoryView = accessoryToolbar;
			accessoryCreateButton.Title = "Send";

			UIBarButtonItem accessoryTextItem = new UIBarButtonItem (accessoryTextView);
			UIBarButtonItem[] buttonItems = new UIBarButtonItem[2];

			//buttonItems.SetValue (photoButton, 0);
			buttonItems.SetValue (accessoryTextItem, 0);
			buttonItems.SetValue (accessoryCreateButton, 1);

			accessoryToolbar.SetItems (buttonItems, true);
			accessoryToolbar.BarTintColor = UIColor.White;

		}


		private void CheckEmptyTextView(){
			if (accessoryTextView.Text.Length == 0) {
				accessoryTextView.Text = " ";
			}
		}


		private void SetExpandableFrameSize(int resizeMode){

			// 0 = RESIZE, 1 = ADD Line, 2 = Subtract Line

			SizeF contentSize = accessoryTextView.ContentSize;
			RectangleF accTxtViewFrame = accessoryTextView.Frame;

			if (resizeMode == 1) {
				contentSize.Height += lineHeight;
			} else if (resizeMode == 2) {
				contentSize.Height -= lineHeight;
			}

			accTxtViewFrame.Size = contentSize;
			accTxtViewFrame.Width = txtViewLength;
			accTxtViewFrame.Y = txtViewY;
			accTxtViewFrame.X = txtViewX;

			accessoryTextView.Frame = accTxtViewFrame;

			RectangleF accessTlbrFrame = accessoryToolbar.Frame;
			accessTlbrFrame.Height = contentSize.Height;
			accessoryToolbar.Frame = accessTlbrFrame;

		}


		private void AddPhoto(){
			try{

				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();

				actionSheet.AddButton ("Take Photo or Video");
				actionSheet.AddButton ("Choose Existing");		
				actionSheet.AddButton ("Cancel");		

				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == (0)) {
						Stext.Camera.TakePicture(this, (obj) =>{
							var photo = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
							var documentsDirectory = Environment.GetFolderPath
								(Environment.SpecialFolder.Personal);
							string jpgFilename = System.IO.Path.Combine (documentsDirectory, "Photo.jpg"); // hardcoded filename, overwritten each time
							NSData imgData = photo.AsJPEG();
							NSError err = null;
							if (imgData.Save(jpgFilename, false, out err)) {
								Console.WriteLine("saved as " + jpgFilename);
								thumbnail = StextUtil.MaxResizeImage(UIImage.FromFile(jpgFilename), 100, 100);
							} else {
								Console.WriteLine("NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
							}
						});
					} else {
						imagePicker = new UIImagePickerController ();
						imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
						imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.PhotoLibrary);
						imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
						imagePicker.Canceled += Handle_Canceled;
						NavigationController.PresentModalViewController(imagePicker, true);
					} 
				};
				actionSheet.ShowInView (View);
			}catch(Exception ex){
				Console.Write(ex.Message);
			}
		}

		protected void Handle_FinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
		{
			// determine what was selected, video or image
			bool isImage = false;
			switch(e.Info[UIImagePickerController.MediaType].ToString()) {
			case "public.image":
				Console.WriteLine("Image selected");
				isImage = true;
				break;
			case "public.video":
				Console.WriteLine("Video selected");
				break;
			}

			// get common info (shared between images and video)
			NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
			if (referenceURL != null)
				Console.WriteLine("Url:"+referenceURL.ToString ());

			// if it was an image, get the other image info
			if(isImage) {
				// get the original image
				UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
				if(originalImage != null) {
					// do something with the image
					Console.WriteLine ("got the original image");
					thumbnail = StextUtil.MaxResizeImage(originalImage, 100, 100); // display
					SendMessage(true);
				}
			} else { // if it's a video
				// get video url
				NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
				if(mediaURL != null) {
					Console.WriteLine(mediaURL.ToString());
				}
			}
			// dismiss the picker
			imagePicker.DismissModalViewControllerAnimated (true);
		}

		void Handle_Canceled (object sender, EventArgs e) {
			imagePicker.DismissModalViewControllerAnimated(true);
		}

		private void SetExpandableTextViewSize(){

			bool didRedraw = false;
			if (accessoryTextView.ContentSize.Height < 33) {
				Console.WriteLine ("*** CONTENT SIZE LESS THAN IT SHOULD BE.  UI GLITCH FIX IT!!! ****");
			}

			if (accessoryTextView.Text == "") {

				CheckEmptyTextView ();
				InitExpandableTextView ();

			} else {
				char[] textChars = accessoryTextView.Text.ToCharArray ();
				if (textChars.Length >= 2 && textChars [0] == 0x20 && textChars [1] == 0xFFFC) {
					return;
				}

				accessoryTextView.Text = accessoryTextView.Text.TrimStart(new char[] {' ', '\t', '\n'});

				if (accessoryTextView.ContentSize.Height < maxInputHeight) {

					if (accessoryTextView.ContentSize.Height != prevAccessoryTextViewHeight) {
						SetExpandableFrameSize (0);
					}

					if (prevIsNewline) {
						if (prevAccessoryTextViewText.Length > accessoryTextView.Text.Length) {
							SetExpandableFrameSize (2);
						}
					}

					if (prevAccessoryTextViewText.Length < accessoryTextView.Text.Length) {
						if (accessoryTextView.Text.EndsWith ("\n")) {
							SetExpandableFrameSize (1);
							prevIsNewline = true;
						}

					} else if (accessoryTextView.Text.EndsWith ("\n")) {
						prevIsNewline = true;
					} else {
						prevIsNewline = false;
					}
				}
			}		

			prevAccessoryTextViewText = accessoryTextView.Text;
			prevAccessoryTextViewHeight = accessoryTextView.ContentSize.Height;

		}


		private void SendMessage(bool isattachment){

//			messages.Add (accessoryTextView.Text);
//			timeStamps.Add(now.ToString("t"));
//			isLeft.Add(false);


//			ThreadStart childref = new ThreadStart(SendMessageThread);
//			Thread childThread = new Thread(childref);
//			childThread.Start();

			InvokeOnMainThread(delegate {
				SendMessageThread(isattachment);
			});

			//cell.SetAsSent();

			ResignFirstResponders ();
			accessoryTextView.Text = "";
			inputFakeMessage.Text = "Start Typing...";
			
			LoadTable(null);

		}

		private void SendMessageThread(bool isattachment){
			DateTime now = DateTime.Now;
			string format = "ddd HH:mm tt";
			bool delivered = false;
			try{
				MessageManager.SendMessage(MessageManager.PrepareOutgoingMessage(accessoryTextView.Text, Number));
				delivered = true;
			}catch(Exception e){
				Console.WriteLine("Exception while sending message...."+e.Message);
			}

			using (var conn= new SQLite.SQLiteConnection(AppDelegate._pathToMessagesDatabase))
			{
				var pmessage = new PushMessage{Thread_id = ThreadID, Number = Number, TimeStamp = AppDelegate.CurrentTimeMillis(), TimeStamp_Sent = Convert.ToInt64(AppDelegate.CurrentTimeMillis()), Read = 0, Message = accessoryTextView.Text, Status = delivered, Service = "PushLocal"};
				conn.Insert(pmessage);
				conn.Commit();
				conn.Close();
			}

			PointF pf = new PointF (0f, (messages.Count - 1) * TableRowHeight - table.Bounds.Size.Height);
			table.SetContentOffset(pf, true);

			ChatBubbleCell cell = new ChatBubbleCell(false, false, delivered);
			cell.Update (accessoryTextView.Text, now.ToString(format));
			TableRowHeight = cell.getHeight();
			if(!delivered)
				cell.SetAsUndelivered();
			if (isattachment)
				cell.setImagePreview(thumbnail);
			LoadTable(cell);

			AddDataToConversation();
		}


		public override void ViewWillAppear (bool animated){
			this.Title = ThreadSelected;
			AddDataToConversation ();
			this.LoadTable (null);
			base.ViewWillAppear (animated);
		}

		public override void ViewWillDisappear (bool animated){
			setThreadID(0);
		}


		private void AnimateToolbar(){
			var transition = new CATransition ();
			transition.Duration = 0.50;
			transition.Type = CATransition.TransitionMoveIn;
			transition.Subtype = CATransition.TransitionFromTop;
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut.ToString ());

			this.toolbarFakeMessage.Layer.AddAnimation (transition, null);
			this.toolbarFakeMessage.Hidden = false;
		}


		private void ResignFirstResponders(){
			this.textMessage.ResignFirstResponder();
			this.inputFakeMessage.ResignFirstResponder ();
			this.accessoryTextView.ResignFirstResponder ();
			this.toolbarFakeMessage.ResignFirstResponder ();
			this.View.EndEditing (true);
		}
	

		public void SetTableSize(){

			int tablePadding = 62;

//			if (View.Frame.Height != 568) {
//				tablePadding = 62;
//			}

			RectangleF tableFrame;
			tableFrame = table.Frame;

			if (!isKeyboardVisible) {
				tableFrame.Height = chatView.Frame.Height - (toolbarFakeMessage.Frame.Height + tablePadding);
			} else {
				tableFrame.Height = chatView.Frame.Height - (keyboardHeight + tablePadding);
			}
			table.Frame = tableFrame;
		}

	
		private void AddDataToConversation(){
			messages = new List<string> ();
			timeStamps = new List<string> ();
			isLeft = new List<Boolean> ();
			message_ids = new List<long>();
			isdelivered = new List<bool>();
			using (var conn = new SQLite.SQLiteConnection(AppDelegate._pathToMessagesDatabase)) {
				// Check if there is an existing thread for this sender
				List<PushMessage> pmList = conn.Query<PushMessage>("SELECT * FROM PushMessage WHERE Thread_id = ?", ThreadID);

				Console.WriteLine("rkolli >>>>> @AddDataToConversation, ThreadID = "+ThreadID+", Message Count = "+pmList.Count());

				foreach (PushMessage pm in pmList) {
					Console.WriteLine("rkolli >>>>> @AddDataToConversation, message = "+pm.Message+", Sender = "+pm.Number+", Timestamp = "+pm.TimeStamp+", Time Sent = "+pm.TimeStamp_Sent+", Thread id = "+pm.Thread_id+", Service = "+pm.Service);
					messages.Add (pm.Message);
					DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(pm.TimeStamp/1000).ToLocalTime();

					timeStamps.Add (""+epoch.ToString("ddd HH:mm tt"));
					message_ids.Add(pm.TimeStamp);
					isdelivered.Add(pm.Status);
					if (pm.Service != "PushLocal") {
						isLeft.Add(true);
					} else {
						isLeft.Add(false);
					}
				}


				conn.Execute("UPDATE PushChatThread Set Read = ? WHERE ID = ?", 0, ThreadID);
				conn.Commit();
				conn.Close();



			}

		}

		private void AddTestData(){

			messages = new List<string> ();
			timeStamps = new List<string> ();
			isLeft = new List<Boolean> ();
	
			messages.Add ("Hey, how's it going");
			timeStamps.Add ("11:03 AM");
			isLeft.Add(true);

			messages.Add ("Pretty good, you want to grab dinner later?  Or a drink?");
			timeStamps.Add ("11:27 AM");
			isLeft.Add (false);
		}


		private void SetupButtons(){

			lockButton.Image = UIImage.FromFile ("Images/icons/lock@2x.png");

			lockButton.Clicked += (sender, e) => {
				LockAction();
			};

			photoButton.Clicked += (sender, e) => {
				//AddPhoto();
			};

//			edit.TouchUpInside += (sender, e) => {
//				DismissKeyboard ();
//				StartEditing();
//			};

			cancel.Clicked += (sender, e) => {
				DismissKeyboard ();
				EndEditing();
			};

			accessoryCreateButton.Clicked += (sender, e) => {
				SendMessage(false);
				accessoryTextView.Text = "";
				ResignFirstResponders();
				AnimateToolbar();
				InitExpandableTextView();
			};

			accessoryTextView.Changed += (sender, e) => {
				SetExpandableTextViewSize();
				if(accessoryTextView.Text.Length >= 1){
					accessoryCreateButton.Enabled = true;
				}else{
					accessoryCreateButton.Enabled = false;
				}

				if(accessoryTextView.Text == " "){
					accessoryCreateButton.Enabled = false;
				}

			};	

		}


		private void LockAction(){
			try{

				UIActionSheet actionSheet;
				actionSheet = new UIActionSheet ();

				actionSheet.AddButton ("Verify Identity");
				actionSheet.AddButton ("End Secure Session");
				actionSheet.AddButton ("Cancel");

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


		public void EndEditing(){

			editToolbar.Hidden = true;
			table.SetEditing(false,true);

			if(source != null)
				source.DidFinishTableEditing(table);

			if (backButton != null) {
				NavigationItem.SetLeftBarButtonItem (backButton, false);
			} else {
				NavigationItem.SetLeftBarButtonItem (null, false);
			}

			NavigationItem.SetRightBarButtonItem (lockButton,false);
		}


		public void StartEditing(){

			editToolbar.Hidden = false;
			if (table.Editing) {
				table.SetEditing(false,true);
			}

//			if(source != null)
//				source.WillBeginTableEditing(table);

			table.SetEditing(true,true);

			if (NavigationController.NavigationBar.BackItem != null) {
				backButton = NavigationController.NavigationBar.BackItem.BackBarButtonItem;
			}

			NavigationItem.SetLeftBarButtonItem (deleteAllButton,false);
			NavigationItem.SetRightBarButtonItem (cancel,false);
		}

		private void DismissKeyboard (){
			this.textMessage.ResignFirstResponder ();
			this.inputFakeMessage.ResignFirstResponder ();
			this.View.EndEditing(true);
		}


		public override void ViewDidLoad (){

			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			NavigationItem.SetRightBarButtonItem (lockButton,false);
			this.center = NSNotificationCenter.DefaultCenter;

			EndEditing ();
			SetupButtons ();

			InitExpandableTextView ();
			AddExpandableAccessoryView ();
			SetExpandableTextViewSize ();

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			center.AddObserver(
				UIKeyboard.WillHideNotification, (notify) => { 
					try{
						isKeyboardVisible = false;
						var keyboardBounds = (NSValue)notify.UserInfo.ObjectForKey(UIKeyboard.BoundsUserInfoKey);
						var keyboardSize = keyboardBounds.RectangleFValue;
						this.keyboardHeight = keyboardSize.Height;
						SetTableSize();
						AnimateToolbar();	
					}catch(Exception ex){
						Console.Write(ex.Message);
					}
				}
			);	


			center.AddObserver(
				UIKeyboard.WillShowNotification, (notify) => { 
					try{
						this.toolbarFakeMessage.Hidden = true;
						this.accessoryTextView.BecomeFirstResponder();

						isKeyboardVisible = true;

						var keyboardBounds = (NSValue)notify.UserInfo.ObjectForKey(UIKeyboard.BoundsUserInfoKey);
						var keyboardSize = keyboardBounds.RectangleFValue;
						this.keyboardHeight = keyboardSize.Height;

						CheckEmptyTextView();
						SetTableSize();

						this.accessoryTextView.BecomeFirstResponder();
					}catch(Exception ex){
						Console.Write(ex.Message);
					}
				}
			);	
		}

		public void setThreadSelected(string value){
			ThreadSelected = value;
		}


		public void setThreadID(int value){
			ThreadID = value;
		}

		public void setNumber(string value){
			Number = value;
		}
	}

}

