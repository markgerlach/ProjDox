using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.Utils;
using DevExpress.Utils.Controls;

namespace mgControls
{
	public class mgDevX_GroupControl : DevExpress.XtraEditors.GroupControl, IXtraResizableControl
	{
		public mgDevX_GroupControl()
			: base()
		{
			// Reset some of the properties on load
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
		}

		#region IXtraResizableControl Members

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CS0067")]
		public event EventHandler SizeChanged;

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
