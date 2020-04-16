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
			//#pragma warning disable 0067
			//		// Just let the warning go
			//		public event EventHandler NotImplemented { add { throw new NotImplementedException(); } remove { } }
			//#pragma warning restore 0067

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain());
			//Application.Run(new frmChild());
		}

//#pragma warning disable 0067
//		// Just let the warning go
//		public event EventHandler NotImplemented { add { throw new NotImplementedException(); } remove { } }
//#pragma warning restore 0067
	}
}
