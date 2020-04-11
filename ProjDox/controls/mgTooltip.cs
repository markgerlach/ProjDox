using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Resources;

using DevExpress.Utils;

namespace mgControls
{
	public class mgTooltip : DevExpress.Utils.ToolTipController
	{
		private Color _titleForeColor = Color.Black;
		private Color _textForeColor = Color.Black;

		private int _titleSpacerHeight = 12;

		public mgTooltip(IContainer cont)
			: base(cont)
		{
		}

		//private static readonly object customDraw = new object();

		//[Description("Enables a tooltip's window to be custom painted.")]
		//public event ToolTipControllerCustomDrawEventHandler CustomDraw
		//{
		//    add { Events.AddHandler(customDraw, value); }
		//    remove { Events.RemoveHandler(customDraw, value); }
		//}
		
		public new virtual void OnCustomDraw(DevExpress.Utils.ToolTipControllerCustomDrawEventArgs e)
		{

			//base.OnCustomDraw(e);
			int left = 16, top = 28;
			Font f = e.ShowInfo.Appearance.Font;
			Rectangle r = e.Bounds;
			//r.Inflate(-1 * 6, -1 * 6);
			r = new Rectangle(left, top, r.Width - (left * 2), r.Height - 12);
			StringFormat format = new StringFormat();

			//e.Cache.DrawRectangle(new Pen(new SolidBrush(Color.Black), 1), r);

			if (e.ShowInfo.IconType != DevExpress.Utils.ToolTipIconType.None)
			{
				System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
				string resourceFile = string.Empty;
				foreach (string resource in a.GetManifestResourceNames())
				{
					if (resource.Contains("mgTooltipResources"))
					{
						resourceFile = resource;
						break;
					}
				}
				ResourceManager rm = new ResourceManager(resourceFile.Replace(".resources", ""), a);
				
				Image img = null;
				switch (e.ShowInfo.IconType)
				{
					case DevExpress.Utils.ToolTipIconType.Application:
						img = (Image)rm.GetObject("application");
						break;
					case DevExpress.Utils.ToolTipIconType.Asterisk:
						img = (Image)rm.GetObject("asterisk");
						break;
					case DevExpress.Utils.ToolTipIconType.Error:
						img = (Image)rm.GetObject("error");
						break;
					case DevExpress.Utils.ToolTipIconType.Exclamation:
					case DevExpress.Utils.ToolTipIconType.Warning:
						img = (Image)rm.GetObject("warning");
						break;
					case DevExpress.Utils.ToolTipIconType.Hand:
						img = (Image)rm.GetObject("hand");
						break;
					case DevExpress.Utils.ToolTipIconType.Information:
						img = (Image)rm.GetObject("information");
						break;
					case DevExpress.Utils.ToolTipIconType.Question:
						img = (Image)rm.GetObject("question");
						break;
					case DevExpress.Utils.ToolTipIconType.WindLogo:
						img = (Image)rm.GetObject("windows");
						break;
					case DevExpress.Utils.ToolTipIconType.None:
						break;
				}

				if (img != null)
				{
					Rectangle rImage = new Rectangle(r.Width - img.Width + r.Left, r.Top, img.Width, img.Height);
					//e.Cache.DrawRectangle(new Pen(new SolidBrush(Color.Black), 1), rImage);
					e.Cache.Graphics.DrawImage(img, rImage);
				}
			}

			if (!String.IsNullOrEmpty(e.ShowInfo.Title))
			{
				Font fTitle = new Font(f.Name, f.Size + 1, FontStyle.Bold);
				e.Cache.DrawString(e.ShowInfo.Title, fTitle, new SolidBrush(_titleForeColor), r, format);
				SizeF sizeTitle = e.Cache.Graphics.MeasureString(e.ShowInfo.Title, fTitle);
				top += (int)sizeTitle.Height + _titleSpacerHeight;
				r = new Rectangle(r.X, top, r.Width, r.Height - (int)sizeTitle.Height - _titleSpacerHeight);
			}

			if (!String.IsNullOrEmpty(e.ShowInfo.ToolTip))
			{
				//e.Cache.DrawRectangle(new Pen(new SolidBrush(Color.Black), 1), r);
				e.Cache.DrawString(e.ShowInfo.ToolTip, f, new SolidBrush(_textForeColor), r, format);
			}

			e.Handled = true;

			// Throw the custom draw event
			//this.OnCustomDraw(e);
		}

		public new virtual void OnCalcSize(DevExpress.Utils.ToolTipControllerCalcSizeEventArgs e)
		{
			//base.OnCalcSize(e);
			//e.Size = new Size(300, 200);

			int height = 16, width = 16;
			int widthConstant = 24;

			// Get the size for the control
			Panel p = new Panel();
			Graphics g = p.CreateGraphics();
			Font f = e.ShowInfo.Appearance.Font;
			if (!String.IsNullOrEmpty(e.ShowInfo.Title))
			{
				Font fTitle = new Font(f.Name, f.Size + 1, FontStyle.Bold);
				SizeF sizeTitle = g.MeasureString(e.ShowInfo.Title, fTitle);
				height += (int)sizeTitle.Height + _titleSpacerHeight;
				if ((int)sizeTitle.Width + 1 + widthConstant > width)
				{
					width = (int)sizeTitle.Width + 1 + widthConstant;
				}
			}

			// Measure the caption
			if (!String.IsNullOrEmpty(e.ShowInfo.ToolTip))
			{
				SizeF size = g.MeasureString(e.ShowInfo.ToolTip, f);
				height += (int)size.Height + 8;
				if ((int)size.Width + 1 + widthConstant > width)
				{
					width = (int)size.Width + 1 + widthConstant;
				}
			}
			
			// Account for the image
			if (e.ShowInfo.IconType != ToolTipIconType.None)
			{
				width += 20;
			}

			
			if (width < 200) { width = 200; }

			g.Dispose();
			p.Dispose();
			e.Size = new Size(width, height);		// Return the value

			//this.OnCalcSize(e);		// Throw the parent event
		}

		public mgTooltip()
		{
		}

		public Color TitleForeColor
		{
			get { return _titleForeColor; }
			set { _titleForeColor = value; }
		}

		public Color TextForeColor
		{
			get { return _textForeColor; }
			set { _textForeColor = value; }
		}
	}
}
