using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace mgControls
{
	/// <summary>
	/// Summary description for ucGradientLabel.
	/// </summary>
	public class mgGradientLabel : System.Windows.Forms.UserControl
	{
		private bool _colorsReversed = false;
		protected int _leadingSpaces = 2;
		private Color _foreColor = Color.White;
		private Color _backColor2 = Color.White;
		private bool _override = false;

		protected System.Windows.Forms.Label lbl;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public mgGradientLabel()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.BackColor = Color.Black;
			this.Font = new Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			RedrawControl();
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
			this.lbl = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lbl
			// 
			this.lbl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl.BackColor = System.Drawing.Color.Transparent;
			this.lbl.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl.ForeColor = System.Drawing.Color.White;
			this.lbl.Location = new System.Drawing.Point(8, 0);
			this.lbl.Name = "lbl";
			this.lbl.Size = new System.Drawing.Size(416, 23);
			this.lbl.TabIndex = 89;
			this.lbl.Text = "Equipment By Individual";
			this.lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ucGradientLabel
			// 
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.lbl);
			this.Name = "ucGradientLabel";
			this.Size = new System.Drawing.Size(432, 24);
			this.Resize += new System.EventHandler(this.ucGradientLabel_Resize);
			this.ResumeLayout(false);

		}
		#endregion

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string Text
		{
			get { return lbl.Text; }
			set { lbl.Text = value; }
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public System.Drawing.ContentAlignment TextAlign
		{
			get { return lbl.TextAlign; }
			set { lbl.TextAlign = value; }
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Font Font
		{
			get { return base.Font; }
			set { lbl.Font = base.Font = value; }
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Color ForeColor
		{
			get { return _foreColor; }
			set { lbl.ForeColor = _foreColor = value; }
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Color BackColor
		{
			get { return base.BackColor; }
			set { base.BackColor = value; RedrawControl(); }
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Color BackColorGradientFade
		{
			get { return _backColor2; }
			set { _backColor2 = value; RedrawControl(); }
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool Override
		{
			get { return _override; }
			set { _override = value; }
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool ColorsReversed
		{
			get { return _colorsReversed; }
			set { _colorsReversed = value; }
		}


		/// <summary>
		/// Redraw the control
		/// </summary>
		public void RedrawControl()
		{
			if (!_colorsReversed)
			{
				this.BackgroundImage = GetGradientImage(this.BackColor, _backColor2,
					this.Height, this.Width, 0);
			}
			else
			{
				this.BackgroundImage = GetGradientImage(_backColor2, this.BackColor,
					this.Height, this.Width, 0);
			}
		}

		private void ucGradientLabel_Resize(object sender, System.EventArgs e)
		{
			// When the control is resized, redraw the element
			RedrawControl();
		}

		/// <summary>
		/// Get a gradient image for a label
		/// </summary>
		/// <returns>A picture stream containing the image</returns>
		public static Image GetGradientImage(Color startColor, Color endColor, int height, int width, float angle)
		{
			// Get the current screen resolution
			if (height == 0 || width == 0) { return null; }
			Rectangle imageSize = new Rectangle(0, 0, width, height);

			Image img = new Bitmap(width + 5, height + 5);
			Graphics g = Graphics.FromImage(img);

			LinearGradientBrush lb = new LinearGradientBrush(imageSize, 
				startColor, endColor, angle);
			g.FillRectangle(lb, imageSize);
			lb.Dispose();
			g.Dispose();
		
			return img;		// Return the icon			
		}
	}
}
