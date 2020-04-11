using System;
using System.Reflection;
using System.Text;
using System.IO;
using System.Data;

namespace mgModel
{
	public class _BaseWinProjectException : Exception
	{
		public _BaseWinProjectException()
		{
		}

		public _BaseWinProjectException(string message)
			: base(message)
		{
		}
	}
}
