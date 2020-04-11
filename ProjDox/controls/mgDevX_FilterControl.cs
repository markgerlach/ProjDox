using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.Filtering;

namespace mgControls
{
	public partial class mgDevX_FilterControl : UserControl
	{
		private bool _existingLabelShowing = false;
		private bool _mouseClicked = false;

		public mgDevX_FilterControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Expose the filter control to the outside world
		/// </summary>
		public DevExpress.XtraEditors.FilterControl FilterControl
		{
			get { return filterControl1; }
			set { filterControl1 = value; }
		}

		/// <summary>
		/// Shows the existing label
		/// </summary>
		public bool ExistingLabelVisible
		{
			get { return _existingLabelShowing; }
			set 
			{ 
				_existingLabelShowing = value;
				layoutExistingFilter.Visibility =
					(_existingLabelShowing ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always :
					DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
			}
		}

		/// <summary>
		/// Expose the Existing Label Text
		/// </summary>
		public string ExistingLabelText
		{
			get { return lblExistingFilter.Text; }
			set { lblExistingFilter.Text = value; }
		}

		private void filterControl1_MouseLeave(object sender, EventArgs e)
		{
			if (!_mouseClicked)
			{
				filterControl1.Focus();
			}
			_mouseClicked = false;
		}

		private void filterControl1_Click(object sender, EventArgs e)
		{
			_mouseClicked = true;
		}
	}
}
