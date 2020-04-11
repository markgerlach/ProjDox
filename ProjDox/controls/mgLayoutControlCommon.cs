using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace mgControls
{
	public class mgLayoutControlCommon : DevExpress.XtraLayout.LayoutControl
	{
		public mgLayoutControlCommon()
			: base()
		{
			this.Root.GroupBordersVisible = false;
			//this.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
			//this.LookAndFeel.UseDefaultLookAndFeel = false;
			//this.Root.AppearanceGroup.BackColor = Color.White;
		}
	}
}
