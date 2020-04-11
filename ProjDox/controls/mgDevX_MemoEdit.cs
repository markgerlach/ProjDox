using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mgControls
{
	public class mgDevX_MemoEdit : DevExpress.XtraEditors.MemoEdit
	{
		public mgDevX_MemoEdit()
			: base()
		{
			mgInit();		// Run the initialization code
		}

		private void mgInit()
		{
			this.Properties.AppearanceFocused.BackColor = System.Drawing.Color.LightYellow;		// Set the focused color
		}
	}
}
