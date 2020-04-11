using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mgCustom
{
	public class HelpSystem
	{
		public static readonly string CustomerCode = "22F";
		public static readonly string ProjectCode = "K9";
		public static readonly string URL = string.Empty;  //"http://<site>/helpSystem/ArticleViewer.aspx?key=";

		/// <summary>
		/// The article prefix to use for getting articles
		/// </summary>
		public static string ArticlePrefix
		{
			get { return URL + ArticlePrefixNoURL; }
		}

		/// <summary>
		/// The article prefix to use for getting articles
		/// </summary>
		public static string ArticlePrefixNoURL
		{
			get { return CustomerCode + ProjectCode; }
		}
	}
}
