using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace mgWinChart.SerializationExample
{
    /// <summary>
    /// An object to serialize or deserialize a specific type of object.
    /// Coded by Alphonso T.
    /// </summary>
    public static class GenericSerializer<T>
    {
        /// <summary>
        /// Serializes an object of the appropriate type into XML.
        /// </summary>
        /// <param name="__obj">The object to serialize.</param>
        /// <returns>A string of XML.</returns>
        public static string Serialize(T __obj)
        {
            string result = string.Empty;
            try
            {
                MemoryStream ms = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);

                serializer.Serialize(writer, __obj);
                ms = (MemoryStream)writer.BaseStream;
                result = Encoding.UTF8.GetString(ms.ToArray());
            }
            catch(Exception exc)
            {
                Console.WriteLine("Serialization error: {0}", exc);
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
            return result;
        }
        /// <summary>
        /// Deserializes a new object of the appropriate type from a string of XML.
        /// </summary>
        /// <param name="objXML">The XML serialization of an object.</param>
        /// <returns>The new object.</returns>
        public static T Deserialize(string objXML)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(objXML));
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            return (T)xs.Deserialize(ms);
        }
    }
}
