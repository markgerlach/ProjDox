using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using DevExpress.Utils;
using DevExpress.Utils.Controls;

using mgCustom;

namespace _BaseWinProjSingleControl
{
	public partial class ucCommonBase : UserControl, I_BaseWinProjectDisposeObjects, I_BaseWinProjectHelpAccessible, IXtraResizableControl
	{
		private string _screenName = string.Empty;
		private string _helpCode = string.Empty;
		private bool _isDirty = false;

		public ucCommonBase()
		{
			InitializeComponent();
		}

		public virtual void DisposeObjects()
		{ 
		}

		public virtual void RefreshControl()
		{
		}

		private void ucCommonBase_Load(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// The Screen Name used for security purposes to determine what the security level is
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string ScreenName
		{
			get { return _screenName; }
			set { _screenName = value; }
		}

		/// <summary>
		/// Tells if the control is dirty
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool IsDirty
		{
			get { return _isDirty; }
			set { _isDirty = value; }
		}

		/// <summary>
		/// Gives the help code to use for help
		/// If blank, the help viewer shows an error message
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
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

		/// <summary>
		/// Show the help system
		/// </summary>
		public void ShowHelp(string helpCode)
		{
			_helpCode = helpCode;
			ShowHelp();		// Show the help system
		}

		public delegate void ConfigControlDirtiedEventHandler(object sender, EventArgs e);
		public event ConfigControlDirtiedEventHandler ConfigControlDirtied;
		protected void OnConfigControlDirtied()
		{
			if (ConfigControlDirtied != null)
			{
				EventArgs e = new EventArgs();
				ConfigControlDirtied(this, e);
			}
		}

		#region IXtraResizableControl Members

		public event EventHandler Changed;

		public bool IsCaptionVisible
		{
			get { return false; }
		}

		public Size MaxSize
		{
			get { return new Size(0, 0); }
		}

		public Size MinSize
		{
			get { return new Size(0, 0); }
		}

		#endregion
	}
}
