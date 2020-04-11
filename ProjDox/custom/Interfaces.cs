using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;

namespace mgCustom
{
	public interface I_BaseWinProjectHelpAccessible
	{
		string HelpCode { get; set; }
		void ShowHelp();
		void ShowHelp(string helpCode);
	}

	public interface I_BaseWinProjectDisposeObjects
	{
		void DisposeObjects();
	}
}
