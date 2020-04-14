using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;

using mgControls;
using mgCustom;
using mgModel;

namespace ProjDox
{
	/// <summary>
	/// Summary description for frmLEADBase.
	/// </summary>
	public class frm_ProjDox : System.Windows.Forms.Form, I_BaseWinProjectHelpAccessible
	{
		//private string _screenName = string.Empty;
		private string _subControlName = string.Empty;
		//private string _screenCode = string.Empty;
		private string _explorerKey = string.Empty;
		private bool _shouldBeClosing = false;
		//private bool _commonTasksHandled = false;
		private bool _askBeforeClosing = true;
		protected mgGradientLabel ucGrLbl;
		//public ReportingCollection rpts = null;
		public frm_ProjDox _callingForm = null;
		//protected System.Windows.Forms.ToolTip tips;
		protected mgTooltip tips;
		private System.ComponentModel.IContainer components;
		private bool _canLoadMultiple = false;
		private bool _hasLinks = false;
		private bool _hasScreenCaptionChanges = true;
		private bool _writeFieldValuesToSecObjects = true;
		private string _securityScreenKeyName = string.Empty;

		//protected ReportRetriever reportview;
		//protected ReportRetriever reportPDFview;
		protected Dictionary<string, bool> _controlState = new Dictionary<string, bool>();
		private bool _isDirty = false;
		private bool _closeOnEscape = true;
		private bool _forPrintOnly = false;
		private string _baseFormText = string.Empty;
		private bool _isLoading = true;
		private string _helpCode = string.Empty;
		private object _datasource = null;
		private string _objectGUID = string.Empty;
		private int _searchActions = 0;

		// Create some guids so that we can use them in reporting
//		private string _baseGUID0 = string.Empty;
//		private string _baseGUID1 = string.Empty;
//		private string _baseGUID2 = string.Empty;
//		private string _baseGUID3 = string.Empty;

		private GenericListItemCollection _publicValueCollection = new GenericListItemCollection();

		/// <summary>
		/// Constructor
		/// </summary>
		public frm_ProjDox()
		{
			InitializeComponent();
//			this._guid = Guid.NewGuid();
			//this.Icon = UserInterface.GetDefaultApplicationIcon();
			this.BackColor = Color.White;
			//this.AutoScale = false;
			this.AutoScaleMode = AutoScaleMode.None;
			this.Activated += new EventHandler(frm_BaseWinProjectBase_Activated);
			this.ShowInTaskbar = false;
			this.KeyPreview = true;
//			this.KeyUp += new KeyEventHandler(frmLEADBase_KeyUp);
			this.Text = "<Make sure you replace this title>";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_ProjDox));
			this.ucGrLbl = new mgControls.mgGradientLabel();
			this.tips = new mgControls.mgTooltip(this.components);
			this.SuspendLayout();
			// 
			// ucGrLbl
			// 
			this.ucGrLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ucGrLbl.BackColor = System.Drawing.Color.Black;
			this.ucGrLbl.BackColorGradientFade = System.Drawing.Color.White;
			this.ucGrLbl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ucGrLbl.BackgroundImage")));
			this.ucGrLbl.ColorsReversed = false;
			this.ucGrLbl.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ucGrLbl.Location = new System.Drawing.Point(-2000, 16);
			this.ucGrLbl.Name = "ucGrLbl";
			this.ucGrLbl.Override = false;
			this.ucGrLbl.Size = new System.Drawing.Size(426, 24);
			this.tips.SetSuperTip(this.ucGrLbl, null);
			this.ucGrLbl.TabIndex = 0;
			this.ucGrLbl.Text = "<your text here>";
			this.ucGrLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tips
			// 
			this.tips.TextForeColor = System.Drawing.Color.Black;
			this.tips.TitleForeColor = System.Drawing.Color.Black;
			// 
			// frmLEADBase
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(448, 365);
			this.Controls.Add(this.ucGrLbl);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.Name = "frm_BaseWinProjSingleControlBase";
			this.tips.SetSuperTip(this, null);
			this.Text = "frm_BaseWinProjSingleControlBase";
			this.Load += new System.EventHandler(this.frm_BaseWinProjectBase_Load);
			this.DoubleClick += new System.EventHandler(this.frm_BaseWinProjectBase_DoubleClick);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frm_BaseWinProjectBase_Closing);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_BaseWinProjectBase_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_BaseWinProjectBase_KeyDown);
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// Figure out based on the keyname which link was clicked
		/// </summary>
		/// <param name="keyName">The Keyname of the link that was clicked</param>
		public virtual void NavTabReportLinkClicked(string keyName)
		{
			MessageBox.Show("Base Report Class Fired");
		}

		/// <summary>
		/// Figure out based on the keyname which link was clicked
		/// </summary>
		/// <param name="keyName">The Keyname of the link that was clicked</param>
		public virtual void NavTabHelpLinkClicked(string keyName)
		{
			MessageBox.Show("Base Help Class Fired");
		}

		/// <summary>
		/// Gives the help code to use for help
		/// If blank, the help viewer shows an error message
		/// </summary>
		public string HelpCode
		{
			get { return _helpCode; }
			set { _helpCode = value; }
		}

		/// <summary>
		/// Show the help system
		/// </summary>
		public void ShowHelp()
		{
			if (String.IsNullOrEmpty(_helpCode))
			{
				MessageBox.Show("There is no help code associated to this screen.",
					"No Help Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				// Show the help
				//mwsCommon.UserInterface.HelpForm.LoadURL(HelpSystem.ArticlePrefix + _helpCode);
				//mwsCommon.UserInterface.HelpForm.Show();
				//mwsCommon.UserInterface.HelpForm.BringToFront();
			}
		}

		///// <summary>
		///// Set the screen key depending on the screen type
		///// </summary>
		///// <param name="type">The type of screen for the key</param>
		//public void SetScreenKey(ScreenObjectFormType type)
		//{
		//	this.ObjectGUID = UserInterface.GetScreenKey(type);		// Set the key
		//}

		/// <summary>
		/// Show the help system
		/// </summary>
		public void ShowHelp(string helpCode)
		{
			_helpCode = helpCode;
			ShowHelp();		// Show the help system
		}

		/// <summary>
		/// Set a method to tell that the form is only being opened up for Print
		/// </summary>
		public bool ForPrintOnly
		{
			get { return _forPrintOnly; }
			set
			{
				_forPrintOnly = value;
				//this.Location = new Point(-10000, -10000);
			}
		}

		/// <summary>
		/// Returns the base form text (set by the text property)
		/// </summary>
		public string BaseFormText
		{
			get { return _baseFormText; }
		}

		/// <summary>
		/// Return the data source associated to the form
		/// </summary>
		public virtual object DataSource
		{
			get { return _datasource; }
			set { _datasource = value; }
		}

		/// <summary>
		/// Override the text property to allow the base title to be set
		/// </summary>
		public override string Text
		{
			get { return base.Text; }
			set 
			{
				_baseFormText = value;
				base.Text = value;
			}
		}

		/// <summary>
		/// Tells if the form is currently in a loading state
		/// </summary>
		public virtual bool IsLoading
		{
			get { return _isLoading; }
			set { _isLoading = value; }
		}

		/// <summary>
		/// The security screen Key Name
		/// </summary>
		public string SecurityScreenKeyName
		{
			get { return _securityScreenKeyName; }
			set { _securityScreenKeyName = value; }
		}

		/// <summary>
		/// Search actions used for loading some forms
		/// </summary>
		public virtual int SearchActions
		{
			get { return _searchActions; }
			set { _searchActions = value; }
		}

		/// <summary>
		/// Appends a value to the base form text (set at the time of the text property being set)
		/// </summary>
		/// <param name="text">The text to append to the base title</param>
		public void AppendToBaseTitle(string text)
		{
			string val = _baseFormText;
			if (!String.IsNullOrEmpty(text) && 
				!String.IsNullOrEmpty(text.Trim()))
			{
				val += (text.StartsWith(" ") ? text : " - " + text);
			}
			base.Text = val;
		}

		private void frm_BaseWinProjectBase_Activated(object sender, EventArgs e)
		{
			//if (this.MdiParent != null &&
			//    typeof(IMDIExternalMethods).IsAssignableFrom(this.MdiParent.GetType()))
			//{
			//    ((IMDIExternalMethods)this.MdiParent).ClearLinks();
			//    if (_hasLinks)
			//    {
			//        ((IMDIExternalMethods)this.MdiParent).ResetLinks(_screenName, _subControlName);
			//    }
			//}
		}

		/// <summary>
		/// Write the security information for the screen
		/// </summary>
		public void WriteSecurityInfo()
		{
			//// Add the System fields if this is the right user
			//if (_writeFieldValuesToSecObjects &&
			//    (Security.CurrentUser.UserName.ToLower() == "mgerlachadmin"))
			//{
			//    string sql = string.Empty;
			//    int recsAffected = 0;
			//    StringBuilder sb = new StringBuilder();
			//    Security.WriteFormElementsToSecObjects(this.Controls, _screenName, ref sb);
			//    if (sb.ToString()))
			//    {
			//        sql = "DELETE FROM tSecObjects WHERE sType = 'Screen' " + 
			//            "AND sScreenName = '" + _screenName + 
			//            "' AND sControlName NOT IN (" + sb.ToString() + ")";
			//        recsAffected = DAL.SQLExecNonQuery(sql);

			//        sql = "DELETE FROM tSecObjects WHERE sType = 'Screen' " + 
			//            "AND sScreenName = '" + _screenName + 
			//            "' AND sControlName IN (" + 
			//            "'btnCancel', " + 
			//            "'btnCancel', " + 
			//            "'btnCancel', " + 
			//            "'btnSave', " + 
			//            "'btnFilter', " + 
			//            "'btnClear', " + 
			//            "'btnExpand', " + 
			//            "'btnCollapse')";
			//        recsAffected = DAL.SQLExecNonQuery(sql);

			//        sql = "DELETE FROM tSecObjects WHERE sType = 'Screen' " + 
			//            "AND sScreenName = '" + _screenName + 
			//            "' AND sControlType IN (" + 
			//            "'Infragistics.Win.UltraWinGrid.UltraGrid', " + 
			//            "'mwsLEAD.usercontrols.ucSearchFilter')";
			//        recsAffected = DAL.SQLExecNonQuery(sql);
			//    }
			//}
		}

//        /// <summary>
//        /// Write the form name for the screen
//        /// </summary>
//        public void WriteFormName()
//        {
//#if DEBUG
//            if (System.Environment.MachineName.ToLower() == "mws-lap3")
//            {
//                string sql = string.Empty;
//                int recsAffected = 0;
//                SqlCommand cmd = null;

//                try
//                {
//                    string screenName = this.ScreenName;
//                    string formName = this.Name;
//                    sql = "IF NOT EXISTS (SELECT sScreenName FROM tSecObjects " + 
//                        "WHERE sScreenName = @psScreenName) " + 
//                        "INSERT INTO tSecObjects (sType, sFormName, sScreenName) " + 
//                        "SELECT 'FormName', @psFormName, @psScreenName";
//                    cmd = new SqlCommand(sql);
//                    cmd.Parameters.Add("@psScreenName", SqlDbType.VarChar, 100).Value = screenName;
//                    cmd.Parameters.Add("@psFormName", SqlDbType.VarChar, 100).Value = formName;
//                    recsAffected = DAL.SQLExecNonQuery(cmd, false, false);
//                }
//                catch (SqlException sqle) { Error.WriteErrorLog(sqle); }
//                catch (Exception err) { Error.WriteErrorLog(err); }
//            }
//#endif
//        }

		public void QuickEntryVisible(bool visible)
		{
			if (visible)
			{
//				this.quickEntry.RefreshControlsCollection();		// Refresh the conntrols based on the current screenName
//				this.quickEntry.Visible = visible;		// Make the quick entry visible or hidden
//				this.ActiveControl = quickEntry;
			}
			else
			{
//				this.quickEntry.PopulateMainFormValues();
//				this.quickEntry.Visible = visible;		// Make the quick entry visible or hidden	
			}
		}

		/// <summary>
		/// Tells whether or not the form has links that are in the lower left hand pane
		/// Defaults to false
		/// </summary>
		public bool HasLinks
		{
			get { return _hasLinks; }
			set { _hasLinks = value; }
		}

		/// <summary>
		/// Write the field values into tSecObjects?
		/// </summary>
		public bool WriteFieldValuesToSecObjects
		{
			get { return _writeFieldValuesToSecObjects; }
			set { _writeFieldValuesToSecObjects = value; }
		}

		//public bool CommonTasksHandled
		//{
		//    get { return _commonTasksHandled; }
		//    set { _commonTasksHandled = value; }
		//}

		public bool IsDirty
		{
			get { return _isDirty; }
			set { _isDirty = value; }
		}

		public bool AskBeforeClosing
		{
			get { return _askBeforeClosing; }
			set { _askBeforeClosing = value; }
		}

		/// <summary>
		/// By default in LEAD, a form derived from LEADBase will close on the escape key being pressed
		/// This property allows the user to disable that ability
		/// For instance, on the search screens where the cancel button may be used to save a view
		/// If this is left "true", cancelling out of saving a view will cancel and shut down the form
		/// </summary>
		public bool CloseOnEscape
		{
			get { return _closeOnEscape; }
			set { _closeOnEscape = value; }
		}

//		/// <summary>
//		/// A guid that can be used for reporting purposes
//		/// </summary>
//		public string BaseGUID0
//		{
//			get { return _baseGUID0; }
//			set { _baseGUID0 = value; }
//		}
//		
//		/// <summary>
//		/// A guid that can be used for reporting purposes
//		/// </summary>
//		public string BaseGUID1
//		{
//			get { return _baseGUID1; }
//			set { _baseGUID1 = value; }
//		}
//		
//		/// <summary>
//		/// A guid that can be used for reporting purposes
//		/// </summary>
//		public string BaseGUID2
//		{
//			get { return _baseGUID2; }
//			set { _baseGUID2 = value; }
//		}
//		
//		/// <summary>
//		/// A guid that can be used for reporting purposes
//		/// </summary>
//		public string BaseGUID3
//		{
//			get { return _baseGUID3; }
//			set { _baseGUID3 = value; }
//		}

		/// <summary>
		/// The Public Value Collection that can be used to find values for reporting purposes
		/// </summary>
		public GenericListItemCollection PublicValueCollection
		{
			get { return _publicValueCollection; }
			set { _publicValueCollection = value; }
		}
		
		///// <summary>
		///// The Screen Name used for security purposes to determine what the security level is
		///// </summary>
		//public string ScreenName
		//{
		//    get { return _screenName; }
		//    set { _screenName = value; }
		//}

		/// <summary>
		/// The Screen Name used for security purposes to determine what the security level is
		/// </summary>
		public string SubControlName
		{
			get { return _subControlName; }
			set { _subControlName = value; }
		}

		///// <summary>
		///// Set the screen code so we can figure out what screen we're working with here
		///// </summary>
		//public string ScreenCode
		//{
		//    get { return _screenCode; }
		//    set { _screenCode = value; }
		//}

		/// <summary>
		/// Set the objectGUID of the screen so we can figure out what screen we're working with here
		/// </summary>
		public string ObjectGUID
		{
			get { return _objectGUID; }
			set { _objectGUID = value; }
		}

		/// <summary>
		/// The form instance that called this form
		/// </summary>
		public frm_ProjDox CallingForm
		{
			get { return _callingForm; }
			set { _callingForm = value; }
		}
		
		/// <summary>
		/// Tells if the screen should be closing or not
		/// </summary>
		public bool ShouldBeClosing
		{
			get { return _shouldBeClosing; }
			set { _shouldBeClosing = value; }
		}

		/// <summary>
		/// The key value supplied by the menu item
		/// </summary>
		public string ExplorerKey
		{
			get { return _explorerKey; }
			set { _explorerKey = value; }
		}

		/// <summary>
		/// Tells whether only one of these can be loaded at a time or if the user can load multiples
		/// </summary>
		public bool CanLoadMultiples
		{
			get { return _canLoadMultiple; }
			set { _canLoadMultiple = value; }
		}

		/// <summary>
		/// Tells whether the screen is going to have caption changes applied
		/// Defaults to True
		/// </summary>
		public bool HasScreenCaptionChanges
		{
			get { return _hasScreenCaptionChanges; }
			set { _hasScreenCaptionChanges = value; }
		}

		/// <summary>
		/// Apply the caption changes from the database
		/// </summary>
		protected void ApplyCaptionChanges()
		{
			if (!_hasScreenCaptionChanges)  { return; }		// Don't process unless we need to

			// Process the captions for the form itself
			//ApplyCaptionChangesFromUtilsCollection(this.Controls);
			//mwsCommon.UserInterface.ApplyCaptionChangesFromUtilsCollection(this.Controls, this.ScreenName);

			#region Commented Code 
			//            string sql = string.Empty;
//            DataTable dt = null;
			
//            try
//            {
//                //sql = "EXEC spGetScreenCaptionReplacements " + 
//                //    "@psScreenName = '" + this.ScreenName.Replace("'", "''") + "'";
//                ////TODO: Change code so that a SqlCommand and parameterized query is used instead of text.
//                ////NOTE: A stored procdedure should be used where possible.
//                //dt = DAL.SQLExecDataTable(sql);
////                if (dt.Rows.Count > 0)
////                {
////                    // Run the recursive function
//////					ApplyCaptionChangesFromDataTable(this.Controls, dt);	
////                }
//            }
//            catch (SqlException sqle) { Error.WriteErrorLog(sqle); }
			//            catch (Exception err) { Error.WriteErrorLog(err); }
			#endregion Commented Code
		}

		/// <summary>
		/// Disable the controls
		/// </summary>
		protected void DisableControls()
		{
			// Clear out the control state
			_controlState = new Dictionary<string, bool>();
			string controlName = this.Name;

			foreach (Control cntrl in this.Controls)
			{
				GetChildControlState(cntrl, controlName);
				cntrl.Enabled = false;
			}
			Application.DoEvents();
		}

		/// <summary>
		/// Get a child control's current enabled state
		/// </summary>
		/// <param name="cntrl">The control to evaluate</param>
		private void GetChildControlState(Control cntrl, string controlName)
		{
			// Get the main control state first
			if (!String.IsNullOrEmpty(cntrl.Name))
			{
				string totalName = controlName + "." + cntrl.Name;
				_controlState.Add(totalName, cntrl.Enabled);

				// Then get all child controls for the control
				if (cntrl.HasChildren)
				{
					foreach (Control cntrlChild in cntrl.Controls)
					{
						GetChildControlState(cntrlChild, totalName);
					}
				}
			}
		}

		/// <summary>
		/// Enable the controls
		/// </summary>
		protected void EnableControls()
		{
			string controlName = this.Name;
			foreach (Control cntrl in this.Controls)
			{
				SetChildControlState(cntrl, controlName);
			}
		}

		/// <summary>
		/// Set a child control's current enabled state
		/// </summary>
		/// <param name="cntrl">The control to evaluate</param>
		private void SetChildControlState(Control cntrl, string controlName)
		{
			// Get the main control state first
			if (!String.IsNullOrEmpty(cntrl.Name))
			{
				string totalName = controlName + "." + cntrl.Name;
				foreach (KeyValuePair<string, bool> kvp in _controlState)
				{
					if (kvp.Key == totalName)
					{
						cntrl.Enabled = kvp.Value;
						break;
					}
				}

				// Then get all child controls for the control
				if (cntrl.HasChildren)
				{
					foreach (Control cntrlChild in cntrl.Controls)
					{
						SetChildControlState(cntrlChild, totalName);
					}
				}
			}
		}

		///// <summary>
		///// Apply the caption changes from the Collection
		///// </summary>
		///// <param name="cntrls">The controls collection to process</param>
		///// <param name="dt">The datatable that contains the replacement values</param>
		//private void ApplyCaptionChangesFromUtilsCollection(System.Windows.Forms.Control.ControlCollection cntrls)
		//{
		//    foreach (Control cntrl in cntrls)
		//    {
		//        if (cntrl is Infragistics.Win.UltraWinTabControl.UltraTabPageControl)
		//        {
		//            // Enumerate through the pages on the tab control
		//            ApplyCaptionChangesFromUtilsCollection(((Infragistics.Win.UltraWinTabControl.UltraTabPageControl)cntrl).Controls);

		//            // Change the text on the tab
		//            if (((Infragistics.Win.UltraWinTabControl.UltraTabPageControl)cntrl).Tab != null)
		//            {
		//                ((Infragistics.Win.UltraWinTabControl.UltraTabPageControl)cntrl).Tab.Text =
		//                    Utils.ScreenCaptions.AlterText(((Infragistics.Win.UltraWinTabControl.UltraTabPageControl)cntrl).Tab.Text, this.ScreenName);
		//            }
		//        }

		//        if (!(cntrl is Infragistics.Win.UltraWinTabControl.UltraTabPageControl) &&
		//            cntrl.HasChildren)
		//        {
		//            // Enumerate through the pages on the tab control
		//            ApplyCaptionChangesFromUtilsCollection(cntrl.Controls);
		//        }

		//        if (cntrl is Label ||
		//            cntrl is Infragistics.Win.UltraWinGrid.UltraGrid ||
		//            cntrl is mwsCommon.ucMWSGrid)
		//        {
		//            if (cntrl is Label)
		//            {
		//                cntrl.Text = Utils.ScreenCaptions.AlterText(cntrl.Text, this.ScreenName);
		//            }
		//            if (cntrl is Infragistics.Win.UltraWinGrid.UltraGrid)
		//            {
		//                // Go through each of the columns
		//                Infragistics.Win.UltraWinGrid.UltraGrid grid = (Infragistics.Win.UltraWinGrid.UltraGrid)cntrl;
		//                for (int band = 0; band < grid.DisplayLayout.Bands.Count; band++)
		//                {
		//                    for (int columnIndex = 0; columnIndex < grid.DisplayLayout.Bands[band].Columns.Count; columnIndex++)
		//                    {
		//                        Infragistics.Win.UltraWinGrid.UltraGridColumn col = grid.DisplayLayout.Bands[band].Columns[columnIndex];
		//                        if (!col.Hidden)
		//                        {
		//                            col.Header.Caption = Utils.ScreenCaptions.AlterText(col.Header.Caption, this.ScreenName);
		//                        }
		//                    }
		//                }
		//            }
		//            if (cntrl is mwsCommon.ucMWSGrid)
		//            {
		//                // Go through each of the columns in the column collection
		//                mwsCommon.ucMWSGrid gr = (mwsCommon.ucMWSGrid)cntrl;
		//                for (int band = 0; band < gr.BandCount; band++)
		//                {
		//                    for (int columnIndex = 0; columnIndex < gr.GetGridColumnByIndex(band).Count; columnIndex++)
		//                    {
		//                        MWSGridColumn col = gr.GetGridColumnByIndex(band)[columnIndex];
		//                        col.HeaderCaption = Utils.ScreenCaptions.AlterText(col.HeaderCaption, this.ScreenName);
		//                    }
		//                }
		//            }
		//        }
		//    }
		//}

		/// <summary>
		/// Find the control in the collection and pass it back
		/// Use this function recursively for nested controls
		/// </summary>
		/// <param name="cntrls">The controls collection to look through</param>
		/// <param name="name">The name of the control to find</param>
		/// <returns>A reference to the control</returns>
		private Control FindControl(System.Windows.Forms.Control.ControlCollection cntrls, string name)
		{
			Control rtv = null;

			foreach (Control cntrl in cntrls)
			{
				if (cntrl is DevExpress.XtraLayout.LayoutControl)
				{
					DevExpress.XtraLayout.LayoutControl layoutControl = (DevExpress.XtraLayout.LayoutControl)cntrl;
					foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl.Items)
					{
						if (item is DevExpress.XtraLayout.LayoutControlItem &&
							!(item is DevExpress.XtraLayout.EmptySpaceItem))
						{
							DevExpress.XtraLayout.LayoutControlItem lci = (DevExpress.XtraLayout.LayoutControlItem)item;
							if (lci.Control is DevExpress.XtraEditors.SplitContainerControl)
							{
								Control target = FindControl(((DevExpress.XtraEditors.SplitContainerControl)lci.Control).Panel1.Controls, name);
								if (target == null) { target = FindControl(((DevExpress.XtraEditors.SplitContainerControl)lci.Control).Panel2.Controls, name); }
								if (target != null)
								{
									rtv = target;
									break;
								}
							}
							else if (lci.Control.Name.ToLower() == name.ToLower())
							{
								rtv = lci.Control;
								break;
							}
							else if (lci.Name.ToLower() == name.ToLower())
							{
								rtv = ((DevExpress.XtraLayout.LayoutControlItem)item).Control;
								break;
							}
						}
					}
				}
				else if (cntrl.Name.ToLower() == name.ToLower())
				{
					rtv = cntrl;
					break;
				}
			}

			return rtv;
		}
		
		///// <summary>
		///// Update the styles on the form
		///// </summary>
		//public void ApplyStyles()
		//{
		//    string pref = Utils.Prefs.GetStringByDistinctName("ColorScheme");
		//    MWSGridTypes scheme = MWSGridTypes.Empty;
		//    switch (pref.ToLower())
		//    {
		//        case "empty":
		//            scheme = MWSGridTypes.Empty;
		//            break;
		//        case "bluestandard":
		//        case "blue":
		//            scheme = MWSGridTypes.BlueStandard;
		//            break;
		//        case "greenstandard":
		//        case "green":
		//            scheme = MWSGridTypes.GreenStandard;
		//            break;
		//        case "redstandard":
		//        case "red":
		//            scheme = MWSGridTypes.RedStandard;
		//            break;
		//        case "purplestandard":
		//        case "purple":
		//            scheme = MWSGridTypes.PurpleStandard;
		//            break;
		//        case "orangestandard":
		//        case "orange":
		//            scheme = MWSGridTypes.OrangeStandard;
		//            break;
		//        case "blueextended":
		//            scheme = MWSGridTypes.BlueExtended;
		//            break;
		//        case "greenextended":
		//            scheme = MWSGridTypes.GreenExtended;
		//            break;
		//        case "redextended":
		//            scheme = MWSGridTypes.RedExtended;
		//            break;
		//        case "purpleextended":
		//            scheme = MWSGridTypes.PurpleExtended;
		//            break;
		//        case "orangeextended":
		//            scheme = MWSGridTypes.OrangeExtended;
		//            break;
		//    }

		//    foreach (Control cntrl in this.Controls)
		//    {
		//        if (cntrl is ucMWSGrid)
		//        {
		//            ((ucMWSGrid)cntrl).GridType = scheme;
		//        }
		//        if (cntrl is ucGradientLabel && 
		//            !((ucGradientLabel)cntrl).Override)
		//        {
		//            ((ucGradientLabel)cntrl).ColorScheme = scheme;
		//        }
		//        if (cntrl is mwsButton)
		//        {
		//            ((mwsButton)cntrl).ColorScheme = scheme;
		//        }
		//        if (cntrl is Infragistics.Win.Misc.UltraLabel && 
		//            cntrl.Name.ToLower() == "lblbackdrop")
		//        {
		//            Color[] colors = Utils.GetColorArrayBasedOnPreference(scheme.ToString().Replace("MWSGridTypes.", ""));
		//            ((Infragistics.Win.Misc.UltraLabel)cntrl).Appearance.BackColor2 = colors[1];
		//        }
		//    }
		//}

		/// <summary>
		/// <para>Delegate used to allow the report generation to continue on the UI Thread.</para>
		/// <para>See information on 
		/// <see cref="http://msdn.microsoft.com/library/en-us/cpguide/html/cpconthreadingobjectsfeatures.asp">MultiThreading</see> 
		/// to get more information. Also information on the 
		/// <see cref="http://msdn.microsoft.com/library/en-us/cpref/html/frlrfsystemwindowsformscontrolclassinvoketopic.asp">Invoke 
		/// Method</see> will 
		/// <see cref="http://msdn.microsoft.com/msdnmag/issues/03/02/Multithreading/default.aspx#S5">explain uses</see>.</para>
		/// </summary>
		/// <param name="obj">Calling Object</param>
		/// <param name="e">Event Arguments Passed: Currently empty</param>
		private delegate void ContinueReportDelegate(object obj,EventArgs e);
		
		///// <summary>
		///// <para>Method to show report when generation of the document 
		///// complets on a different thread.</para>
		///// <para>See information on 
		///// <see cref="http://msdn.microsoft.com/library/en-us/cpguide/html/cpconthreadingobjectsfeatures.asp">MultiThreading</see> 
		///// to get more information. Also information on the 
		///// <see cref="http://msdn.microsoft.com/library/en-us/cpref/html/frlrfsystemwindowsformscontrolclassinvoketopic.asp">Invoke 
		///// Method</see> will 
		///// <see cref="http://msdn.microsoft.com/msdnmag/issues/03/02/Multithreading/default.aspx#S5">explain uses</see>.</para>
		///// </summary>
		///// <param name="obj">Calling Object</param>
		///// <param name="e">Event Arguments Passed: Currently empty</param>
		//private void DisplayReport(object obj,EventArgs e)
		//{
		//    if(InvokeRequired)
		//    {
		//        object[] pList = { obj, e };
		//        Invoke(new ContinueReportDelegate(this.DisplayReport),pList);
		//        return;
		//    }
		//    mwsLEAD.common.InstantReporting.ShowReport((ReportRetriever)obj);
		//    Utils.DefaultCursor();
		//    //reportview.Dispose();
		//    //reportPDFview.Dispose();						
		//}

		/// <summary>
		/// Method to dispose of report objects.
		/// </summary>
		//private void DisposeReports()
		//{
		//    if (reportview!=null && !reportview.Disposed)
		//    {
		//        reportview.Dispose();
		//        reportview=null;
				
		//    }

		//    if (reportPDFview!=null && !reportPDFview.Disposed)
		//    {
		//        reportPDFview.Dispose();
		//        reportPDFview=null;
				
		//    }
			
		//}

		///// <summary>
		///// The report generation method when the report name and specific
		///// GUID are known for an item. The Report Receiver class is used 
		///// to generate the report for viewing. Once the object is done 
		///// with the initial setup, the general 
		///// <see cref="StartReporting()">StartReporting</see> method is 
		///// called.
		///// </summary>
		///// <param name="reportName">Name of the Report to generate</param>
		///// <param name="contactGUID">The GUID to filter on.</param>
		//protected void StartReporting(string reportName,string contactGUID)
		//{
		//    Utils.WaitCursor();
		//    Application.DoEvents();
			
		//    //DisposeReports();

		//    //mwsLEAD.common.InstantReporting.RunReport("personneliereportfull", contactGUID);

		//    reportview =new ReportRetriever(reportName, contactGUID);
		//    reportPDFview =new ReportRetriever(reportName, contactGUID);
		//    StartReporting();
		//}

		///// <summary>
		///// The report generation method when the report name and the
		///// initial value collection are known for an item. The 
		///// Report Retriever class is used 
		///// to generate the report for viewing. Once the object is done 
		///// with the initial setup, the general 
		///// <see cref="StartReporting()">StartReporting</see> method is 
		///// called.
		///// </summary>
		///// <param name="reportName">Name of the Report to generate</param>
		///// <param name="valueCollection">list of values sent for the report</param>
		//protected void StartReporting(string reportName,GenericListItemCollection valueCollection)
		//{
		//    Utils.WaitCursor();
		//    Application.DoEvents();
			
		//    //DisposeReports();

		//    //mwsLEAD.common.InstantReporting.RunReport("personneliereportfull", contactGUID);

		//    reportview =new ReportRetriever(reportName, valueCollection);
		//    reportPDFview =new ReportRetriever(reportName, valueCollection);
		//    StartReporting();
		//}

		///// <summary>
		///// <para>The Report Receiver class is used 
		///// to generate the report for viewing. The callback delegate 
		///// is created and assigned to the event. Then the report 
		///// generation is fired off and run on a different thread. 
		///// For the PDF report, the same parameters are assigned to 
		///// the object and the report is run on a different thread.</para>
		///// <seealso cref="ReportRetriever"/>
		///// </summary>
		//private void StartReporting()
		//{
		//    // Assign the event handler for 
		//    // when the report is complete
		//    reportview.ReportCompleted+=new ReportRetriever.ReportCompletedDelegate(this.DisplayReport);
			
		//    // Get the report Criteria
		//    // This is called here because 
		//    // some reports have a UI that 
		//    // appears and that must be 
		//    // done from the UI thread.
		//    reportview.GetCriteria();
			
		//    // Assign the same parameters 
		//    // to the PDF version of the 
		//    // report.
		//    reportPDFview.ExportToPDF=true;
		//    reportPDFview.PDFFileName=reportview.PDFFileName;
		//    reportPDFview.Criteria=reportview.Criteria;

		//    // Go to the same event handler 
		//    // when the PDF report is 
		//    // finished generating.
		//    reportPDFview.ReportCompleted+=new ReportRetriever.ReportCompletedDelegate(this.DisplayReport);
			
		//    //Start generating the report
		//    reportview.CreateReport();
		//    reportPDFview.CreateReport();
		//}

		/// <summary>
		/// Placeholder for the populate function in each of the derived classes
		/// </summary>
		/// <param name="recGUID">The record guid to populate the form for</param>
		public virtual void Populate(string recGUID)
		{
		}

		private void frm_BaseWinProjectBase_DoubleClick(object sender, System.EventArgs e)
		{
#if RELEASE
#else
			// If they double-click on the form with the ctrl key pressed, 
			// check to see if they want to replace the stuff in the sec objects table for the field
			if (Control.ModifierKeys == Keys.Control)
			{
				if (MessageBox.Show("Are you sure you want to rebuild " + 
					"security information for this screen?", "Rebuild Security for Screen...", 
					MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) ==
					DialogResult.Yes)
				{
					string sql = string.Empty;
					//int recsAffected = 0;
			
					try
					{
						UserInterface.WaitCursor();  Application.DoEvents();

						//// First delete all the records
						//sql = "DELETE FROM tSecObjects WHERE sType = 'Screen' AND " + 
						//    "sScreenName = '" + _screenName + "'";
						//recsAffected = DAL.SQLExecNonQuery(sql);

						// Then recreate them
						WriteSecurityInfo();

						UserInterface.DefaultCursor(); Application.DoEvents();
					}
					catch (SqlException sqle) { Error.WriteErrorLog(sqle); }
					catch (Exception err) { Error.WriteErrorLog(err); }
				}
			}
#endif
		}

		private void frm_BaseWinProjectBase_Load(object sender, System.EventArgs e)
		{
			if (this.DesignMode) { return; }

			//ApplyStyles();

			//ApplySecurity(this.ScreenName);		// Apply the screen security to the form
			
			//WriteFormName();

			ApplyCaptionChanges();		// Apply the form captions

			//UserInterface.ResizeLayoutControls();		// Resize the layout controls on the form if they exist
		}

		private void frm_BaseWinProjectBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.DesignMode) { return; }

			// Clear out the styles
			//if (this.MdiParent != null &&
			//	typeof(IMDIExternalMethods).IsAssignableFrom(this.MdiParent.GetType()))
			//{
			//	((IMDIExternalMethods)this.MdiParent).ResetQuickView();
			//}

			// Commented out by MG on 6/13/2008
			//if (mwsLEAD.common.UserInterface._mainMDIForm != null)
			//{
			//    mwsLEAD.common.UserInterface._mainMDIForm.ResetLinks();
			//}
			// End Commented out by MG on 6/13/2008
		}

		private void frm_BaseWinProjectBase_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (_closeOnEscape &&
				e.KeyCode == Keys.Escape)
			{
				try
				{
					this.Close();
				}
				//catch (InvalidOperationException invOpException)
				//{
				//	// Don't do anything
				//}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}

		private void frm_BaseWinProjectBase_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_askBeforeClosing && _isDirty)
			{
				if (MessageBox.Show("Changes have been made to this screen that have not been saved.\r\n\r\n" +
					"Are you sure you want to discard these changes?", "Discard Changes?",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					// When the form is closing, go through and figure out if we can dispose of the objects
					foreach (Control c in this.Controls)
					{
						if (c.HasChildren) { CloseObjectsForControl(c); }
						if (c is I_BaseWinProjectDisposeObjects) { ((I_BaseWinProjectDisposeObjects)c).DisposeObjects(); }
					}
					e.Cancel = false;
				}
				else
				{
					e.Cancel = true;
				}
			}
		}

		private void CloseObjectsForControl(Control c)
		{
			if (c is DevExpress.XtraLayout.LayoutControl)
			{
				foreach (Control cEnum in c.Controls)
				{
					if (cEnum.HasChildren) { CloseObjectsForControl(cEnum); }
					if (cEnum is I_BaseWinProjectDisposeObjects) { ((I_BaseWinProjectDisposeObjects)cEnum).DisposeObjects(); }
				}
			}
		}
	}
}
