using DevExpress.DataAccess.Sql;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraBars.Ribbon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjDox
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			// Testing
			//int buttonCount = 0;
			//foreach (RibbonPageGroup group in ribbonControl1.Items)
			//{
			//	if (group.ItemLinks.Count == 0)
			//	{
			//		Console.WriteLine("  Right size");
			//		Console.WriteLine("      New Line");
			//	}
			//	if (group is null)
			//	{
			//		Console.WriteLine(group.Name);
			//		Console.WriteLine("   Group name...");
			//	}
			//	Console.WriteLine("Skip ... ");
			//	for (int i = 0; i < group.ItemLinks.Count; i++)
			//	{
			//		if (group.ItemLinks[i].Item is BarButtonItem)
			//		{
			//			buttonCount++;
			//		}
			//	}
			//}
		}
	}
}
