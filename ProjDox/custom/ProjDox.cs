using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace mgCustom
{
	public class _BaseWinProjectSerializer
	{
		/// <summary>
		/// Serialize an object to a string
		/// </summary>
		/// <param name="o">The object o</param>
		/// <returns>The string as the finished serialized object</returns>
		public static string SerlializeObjectToString<T>(T obj)
		{
			string xmlString = null;
			try
			{
				MemoryStream strm = new MemoryStream();
				XmlSerializer x = new XmlSerializer(typeof(T));
				XmlTextWriter xmlTextWriter = new XmlTextWriter(strm, Encoding.ASCII);
				x.Serialize(xmlTextWriter, obj);
				strm = (MemoryStream)xmlTextWriter.BaseStream;
				xmlString = Encoding.ASCII.GetString(strm.GetBuffer());

				// Remove any nulls as it jacks up what goes into the DB
				if (xmlString.IndexOf("\0") > -1)
				{
					xmlString = xmlString.Substring(0, xmlString.IndexOf("\0"));
				}
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				xmlString = string.Empty;
			}
			return xmlString;
		}

		/// <summary>
		/// Deserialize the object frrom the string
		/// </summary>
		/// <param name="s">The string to deserialize</param>
		/// <returns>The completed object</returns>
		public static T DeserializeObjectFromString<T>(string xml)
		{
			XmlSerializer xs = new XmlSerializer(typeof(T));
			MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xml));
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);
			return (T)xs.Deserialize(memoryStream);
		}
	}
}
