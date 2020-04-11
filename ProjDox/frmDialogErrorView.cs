using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.Drawing;

using DevExpress.XtraNavBar;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;

using mgModel;

namespace _BaseWinProjSingleControl
{
    public partial class frmDialogErrorView : frm_BaseWinProjSingleControlBase
	{
		private ClassGenExceptionCollection _coll = new ClassGenExceptionCollection();

		public frmDialogErrorView(ClassGenExceptionCollection coll)
		{
			InitializeComponent();
			_coll = coll;
			this.Text = "Errors were found...";
		}

		public frmDialogErrorView(ClassGenExceptionCollection coll, string processDescription)
		{
			InitializeComponent();
			_coll = coll;
			this.Text = "Errors were found...";
			ucErrorView.ProcessDescription = processDescription;
		}

		/// <summary>
		/// The hyperlink URL
		/// </summary>
		public string HyperlinkURL
		{
			get { return ucErrorView.HyperlinkURL; }
			set { ucErrorView.HyperlinkURL = value; }
		}

		/// <summary>
		/// The hyperlink Text
		/// </summary>
		public string HyperlinkText
		{
			get { return ucErrorView.HyperlinkText; }
			set { ucErrorView.HyperlinkText = value; }
		}

		/// <summary>
		/// The hyperlink Text and URL
		/// </summary>
		public string HyperlinkTextAndURL
		{
			set { ucErrorView.HyperlinkTextAndURL = value; }
		}

		/// <summary>
		/// The first user defined image
		/// </summary>
		public Image UserDef1Image
		{
			get { return ucErrorView.UserDef1Image; }
			set { ucErrorView.UserDef1Image = value; }
		}

		/// <summary>
		/// The second user defined image
		/// </summary>
		public Image UserDef2Image
		{
			get { return ucErrorView.UserDef2Image; }
			set { ucErrorView.UserDef2Image = value; }
		}

		/// <summary>
		/// The third user defined image
		/// </summary>
		public Image UserDef3Image
		{
			get { return ucErrorView.UserDef3Image; }
			set { ucErrorView.UserDef3Image = value; }
		}

		/// <summary>
		/// Tells if the form is part of a continuable process
		/// </summary>
		public bool ContinuableForm
		{
			get { return ucErrorView.ContinuableForm; }
			set { ucErrorView.ContinuableForm = value; }
		}

		/// <summary>
		/// Tells if the form is part of a cancelable process
		/// </summary>
		public bool CancelableForm
		{
			get { return ucErrorView.CancelableForm; }
			set { ucErrorView.CancelableForm = value; }
		}

		/// <summary>
		/// Tells if the form is part of a printable process
		/// </summary>
		public bool PrintableForm
		{
			get { return ucErrorView.PrintableForm; }
			set { ucErrorView.PrintableForm = value; }
		}

		/// <summary>
		/// The broken rule image
		/// </summary>
		public Image BrokenRuleImage
		{
			get { return ucErrorView.BrokenRuleImage; }
			set { ucErrorView.BrokenRuleImage = value; }
		}

		/// <summary>
		/// The process description
		/// </summary>
		public string ProcessDescription
		{
			get { return ucErrorView.ProcessDescription; }
			set 
			{ 
				ucErrorView.ProcessDescription = value;
				this.Text = (!String.IsNullOrEmpty(ucErrorView.ProcessDescription) ? 
					ucErrorView.ProcessDescription + " - " : "") + "Errors were found...";
			}
		}

		/// <summary>
		/// Set the form for information only
		/// </summary>
		public void InformationOnly()
		{
			this.ContinuableForm = false;
			this.Text = "Information...";
			ucErrorView.PrintButtonText = "&Print Messages";
			ucErrorView.CancelButtonText = "&OK";
			ucErrorView.CancelButtonColor = mgButtonGUIButtonColor.Blue;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.P | Keys.Control))
			{
				ucErrorView.PrintForm();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void frmDialogErrorView_Load(object sender, EventArgs e)
		{
			// Change the icon
			//this.Icon = mwsLEAD.common.UserInterface._mainMDIForm.Icon;
			this.Icon = mgCustom.UserInterface.GetDefaultApplicationIcon(); 

			// Show the form
			this.Show();

			// Reset the error collection
			ucErrorView.ClassGenExceptionCollection = _coll;

			Application.DoEvents();
			ucErrorView.ResetColumnWidths();
		}

		private void ucErrorView_ControlClosed(object sender, EventArgs e)
		{
			// When the close button is clicked, shut down the form
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void frmDialogErrorView_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				this.DialogResult = DialogResult.Cancel;
				this.Close();
				e.Handled = true;
			}
		}

		private void ucErrorView_ContinueProcessing(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Ignore;
			this.Close();
		}

		private void ucErrorView_CancelProcessing(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		/// <summary>
		/// The print button text
		/// </summary>
		public string PrintButtonText
		{
			get { return ucErrorView.PrintButtonText; }
			set { ucErrorView.PrintButtonText = value; }
		}

		/// <summary>
		/// The cancel button text
		/// </summary>
		public string CancelButtonText
		{
			get { return ucErrorView.CancelButtonText; }
			set { ucErrorView.CancelButtonText = value; }
		}

		/// <summary>
		/// The continue button text
		/// </summary>
		public string ContinueButtonText
		{
			get { return ucErrorView.ContinueButtonText; }
			set { ucErrorView.ContinueButtonText = value; }
		}

		/// <summary>
		/// Gets/Sets the Print Button Color
		/// </summary>
		public mgButtonGUIButtonColor PrintButtonColor
		{
			get { return ucErrorView.PrintButtonColor; }
			set { ucErrorView.PrintButtonColor = value; }
		}

		/// <summary>
		/// Gets/Sets the Cancel Button Color
		/// </summary>
		public mgButtonGUIButtonColor CancelButtonColor
		{
			get { return ucErrorView.CancelButtonColor; }
			set { ucErrorView.CancelButtonColor = value; }
		}

		/// <summary>
		/// Gets/Sets the Continue Button Color
		/// </summary>
		public mgButtonGUIButtonColor ContinueButtonColor
		{
			get { return ucErrorView.ContinueButtonColor; }
			set { ucErrorView.ContinueButtonColor = value; }
		}
	}
}
