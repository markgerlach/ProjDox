using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mgControls
{
	public class mgDevX_TextEdit : DevExpress.XtraEditors.TextEdit
	{
		public mgDevX_TextEdit()
			: base()
		{
			mgInit();		// Run the initialization code
		}

		private void mgInit()
		{
			this.Properties.AppearanceFocused.BackColor = System.Drawing.Color.LightYellow;		// Set the focused color
			this.Properties.AppearanceReadOnly.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Properties.AppearanceDisabled.BackColor = System.Drawing.SystemColors.ControlLight;
		}

		//public bool Enabled
		//{
		//    get
		//    {
		//        return base.Enabled;
		//    }
		//    set
		//    {
		//        base.Enabled = value;
		//        if (value)
		//        {
		//            // Go ahead and reset the colors
		//            this.Properties.AppearanceFocused.BackColor = System.Drawing.Color.LightYellow;		// Set the focused color
		//            this.Properties.AppearanceReadOnly.BackColor = System.Drawing.SystemColors.ControlLight;
		//        }
		//        else
		//        {
		//            this.Properties.AppearanceFocused.BackColor = System.Drawing.Color.LightYellow;		// Set the focused color
		//            this.Properties.AppearanceReadOnly.BackColor = System.Drawing.SystemColors.ControlLight;
		//        }
		//    }
		//}
	}
}
