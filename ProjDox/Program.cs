using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: CLSCompliant(true)]
namespace ProjDox
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new frmMain());
			//Application.Run(new frmChild());

			//Application.Run(new frmMain());
			Application.Run(new Form1());
			//Application.Run(new MDIParent1());
		}
	}
}
