using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace mgControls
{
	public class mgTransparentLabel : System.Windows.Forms.Label
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public mgTransparentLabel()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.BackColor = Color.Transparent;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		protected override CreateParams CreateParams 
		{ 
			get 
			{ 
				CreateParams cp = base.CreateParams; 
				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 
				return cp; 
			} 
		} 
	}
}
