//
// ht

#region Using Block
using System;
using System.Globalization;
using System.Xml;
using ServiceStack.Text.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using ServiceStack.Text.WP;
using System.Collections;
using System.Text;
using System.Dynamic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Emit;
using ServiceStack.Text.Jsv;
using System.IO;
using ServiceStack.Text.Reflection;
using ServiceStack.Text.Common;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using ServiceStack.Common.Support;
using ServiceStack.Text;
using ServiceStack.Text.Support;
using System.Runtime.CompilerServices;
using System.IO.IsolatedStorage;
using System.Net;
using System.IO.Compression;
#endregion Using Block

/// ********   File: \Common\DateTimeSerializer.cs
#region Common_DateTimeSerializer.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	public static class DateTimeSerializer
	{
		public const string ShortDateTimeFormat = "yyyy-MM-dd";					//11
		public const string DefaultDateTimeFormat = "dd/MM/yyyy HH:mm:ss";		//20
		public const string DefaultDateTimeFormatWithFraction = "dd/MM/yyyy HH:mm:ss.fff";	//24
		public const string XsdDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffffZ";	//29
		public const string XsdDateTimeFormat3F = "yyyy-MM-ddTHH:mm:ss.fffZ";	//25
		public const string XsdDateTimeFormatSeconds = "yyyy-MM-ddTHH:mm:ssZ";	//21

		public const string EscapedWcfJsonPrefix = "\\/Date(";
		public const string EscapedWcfJsonSuffix = ")\\/";
		public const string WcfJsonPrefix = "/Date(";
		public const char WcfJsonSuffix = ')';

        public static DateTime? ParseShortestNullableXsdDateTime(string dateTimeStr)
        {
            if (dateTimeStr == null)
                return null;

            return ParseShortestXsdDateTime(dateTimeStr);
        }

	    public static DateTime ParseShortestXsdDateTime(string dateTimeStr)
		{
			if (string.IsNullOrEmpty(dateTimeStr))
				return DateTime.MinValue;

			if (dateTimeStr.StartsWith(EscapedWcfJsonPrefix) || dateTimeStr.StartsWith(WcfJsonPrefix))
				return ParseWcfJsonDate(dateTimeStr);

			if (dateTimeStr.Length == DefaultDateTimeFormat.Length
				|| dateTimeStr.Length == DefaultDateTimeFormatWithFraction.Length)
				return DateTime.Parse(dateTimeStr, CultureInfo.InvariantCulture);

			if (dateTimeStr.Length == XsdDateTimeFormatSeconds.Length)
				return DateTime.ParseExact(dateTimeStr, XsdDateTimeFormatSeconds, null,
										   DateTimeStyles.AdjustToUniversal);

            if (dateTimeStr.Length >= XsdDateTimeFormat3F.Length
                && dateTimeStr.Length <= XsdDateTimeFormat.Length)
            {
                var dateTimeType = JsConfig.DateHandler != JsonDateHandler.ISO8601
                    ? XmlDateTimeSerializationMode.Local
                    : XmlDateTimeSerializationMode.RoundtripKind;

                return XmlConvert.ToDateTime(dateTimeStr, dateTimeType);
            }

            return DateTime.Parse(dateTimeStr, null, DateTimeStyles.AssumeLocal);
        }

		public static string ToDateTimeString(DateTime dateTime)
		{
			return dateTime.ToStableUniversalTime().ToString(XsdDateTimeFormat);
		}

		public static DateTime ParseDateTime(string dateTimeStr)
		{
			return DateTime.ParseExact(dateTimeStr, XsdDateTimeFormat, null);
		}

		public static DateTimeOffset ParseDateTimeOffset(string dateTimeOffsetStr)
		{
            if (string.IsNullOrEmpty(dateTimeOffsetStr)) return default(DateTimeOffset);

            // for interop, do not assume format based on config
            // format: prefer TimestampOffset, DCJSCompatible
            if (dateTimeOffsetStr.StartsWith(EscapedWcfJsonPrefix) ||
                dateTimeOffsetStr.StartsWith(WcfJsonPrefix))
            {
                return ParseWcfJsonDateOffset(dateTimeOffsetStr);
            }

            // format: next preference ISO8601
			// assume utc when no offset specified
            if (dateTimeOffsetStr.LastIndexOfAny(TimeZoneChars) < 10)
            {
                if (!dateTimeOffsetStr.EndsWith("Z")) dateTimeOffsetStr += "Z";
#if __MonoCS__
                // Without that Mono uses a Local timezone))
                dateTimeOffsetStr = dateTimeOffsetStr.Substring(0, dateTimeOffsetStr.Length - 1) + "+00:00"; 
#endif
            }

            return DateTimeOffset.Parse(dateTimeOffsetStr, CultureInfo.InvariantCulture);
		}

		public static string ToXsdDateTimeString(DateTime dateTime)
		{
			return XmlConvert.ToString(dateTime.ToStableUniversalTime(), XmlDateTimeSerializationMode.Utc);
		}

        public static string ToXsdTimeSpanString(TimeSpan timeSpan)
        {
            var r = XmlConvert.ToString(timeSpan);
#if __MonoCS__
            // Mono returns DT even if time is 00:00:00
            if (r.EndsWith("DT")) return r.Substring(0, r.Length - 1);
#endif
            return r;
        }

        public static string ToXsdTimeSpanString(TimeSpan? timeSpan)
        {
            return (timeSpan != null) ? ToXsdTimeSpanString(timeSpan.Value) : null;
        }

		public static DateTime ParseXsdDateTime(string dateTimeStr)
		{
			return XmlConvert.ToDateTime(dateTimeStr, XmlDateTimeSerializationMode.Utc);
		}

        public static TimeSpan ParseTimeSpan(string dateTimeStr)
        {
            return dateTimeStr.StartsWith("P") || dateTimeStr.StartsWith("-P")
                ? ParseXsdTimeSpan(dateTimeStr)
                : TimeSpan.Parse(dateTimeStr);
        }

        public static TimeSpan ParseXsdTimeSpan(string dateTimeStr)
        {
            return XmlConvert.ToTimeSpan(dateTimeStr);
        }

        public static TimeSpan? ParseNullableTimeSpan(string dateTimeStr)
        {
            return string.IsNullOrEmpty(dateTimeStr) 
                ? (TimeSpan?) null 
                : ParseTimeSpan(dateTimeStr);
        }

        public static TimeSpan? ParseXsdNullableTimeSpan(string dateTimeStr)
        {
            return String.IsNullOrEmpty(dateTimeStr) ?
                null :
                new TimeSpan?(XmlConvert.ToTimeSpan(dateTimeStr));
        }

		public static string ToShortestXsdDateTimeString(DateTime dateTime)
		{
			var timeOfDay = dateTime.TimeOfDay;

			if (timeOfDay.Ticks == 0)
				return dateTime.ToString(ShortDateTimeFormat);

			if (timeOfDay.Milliseconds == 0)
				return dateTime.ToStableUniversalTime().ToString(XsdDateTimeFormatSeconds);

			return ToXsdDateTimeString(dateTime);
		}

		static readonly char[] TimeZoneChars = new[] { '+', '-' };

		/// <summary>
		/// WCF Json format: /Date(unixts+0000)/
		/// </summary>
		/// <param name="wcfJsonDate"></param>
		/// <returns></returns>
		public static DateTimeOffset ParseWcfJsonDateOffset(string wcfJsonDate)
		{
			if (wcfJsonDate[0] == '\\')
			{
				wcfJsonDate = wcfJsonDate.Substring(1);
			}

			var suffixPos = wcfJsonDate.IndexOf(WcfJsonSuffix);
			var timeString = (suffixPos < 0) ? wcfJsonDate : wcfJsonDate.Substring(WcfJsonPrefix.Length, suffixPos - WcfJsonPrefix.Length);

			// for interop, do not assume format based on config
			if (!wcfJsonDate.StartsWith(WcfJsonPrefix))
			{
				return DateTimeOffset.Parse(timeString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
			}

			var timeZonePos = timeString.LastIndexOfAny(TimeZoneChars);
			var timeZone = timeZonePos <= 0 ? string.Empty : timeString.Substring(timeZonePos);
			var unixTimeString = timeString.Substring(0, timeString.Length - timeZone.Length);

			var unixTime = long.Parse(unixTimeString);

			if (timeZone == string.Empty)
			{
				// when no timezone offset is supplied, then treat the time as UTC
				return unixTime.FromUnixTimeMs();
			}

			if (JsConfig.DateHandler == JsonDateHandler.DCJSCompatible)
			{
				// DCJS ignores the offset and considers it local time if any offset exists
				// REVIEW: DCJS shoves offset in a separate field 'offsetMinutes', we have the offset in the format, so shouldn't we use it?
				return unixTime.FromUnixTimeMs().ToLocalTime();
			}

			var offset = timeZone.FromTimeOffsetString();
			var date = unixTime.FromUnixTimeMs();
			return new DateTimeOffset(date.Ticks, offset);
		}

		/// <summary>
		/// WCF Json format: /Date(unixts+0000)/
		/// </summary>
		/// <param name="wcfJsonDate"></param>
		/// <returns></returns>
		public static DateTime ParseWcfJsonDate(string wcfJsonDate)
		{
			if (wcfJsonDate[0] == JsonUtils.EscapeChar)
			{
				wcfJsonDate = wcfJsonDate.Substring(1);
			}

			var suffixPos = wcfJsonDate.IndexOf(WcfJsonSuffix);
			var timeString = wcfJsonDate.Substring(WcfJsonPrefix.Length, suffixPos - WcfJsonPrefix.Length);

            // for interop, do not assume format based on config
            if (!wcfJsonDate.StartsWith(WcfJsonPrefix))
            {
				return DateTime.Parse(timeString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
			}

			var timeZonePos = timeString.LastIndexOfAny(TimeZoneChars);
			var timeZone = timeZonePos <= 0 ? string.Empty : timeString.Substring(timeZonePos);
			var unixTimeString = timeString.Substring(0, timeString.Length - timeZone.Length);

			var unixTime = long.Parse(unixTimeString);

			if (timeZone == string.Empty)
			{
                // when no timezone offset is supplied, then treat the time as UTC
				return unixTime.FromUnixTimeMs();
			}

			if (JsConfig.DateHandler == JsonDateHandler.DCJSCompatible)
			{
                // DCJS ignores the offset and considers it local time if any offset exists
				return unixTime.FromUnixTimeMs().ToLocalTime();
			}

            var offset = timeZone.FromTimeOffsetString();
            var date = unixTime.FromUnixTimeMs(offset);
            return new DateTimeOffset(date, offset).DateTime;
		}

		public static string ToWcfJsonDate(DateTime dateTime)
		{
			if (JsConfig.DateHandler == JsonDateHandler.ISO8601)
			{
			    return dateTime.ToString("o", CultureInfo.InvariantCulture);
			}

			var timestamp = dateTime.ToUnixTimeMs();
			var offset = dateTime.Kind == DateTimeKind.Utc
				? string.Empty
				: TimeZoneInfo.Local.GetUtcOffset(dateTime).ToTimeOffsetString();

			return EscapedWcfJsonPrefix + timestamp + offset + EscapedWcfJsonSuffix;
		}

		public static string ToWcfJsonDateTimeOffset(DateTimeOffset dateTimeOffset)
		{
			if (JsConfig.DateHandler == JsonDateHandler.ISO8601)
			{
				return dateTimeOffset.ToString("o", CultureInfo.InvariantCulture);
			}

			var timestamp = dateTimeOffset.Ticks.ToUnixTimeMs();
			var offset = dateTimeOffset.Offset == TimeSpan.Zero
				? string.Empty
				: dateTimeOffset.Offset.ToTimeOffsetString();

			return EscapedWcfJsonPrefix + timestamp + offset + EscapedWcfJsonSuffix;
		}
	}
}
#endregion Common_DateTimeSerializer.cs

/// ********   File: \Common\DeserializeArray.cs
#region Common_DeserializeArray.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	internal static class DeserializeArrayWithElements<TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static Dictionary<Type, ParseArrayOfElementsDelegate> ParseDelegateCache 
			= new Dictionary<Type, ParseArrayOfElementsDelegate>();

		private delegate object ParseArrayOfElementsDelegate(string value, ParseStringDelegate parseFn);

		public static Func<string, ParseStringDelegate, object> GetParseFn(Type type)
		{
			ParseArrayOfElementsDelegate parseFn;
			if (ParseDelegateCache.TryGetValue(type, out parseFn)) return parseFn.Invoke;

            var genericType = typeof(DeserializeArrayWithElements<,>).MakeGenericType(type, typeof(TSerializer));
            var mi = genericType.GetMethod("ParseGenericArray", BindingFlags.Public | BindingFlags.Static);
            parseFn = (ParseArrayOfElementsDelegate)Delegate.CreateDelegate(typeof(ParseArrayOfElementsDelegate), mi);

            Dictionary<Type, ParseArrayOfElementsDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<Type, ParseArrayOfElementsDelegate>(ParseDelegateCache);
                newCache[type] = parseFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

			return parseFn.Invoke;
		}
	}

	internal static class DeserializeArrayWithElements<T, TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		public static T[] ParseGenericArray(string value, ParseStringDelegate elementParseFn)
		{
			if ((value = DeserializeListWithElements<TSerializer>.StripList(value)) == null) return null;
			if (value == string.Empty) return new T[0];

			if (value[0] == JsWriter.MapStartChar)
			{
				var itemValues = new List<string>();
				var i = 0;
				do
				{
				    itemValues.Add(Serializer.EatTypeValue(value, ref i));
				    Serializer.EatItemSeperatorOrMapEndChar(value, ref i);
				} while (i < value.Length);

				var results = new T[itemValues.Count];
				for (var j=0; j < itemValues.Count; j++)
				{
					results[j] = (T)elementParseFn(itemValues[j]);
				}
				return results;
			}
			else
			{
				var to = new List<T>();
				var valueLength = value.Length;

				var i = 0;
				while (i < valueLength)
				{
					var elementValue = Serializer.EatValue(value, ref i);
					var listValue = elementValue;
					to.Add((T)elementParseFn(listValue));
					if(Serializer.EatItemSeperatorOrMapEndChar(value, ref i)
                        && i == valueLength)
					{
                        // If we ate a separator and we are at the end of the value, 
                        // it means the last element is empty => add default
                        to.Add(default(T));
					}
				}

				return to.ToArray();
			}
		}
	}

	internal static class DeserializeArray<TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static Dictionary<Type, ParseStringDelegate> ParseDelegateCache = new Dictionary<Type, ParseStringDelegate>();

		public static ParseStringDelegate GetParseFn(Type type)
		{
			ParseStringDelegate parseFn;
            if (ParseDelegateCache.TryGetValue(type, out parseFn)) return parseFn;

            var genericType = typeof(DeserializeArray<,>).MakeGenericType(type, typeof(TSerializer));
            var mi = genericType.GetMethod("GetParseFn", BindingFlags.Public | BindingFlags.Static);
            var parseFactoryFn = (Func<ParseStringDelegate>)Delegate.CreateDelegate(
                typeof(Func<ParseStringDelegate>), mi);
            parseFn = parseFactoryFn();
            
            Dictionary<Type, ParseStringDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<Type, ParseStringDelegate>(ParseDelegateCache);
                newCache[type] = parseFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

			return parseFn;
		}
	}

	internal static class DeserializeArray<T, TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		private static readonly ParseStringDelegate CacheFn;

		static DeserializeArray()
		{
			CacheFn = GetParseFn();
		}

		public static ParseStringDelegate Parse
		{
			get { return CacheFn; }
		}

		public static ParseStringDelegate GetParseFn()
		{
			var type = typeof (T);
			if (!type.IsArray)
				throw new ArgumentException(string.Format("Type {0} is not an Array type", type.FullName));

			if (type == typeof(string[]))
				return ParseStringArray;
			if (type == typeof(byte[]))
				return ParseByteArray;

			var elementType = type.GetElementType();
			var elementParseFn = Serializer.GetParseFn(elementType);
			if (elementParseFn != null)
			{
				var parseFn = DeserializeArrayWithElements<TSerializer>.GetParseFn(elementType);
				return value => parseFn(value, elementParseFn);
			}
			return null;
		}

		public static string[] ParseStringArray(string value)
		{
			if ((value = DeserializeListWithElements<TSerializer>.StripList(value)) == null) return null;
			return value == string.Empty
					? new string[0]
					: DeserializeListWithElements<TSerializer>.ParseStringList(value).ToArray();
		}
		
		public static byte[] ParseByteArray(string value)
		{
			if ((value = DeserializeListWithElements<TSerializer>.StripList(value)) == null) return null;
			if ((value = Serializer.UnescapeSafeString(value)) == null) return null;
			return value == string.Empty
			       	? new byte[0]
			       	: Convert.FromBase64String(value);
		}
	}
}
#endregion Common_DeserializeArray.cs

/// ********   File: \Common\DeserializeBuiltin.cs
#region Common_DeserializeBuiltin.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	public static class DeserializeBuiltin<T>
	{
		private static readonly ParseStringDelegate CachedParseFn;
		static DeserializeBuiltin()
		{
			CachedParseFn = GetParseFn();
		}

		public static ParseStringDelegate Parse
		{
			get { return CachedParseFn; }
		}

		private static ParseStringDelegate GetParseFn()
		{
			//Note the generic typeof(T) is faster than using var type = typeof(T)
			if (typeof(T) == typeof(bool))
				return value => bool.Parse(value);
			if (typeof(T) == typeof(byte))
				return value => byte.Parse(value);
			if (typeof(T) == typeof(sbyte))
				return value => sbyte.Parse(value);
			if (typeof(T) == typeof(short))
				return value => short.Parse(value);
			if (typeof(T) == typeof(int))
				return value => int.Parse(value);
			if (typeof(T) == typeof(long))
				return value => long.Parse(value);
			if (typeof(T) == typeof(float))
				return value => float.Parse(value, CultureInfo.InvariantCulture);
			if (typeof(T) == typeof(double))
				return value => double.Parse(value, CultureInfo.InvariantCulture);
			if (typeof(T) == typeof(decimal))
				return value => decimal.Parse(value, CultureInfo.InvariantCulture);

			if (typeof(T) == typeof(Guid))
				return value => new Guid(value);
			if (typeof(T) == typeof(DateTime?))
				return value => DateTimeSerializer.ParseShortestNullableXsdDateTime(value);
            if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
                return value => DateTimeSerializer.ParseShortestXsdDateTime(value);
			if (typeof(T) == typeof(DateTimeOffset) || typeof(T) == typeof(DateTimeOffset?))
				return value => DateTimeSerializer.ParseDateTimeOffset(value);
            if (typeof(T) == typeof(TimeSpan))
                return value => DateTimeSerializer.ParseTimeSpan(value);
			if (typeof(T) == typeof(TimeSpan?))
                return value => DateTimeSerializer.ParseNullableTimeSpan(value);
#if !MONOTOUCH && !SILVERLIGHT && !XBOX && !ANDROID
			if (typeof(T) == typeof(System.Data.Linq.Binary))
				return value => new System.Data.Linq.Binary(Convert.FromBase64String(value));
#endif				
			if (typeof(T) == typeof(char))
			{
				char cValue;
				return value => char.TryParse(value, out cValue) ? cValue : '\0';
			}
			if (typeof(T) == typeof(ushort))
				return value => ushort.Parse(value);
			if (typeof(T) == typeof(uint))
				return value => uint.Parse(value);
			if (typeof(T) == typeof(ulong))
				return value => ulong.Parse(value);

			if (typeof(T) == typeof(bool?))
				return value => string.IsNullOrEmpty(value) ? (bool?)null : bool.Parse(value);
			if (typeof(T) == typeof(byte?))
				return value => string.IsNullOrEmpty(value) ? (byte?)null : byte.Parse(value);
			if (typeof(T) == typeof(sbyte?))
				return value => string.IsNullOrEmpty(value) ? (sbyte?)null : sbyte.Parse(value);
			if (typeof(T) == typeof(short?))
				return value => string.IsNullOrEmpty(value) ? (short?)null : short.Parse(value);
			if (typeof(T) == typeof(int?))
				return value => string.IsNullOrEmpty(value) ? (int?)null : int.Parse(value);
			if (typeof(T) == typeof(long?))
				return value => string.IsNullOrEmpty(value) ? (long?)null : long.Parse(value);
			if (typeof(T) == typeof(float?))
				return value => string.IsNullOrEmpty(value) ? (float?)null : float.Parse(value, CultureInfo.InvariantCulture);
			if (typeof(T) == typeof(double?))
				return value => string.IsNullOrEmpty(value) ? (double?)null : double.Parse(value, CultureInfo.InvariantCulture);
			if (typeof(T) == typeof(decimal?))
				return value => string.IsNullOrEmpty(value) ? (decimal?)null : decimal.Parse(value, CultureInfo.InvariantCulture);
			
			if (typeof(T) == typeof(TimeSpan?))
				return value => string.IsNullOrEmpty(value) ? (TimeSpan?)null : TimeSpan.Parse(value);
			if (typeof(T) == typeof(Guid?))
				return value => string.IsNullOrEmpty(value) ? (Guid?)null : new Guid(value);
			if (typeof(T) == typeof(ushort?))
				return value => string.IsNullOrEmpty(value) ? (ushort?)null : ushort.Parse(value);
			if (typeof(T) == typeof(uint?))
				return value => string.IsNullOrEmpty(value) ? (uint?)null : uint.Parse(value);
			if (typeof(T) == typeof(ulong?))
				return value => string.IsNullOrEmpty(value) ? (ulong?)null : ulong.Parse(value);
			
			if (typeof(T) == typeof(char?))
			{
				char cValue;
				return value => string.IsNullOrEmpty(value) ? (char?)null : char.TryParse(value, out cValue) ? cValue : '\0';
			}
			
			return null;
		}
	}
}
#endregion Common_DeserializeBuiltin.cs

/// ********   File: \Common\DeserializeCollection.cs
#region Common_DeserializeCollection.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

#if WINDOWS_PHONE
#endif

namespace ServiceStack.Text.Common
{
    internal static class DeserializeCollection<TSerializer>
        where TSerializer : ITypeSerializer
    {
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        public static ParseStringDelegate GetParseMethod(Type type)
        {
            var collectionInterface = type.GetTypeWithGenericInterfaceOf(typeof(ICollection<>));
            if (collectionInterface == null)
                throw new ArgumentException(string.Format("Type {0} is not of type ICollection<>", type.FullName));

            //optimized access for regularly used types
            if (type.HasInterface(typeof(ICollection<string>)))
                return value => ParseStringCollection(value, type);

            if (type.HasInterface(typeof(ICollection<int>)))
                return value => ParseIntCollection(value, type);

            var elementType =  collectionInterface.GetGenericArguments()[0];

            var supportedTypeParseMethod = Serializer.GetParseFn(elementType);
            if (supportedTypeParseMethod != null)
            {
                var createCollectionType = type.HasAnyTypeDefinitionsOf(typeof(ICollection<>))
					? null : type;

                return value => ParseCollectionType(value, createCollectionType, elementType, supportedTypeParseMethod);
            }

            return null;
        }

        public static ICollection<string> ParseStringCollection(string value, Type createType)
        {
            var items = DeserializeArrayWithElements<string, TSerializer>.ParseGenericArray(value, Serializer.ParseString);
            return CollectionExtensions.CreateAndPopulate(createType, items);
        }

        public static ICollection<int> ParseIntCollection(string value, Type createType)
        {
            var items = DeserializeArrayWithElements<int, TSerializer>.ParseGenericArray(value, x => int.Parse(x));
            return CollectionExtensions.CreateAndPopulate(createType, items);
        }

        public static ICollection<T> ParseCollection<T>(string value, Type createType, ParseStringDelegate parseFn)
        {
            if (value == null) return null;

            var items = DeserializeArrayWithElements<T, TSerializer>.ParseGenericArray(value, parseFn);
            return CollectionExtensions.CreateAndPopulate(createType, items);
        }

        private static Dictionary<Type, ParseCollectionDelegate> ParseDelegateCache 
			= new Dictionary<Type, ParseCollectionDelegate>();

        private delegate object ParseCollectionDelegate(string value, Type createType, ParseStringDelegate parseFn);

        public static object ParseCollectionType(string value, Type createType, Type elementType, ParseStringDelegate parseFn)
        {
            ParseCollectionDelegate parseDelegate;
            if (ParseDelegateCache.TryGetValue(elementType, out parseDelegate))
                return parseDelegate(value, createType, parseFn);

            var mi = typeof(DeserializeCollection<TSerializer>).GetMethod("ParseCollection", BindingFlags.Static | BindingFlags.Public);
            var genericMi = mi.MakeGenericMethod(new[] { elementType });
            parseDelegate = (ParseCollectionDelegate)Delegate.CreateDelegate(typeof(ParseCollectionDelegate), genericMi);

            Dictionary<Type, ParseCollectionDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<Type, ParseCollectionDelegate>(ParseDelegateCache);
                newCache[elementType] = parseDelegate;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

            return parseDelegate(value, createType, parseFn);
        }
    }
}
#endregion Common_DeserializeCollection.cs

/// ********   File: \Common\DeserializeDictionary.cs
#region Common_DeserializeDictionary.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	internal static class DeserializeDictionary<TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		const int KeyIndex = 0;
		const int ValueIndex = 1;

		public static ParseStringDelegate GetParseMethod(Type type)
		{
			var mapInterface = type.GetTypeWithGenericInterfaceOf(typeof(IDictionary<,>));
			if (mapInterface == null) {
#if !SILVERLIGHT
                if (type == typeof(Hashtable))
                {
                    return ParseHashtable;
                }
#endif
                if (type == typeof(IDictionary))
                {
					return GetParseMethod(typeof(Dictionary<object, object>));
				}
				throw new ArgumentException(string.Format("Type {0} is not of type IDictionary<,>", type.FullName));
			}

			//optimized access for regularly used types
            if (type == typeof(Dictionary<string, string>))
            {
                return ParseStringDictionary;
            }
            if (type == typeof(JsonObject))
            {
                return ParseJsonObject;
            }

            var dictionaryArgs = mapInterface.GetGenericArguments();

			var keyTypeParseMethod = Serializer.GetParseFn(dictionaryArgs[KeyIndex]);
			if (keyTypeParseMethod == null) return null;

			var valueTypeParseMethod = Serializer.GetParseFn(dictionaryArgs[ValueIndex]);
			if (valueTypeParseMethod == null) return null;

			var createMapType = type.HasAnyTypeDefinitionsOf(typeof(Dictionary<,>), typeof(IDictionary<,>))
				? null : type;

			return value => ParseDictionaryType(value, createMapType, dictionaryArgs, keyTypeParseMethod, valueTypeParseMethod);
		}

        public static JsonObject ParseJsonObject(string value)
        {
            var index = VerifyAndGetStartIndex(value, typeof(JsonObject));

            var result = new JsonObject();

            if (JsonTypeSerializer.IsEmptyMap(value)) return result;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementValue = Serializer.EatValue(value, ref index);

                var mapKey = keyValue;
                var mapValue = elementValue;

                result[mapKey] = mapValue;

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return result;
        }

#if !SILVERLIGHT
        public static Hashtable ParseHashtable(string value)
        {
            var index = VerifyAndGetStartIndex(value, typeof(Hashtable));

            var result = new Hashtable();

            if (JsonTypeSerializer.IsEmptyMap(value)) return result;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementValue = Serializer.EatValue(value, ref index);

                var mapKey = keyValue;
                var mapValue = elementValue;

                result[mapKey] = mapValue;

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return result;
        }
#endif

        public static Dictionary<string, string> ParseStringDictionary(string value)
        {
            var index = VerifyAndGetStartIndex(value, typeof(Dictionary<string, string>));

            var result = new Dictionary<string, string>();

            if (JsonTypeSerializer.IsEmptyMap(value)) return result;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementValue = Serializer.EatValue(value, ref index);

                var mapKey = Serializer.UnescapeString(keyValue);
                var mapValue = Serializer.UnescapeString(elementValue);

                result[mapKey] = mapValue;

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return result;
        }

		public static IDictionary<TKey, TValue> ParseDictionary<TKey, TValue>(
			string value, Type createMapType,
			ParseStringDelegate parseKeyFn, ParseStringDelegate parseValueFn)
		{
			if (value == null) return null;

			var tryToParseItemsAsDictionaries =
				JsConfig.ConvertObjectTypesIntoStringDictionary && typeof(TValue) == typeof(object);
			var tryToParseItemsAsPrimitiveTypes =
				JsConfig.TryToParsePrimitiveTypeValues && typeof(TValue) == typeof(object);

			var index = VerifyAndGetStartIndex(value, createMapType);

			var to = (createMapType == null)
				? new Dictionary<TKey, TValue>()
				: (IDictionary<TKey, TValue>)createMapType.CreateInstance();

            if (JsonTypeSerializer.IsEmptyMap(value)) return to;

			var valueLength = value.Length;
			while (index < valueLength) 
            {
				var keyValue = Serializer.EatMapKey(value, ref index);
				Serializer.EatMapKeySeperator(value, ref index);
			    var elementStartIndex = index;
				var elementValue = Serializer.EatTypeValue(value, ref index);

				var mapKey = (TKey)parseKeyFn(keyValue);

				if (tryToParseItemsAsDictionaries)
				{
                    Serializer.EatWhitespace(value, ref elementStartIndex);
					if (elementStartIndex < valueLength && value[elementStartIndex] == JsWriter.MapStartChar)
					{
						var tmpMap = ParseDictionary<TKey, TValue>(elementValue, createMapType, parseKeyFn, parseValueFn);
                        if (tmpMap != null && tmpMap.Count > 0) {
                            to[mapKey] = (TValue) tmpMap;
                        }
					} 
                    else if (elementStartIndex < valueLength && value[elementStartIndex] == JsWriter.ListStartChar) 
                    {
                        to[mapKey] = (TValue) DeserializeList<List<object>, TSerializer>.Parse(elementValue);
                    } 
                    else 
                    {
				        to[mapKey] = (TValue) (tryToParseItemsAsPrimitiveTypes && elementStartIndex < valueLength
				                        ? DeserializeType<TSerializer>.ParsePrimitive(elementValue, value[elementStartIndex])
				                        : parseValueFn(elementValue));
                    }
				}
                else
                {
                    if (tryToParseItemsAsPrimitiveTypes && elementStartIndex < valueLength) {
                        Serializer.EatWhitespace(value, ref elementStartIndex);
				        to[mapKey] = (TValue) DeserializeType<TSerializer>.ParsePrimitive(elementValue, value[elementStartIndex]);
                    } else {
                        to[mapKey] = (TValue) parseValueFn(elementValue);
                    }
				}

				Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
			}

			return to;
		}

		private static int VerifyAndGetStartIndex(string value, Type createMapType)
		{
			var index = 0;
			if (!Serializer.EatMapStartChar(value, ref index))
			{
				//Don't throw ex because some KeyValueDataContractDeserializer don't have '{}'
				Tracer.Instance.WriteDebug("WARN: Map definitions should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
					JsWriter.MapStartChar, createMapType != null ? createMapType.Name : "Dictionary<,>", value.Substring(0, value.Length < 50 ? value.Length : 50));
			}
			return index;
		}

		private static Dictionary<string, ParseDictionaryDelegate> ParseDelegateCache
			= new Dictionary<string, ParseDictionaryDelegate>();

		private delegate object ParseDictionaryDelegate(string value, Type createMapType,
			ParseStringDelegate keyParseFn, ParseStringDelegate valueParseFn);

		public static object ParseDictionaryType(string value, Type createMapType, Type[] argTypes,
			ParseStringDelegate keyParseFn, ParseStringDelegate valueParseFn)
		{

			ParseDictionaryDelegate parseDelegate;
			var key = GetTypesKey(argTypes);
			if (ParseDelegateCache.TryGetValue(key, out parseDelegate))
                return parseDelegate(value, createMapType, keyParseFn, valueParseFn);

            var mi = typeof(DeserializeDictionary<TSerializer>).GetMethod("ParseDictionary", BindingFlags.Static | BindingFlags.Public);
            var genericMi = mi.MakeGenericMethod(argTypes);
            parseDelegate = (ParseDictionaryDelegate)Delegate.CreateDelegate(typeof(ParseDictionaryDelegate), genericMi);

            Dictionary<string, ParseDictionaryDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<string, ParseDictionaryDelegate>(ParseDelegateCache);
                newCache[key] = parseDelegate;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

			return parseDelegate(value, createMapType, keyParseFn, valueParseFn);
		}

		private static string GetTypesKey(params Type[] types)
		{
			var sb = new StringBuilder(256);
			foreach (var type in types)
			{
				if (sb.Length > 0)
					sb.Append(">");

				sb.Append(type.FullName);
			}
			return sb.ToString();
		}
	}
}
#endregion Common_DeserializeDictionary.cs

/// ********   File: \Common\DeserializeDynamic.cs
#region Common_DeserializeDynamic.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//
#if NET40

namespace ServiceStack.Text.Common
{
	internal static class DeserializeDynamic<TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		private static readonly ParseStringDelegate CachedParseFn;
		static DeserializeDynamic()
		{
			CachedParseFn = ParseDynamic;
		}

		public static ParseStringDelegate Parse
		{
			get { return CachedParseFn; }
		}

        public static IDynamicMetaObjectProvider ParseDynamic(string value)
        {
            var index = VerifyAndGetStartIndex(value, typeof(ExpandoObject));

            var result = new ExpandoObject();

            if (JsonTypeSerializer.IsEmptyMap(value)) return result;

            var container = (IDictionary<String, Object>) result;

            var tryToParsePrimitiveTypes = JsConfig.TryToParsePrimitiveTypeValues;

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var keyValue = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var elementValue = Serializer.EatValue(value, ref index);

                var mapKey = Serializer.UnescapeString(keyValue);

                if (JsonUtils.IsJsObject(elementValue)) {
                    container[mapKey] = ParseDynamic(elementValue);
                } else if (JsonUtils.IsJsArray(elementValue)) {
                    container[mapKey] = DeserializeList<List<object>, TSerializer>.Parse(elementValue);
                } else if (tryToParsePrimitiveTypes) {
                    container[mapKey] = DeserializeType<TSerializer>.ParsePrimitive(elementValue) ?? Serializer.UnescapeString(elementValue);
                } else {
                    container[mapKey] = Serializer.UnescapeString(elementValue);
                }

                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return result;
        }

		private static int VerifyAndGetStartIndex(string value, Type createMapType)
		{
			var index = 0;
			if (!Serializer.EatMapStartChar(value, ref index))
			{
				//Don't throw ex because some KeyValueDataContractDeserializer don't have '{}'
				Tracer.Instance.WriteDebug("WARN: Map definitions should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
					JsWriter.MapStartChar, createMapType != null ? createMapType.Name : "Dictionary<,>", value.Substring(0, value.Length < 50 ? value.Length : 50));
			}
			return index;
		}
	}
}
#endif
#endregion Common_DeserializeDynamic.cs

/// ********   File: \Common\DeserializeKeyValuePair.cs
#region Common_DeserializeKeyValuePair.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
    internal static class DeserializeKeyValuePair<TSerializer>
        where TSerializer : ITypeSerializer
    {
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        const int KeyIndex = 0;
        const int ValueIndex = 1;

        public static ParseStringDelegate GetParseMethod(Type type)
        {
            var mapInterface = type.GetTypeWithGenericInterfaceOf(typeof(KeyValuePair<,>));

            var keyValuePairArgs = mapInterface.GetGenericArguments();

            var keyTypeParseMethod = Serializer.GetParseFn(keyValuePairArgs[KeyIndex]);
            if (keyTypeParseMethod == null) return null;

            var valueTypeParseMethod = Serializer.GetParseFn(keyValuePairArgs[ValueIndex]);
            if (valueTypeParseMethod == null) return null;

            var createMapType = type.HasAnyTypeDefinitionsOf(typeof(KeyValuePair<,>))
                ? null : type;

            return value => ParseKeyValuePairType(value, createMapType, keyValuePairArgs, keyTypeParseMethod, valueTypeParseMethod);
        }
        
        public static object ParseKeyValuePair<TKey, TValue>(
            string value, Type createMapType,
            ParseStringDelegate parseKeyFn, ParseStringDelegate parseValueFn)
        {
            if (value == null) return default(KeyValuePair<TKey, TValue>);

            var index = 1;

            if (JsonTypeSerializer.IsEmptyMap(value)) return new KeyValuePair<TKey, TValue>();
            var keyValue = default(TKey);
            var valueValue = default(TValue);

            var valueLength = value.Length;
            while (index < valueLength)
            {
                var key = Serializer.EatMapKey(value, ref index);
                Serializer.EatMapKeySeperator(value, ref index);
                var keyElementValue = Serializer.EatTypeValue(value, ref index);

                if (string.Compare(key, "key", StringComparison.InvariantCultureIgnoreCase) == 0)
                    keyValue = (TKey)parseKeyFn(keyElementValue);
                else if (string.Compare(key, "value", StringComparison.InvariantCultureIgnoreCase) == 0)
                    valueValue = (TValue) parseValueFn(keyElementValue);
                else
                    throw new SerializationException("Incorrect KeyValuePair property: " + key);
                Serializer.EatItemSeperatorOrMapEndChar(value, ref index);
            }

            return new KeyValuePair<TKey, TValue>(keyValue, valueValue);
        }

        private static Dictionary<string, ParseKeyValuePairDelegate> ParseDelegateCache
            = new Dictionary<string, ParseKeyValuePairDelegate>();

        private delegate object ParseKeyValuePairDelegate(string value, Type createMapType,
            ParseStringDelegate keyParseFn, ParseStringDelegate valueParseFn);

        public static object ParseKeyValuePairType(string value, Type createMapType, Type[] argTypes,
            ParseStringDelegate keyParseFn, ParseStringDelegate valueParseFn)
        {

            ParseKeyValuePairDelegate parseDelegate;
            var key = GetTypesKey(argTypes);
            if (ParseDelegateCache.TryGetValue(key, out parseDelegate))
                return parseDelegate(value, createMapType, keyParseFn, valueParseFn);

            var mi = typeof(DeserializeKeyValuePair<TSerializer>).GetMethod("ParseKeyValuePair", BindingFlags.Static | BindingFlags.Public);
            var genericMi = mi.MakeGenericMethod(argTypes);
            parseDelegate = (ParseKeyValuePairDelegate)Delegate.CreateDelegate(typeof(ParseKeyValuePairDelegate), genericMi);

            Dictionary<string, ParseKeyValuePairDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<string, ParseKeyValuePairDelegate>(ParseDelegateCache);
                newCache[key] = parseDelegate;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

            return parseDelegate(value, createMapType, keyParseFn, valueParseFn);
        }

        private static string GetTypesKey(params Type[] types)
        {
            var sb = new StringBuilder(256);
            foreach (var type in types)
            {
                if (sb.Length > 0)
                    sb.Append(">");

                sb.Append(type.FullName);
            }
            return sb.ToString();
        }
    }
}
#endregion Common_DeserializeKeyValuePair.cs

/// ********   File: \Common\DeserializeListWithElements.cs
#region Common_DeserializeListWithElements.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	internal static class DeserializeListWithElements<TSerializer>
		where TSerializer : ITypeSerializer
	{
		internal static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		private static Dictionary<Type, ParseListDelegate> ParseDelegateCache 
			= new Dictionary<Type, ParseListDelegate>();

		private delegate object ParseListDelegate(string value, Type createListType, ParseStringDelegate parseFn);

		public static Func<string, Type, ParseStringDelegate, object> GetListTypeParseFn(
			Type createListType, Type elementType, ParseStringDelegate parseFn)
		{
			ParseListDelegate parseDelegate;
			if (ParseDelegateCache.TryGetValue(elementType, out parseDelegate))
                return parseDelegate.Invoke;

            var genericType = typeof(DeserializeListWithElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetMethod("ParseGenericList", BindingFlags.Static | BindingFlags.Public);
            parseDelegate = (ParseListDelegate)Delegate.CreateDelegate(typeof(ParseListDelegate), mi);

            Dictionary<Type, ParseListDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseDelegateCache;
                newCache = new Dictionary<Type, ParseListDelegate>(ParseDelegateCache);
                newCache[elementType] = parseDelegate;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseDelegateCache, newCache, snapshot), snapshot));

			return parseDelegate.Invoke;
		}

		internal static string StripList(string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;

			const int startQuotePos = 1;
			const int endQuotePos = 2;
			return value[0] == JsWriter.ListStartChar
			       	? value.Substring(startQuotePos, value.Length - endQuotePos)
			       	: value;
		}

		public static List<string> ParseStringList(string value)
		{
			if ((value = StripList(value)) == null) return null;
			if (value == string.Empty) return new List<string>();

			var to = new List<string>();
			var valueLength = value.Length;

			var i = 0;
			while (i < valueLength)
			{
				var elementValue = Serializer.EatValue(value, ref i);
                var listValue = Serializer.UnescapeString(elementValue);
				to.Add(listValue);
				Serializer.EatItemSeperatorOrMapEndChar(value, ref i);
			}

			return to;
		}

		public static List<int> ParseIntList(string value)
		{
			if ((value = StripList(value)) == null) return null;
			if (value == string.Empty) return new List<int>();

			var intParts = value.Split(JsWriter.ItemSeperator);
			var intValues = new List<int>(intParts.Length);
			foreach (var intPart in intParts)
			{
				intValues.Add(int.Parse(intPart));
			}
			return intValues;
		}
	}

	internal static class DeserializeListWithElements<T, TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		public static ICollection<T> ParseGenericList(string value, Type createListType, ParseStringDelegate parseFn)
		{
			if ((value = DeserializeListWithElements<TSerializer>.StripList(value)) == null) return null;

            var isReadOnly = createListType != null 
                && (createListType.IsGenericType && createListType.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>));

			var to = (createListType == null || isReadOnly)
			    ? new List<T>()
			    : (ICollection<T>)createListType.CreateInstance();

			if (value == string.Empty) return to;

			var tryToParseItemsAsPrimitiveTypes =
				JsConfig.TryToParsePrimitiveTypeValues && typeof(T) == typeof(object);

			if (!string.IsNullOrEmpty(value))
			{
				var valueLength = value.Length;
				var i = 0;
                Serializer.EatWhitespace(value, ref i);
				if (i < valueLength && value[i] == JsWriter.MapStartChar)
				{
					do
					{
						var itemValue = Serializer.EatTypeValue(value, ref i);
						to.Add((T)parseFn(itemValue));
                        Serializer.EatWhitespace(value, ref i);
					} while (++i < value.Length);
				}
				else
				{

					while (i < valueLength)
					{
                        var startIndex = i;
						var elementValue = Serializer.EatValue(value, ref i);
						var listValue = elementValue;
                        if (listValue != null) {
                            if (tryToParseItemsAsPrimitiveTypes) {
                                Serializer.EatWhitespace(value, ref startIndex);
				                to.Add((T) DeserializeType<TSerializer>.ParsePrimitive(elementValue, value[startIndex]));
                            } else {
                                to.Add((T) parseFn(elementValue));
                            }
                        }

					    if (Serializer.EatItemSeperatorOrMapEndChar(value, ref i)
					        && i == valueLength)
					    {
					        // If we ate a separator and we are at the end of the value, 
					        // it means the last element is empty => add default
					        to.Add(default(T));
					    }
					}

				}
			}
			
			//TODO: 8-10-2011 -- this CreateInstance call should probably be moved over to ReflectionExtensions, 
			//but not sure how you'd like to go about caching constructors with parameters -- I would probably build a NewExpression, .Compile to a LambdaExpression and cache
			return isReadOnly ? (ICollection<T>)Activator.CreateInstance(createListType, to) : to;
		}
	}

	internal static class DeserializeList<T, TSerializer>
		where TSerializer : ITypeSerializer
	{
		private readonly static ParseStringDelegate CacheFn;

		static DeserializeList()
		{
			CacheFn = GetParseFn();
		}

		public static ParseStringDelegate Parse
		{
			get { return CacheFn; }
		}

		public static ParseStringDelegate GetParseFn()
		{
			var listInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IList<>));
			if (listInterface == null)
				throw new ArgumentException(string.Format("Type {0} is not of type IList<>", typeof(T).FullName));

			//optimized access for regularly used types
			if (typeof(T) == typeof(List<string>))
				return DeserializeListWithElements<TSerializer>.ParseStringList;

			if (typeof(T) == typeof(List<int>))
				return DeserializeListWithElements<TSerializer>.ParseIntList;

			var elementType = listInterface.GetGenericArguments()[0];

			var supportedTypeParseMethod = DeserializeListWithElements<TSerializer>.Serializer.GetParseFn(elementType);
			if (supportedTypeParseMethod != null)
			{
				var createListType = typeof(T).HasAnyTypeDefinitionsOf(typeof(List<>), typeof(IList<>))
					? null : typeof(T);

				var parseFn = DeserializeListWithElements<TSerializer>.GetListTypeParseFn(createListType, elementType, supportedTypeParseMethod);
				return value => parseFn(value, createListType, supportedTypeParseMethod);
			}

			return null;
		}

	}

	internal static class DeserializeEnumerable<T, TSerializer>
		where TSerializer : ITypeSerializer
	{
		private readonly static ParseStringDelegate CacheFn;

		static DeserializeEnumerable()
		{
			CacheFn = GetParseFn();
		}

		public static ParseStringDelegate Parse
		{
			get { return CacheFn; }
		}

		public static ParseStringDelegate GetParseFn()
		{
			var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
			if (enumerableInterface == null)
				throw new ArgumentException(string.Format("Type {0} is not of type IEnumerable<>", typeof(T).FullName));

			//optimized access for regularly used types
			if (typeof(T) == typeof(IEnumerable<string>))
				return DeserializeListWithElements<TSerializer>.ParseStringList;

			if (typeof(T) == typeof(IEnumerable<int>))
				return DeserializeListWithElements<TSerializer>.ParseIntList;

			var elementType = enumerableInterface.GetGenericArguments()[0];

			var supportedTypeParseMethod = DeserializeListWithElements<TSerializer>.Serializer.GetParseFn(elementType);
			if (supportedTypeParseMethod != null)
			{
				const Type createListTypeWithNull = null; //Use conversions outside this class. see: Queue

				var parseFn = DeserializeListWithElements<TSerializer>.GetListTypeParseFn(
					createListTypeWithNull, elementType, supportedTypeParseMethod);

				return value => parseFn(value, createListTypeWithNull, supportedTypeParseMethod);
			}

			return null;
		}

	}
}
#endregion Common_DeserializeListWithElements.cs

/// ********   File: \Common\DeserializeSpecializedCollections.cs
#region Common_DeserializeSpecializedCollections.cs

namespace ServiceStack.Text.Common
{
	internal static class DeserializeSpecializedCollections<T, TSerializer>
		where TSerializer : ITypeSerializer
	{
		private readonly static ParseStringDelegate CacheFn;

		static DeserializeSpecializedCollections()
		{
			CacheFn = GetParseFn();
		}

		public static ParseStringDelegate Parse
		{
			get { return CacheFn; }
		}

		public static ParseStringDelegate GetParseFn()
		{
			if (typeof(T).HasAnyTypeDefinitionsOf(typeof(Queue<>)))
			{
				if (typeof(T) == typeof(Queue<string>))
					return ParseStringQueue;

				if (typeof(T) == typeof(Queue<int>))
					return ParseIntQueue;

				return GetGenericQueueParseFn();
			}

			if (typeof(T).HasAnyTypeDefinitionsOf(typeof(Stack<>)))
			{
				if (typeof(T) == typeof(Stack<string>))
					return ParseStringStack;

				if (typeof(T) == typeof(Stack<int>))
					return ParseIntStack;

				return GetGenericStackParseFn();
			}

#if !SILVERLIGHT
			if (typeof(T) == typeof(StringCollection))
			{
				return ParseStringCollection<TSerializer>;
			}
#endif

			return GetGenericEnumerableParseFn();
		}

		public static Queue<string> ParseStringQueue(string value)
		{
			var parse = (IEnumerable<string>)DeserializeList<List<string>, TSerializer>.Parse(value);
			return new Queue<string>(parse);
		}

		public static Queue<int> ParseIntQueue(string value)
		{
			var parse = (IEnumerable<int>)DeserializeList<List<int>, TSerializer>.Parse(value);
			return new Queue<int>(parse);
		}

#if !SILVERLIGHT
        public static StringCollection ParseStringCollection<TS>(string value) where TS : ITypeSerializer
		{
            if ((value = DeserializeListWithElements<TS>.StripList(value)) == null) return null;
			return value == String.Empty
			       ? new StringCollection()
			       : ToStringCollection(DeserializeListWithElements<TSerializer>.ParseStringList(value));
		}

		public static StringCollection ToStringCollection(List<string> items)
		{
			var to = new StringCollection();
			foreach (var item in items)
			{
				to.Add(item);
			}
			return to;
		}
#endif

		internal static ParseStringDelegate GetGenericQueueParseFn()
		{
			var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
			var elementType = enumerableInterface.GetGenericArguments()[0];

			var genericType = typeof(SpecializedQueueElements<>).MakeGenericType(elementType);

			var mi = genericType.GetMethod("ConvertToQueue", BindingFlags.Static | BindingFlags.Public);

			var convertToQueue = (ConvertObjectDelegate)Delegate.CreateDelegate(typeof(ConvertObjectDelegate), mi);

			var parseFn = DeserializeEnumerable<T, TSerializer>.GetParseFn();

			return x => convertToQueue(parseFn(x));
		}

		public static Stack<string> ParseStringStack(string value)
		{
			var parse = (IEnumerable<string>)DeserializeList<List<string>, TSerializer>.Parse(value);
			return new Stack<string>(parse);
		}

		public static Stack<int> ParseIntStack(string value)
		{
			var parse = (IEnumerable<int>)DeserializeList<List<int>, TSerializer>.Parse(value);
			return new Stack<int>(parse);
		}

		internal static ParseStringDelegate GetGenericStackParseFn()
		{
			var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
			var elementType = enumerableInterface.GetGenericArguments()[0];

			var genericType = typeof(SpecializedQueueElements<>).MakeGenericType(elementType);

			var mi = genericType.GetMethod("ConvertToStack", BindingFlags.Static | BindingFlags.Public);

			var convertToQueue = (ConvertObjectDelegate)Delegate.CreateDelegate(typeof(ConvertObjectDelegate), mi);

			var parseFn = DeserializeEnumerable<T, TSerializer>.GetParseFn();

			return x => convertToQueue(parseFn(x));
		}

		public static ParseStringDelegate GetGenericEnumerableParseFn()
		{
			var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
			var elementType = enumerableInterface.GetGenericArguments()[0];

			var genericType = typeof(SpecializedEnumerableElements<,>).MakeGenericType(typeof(T), elementType);

			var fi = genericType.GetField("ConvertFn", BindingFlags.Static | BindingFlags.Public);

			var convertFn = fi.GetValue(null) as ConvertObjectDelegate;
			if (convertFn == null) return null;

			var parseFn = DeserializeEnumerable<T, TSerializer>.GetParseFn();

			return x => convertFn(parseFn(x));
		}
	}

	internal class SpecializedQueueElements<T>
	{
		public static Queue<T> ConvertToQueue(object enumerable)
		{
			if (enumerable == null) return null;
			return new Queue<T>((IEnumerable<T>)enumerable);
		}

		public static Stack<T> ConvertToStack(object enumerable)
		{
			if (enumerable == null) return null;
			return new Stack<T>((IEnumerable<T>)enumerable);
		}
	}

	internal class SpecializedEnumerableElements<TCollection, T>
	{
		public static ConvertObjectDelegate ConvertFn;

		static SpecializedEnumerableElements()
		{
			foreach (var ctorInfo in typeof(TCollection).GetConstructors())
			{
				var ctorParams = ctorInfo.GetParameters();
				if (ctorParams.Length != 1) continue;
				var ctorParam = ctorParams[0];
				if (typeof(IEnumerable).IsAssignableFrom(ctorParam.ParameterType)
					|| ctorParam.ParameterType.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>)))
				{
					ConvertFn = fromObject => {
						var to = Activator.CreateInstance(typeof(TCollection), fromObject);
						return to;
					};
					return;
				}
			}

			if (typeof(TCollection).IsOrHasGenericInterfaceTypeOf(typeof(ICollection<>)))
			{
				ConvertFn = ConvertFromCollection;
			}
		}

		public static object Convert(object enumerable)
		{
			return ConvertFn(enumerable);
		}

		public static object ConvertFromCollection(object enumerable)
		{
			var to = (ICollection<T>)typeof(TCollection).CreateInstance();
			var from = (IEnumerable<T>)enumerable;
			foreach (var item in from)
			{
				to.Add(item);
			}
			return to;
		}
	}
}
#endregion Common_DeserializeSpecializedCollections.cs

/// ********   File: \Common\DeserializeType.cs
#region Common_DeserializeType.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

#if !XBOX && !MONOTOUCH && !SILVERLIGHT
#endif

namespace ServiceStack.Text.Common
{
	internal static class DeserializeType<TSerializer>
        where TSerializer : ITypeSerializer
    {
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        public static ParseStringDelegate GetParseMethod(TypeConfig typeConfig)
        {
            var type = typeConfig.Type;

            if (!type.IsClass || type.IsAbstract || type.IsInterface) return null;

            var map = DeserializeTypeRef.GetTypeAccessorMap(typeConfig, Serializer);

			var ctorFn = JsConfig.ModelFactory(type);
            if (map == null) 
                return value => ctorFn();
            
            return typeof(TSerializer) == typeof(Json.JsonTypeSerializer)
				? (ParseStringDelegate)(value => DeserializeTypeRefJson.StringToType(type, value, ctorFn, map))
				: value => DeserializeTypeRefJsv.StringToType(type, value, ctorFn, map);
        }

        public static object ObjectStringToType(string strType)
        {
            var type = ExtractType(strType);
            if (type != null)
            {
                var parseFn = Serializer.GetParseFn(type);
                var propertyValue = parseFn(strType);
                return propertyValue;
            }

            if (JsConfig.ConvertObjectTypesIntoStringDictionary && !string.IsNullOrEmpty(strType)) {
                if (strType[0] == JsWriter.MapStartChar) {
                    var dynamicMatch = DeserializeDictionary<TSerializer>.ParseDictionary<string, object>(strType, null, Serializer.UnescapeString, Serializer.UnescapeString);
                    if (dynamicMatch != null && dynamicMatch.Count > 0) {
                        return dynamicMatch;
                    }
                }

                if (strType[0] == JsWriter.ListStartChar) {
                    return DeserializeList<List<object>, TSerializer>.Parse(strType);
                }
            }

            return Serializer.UnescapeString(strType);
        }

        public static Type ExtractType(string strType)
        {
            var typeAttrInObject = Serializer.TypeAttrInObject;
            if (strType != null
				&& strType.Length > typeAttrInObject.Length
				&& strType.Substring(0, typeAttrInObject.Length) == typeAttrInObject)
            {
                var propIndex = typeAttrInObject.Length;
                var typeName = Serializer.UnescapeSafeString(Serializer.EatValue(strType, ref propIndex));

                var type = JsConfig.TypeFinder.Invoke(typeName);

				if (type == null) {
					Tracer.Instance.WriteWarning("Could not find type: " + typeName);
					return null;
				}

#if !SILVERLIGHT && !MONOTOUCH
				if (type.IsInterface || type.IsAbstract) {
					return DynamicProxy.GetInstanceFor(type).GetType();
				}
#endif

                return type;
            }
            return null;
        }

        public static object ParseAbstractType<T>(string value)
        {
            if (typeof(T).IsAbstract)
            {
                if (string.IsNullOrEmpty(value)) return null;
                var concreteType = ExtractType(value);
                if (concreteType != null)
                {
                    return Serializer.GetParseFn(concreteType)(value);
                }
                Tracer.Instance.WriteWarning(
                    "Could not deserialize Abstract Type with unknown concrete type: " + typeof(T).FullName);
            }
            return null;
        }

        public static object ParseQuotedPrimitive(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

#if NET40
            Guid guidValue;
            if (Guid.TryParse(value, out guidValue)) return guidValue;
#endif
            if (value.StartsWith(DateTimeSerializer.EscapedWcfJsonPrefix) || value.StartsWith(DateTimeSerializer.WcfJsonPrefix)) {
                return DateTimeSerializer.ParseWcfJsonDate(value);
            } 
            
            if (JsConfig.DateHandler == JsonDateHandler.ISO8601)
            {
                // check that we have UTC ISO8601 date:
                // YYYY-MM-DDThh:mm:ssZ
                // YYYY-MM-DDThh:mm:ss+02:00
                // YYYY-MM-DDThh:mm:ss-02:00
                if (value.Length > 14 && value[10] == 'T' && 
                    (value.EndsWith("Z", StringComparison.InvariantCulture) 
                        || value[value.Length - 6] == '+'
                        || value[value.Length - 6] == '-'))
                {
                    return DateTimeSerializer.ParseShortestXsdDateTime(value);
                }
            }

            return Serializer.UnescapeString(value);
        }

        public static object ParsePrimitive(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            bool boolValue;
            if (bool.TryParse(value, out boolValue)) return boolValue;

            decimal decimalValue;
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimalValue))
            {
                if (decimalValue == decimal.Truncate(decimalValue))
                {
                    if (decimalValue <= ulong.MaxValue && decimalValue >= 0) return (ulong)decimalValue;
                    if (decimalValue <= long.MaxValue && decimalValue >= long.MinValue)
                    {
                        var longValue = (long)decimalValue;
                        if (longValue <= sbyte.MaxValue && longValue >= sbyte.MinValue) return (sbyte)longValue;
                        if (longValue <= byte.MaxValue && longValue >= byte.MinValue) return (byte)longValue;
                        if (longValue <= short.MaxValue && longValue >= short.MinValue) return (short)longValue;
                        if (longValue <= ushort.MaxValue && longValue >= ushort.MinValue) return (ushort)longValue;
                        if (longValue <= int.MaxValue && longValue >= int.MinValue) return (int)longValue;
                        if (longValue <= uint.MaxValue && longValue >= uint.MinValue) return (uint)longValue;
                    }
                }
                return decimalValue;
            }

            float floatValue;
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out floatValue)) return floatValue;

            double doubleValue;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out doubleValue)) return doubleValue;

            return null;
        }

        internal static object ParsePrimitive(string value, char firstChar)
        {
            if (typeof(TSerializer) == typeof(JsonTypeSerializer)) {
                return firstChar == JsWriter.QuoteChar
                           ? ParseQuotedPrimitive(value)
                           : ParsePrimitive(value);
            }
            return (ParsePrimitive(value) ?? ParseQuotedPrimitive(value));
        }
    }

    internal class TypeAccessor
    {
        internal ParseStringDelegate GetProperty;
        internal SetPropertyDelegate SetProperty;
        internal Type PropertyType;

        public static Type ExtractType(ITypeSerializer Serializer, string strType)
        {
            var typeAttrInObject = Serializer.TypeAttrInObject;

            if (strType != null
				&& strType.Length > typeAttrInObject.Length
				&& strType.Substring(0, typeAttrInObject.Length) == typeAttrInObject)
            {
                var propIndex = typeAttrInObject.Length;
                var typeName = Serializer.EatValue(strType, ref propIndex);
                var type = JsConfig.TypeFinder.Invoke(typeName);

                if (type == null)
                    Tracer.Instance.WriteWarning("Could not find type: " + typeName);

                return type;
            }
            return null;
        }

        public static TypeAccessor Create(ITypeSerializer serializer, TypeConfig typeConfig, PropertyInfo propertyInfo)
        {
            return new TypeAccessor
            {
                PropertyType = propertyInfo.PropertyType,
                GetProperty = serializer.GetParseFn(propertyInfo.PropertyType),
                SetProperty = GetSetPropertyMethod(typeConfig, propertyInfo),
            };
        }

        private static SetPropertyDelegate GetSetPropertyMethod(TypeConfig typeConfig, PropertyInfo propertyInfo)
        {
            if (propertyInfo.ReflectedType != propertyInfo.DeclaringType)
                propertyInfo = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name);

            if (!propertyInfo.CanWrite && !typeConfig.EnableAnonymousFieldSetterses) return null;

            FieldInfo fieldInfo = null;
            if (!propertyInfo.CanWrite)
            {
                //TODO: What string comparison is used in SST?
				string fieldNameFormat = Env.IsMono ? "<{0}>" : "<{0}>i__Field";
                var fieldName = string.Format(fieldNameFormat, propertyInfo.Name);
                var fieldInfos = typeConfig.Type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField);
                foreach (var f in fieldInfos)
                {
                    if (f.IsInitOnly && f.FieldType == propertyInfo.PropertyType && f.Name == fieldName)
                    {
                        fieldInfo = f;
                        break;
                    }
                }

                if (fieldInfo == null) return null;
            }

#if SILVERLIGHT || MONOTOUCH || XBOX
            if (propertyInfo.CanWrite)
            {
                var setMethodInfo = propertyInfo.GetSetMethod(true);
                return (instance, value) => setMethodInfo.Invoke(instance, new[] { value });
            }
            if (fieldInfo == null) return null;
            return (instance, value) => fieldInfo.SetValue(instance, value);
#else
			return propertyInfo.CanWrite
				? CreateIlPropertySetter(propertyInfo)
				: CreateIlFieldSetter(fieldInfo);
#endif
        }

#if !SILVERLIGHT && !MONOTOUCH && !XBOX

		private static SetPropertyDelegate CreateIlPropertySetter(PropertyInfo propertyInfo)
		{
			var propSetMethod = propertyInfo.GetSetMethod(true);
			if (propSetMethod == null)
				return null;

			var setter = CreateDynamicSetMethod(propertyInfo);

			var generator = setter.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
			generator.Emit(OpCodes.Ldarg_1);

			generator.Emit(propertyInfo.PropertyType.IsClass
				? OpCodes.Castclass
				: OpCodes.Unbox_Any,
				propertyInfo.PropertyType);

			generator.EmitCall(OpCodes.Callvirt, propSetMethod, (Type[])null);
			generator.Emit(OpCodes.Ret);

			return (SetPropertyDelegate)setter.CreateDelegate(typeof(SetPropertyDelegate));
		}

		private static SetPropertyDelegate CreateIlFieldSetter(FieldInfo fieldInfo)
		{
			var setter = CreateDynamicSetMethod(fieldInfo);

			var generator = setter.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
			generator.Emit(OpCodes.Ldarg_1);

			generator.Emit(fieldInfo.FieldType.IsClass
				? OpCodes.Castclass
				: OpCodes.Unbox_Any,
				fieldInfo.FieldType);

			generator.Emit(OpCodes.Stfld, fieldInfo);
			generator.Emit(OpCodes.Ret);

			return (SetPropertyDelegate)setter.CreateDelegate(typeof(SetPropertyDelegate));
		}

		private static DynamicMethod CreateDynamicSetMethod(MemberInfo memberInfo)
		{
			var args = new[] { typeof(object), typeof(object) };
			var name = string.Format("_{0}{1}_", "Set", memberInfo.Name);
			var returnType = typeof(void);

			return !memberInfo.DeclaringType.IsInterface
				? new DynamicMethod(name, returnType, args, memberInfo.DeclaringType, true)
				: new DynamicMethod(name, returnType, args, memberInfo.Module, true);
		}
#endif

        internal static SetPropertyDelegate GetSetPropertyMethod(Type type, PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite || propertyInfo.GetIndexParameters().Any()) return null;

#if SILVERLIGHT || MONOTOUCH || XBOX
            var setMethodInfo = propertyInfo.GetSetMethod(true);
            return (instance, value) => setMethodInfo.Invoke(instance, new[] { value });
#else
			return CreateIlPropertySetter(propertyInfo);
#endif
        }
    }
}

#endregion Common_DeserializeType.cs

/// ********   File: \Common\DeserializeTypeRef.cs
#region Common_DeserializeTypeRef.cs

namespace ServiceStack.Text.Common
{
	internal static class DeserializeTypeRef
	{
		internal static SerializationException CreateSerializationError(Type type, string strType)
		{
			return new SerializationException(String.Format(
			"Type definitions should start with a '{0}', expecting serialized type '{1}', got string starting with: {2}",
			JsWriter.MapStartChar, type.Name, strType.Substring(0, strType.Length < 50 ? strType.Length : 50)));
		}

	    internal static SerializationException GetSerializationException(string propertyName, string propertyValueString, Type propertyType, Exception e)
	    {
	        var serializationException = new SerializationException(String.Format("Failed to set property '{0}' with '{1}'", propertyName, propertyValueString), e);
	        if (propertyName != null) {
	            serializationException.Data.Add("propertyName", propertyName);
	        }
	        if (propertyValueString != null) {
	            serializationException.Data.Add("propertyValueString", propertyValueString);
	        }
	        if (propertyType != null) {
	            serializationException.Data.Add("propertyType", propertyType);
	        }
	        return serializationException;
	    }

        internal static Dictionary<string, TypeAccessor> GetTypeAccessorMap(TypeConfig typeConfig, ITypeSerializer serializer)
        {
            var type = typeConfig.Type;

            var propertyInfos = type.GetSerializableProperties();
            if (propertyInfos.Length == 0) return null;

            var map = new Dictionary<string, TypeAccessor>(StringComparer.OrdinalIgnoreCase);

            var isDataContract = type.IsDto();
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyName = propertyInfo.Name;
                if (isDataContract)
                {
                    var dcsDataMember = propertyInfo.GetDataMember();
                    if (dcsDataMember != null && dcsDataMember.Name != null)
                    {
                        propertyName = dcsDataMember.Name;
                    }
                }
                map[propertyName] = TypeAccessor.Create(serializer, typeConfig, propertyInfo);
            }
            return map;
        }

		/* The old Reference generic implementation
		internal static object StringToType(
			ITypeSerializer Serializer, 
			Type type, 
			string strType, 
			EmptyCtorDelegate ctorFn, 
			Dictionary<string, TypeAccessor> typeAccessorMap)
		{
			var index = 0;

			if (strType == null)
				return null;

			if (!Serializer.EatMapStartChar(strType, ref index))
				throw DeserializeTypeRef.CreateSerializationError(type, strType);

			if (strType == JsWriter.EmptyMap) return ctorFn();

			object instance = null;

			var strTypeLength = strType.Length;
			while (index < strTypeLength)
			{
				var propertyName = Serializer.EatMapKey(strType, ref index);

				Serializer.EatMapKeySeperator(strType, ref index);

				var propertyValueStr = Serializer.EatValue(strType, ref index);
				var possibleTypeInfo = propertyValueStr != null && propertyValueStr.Length > 1 && propertyValueStr[0] == '_';

				if (possibleTypeInfo && propertyName == JsWriter.TypeAttr)
				{
					var typeName = Serializer.ParseString(propertyValueStr);
					instance = ReflectionExtensions.CreateInstance(typeName);
					if (instance == null)
					{
						Tracer.Instance.WriteWarning("Could not find type: " + propertyValueStr);
					}
					else
					{
						//If __type info doesn't match, ignore it.
						if (!type.IsInstanceOfType(instance))
							instance = null;
					}

					Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
					continue;
				}

				if (instance == null) instance = ctorFn();

				TypeAccessor typeAccessor;
				typeAccessorMap.TryGetValue(propertyName, out typeAccessor);

				var propType = possibleTypeInfo ? TypeAccessor.ExtractType(Serializer, propertyValueStr) : null;
				if (propType != null)
				{
					try
					{
						if (typeAccessor != null)
						{
							var parseFn = Serializer.GetParseFn(propType);
							var propertyValue = parseFn(propertyValueStr);
							typeAccessor.SetProperty(instance, propertyValue);
						}

						Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);

						continue;
					}
					catch
					{
						Tracer.Instance.WriteWarning("WARN: failed to set dynamic property {0} with: {1}", propertyName, propertyValueStr);
					}
				}

				if (typeAccessor != null && typeAccessor.GetProperty != null && typeAccessor.SetProperty != null)
				{
					try
					{
						var propertyValue = typeAccessor.GetProperty(propertyValueStr);
						typeAccessor.SetProperty(instance, propertyValue);
					}
					catch
					{
						Tracer.Instance.WriteWarning("WARN: failed to set property {0} with: {1}", propertyName, propertyValueStr);
					}
				}

				Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
			}

			return instance;
		}
		*/
	}

	//The same class above but JSON-specific to enable inlining in this hot class.
}
#endregion Common_DeserializeTypeRef.cs

/// ********   File: \Common\DeserializeTypeRefJson.cs
#region Common_DeserializeTypeRefJson.cs

namespace ServiceStack.Text.Common
{
    // Provides a contract for mapping properties to their type accessors
    internal interface IPropertyNameResolver
    {
        TypeAccessor GetTypeAccessorForProperty(string propertyName, Dictionary<string, TypeAccessor> typeAccessorMap);
    }
    // The default behavior is that the target model must match property names exactly
    internal class DefaultPropertyNameResolver : IPropertyNameResolver
    {
        public virtual TypeAccessor GetTypeAccessorForProperty(string propertyName, Dictionary<string, TypeAccessor> typeAccessorMap)
        {
            TypeAccessor typeAccessor;
            typeAccessorMap.TryGetValue(propertyName, out typeAccessor);
            return typeAccessor;
        }
    }
    // The lenient behavior is that properties on the target model can be .NET-cased, while the source JSON can differ
    internal class LenientPropertyNameResolver : DefaultPropertyNameResolver
    {
        
        public override TypeAccessor GetTypeAccessorForProperty(string propertyName, Dictionary<string, TypeAccessor> typeAccessorMap)
        {
            TypeAccessor typeAccessor;
            
            // camelCase is already supported by default, so no need to add another transform in the tree
            return typeAccessorMap.TryGetValue(TransformFromLowercaseUnderscore(propertyName), out typeAccessor)
                       ? typeAccessor
                       : base.GetTypeAccessorForProperty(propertyName, typeAccessorMap);
        }

        private static string TransformFromLowercaseUnderscore(string propertyName)
        {
            // "lowercase_underscore" -> LowercaseUnderscore
            return propertyName.ToTitleCase();
        }

    }

	internal static class DeserializeTypeRefJson
	{
	    public static readonly IPropertyNameResolver DefaultPropertyNameResolver = new DefaultPropertyNameResolver();
        public static readonly IPropertyNameResolver LenientPropertyNameResolver = new LenientPropertyNameResolver();
        public static IPropertyNameResolver PropertyNameResolver = DefaultPropertyNameResolver;

        private static readonly JsonTypeSerializer Serializer = (JsonTypeSerializer)JsonTypeSerializer.Instance;

		internal static object StringToType(
		Type type,
		string strType,
		EmptyCtorDelegate ctorFn,
		Dictionary<string, TypeAccessor> typeAccessorMap)
		{
			var index = 0;

			if (strType == null)
				return null;

			//if (!Serializer.EatMapStartChar(strType, ref index))
			for (; index < strType.Length; index++) { var c = strType[index]; if (c >= JsonTypeSerializer.WhiteSpaceFlags.Length || !JsonTypeSerializer.WhiteSpaceFlags[c]) break; } //Whitespace inline
			if (strType[index++] != JsWriter.MapStartChar)
				throw DeserializeTypeRef.CreateSerializationError(type, strType);

            if (JsonTypeSerializer.IsEmptyMap(strType)) return ctorFn();

			object instance = null;

			var strTypeLength = strType.Length;
			while (index < strTypeLength)
			{
				var propertyName = JsonTypeSerializer.ParseJsonString(strType, ref index);

				//Serializer.EatMapKeySeperator(strType, ref index);
				for (; index < strType.Length; index++) { var c = strType[index]; if (c >= JsonTypeSerializer.WhiteSpaceFlags.Length || !JsonTypeSerializer.WhiteSpaceFlags[c]) break; } //Whitespace inline
				if (strType.Length != index) index++;

				var propertyValueStr = Serializer.EatValue(strType, ref index);
				var possibleTypeInfo = propertyValueStr != null && propertyValueStr.Length > 1;

				//if we already have an instance don't check type info, because then we will have a half deserialized object
				//we could throw here or just use the existing instance.
				if (instance == null && possibleTypeInfo && propertyName == JsWriter.TypeAttr)
				{
					var explicitTypeName = Serializer.ParseString(propertyValueStr);
                    var explicitType = Type.GetType(explicitTypeName);
                    if (explicitType != null && !explicitType.IsInterface && !explicitType.IsAbstract) {
                        instance = explicitType.CreateInstance();
                    }

					if (instance == null)
					{
						Tracer.Instance.WriteWarning("Could not find type: " + propertyValueStr);
					}
					else
					{
						//If __type info doesn't match, ignore it.
						if (!type.IsInstanceOfType(instance)) {
						    instance = null;
						} else {
						    var derivedType = instance.GetType();
                            if (derivedType != type) {
						        var derivedTypeConfig = new TypeConfig(derivedType);
						        var map = DeserializeTypeRef.GetTypeAccessorMap(derivedTypeConfig, Serializer);
                                if (map != null) {
                                    typeAccessorMap = map;
                                }
                            }
						}
					}

					Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
					continue;
				}

				if (instance == null) instance = ctorFn();

                var typeAccessor = PropertyNameResolver.GetTypeAccessorForProperty(propertyName, typeAccessorMap);
                
				var propType = possibleTypeInfo && propertyValueStr[0] == '_' ? TypeAccessor.ExtractType(Serializer, propertyValueStr) : null;
				if (propType != null)
				{
					try
					{
						if (typeAccessor != null)
						{
							//var parseFn = Serializer.GetParseFn(propType);
							var parseFn = JsonReader.GetParseFn(propType);

							var propertyValue = parseFn(propertyValueStr);
							typeAccessor.SetProperty(instance, propertyValue);
						}

						//Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
						for (; index < strType.Length; index++) { var c = strType[index]; if (c >= JsonTypeSerializer.WhiteSpaceFlags.Length || !JsonTypeSerializer.WhiteSpaceFlags[c]) break; } //Whitespace inline
						if (index != strType.Length)
						{
							var success = strType[index] == JsWriter.ItemSeperator || strType[index] == JsWriter.MapEndChar;
							index++;
							if (success)
								for (; index < strType.Length; index++) { var c = strType[index]; if (c >= JsonTypeSerializer.WhiteSpaceFlags.Length || !JsonTypeSerializer.WhiteSpaceFlags[c]) break; } //Whitespace inline
						}

						continue;
					}
					catch(Exception e)
					{
                        if (JsConfig.ThrowOnDeserializationError) throw DeserializeTypeRef.GetSerializationException(propertyName, propertyValueStr, propType, e);
						else Tracer.Instance.WriteWarning("WARN: failed to set dynamic property {0} with: {1}", propertyName, propertyValueStr);
					}
				}

				if (typeAccessor != null && typeAccessor.GetProperty != null && typeAccessor.SetProperty != null)
				{
					try
					{
						var propertyValue = typeAccessor.GetProperty(propertyValueStr);
						typeAccessor.SetProperty(instance, propertyValue);
					}
					catch(Exception e)
					{
                        if (JsConfig.ThrowOnDeserializationError) throw DeserializeTypeRef.GetSerializationException(propertyName, propertyValueStr, typeAccessor.PropertyType, e);
                        else Tracer.Instance.WriteWarning("WARN: failed to set property {0} with: {1}", propertyName, propertyValueStr);
					}
				}

				//Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
				for (; index < strType.Length; index++) { var c = strType[index]; if (c >= JsonTypeSerializer.WhiteSpaceFlags.Length || !JsonTypeSerializer.WhiteSpaceFlags[c]) break; } //Whitespace inline
				if (index != strType.Length)
				{
					var success = strType[index] == JsWriter.ItemSeperator || strType[index] == JsWriter.MapEndChar;
					index++;
					if (success)
						for (; index < strType.Length; index++) { var c = strType[index]; if (c >= JsonTypeSerializer.WhiteSpaceFlags.Length || !JsonTypeSerializer.WhiteSpaceFlags[c]) break; } //Whitespace inline
				}

			}

			return instance;
		}
	}
}

#endregion Common_DeserializeTypeRefJson.cs

/// ********   File: \Common\DeserializeTypeRefJsv.cs
#region Common_DeserializeTypeRefJsv.cs

namespace ServiceStack.Text.Common
{
	internal static class DeserializeTypeRefJsv
	{
		private static readonly JsvTypeSerializer Serializer = (JsvTypeSerializer)JsvTypeSerializer.Instance;

		internal static object StringToType(
			Type type, 
			string strType, 
			EmptyCtorDelegate ctorFn, 
			Dictionary<string, TypeAccessor> typeAccessorMap)
		{
			var index = 0;

			if (strType == null)
				return null;

			//if (!Serializer.EatMapStartChar(strType, ref index))
			if (strType[index++] != JsWriter.MapStartChar)
				throw DeserializeTypeRef.CreateSerializationError(type, strType);

            if (JsonTypeSerializer.IsEmptyMap(strType)) return ctorFn();

			object instance = null;

			var strTypeLength = strType.Length;
			while (index < strTypeLength)
			{
				var propertyName = Serializer.EatMapKey(strType, ref index);

				//Serializer.EatMapKeySeperator(strType, ref index);
				index++;

				var propertyValueStr = Serializer.EatValue(strType, ref index);
				var possibleTypeInfo = propertyValueStr != null && propertyValueStr.Length > 1;

				if (possibleTypeInfo && propertyName == JsWriter.TypeAttr)
				{
					var explicitTypeName = Serializer.ParseString(propertyValueStr);
                    var explicitType = Type.GetType(explicitTypeName);
                    if (explicitType != null && !explicitType.IsInterface && !explicitType.IsAbstract) {
                        instance = explicitType.CreateInstance();
                    }

                    if (instance == null)
					{
						Tracer.Instance.WriteWarning("Could not find type: " + propertyValueStr);
					}
					else
					{
						//If __type info doesn't match, ignore it.
						if (!type.IsInstanceOfType(instance)) {
							instance = null;
						} else {
						    var derivedType = instance.GetType();
                            if (derivedType != type) {
						        var derivedTypeConfig = new TypeConfig(derivedType);
						        var map = DeserializeTypeRef.GetTypeAccessorMap(derivedTypeConfig, Serializer);
                                if (map != null) {
                                    typeAccessorMap = map;
                                }
                            }
						}
					}

					//Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
					if (index != strType.Length) index++;

					continue;
				}

				if (instance == null) instance = ctorFn();

				TypeAccessor typeAccessor;
				typeAccessorMap.TryGetValue(propertyName, out typeAccessor);

				var propType = possibleTypeInfo && propertyValueStr[0] == '_' ? TypeAccessor.ExtractType(Serializer, propertyValueStr) : null;
				if (propType != null)
				{
					try
					{
						if (typeAccessor != null)
						{
							var parseFn = Serializer.GetParseFn(propType);
							var propertyValue = parseFn(propertyValueStr);
							typeAccessor.SetProperty(instance, propertyValue);
						}

						//Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
						if (index != strType.Length) index++;

						continue;
					}
					catch(Exception e)
					{
                        if (JsConfig.ThrowOnDeserializationError) throw DeserializeTypeRef.GetSerializationException(propertyName, propertyValueStr, propType, e);
						else Tracer.Instance.WriteWarning("WARN: failed to set dynamic property {0} with: {1}", propertyName, propertyValueStr);
					}
				}

				if (typeAccessor != null && typeAccessor.GetProperty != null && typeAccessor.SetProperty != null)
				{
					try
					{
						var propertyValue = typeAccessor.GetProperty(propertyValueStr);
						typeAccessor.SetProperty(instance, propertyValue);
					}
					catch(Exception e)
					{
                        if (JsConfig.ThrowOnDeserializationError) throw DeserializeTypeRef.GetSerializationException(propertyName, propertyValueStr, propType, e);
                        else Tracer.Instance.WriteWarning("WARN: failed to set property {0} with: {1}", propertyName, propertyValueStr);
					}
				}

				//Serializer.EatItemSeperatorOrMapEndChar(strType, ref index);
				if (index != strType.Length) index++;
			}

			return instance;
		}
	}

	//The same class above but JSON-specific to enable inlining in this hot class.
}
#endregion Common_DeserializeTypeRefJsv.cs

/// ********   File: \Common\DeserializeTypeUtils.cs
#region Common_DeserializeTypeUtils.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	public class DeserializeTypeUtils
	{
		public static ParseStringDelegate GetParseMethod(Type type)
		{
			var typeConstructor = GetTypeStringConstructor(type);
			if (typeConstructor != null)
			{
				return value => typeConstructor.Invoke(new object[] { value });
			}

			return null;
		}

		/// <summary>
		/// Get the type(string) constructor if exists
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static ConstructorInfo GetTypeStringConstructor(Type type)
		{
			foreach (var ci in type.GetConstructors())
			{
				var paramInfos = ci.GetParameters();
				var matchFound = (paramInfos.Length == 1 && paramInfos[0].ParameterType == typeof(string));
				if (matchFound)
				{
					return ci;
				}
			}
			return null;
		}

	}
}
#endregion Common_DeserializeTypeUtils.cs

/// ********   File: \Common\ITypeSerializer.cs
#region Common_ITypeSerializer.cs

namespace ServiceStack.Text.Common
{
	internal interface ITypeSerializer
	{
        bool IncludeNullValues { get; }
        string TypeAttrInObject { get; }

		WriteObjectDelegate GetWriteFn<T>();
		WriteObjectDelegate GetWriteFn(Type type);
		ServiceStack.Text.Json.TypeInfo GetTypeInfo(Type type);

		void WriteRawString(TextWriter writer, string value);
		void WritePropertyName(TextWriter writer, string value);

		void WriteBuiltIn(TextWriter writer, object value);
		void WriteObjectString(TextWriter writer, object value);
		void WriteException(TextWriter writer, object value);
		void WriteString(TextWriter writer, string value);
		void WriteDateTime(TextWriter writer, object oDateTime);
		void WriteNullableDateTime(TextWriter writer, object dateTime);
		void WriteDateTimeOffset(TextWriter writer, object oDateTimeOffset);
		void WriteNullableDateTimeOffset(TextWriter writer, object dateTimeOffset);
        void WriteTimeSpan(TextWriter writer, object dateTimeOffset);
        void WriteNullableTimeSpan(TextWriter writer, object dateTimeOffset);
		void WriteGuid(TextWriter writer, object oValue);
		void WriteNullableGuid(TextWriter writer, object oValue);
		void WriteBytes(TextWriter writer, object oByteValue);
		void WriteChar(TextWriter writer, object charValue);
		void WriteByte(TextWriter writer, object byteValue);
		void WriteInt16(TextWriter writer, object intValue);
		void WriteUInt16(TextWriter writer, object intValue);
		void WriteInt32(TextWriter writer, object intValue);
		void WriteUInt32(TextWriter writer, object uintValue);
		void WriteInt64(TextWriter writer, object longValue);
		void WriteUInt64(TextWriter writer, object ulongValue);
		void WriteBool(TextWriter writer, object boolValue);
		void WriteFloat(TextWriter writer, object floatValue);
		void WriteDouble(TextWriter writer, object doubleValue);
        void WriteDecimal(TextWriter writer, object decimalValue);
        void WriteEnum(TextWriter writer, object enumValue);
        void WriteEnumFlags(TextWriter writer, object enumFlagValue);
		void WriteLinqBinary(TextWriter writer, object linqBinaryValue);

		//object EncodeMapKey(object value);

		ParseStringDelegate GetParseFn<T>();
		ParseStringDelegate GetParseFn(Type type);

		string ParseRawString(string value);
        string ParseString(string value);
        string UnescapeString(string value);
        string UnescapeSafeString(string value);
        string EatTypeValue(string value, ref int i);
		bool EatMapStartChar(string value, ref int i);
		string EatMapKey(string value, ref int i);
		bool EatMapKeySeperator(string value, ref int i);
	    void EatWhitespace(string value, ref int i);
		string EatValue(string value, ref int i);
		bool EatItemSeperatorOrMapEndChar(string value, ref int i);
	}
}
#endregion Common_ITypeSerializer.cs

/// ********   File: \Common\JsDelegates.cs
#region Common_JsDelegates.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	internal delegate void WriteListDelegate(TextWriter writer, object oList, WriteObjectDelegate toStringFn);

	internal delegate void WriteGenericListDelegate<T>(TextWriter writer, IList<T> list, WriteObjectDelegate toStringFn);

	internal delegate void WriteDelegate(TextWriter writer, object value);

	internal delegate ParseStringDelegate ParseFactoryDelegate();

	internal delegate void WriteObjectDelegate(TextWriter writer, object obj);

	public delegate void SetPropertyDelegate(object instance, object propertyValue);

	public delegate object ParseStringDelegate(string stringValue);

	public delegate object ConvertObjectDelegate(object fromObject);

    public delegate object ConvertInstanceDelegate(object obj, Type type);
}

#endregion Common_JsDelegates.cs

/// ********   File: \Common\JsReader.cs
#region Common_JsReader.cs

namespace ServiceStack.Text.Common
{
	internal class JsReader<TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		public ParseStringDelegate GetParseFn<T>()
		{
		    var onDeserializedFn = JsConfig<T>.OnDeserializedFn;
            if (onDeserializedFn != null) {
                return value => onDeserializedFn((T)GetCoreParseFn<T>()(value));
            }

		    return GetCoreParseFn<T>();
		}

	    private ParseStringDelegate GetCoreParseFn<T>()
		{
			var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

			if (JsConfig<T>.HasDeserializeFn)
                return value => JsConfig<T>.ParseFn(Serializer, value);

			if (type.IsEnum)
			{
				return x => Enum.Parse(type, x, true);
			}

			if (type == typeof(string))
				return Serializer.UnescapeString;

			if (type == typeof(object))
				return DeserializeType<TSerializer>.ObjectStringToType;

			var specialParseFn = ParseUtils.GetSpecialParseMethod(type);
			if (specialParseFn != null)
				return specialParseFn;

			if (type.IsEnum)
				return x => Enum.Parse(type, x, true);

			if (type.IsArray)
			{
				return DeserializeArray<T, TSerializer>.Parse;
			}

			var builtInMethod = DeserializeBuiltin<T>.Parse;
			if (builtInMethod != null)
				return value => builtInMethod(Serializer.UnescapeSafeString(value));

			if (type.IsGenericType())
			{
				if (type.IsOrHasGenericInterfaceTypeOf(typeof(IList<>)))
					return DeserializeList<T, TSerializer>.Parse;

				if (type.IsOrHasGenericInterfaceTypeOf(typeof(IDictionary<,>)))
					return DeserializeDictionary<TSerializer>.GetParseMethod(type);

				if (type.IsOrHasGenericInterfaceTypeOf(typeof(ICollection<>)))
					return DeserializeCollection<TSerializer>.GetParseMethod(type);

				if (type.HasAnyTypeDefinitionsOf(typeof(Queue<>))
					|| type.HasAnyTypeDefinitionsOf(typeof(Stack<>)))
					return DeserializeSpecializedCollections<T, TSerializer>.Parse;

                if (type.IsOrHasGenericInterfaceTypeOf(typeof(KeyValuePair<,>)))
                    return DeserializeKeyValuePair<TSerializer>.GetParseMethod(type);

				if (type.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>)))
					return DeserializeEnumerable<T, TSerializer>.Parse;
			}

#if NET40
            if (typeof (T).IsAssignableFrom(typeof (System.Dynamic.IDynamicMetaObjectProvider)) ||
	            typeof (T).HasInterface(typeof (System.Dynamic.IDynamicMetaObjectProvider))) 
            {
                return DeserializeDynamic<TSerializer>.Parse;
            }
#endif

            var isDictionary = typeof(T).IsAssignableFrom(typeof(IDictionary))
                || typeof(T).HasInterface(typeof(IDictionary));
            if (isDictionary)
            {
                return DeserializeDictionary<TSerializer>.GetParseMethod(type);
            }

			var isEnumerable = typeof(T).IsAssignableFrom(typeof(IEnumerable))
				|| typeof(T).HasInterface(typeof(IEnumerable));
			if (isEnumerable)
			{
				var parseFn = DeserializeSpecializedCollections<T, TSerializer>.Parse;
				if (parseFn != null) return parseFn;
			}

			if (type.IsValueType) 
			{
				var staticParseMethod = StaticParseMethod<T>.Parse;
				if (staticParseMethod != null)
					return value => staticParseMethod(Serializer.UnescapeSafeString(value));
			}
			else
			{
				var staticParseMethod = StaticParseRefTypeMethod<TSerializer, T>.Parse;
				if (staticParseMethod != null)
					return value => staticParseMethod(Serializer.UnescapeSafeString(value));
			}

			var typeConstructor = DeserializeType<TSerializer>.GetParseMethod(TypeConfig<T>.GetState());
			if (typeConstructor != null)
				return typeConstructor;

			var stringConstructor = DeserializeTypeUtils.GetParseMethod(type);
			if (stringConstructor != null) return stringConstructor;

			return DeserializeType<TSerializer>.ParseAbstractType<T>;
		}
		
	}
}
#endregion Common_JsReader.cs

/// ********   File: \Common\JsState.cs
#region Common_JsState.cs

namespace ServiceStack.Text.Common
{
	internal static class JsState
	{
		//Exposing field for perf
		[ThreadStatic] internal static int WritingKeyCount = 0;

		[ThreadStatic] internal static bool IsWritingValue = false;

		[ThreadStatic] internal static bool IsWritingDynamic = false;
	}
}
#endregion Common_JsState.cs

/// ********   File: \Common\JsWriter.cs
#region Common_JsWriter.cs

namespace ServiceStack.Text.Common
{
    public static class JsWriter
    {
        public const string TypeAttr = "__type";

        public const char MapStartChar = '{';
        public const char MapKeySeperator = ':';
        public const char ItemSeperator = ',';
        public const char MapEndChar = '}';
        public const string MapNullValue = "\"\"";
        public const string EmptyMap = "{}";

        public const char ListStartChar = '[';
        public const char ListEndChar = ']';
        public const char ReturnChar = '\r';
        public const char LineFeedChar = '\n';

        public const char QuoteChar = '"';
        public const string QuoteString = "\"";
        public const string EscapedQuoteString = "\\\"";
        public const string ItemSeperatorString = ",";
        public const string MapKeySeperatorString = ":";

        public static readonly char[] CsvChars = new[] { ItemSeperator, QuoteChar };
        public static readonly char[] EscapeChars = new[] { QuoteChar, MapKeySeperator, ItemSeperator, MapStartChar, MapEndChar, ListStartChar, ListEndChar, ReturnChar, LineFeedChar };

        private const int LengthFromLargestChar = '}' + 1;
        private static readonly bool[] EscapeCharFlags = new bool[LengthFromLargestChar];

        static JsWriter()
        {
            foreach (var escapeChar in EscapeChars)
            {
                EscapeCharFlags[escapeChar] = true;
            }
            var loadConfig = JsConfig.EmitCamelCaseNames; //force load
        }

        public static void WriteDynamic(Action callback)
        {
            JsState.IsWritingDynamic = true;
            try
            {
                callback();
            }
            finally
            {
                JsState.IsWritingDynamic = false;
            }
        }

        /// <summary>
        /// micro optimizations: using flags instead of value.IndexOfAny(EscapeChars)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasAnyEscapeChars(string value)
        {
            var len = value.Length;
            for (var i = 0; i < len; i++)
            {
                var c = value[i];
                if (c >= LengthFromLargestChar || !EscapeCharFlags[c]) continue;
                return true;
            }
            return false;
        }

        internal static void WriteItemSeperatorIfRanOnce(TextWriter writer, ref bool ranOnce)
        {
            if (ranOnce)
                writer.Write(ItemSeperator);
            else
                ranOnce = true;

            foreach (var escapeChar in EscapeChars)
            {
                EscapeCharFlags[escapeChar] = true;
            }
        }

        internal static bool ShouldUseDefaultToStringMethod(Type type)
        {
            return type == typeof(byte) || type == typeof(byte?)
                || type == typeof(short) || type == typeof(short?)
                || type == typeof(ushort) || type == typeof(ushort?)
                || type == typeof(int) || type == typeof(int?)
                || type == typeof(uint) || type == typeof(uint?)
                || type == typeof(long) || type == typeof(long?)
                || type == typeof(ulong) || type == typeof(ulong?)
                || type == typeof(bool) || type == typeof(bool?)
                || type == typeof(DateTime) || type == typeof(DateTime?)
                || type == typeof(Guid) || type == typeof(Guid?)
                || type == typeof(float) || type == typeof(float?)
                || type == typeof(double) || type == typeof(double?)
                || type == typeof(decimal) || type == typeof(decimal?);
        }

        internal static ITypeSerializer GetTypeSerializer<TSerializer>()
        {
            if (typeof(TSerializer) == typeof(JsvTypeSerializer))
                return JsvTypeSerializer.Instance;

            if (typeof(TSerializer) == typeof(JsonTypeSerializer))
                return JsonTypeSerializer.Instance;

            throw new NotSupportedException(typeof(TSerializer).Name);
        }

        public static void WriteEnumFlags(TextWriter writer, object enumFlagValue)
        {
            if (enumFlagValue == null) return;

            var typeCode = Type.GetTypeCode(Enum.GetUnderlyingType(enumFlagValue.GetType()));

            switch (typeCode)
            {
                case TypeCode.Byte:
                    writer.Write((byte)enumFlagValue);
                    break;
                case TypeCode.Int16:
                    writer.Write((short)enumFlagValue);
                    break;
				case TypeCode.UInt16:
                    writer.Write((ushort)enumFlagValue);
                    break;
                case TypeCode.Int32:
                    writer.Write((int)enumFlagValue);
                    break;
                case TypeCode.UInt32:
                    writer.Write((uint)enumFlagValue);
                    break;
                case TypeCode.Int64:
                    writer.Write((long)enumFlagValue);
                    break;
                case TypeCode.UInt64:
                    writer.Write((ulong)enumFlagValue);
                    break;
                default:
                    writer.Write((int)enumFlagValue);
                    break;
            }
        }
	}

    internal class JsWriter<TSerializer>
        where TSerializer : ITypeSerializer
    {
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        public JsWriter()
        {
            this.SpecialTypes = new Dictionary<Type, WriteObjectDelegate>
        	{
        		{ typeof(Uri), Serializer.WriteObjectString },
        		{ typeof(Type), WriteType },
        		{ typeof(Exception), Serializer.WriteException },
#if !MONOTOUCH && !SILVERLIGHT && !XBOX  && !ANDROID
                { typeof(System.Data.Linq.Binary), Serializer.WriteLinqBinary },
#endif
        	};
        }

        public WriteObjectDelegate GetValueTypeToStringMethod(Type type)
        {
			if (type == typeof(char) || type == typeof(char?))
				return Serializer.WriteChar;
			if (type == typeof(int) || type == typeof(int?))
				return Serializer.WriteInt32;
			if (type == typeof(long) || type == typeof(long?))
				return Serializer.WriteInt64;
			if (type == typeof(ulong) || type == typeof(ulong?))
				return Serializer.WriteUInt64;
			if (type == typeof(uint) || type == typeof(uint?))
				return Serializer.WriteUInt32;

			if (type == typeof(byte) || type == typeof(byte?))
				return Serializer.WriteByte;

			if (type == typeof(short) || type == typeof(short?))
				return Serializer.WriteInt16;
            if (type == typeof(ushort) || type == typeof(ushort?))
				return Serializer.WriteUInt16;

            if (type == typeof(bool) || type == typeof(bool?))
                return Serializer.WriteBool;

            if (type == typeof(DateTime))
                return Serializer.WriteDateTime;

            if (type == typeof(DateTime?))
                return Serializer.WriteNullableDateTime;

			if (type == typeof(DateTimeOffset))
				return Serializer.WriteDateTimeOffset;

			if (type == typeof(DateTimeOffset?))
				return Serializer.WriteNullableDateTimeOffset;

            if (type == typeof(TimeSpan))
                return Serializer.WriteTimeSpan;

            if (type == typeof(TimeSpan?))
                return Serializer.WriteNullableTimeSpan;

            if (type == typeof(Guid))
                return Serializer.WriteGuid;

            if (type == typeof(Guid?))
                return Serializer.WriteNullableGuid;

            if (type == typeof(float) || type == typeof(float?))
                return Serializer.WriteFloat;

            if (type == typeof(double) || type == typeof(double?))
                return Serializer.WriteDouble;

            if (type == typeof(decimal) || type == typeof(decimal?))
                return Serializer.WriteDecimal;

            if (type.IsEnum || type.UnderlyingSystemType.IsEnum)
                return type.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0 
                    ? (WriteObjectDelegate)Serializer.WriteEnumFlags
                    : Serializer.WriteEnum;

            return Serializer.WriteObjectString;
        }

        internal WriteObjectDelegate GetWriteFn<T>()
        {
            if (typeof (T) == typeof (string)) {
                return Serializer.WriteObjectString;
            }

		    var onSerializingFn = JsConfig<T>.OnSerializingFn;
            if (onSerializingFn != null) {
                return (w, x) => GetCoreWriteFn<T>()(w, onSerializingFn((T)x));
            }

            return GetCoreWriteFn<T>();
        }

        private WriteObjectDelegate GetCoreWriteFn<T>()
        {
            if ((typeof(T).IsValueType && !JsConfig.TreatAsRefType(typeof(T))) ||
                JsConfig<T>.HasSerializeFn)
            {
                return JsConfig<T>.HasSerializeFn
                    ? JsConfig<T>.WriteFn<TSerializer>
                    : GetValueTypeToStringMethod(typeof(T));
            }

            var specialWriteFn = GetSpecialWriteFn(typeof(T));
            if (specialWriteFn != null)
            {
                return specialWriteFn;
            }

            if (typeof(T).IsArray)
            {
                if (typeof(T) == typeof(byte[]))
                    return (w, x) => WriteLists.WriteBytes(Serializer, w, x);

                if (typeof(T) == typeof(string[]))
                    return (w, x) => WriteLists.WriteStringArray(Serializer, w, x);

                if (typeof(T) == typeof(int[]))
                    return WriteListsOfElements<int, TSerializer>.WriteGenericArrayValueType;
                if (typeof(T) == typeof(long[]))
                    return WriteListsOfElements<long, TSerializer>.WriteGenericArrayValueType;

                var elementType = typeof(T).GetElementType();
                var writeFn = WriteListsOfElements<TSerializer>.GetGenericWriteArray(elementType);
                return writeFn;
            }

            if (typeof(T).IsGenericType() ||
                typeof(T).HasInterface(typeof(IDictionary<string, object>))) // is ExpandoObject?
            {
                if (typeof(T).IsOrHasGenericInterfaceTypeOf(typeof(IList<>)))
                    return WriteLists<T, TSerializer>.Write;

                var mapInterface = typeof(T).GetTypeWithGenericTypeDefinitionOf(typeof(IDictionary<,>));
                if (mapInterface != null)
                {
                    var mapTypeArgs = mapInterface.GetGenericArguments();
                    var writeFn = WriteDictionary<TSerializer>.GetWriteGenericDictionary(
                        mapTypeArgs[0], mapTypeArgs[1]);

                    var keyWriteFn = Serializer.GetWriteFn(mapTypeArgs[0]);
                    var valueWriteFn = typeof(T) == typeof(JsonObject)
                        ? JsonObject.WriteValue
                        : Serializer.GetWriteFn(mapTypeArgs[1]);

                    return (w, x) => writeFn(w, x, keyWriteFn, valueWriteFn);
                }

                var enumerableInterface = typeof(T).GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));
                if (enumerableInterface != null)
                {
                    var elementType = enumerableInterface.GetGenericArguments()[0];
                    var writeFn = WriteListsOfElements<TSerializer>.GetGenericWriteEnumerable(elementType);
                    return writeFn;
                }
            }

            var isDictionary = typeof(T).IsAssignableFrom(typeof(IDictionary))
                || typeof(T).HasInterface(typeof(IDictionary));
            if (isDictionary)
            {
                return WriteDictionary<TSerializer>.WriteIDictionary;
            }
            
            var isEnumerable = typeof(T).IsAssignableFrom(typeof(IEnumerable))
                || typeof(T).HasInterface(typeof(IEnumerable));
            if (isEnumerable)
            {
                return WriteListsOfElements<TSerializer>.WriteIEnumerable;
            }

            if (typeof(T).IsClass || typeof(T).IsInterface || JsConfig.TreatAsRefType(typeof(T)))
            {
                var typeToStringMethod = WriteType<T, TSerializer>.Write;
                if (typeToStringMethod != null)
                {
                    return typeToStringMethod;
                }
            }

            return Serializer.WriteBuiltIn;
        }

        public Dictionary<Type, WriteObjectDelegate> SpecialTypes;

        public WriteObjectDelegate GetSpecialWriteFn(Type type)
        {
            WriteObjectDelegate writeFn = null;
            if (SpecialTypes.TryGetValue(type, out writeFn))
                return writeFn;

            if (type.IsInstanceOfType(typeof(Type)))
                return WriteType;

            if (type.IsInstanceOf(typeof(Exception)))
                return Serializer.WriteException;

            return null;
        }

        public void WriteType(TextWriter writer, object value)
        {
            Serializer.WriteRawString(writer, JsConfig.TypeWriter((Type)value));
        }

    }
}
#endregion Common_JsWriter.cs

/// ********   File: \Common\ParseUtils.cs
#region Common_ParseUtils.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	internal static class ParseUtils
	{
		public static object NullValueType(Type type)
		{
			return ReflectionExtensions.GetDefaultValue(type);
		}

		public static object ParseObject(string value)
		{
			return value;
		}

		public static object ParseEnum(Type type, string value)
		{
			return Enum.Parse(type, value, false);
		}

		public static ParseStringDelegate GetSpecialParseMethod(Type type)
		{
			if (type == typeof(Uri))
				return x => new Uri(x.FromCsvField());

			//Warning: typeof(object).IsInstanceOfType(typeof(Type)) == True??
			if (type.IsInstanceOfType(typeof(Type)))
				return ParseType;

			if (type == typeof(Exception))
				return x => new Exception(x);

			if (type.IsInstanceOf(typeof(Exception)))
				return DeserializeTypeUtils.GetParseMethod(type);

			return null;
		}

		public static Type ParseType(string assemblyQualifiedName)
		{
			return Type.GetType(assemblyQualifiedName.FromCsvField());
		}
	}

}
#endregion Common_ParseUtils.cs

/// ********   File: \Common\StaticParseMethod.cs
#region Common_StaticParseMethod.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	internal delegate object ParseDelegate(string value);

	public static class StaticParseMethod<T>
	{
		const string ParseMethod = "Parse";

		private static readonly ParseStringDelegate CacheFn;

		public static ParseStringDelegate Parse
		{
			get { return CacheFn; }
		}

		static StaticParseMethod()
		{
			CacheFn = GetParseFn();
		}

		public static ParseStringDelegate GetParseFn()
		{
			// Get the static Parse(string) method on the type supplied
			var parseMethodInfo = typeof(T).GetMethod(
				ParseMethod, BindingFlags.Public | BindingFlags.Static, null,
				new[] { typeof(string) }, null);

			if (parseMethodInfo == null) return null;

			ParseDelegate parseDelegate;
			try
			{
				parseDelegate = (ParseDelegate)Delegate.CreateDelegate(typeof(ParseDelegate), parseMethodInfo);
			}
			catch (ArgumentException)
			{
				//Try wrapping strongly-typed return with wrapper fn.
				var typedParseDelegate = (Func<string, T>)Delegate.CreateDelegate(typeof(Func<string, T>), parseMethodInfo);
				parseDelegate = x => typedParseDelegate(x);
			}
			if (parseDelegate != null)
				return value => parseDelegate(value.FromCsvField());

			return null;
		}
	}

	internal static class StaticParseRefTypeMethod<TSerializer, T>
		where TSerializer : ITypeSerializer
	{
		static string ParseMethod = typeof(TSerializer) == typeof(JsvTypeSerializer)
			? "ParseJsv"
			: "ParseJson";

		private static readonly ParseStringDelegate CacheFn;

		public static ParseStringDelegate Parse
		{
			get { return CacheFn; }
		}

		static StaticParseRefTypeMethod()
		{			
			CacheFn = GetParseFn();
		}

		public static ParseStringDelegate GetParseFn()
		{
			// Get the static Parse(string) method on the type supplied
			var parseMethodInfo = typeof(T).GetMethod(
				ParseMethod, BindingFlags.Public | BindingFlags.Static, null,
				new[] { typeof(string) }, null);

			if (parseMethodInfo == null) return null;

			ParseDelegate parseDelegate;
			try
			{
				parseDelegate = (ParseDelegate)Delegate.CreateDelegate(typeof(ParseDelegate), parseMethodInfo);
			}
			catch (ArgumentException)
			{
				//Try wrapping strongly-typed return with wrapper fn.
				var typedParseDelegate = (Func<string, T>)Delegate.CreateDelegate(typeof(Func<string, T>), parseMethodInfo);
				parseDelegate = x => typedParseDelegate(x);
			}
			if (parseDelegate != null)
				return value => parseDelegate(value);

			return null;
		}
	}

}
#endregion Common_StaticParseMethod.cs

/// ********   File: \Common\WriteDictionary.cs
#region Common_WriteDictionary.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	internal delegate void WriteMapDelegate(
		TextWriter writer,
		object oMap,
		WriteObjectDelegate writeKeyFn,
		WriteObjectDelegate writeValueFn);

	internal static class WriteDictionary<TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		internal class MapKey
		{
			internal Type KeyType;
			internal Type ValueType;

			public MapKey(Type keyType, Type valueType)
			{
				KeyType = keyType;
				ValueType = valueType;
			}

			public bool Equals(MapKey other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return Equals(other.KeyType, KeyType) && Equals(other.ValueType, ValueType);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof(MapKey)) return false;
				return Equals((MapKey)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((KeyType != null ? KeyType.GetHashCode() : 0) * 397) ^ (ValueType != null ? ValueType.GetHashCode() : 0);
				}
			}
		}

		static Dictionary<MapKey, WriteMapDelegate> CacheFns = new Dictionary<MapKey, WriteMapDelegate>();

		public static Action<TextWriter, object, WriteObjectDelegate, WriteObjectDelegate>
			GetWriteGenericDictionary(Type keyType, Type valueType)
		{
			WriteMapDelegate writeFn;
            var mapKey = new MapKey(keyType, valueType);
            if (CacheFns.TryGetValue(mapKey, out writeFn)) return writeFn.Invoke;

            var genericType = typeof(ToStringDictionaryMethods<,,>).MakeGenericType(keyType, valueType, typeof(TSerializer));
            var mi = genericType.GetMethod("WriteIDictionary", BindingFlags.Static | BindingFlags.Public);
            writeFn = (WriteMapDelegate)Delegate.CreateDelegate(typeof(WriteMapDelegate), mi);

            Dictionary<MapKey, WriteMapDelegate> snapshot, newCache;
            do
            {
                snapshot = CacheFns;
                newCache = new Dictionary<MapKey, WriteMapDelegate>(CacheFns);
                newCache[mapKey] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref CacheFns, newCache, snapshot), snapshot));
            
            return writeFn.Invoke;
		}

		public static void WriteIDictionary(TextWriter writer, object oMap)
		{
			WriteObjectDelegate writeKeyFn = null;
			WriteObjectDelegate writeValueFn = null;

			writer.Write(JsWriter.MapStartChar);
			var encodeMapKey = false;

			var map = (IDictionary)oMap;
			var ranOnce = false;
			foreach (var key in map.Keys)
			{
				var dictionaryValue = map[key];

                var isNull = (dictionaryValue == null);
                if (isNull && !Serializer.IncludeNullValues) continue;

				if (writeKeyFn == null)
				{
					var keyType = key.GetType();
					writeKeyFn = Serializer.GetWriteFn(keyType);
					encodeMapKey = Serializer.GetTypeInfo(keyType).EncodeMapKey;
				}

				if (writeValueFn == null)
					writeValueFn = Serializer.GetWriteFn(dictionaryValue.GetType());

				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

				JsState.WritingKeyCount++;
				JsState.IsWritingValue = false;

				if (encodeMapKey)
				{
					JsState.IsWritingValue = true; //prevent ""null""
					writer.Write(JsWriter.QuoteChar);
					writeKeyFn(writer, key);
					writer.Write(JsWriter.QuoteChar);
				}
				else
				{
					writeKeyFn(writer, key);
				}

				JsState.WritingKeyCount--;

				writer.Write(JsWriter.MapKeySeperator);

                if (isNull)
                {
                    writer.Write(JsonUtils.Null);
                }
                else
                {
                    JsState.IsWritingValue = true;
                    writeValueFn(writer, dictionaryValue);
                    JsState.IsWritingValue = false;
                }
			}

			writer.Write(JsWriter.MapEndChar);
		}
	}

	internal static class ToStringDictionaryMethods<TKey, TValue, TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		public static void WriteIDictionary(
			TextWriter writer,
			object oMap,
			WriteObjectDelegate writeKeyFn,
			WriteObjectDelegate writeValueFn)
		{
			if (writer == null) return; //AOT
			WriteGenericIDictionary(writer, (IDictionary<TKey, TValue>)oMap, writeKeyFn, writeValueFn);
		}

		public static void WriteGenericIDictionary(
			TextWriter writer,
			IDictionary<TKey, TValue> map,
			WriteObjectDelegate writeKeyFn,
			WriteObjectDelegate writeValueFn)
		{
		    if (map == null)
		    {
		        writer.Write(JsonUtils.Null);
                return;
		    }
			writer.Write(JsWriter.MapStartChar);

			var encodeMapKey = Serializer.GetTypeInfo(typeof(TKey)).EncodeMapKey;

			var ranOnce = false;
			foreach (var kvp in map)
			{
                var isNull = (kvp.Value == null);
                if (isNull && !Serializer.IncludeNullValues) continue;

				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

				JsState.WritingKeyCount++;
                JsState.IsWritingValue = false;

				if (encodeMapKey)
				{
					JsState.IsWritingValue = true; //prevent ""null""
					writer.Write(JsWriter.QuoteChar);
					writeKeyFn(writer, kvp.Key);
					writer.Write(JsWriter.QuoteChar);
				}
				else
				{
					writeKeyFn(writer, kvp.Key);
				}
				
				JsState.WritingKeyCount--;

				writer.Write(JsWriter.MapKeySeperator);

                if (isNull)
                {
                    writer.Write(JsonUtils.Null);
                }
                else
                {
                    JsState.IsWritingValue = true;
                    writeValueFn(writer, kvp.Value);
                    JsState.IsWritingValue = false;
                }
			}

			writer.Write(JsWriter.MapEndChar);
		}
	}
}
#endregion Common_WriteDictionary.cs

/// ********   File: \Common\WriteLists.cs
#region Common_WriteLists.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	internal static class WriteListsOfElements<TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		static Dictionary<Type, WriteObjectDelegate> ListCacheFns = new Dictionary<Type, WriteObjectDelegate>();

		public static WriteObjectDelegate GetListWriteFn(Type elementType)
		{
			WriteObjectDelegate writeFn;
            if (ListCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetMethod("WriteList", BindingFlags.Static | BindingFlags.Public);
            writeFn = (WriteObjectDelegate)Delegate.CreateDelegate(typeof(WriteObjectDelegate), mi);

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = ListCacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(ListCacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ListCacheFns, newCache, snapshot), snapshot));
            
            return writeFn;
		}

		static Dictionary<Type, WriteObjectDelegate> IListCacheFns = new Dictionary<Type, WriteObjectDelegate>();

		public static WriteObjectDelegate GetIListWriteFn(Type elementType)
		{
			WriteObjectDelegate writeFn;
            if (IListCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetMethod("WriteIList", BindingFlags.Static | BindingFlags.Public);
            writeFn = (WriteObjectDelegate)Delegate.CreateDelegate(typeof(WriteObjectDelegate), mi);

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = IListCacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(IListCacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref IListCacheFns, newCache, snapshot), snapshot));
            
            return writeFn;
		}

		static Dictionary<Type, WriteObjectDelegate> CacheFns = new Dictionary<Type, WriteObjectDelegate>();

		public static WriteObjectDelegate GetGenericWriteArray(Type elementType)
		{
			WriteObjectDelegate writeFn;
            if (CacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetMethod("WriteArray", BindingFlags.Static | BindingFlags.Public);
            writeFn = (WriteObjectDelegate)Delegate.CreateDelegate(typeof(WriteObjectDelegate), mi);

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = CacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(CacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref CacheFns, newCache, snapshot), snapshot));

			return writeFn;
		}

		static Dictionary<Type, WriteObjectDelegate> EnumerableCacheFns = new Dictionary<Type, WriteObjectDelegate>();

		public static WriteObjectDelegate GetGenericWriteEnumerable(Type elementType)
		{
			WriteObjectDelegate writeFn;
            if (EnumerableCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetMethod("WriteEnumerable", BindingFlags.Static | BindingFlags.Public);
            writeFn = (WriteObjectDelegate)Delegate.CreateDelegate(typeof(WriteObjectDelegate), mi);

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = EnumerableCacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(EnumerableCacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref EnumerableCacheFns, newCache, snapshot), snapshot));

			return writeFn;
		}

		static Dictionary<Type, WriteObjectDelegate> ListValueTypeCacheFns = new Dictionary<Type, WriteObjectDelegate>();

		public static WriteObjectDelegate GetWriteListValueType(Type elementType)
		{
			WriteObjectDelegate writeFn;
            if (ListValueTypeCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetMethod("WriteListValueType", BindingFlags.Static | BindingFlags.Public);
            writeFn = (WriteObjectDelegate)Delegate.CreateDelegate(typeof(WriteObjectDelegate), mi);

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = ListValueTypeCacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(ListValueTypeCacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ListValueTypeCacheFns, newCache, snapshot), snapshot));

			return writeFn;
		}

		static Dictionary<Type, WriteObjectDelegate> IListValueTypeCacheFns = new Dictionary<Type, WriteObjectDelegate>();

		public static WriteObjectDelegate GetWriteIListValueType(Type elementType)
		{
			WriteObjectDelegate writeFn;

            if (IListValueTypeCacheFns.TryGetValue(elementType, out writeFn)) return writeFn;

            var genericType = typeof(WriteListsOfElements<,>).MakeGenericType(elementType, typeof(TSerializer));
            var mi = genericType.GetMethod("WriteIListValueType", BindingFlags.Static | BindingFlags.Public);
            writeFn = (WriteObjectDelegate)Delegate.CreateDelegate(typeof(WriteObjectDelegate), mi);

            Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
            do
            {
                snapshot = IListValueTypeCacheFns;
                newCache = new Dictionary<Type, WriteObjectDelegate>(IListValueTypeCacheFns);
                newCache[elementType] = writeFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref IListValueTypeCacheFns, newCache, snapshot), snapshot));

			return writeFn;
		}

		public static void WriteIEnumerable(TextWriter writer, object oValueCollection)
		{
			WriteObjectDelegate toStringFn = null;

			writer.Write(JsWriter.ListStartChar);

			var valueCollection = (IEnumerable)oValueCollection;
			var ranOnce = false;
			foreach (var valueItem in valueCollection)
			{
				if (toStringFn == null)
					toStringFn = Serializer.GetWriteFn(valueItem.GetType());

				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

				toStringFn(writer, valueItem);
			}

			writer.Write(JsWriter.ListEndChar);
		}
	}

	internal static class WriteListsOfElements<T, TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly WriteObjectDelegate ElementWriteFn;

		static WriteListsOfElements()
		{
			ElementWriteFn = JsWriter.GetTypeSerializer<TSerializer>().GetWriteFn<T>();
		}

		public static void WriteList(TextWriter writer, object oList)
		{
			WriteGenericIList(writer, (IList<T>)oList);
		}

		public static void WriteGenericList(TextWriter writer, List<T> list)
		{
			writer.Write(JsWriter.ListStartChar);

			var ranOnce = false;
			var listLength = list.Count;
			for (var i = 0; i < listLength; i++)
			{
				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
				ElementWriteFn(writer, list[i]);
			}

			writer.Write(JsWriter.ListEndChar);
		}

		public static void WriteListValueType(TextWriter writer, object list)
		{
			WriteGenericListValueType(writer, (List<T>)list);
		}

		public static void WriteGenericListValueType(TextWriter writer, List<T> list)
		{
			if (list == null) return; //AOT

			writer.Write(JsWriter.ListStartChar);

			var ranOnce = false;
			var listLength = list.Count;
			for (var i = 0; i < listLength; i++)
			{
				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
				ElementWriteFn(writer, list[i]);
			}

			writer.Write(JsWriter.ListEndChar);
		}

		public static void WriteIList(TextWriter writer, object oList)
		{
			WriteGenericIList(writer, (IList<T>)oList);
		}

		public static void WriteGenericIList(TextWriter writer, IList<T> list)
		{
			if (list == null) return;
			writer.Write(JsWriter.ListStartChar);

			var ranOnce = false;
			var listLength = list.Count;
			try
			{
				for (var i = 0; i < listLength; i++)
				{
					JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
					ElementWriteFn(writer, list[i]);
				}

			}
			catch (Exception ex)
			{
				Tracer.Instance.WriteError(ex);
				throw;
			}
			writer.Write(JsWriter.ListEndChar);
		}

		public static void WriteIListValueType(TextWriter writer, object list)
		{
			WriteGenericIListValueType(writer, (IList<T>)list);
		}

		public static void WriteGenericIListValueType(TextWriter writer, IList<T> list)
		{
			if (list == null) return; //AOT

			writer.Write(JsWriter.ListStartChar);

			var ranOnce = false;
			var listLength = list.Count;
			for (var i = 0; i < listLength; i++)
			{
				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
				ElementWriteFn(writer, list[i]);
			}

			writer.Write(JsWriter.ListEndChar);
		}

		public static void WriteArray(TextWriter writer, object oArrayValue)
		{
			if (oArrayValue == null) return;
			WriteGenericArray(writer, (Array)oArrayValue);
		}

		public static void WriteGenericArrayValueType (TextWriter writer, object oArray)
		{
			WriteGenericArrayValueType(writer, (T[])oArray);
		}

		public static void WriteGenericArrayValueType(TextWriter writer, T[] array)
		{
			if (array == null) return;
			writer.Write(JsWriter.ListStartChar);

			var ranOnce = false;
			var arrayLength = array.Length;
			for (var i = 0; i < arrayLength; i++)
			{
				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
				ElementWriteFn(writer, array[i]);
			}

			writer.Write(JsWriter.ListEndChar);
		}

        private static void WriteGenericArrayMultiDimension(TextWriter writer, Array array, int rank, int[] indices)
        {
            var ranOnce = false;
            writer.Write(JsWriter.ListStartChar);
            for (int i = 0; i < array.GetLength(rank); i++)
            {
                JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                indices[rank] = i;

                if (rank < (array.Rank - 1))
                    WriteGenericArrayMultiDimension(writer, array, rank + 1, indices);
                else
                    ElementWriteFn(writer, array.GetValue(indices));
            }
            writer.Write(JsWriter.ListEndChar);
        }

        public static void WriteGenericArray(TextWriter writer, Array array)
        {
            WriteGenericArrayMultiDimension(writer, array, 0, new int[array.Rank]);
        }
		public static void WriteEnumerable(TextWriter writer, object oEnumerable)
		{
			WriteGenericEnumerable(writer, (IEnumerable<T>)oEnumerable);
		}

		public static void WriteGenericEnumerable(TextWriter writer, IEnumerable<T> enumerable)
		{
			if (enumerable == null) return;
			writer.Write(JsWriter.ListStartChar);

			var ranOnce = false;
			foreach (var value in enumerable)
			{
				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
				ElementWriteFn(writer, value);
			}

			writer.Write(JsWriter.ListEndChar);
		}

		public static void WriteGenericEnumerableValueType(TextWriter writer, IEnumerable<T> enumerable)
		{
			writer.Write(JsWriter.ListStartChar);

			var ranOnce = false;
			foreach (var value in enumerable)
			{
				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
				ElementWriteFn(writer, value);
			}

			writer.Write(JsWriter.ListEndChar);
		}
	}

	internal static class WriteLists
	{
		public static void WriteListString(ITypeSerializer serializer, TextWriter writer, object list)
		{
			WriteListString(serializer, writer, (List<string>)list);
		}

		public static void WriteListString(ITypeSerializer serializer, TextWriter writer, List<string> list)
		{
			writer.Write(JsWriter.ListStartChar);

			var ranOnce = false;
			list.ForEach(x =>
			{
				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
				serializer.WriteString(writer, x);
			});

			writer.Write(JsWriter.ListEndChar);
		}

		public static void WriteIListString(ITypeSerializer serializer, TextWriter writer, object list)
		{
			WriteIListString(serializer, writer, (IList<string>)list);
		}

		public static void WriteIListString(ITypeSerializer serializer, TextWriter writer, IList<string> list)
		{
			writer.Write(JsWriter.ListStartChar);

			var ranOnce = false;
			var listLength = list.Count;
			for (var i = 0; i < listLength; i++)
			{
				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
				serializer.WriteString(writer, list[i]);
			}

			writer.Write(JsWriter.ListEndChar);
		}

		public static void WriteBytes(ITypeSerializer serializer, TextWriter writer, object byteValue)
		{
			if (byteValue == null) return;
			serializer.WriteBytes(writer, byteValue);
		}

		public static void WriteStringArray(ITypeSerializer serializer, TextWriter writer, object oList)
		{
			writer.Write(JsWriter.ListStartChar);

			var list = (string[])oList;
			var ranOnce = false;
			var listLength = list.Length;
			for (var i = 0; i < listLength; i++)
			{
				JsWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
				serializer.WriteString(writer, list[i]);
			}

			writer.Write(JsWriter.ListEndChar);
		}
	}

	internal static class WriteLists<T, TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly WriteObjectDelegate CacheFn;
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		static WriteLists()
		{
			CacheFn = GetWriteFn();
		}

		public static WriteObjectDelegate Write
		{
			get { return CacheFn; }
		}

		public static WriteObjectDelegate GetWriteFn()
		{
			var type = typeof(T);

			var listInterface = type.GetTypeWithGenericTypeDefinitionOf(typeof(IList<>));
			if (listInterface == null)
				throw new ArgumentException(string.Format("Type {0} is not of type IList<>", type.FullName));

			//optimized access for regularly used types
			if (type == typeof(List<string>))
				return (w, x) => WriteLists.WriteListString(Serializer, w, x);
			if (type == typeof(IList<string>))
				return (w, x) => WriteLists.WriteIListString(Serializer, w, x);

			if (type == typeof(List<int>))
				return WriteListsOfElements<int, TSerializer>.WriteListValueType;
			if (type == typeof(IList<int>))
				return WriteListsOfElements<int, TSerializer>.WriteIListValueType;

			if (type == typeof(List<long>))
				return WriteListsOfElements<long, TSerializer>.WriteListValueType;
			if (type == typeof(IList<long>))
				return WriteListsOfElements<long, TSerializer>.WriteIListValueType;

			var elementType = listInterface.GetGenericArguments()[0];

			var isGenericList = typeof(T).IsGenericType
				&& typeof(T).GetGenericTypeDefinition() == typeof(List<>);

			if (elementType.IsValueType
				&& JsWriter.ShouldUseDefaultToStringMethod(elementType))
			{
				if (isGenericList)
					return WriteListsOfElements<TSerializer>.GetWriteListValueType(elementType);

				return WriteListsOfElements<TSerializer>.GetWriteIListValueType(elementType);
			}

			return isGenericList
				? WriteListsOfElements<TSerializer>.GetListWriteFn(elementType)
				: WriteListsOfElements<TSerializer>.GetIListWriteFn(elementType);
		}

	}
}
#endregion Common_WriteLists.cs

/// ********   File: \Common\WriteType.cs
#region Common_WriteType.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Common
{
	internal static class WriteType<T, TSerializer>
		where TSerializer : ITypeSerializer
	{
		private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

		private static readonly WriteObjectDelegate CacheFn;
		internal static TypePropertyWriter[] PropertyWriters;
		private static readonly WriteObjectDelegate WriteTypeInfo;

		private static bool IsIncluded
		{
			get { return (JsConfig.IncludeTypeInfo || JsConfig<T>.IncludeTypeInfo); }
		}
		private static bool IsExcluded
		{
			get { return (JsConfig.ExcludeTypeInfo || JsConfig<T>.ExcludeTypeInfo); }
		}

		static WriteType()
		{
			CacheFn = Init() ? GetWriteFn() : WriteEmptyType;

			if (IsIncluded)
			{
				WriteTypeInfo = TypeInfoWriter;
			}
			if (typeof(T).IsAbstract)
			{
				WriteTypeInfo = TypeInfoWriter;	
				if(!JsConfig.PreferInterfaces || !typeof(T).IsInterface)
				{
					CacheFn = WriteAbstractProperties;				
				}
			}
		}

		public static void TypeInfoWriter(TextWriter writer, object obj)
		{
			TryWriteTypeInfo(writer, obj);
		}

		private static bool ShouldSkipType() { return IsExcluded && !IsIncluded; }

		private static bool TryWriteSelfType (TextWriter writer) {
			if (ShouldSkipType()) return false;

			Serializer.WriteRawString(writer, JsConfig.TypeAttr);
			writer.Write(JsWriter.MapKeySeperator);
			Serializer.WriteRawString(writer, JsConfig.TypeWriter(typeof(T)));
			return true;
		}

		private static bool TryWriteTypeInfo(TextWriter writer, object obj)
		{
			if (obj == null || ShouldSkipType()) return false;

			Serializer.WriteRawString(writer, JsConfig.TypeAttr);
			writer.Write(JsWriter.MapKeySeperator);
			Serializer.WriteRawString(writer, JsConfig.TypeWriter(obj.GetType()));
			return true;
		}

		public static WriteObjectDelegate Write
		{
			get { return CacheFn; }
		}

		private static WriteObjectDelegate GetWriteFn()
		{
			return WriteProperties;
		}

		private static bool Init()
		{
			if (!typeof(T).IsClass && !typeof(T).IsInterface && !JsConfig.TreatAsRefType(typeof(T))) return false;

			var propertyInfos = TypeConfig<T>.Properties;
            var propertyNamesLength = propertyInfos.Length;
            PropertyWriters = new TypePropertyWriter[propertyNamesLength];

            if (propertyNamesLength == 0 && !JsState.IsWritingDynamic)
			{
				return typeof(T).IsDto();
			}

			// NOTE: very limited support for DataContractSerialization (DCS)
			//	NOT supporting Serializable
			//	support for DCS is intended for (re)Name of properties and Ignore by NOT having a DataMember present
		    var isDataContract = typeof(T).IsDto();
			for (var i = 0; i < propertyNamesLength; i++)
			{
				var propertyInfo = propertyInfos[i];

				string propertyName, propertyNameCLSFriendly, propertyNameLowercaseUnderscore;

				if (isDataContract)
				{
				    var dcsDataMember = propertyInfo.GetDataMember();
					if (dcsDataMember == null) continue;

					propertyName = dcsDataMember.Name ?? propertyInfo.Name;
					propertyNameCLSFriendly = dcsDataMember.Name ?? propertyName.ToCamelCase();
				    propertyNameLowercaseUnderscore = dcsDataMember.Name ?? propertyName.ToLowercaseUnderscore();
				}
				else
				{
					propertyName = propertyInfo.Name;
					propertyNameCLSFriendly = propertyName.ToCamelCase();
                    propertyNameLowercaseUnderscore = propertyName.ToLowercaseUnderscore();
				}

			    var propertyType = propertyInfo.PropertyType;
			    var suppressDefaultValue = propertyType.IsValueType && JsConfig.HasSerializeFn.Contains(propertyType)
			        ? propertyType.GetDefaultValue()
			        : null;

				PropertyWriters[i] = new TypePropertyWriter
				(
					propertyName,
					propertyNameCLSFriendly,
                    propertyNameLowercaseUnderscore,
					propertyInfo.GetValueGetter<T>(),
                    Serializer.GetWriteFn(propertyType),
                    suppressDefaultValue
				);
			}

			return true;
		}

		internal struct TypePropertyWriter
		{
			internal string PropertyName
			{
				get
				{
				    return (JsConfig.EmitCamelCaseNames)
				               ? propertyNameCLSFriendly
				               : (JsConfig.EmitLowercaseUnderscoreNames)
				                     ? propertyNameLowercaseUnderscore
				                     : propertyName;
				}
			}
			internal readonly string propertyName;
			internal readonly string propertyNameCLSFriendly;
            internal readonly string propertyNameLowercaseUnderscore;
			internal readonly Func<T, object> GetterFn;
            internal readonly WriteObjectDelegate WriteFn;
            internal readonly object DefaultValue;

			public TypePropertyWriter(string propertyName, string propertyNameCLSFriendly, string propertyNameLowercaseUnderscore,
				Func<T, object> getterFn, WriteObjectDelegate writeFn, object defaultValue)
			{
				this.propertyName = propertyName;
				this.propertyNameCLSFriendly = propertyNameCLSFriendly;
			    this.propertyNameLowercaseUnderscore = propertyNameLowercaseUnderscore;
				this.GetterFn = getterFn;
				this.WriteFn = writeFn;
			    this.DefaultValue = defaultValue;
			}
		}

		public static void WriteEmptyType(TextWriter writer, object value)
		{
			writer.Write(JsWriter.EmptyMap);
		}

		public static void WriteAbstractProperties(TextWriter writer, object value)
		{
			if (value == null)
			{
				writer.Write(JsWriter.EmptyMap);
				return;
			}
			var valueType = value.GetType();
			if (valueType.IsAbstract)
			{
				WriteProperties(writer, value);
				return;
			}

			var writeFn = Serializer.GetWriteFn(valueType);			
			if (!JsConfig<T>.ExcludeTypeInfo) JsState.IsWritingDynamic = true;
			writeFn(writer, value);
			if (!JsConfig<T>.ExcludeTypeInfo) JsState.IsWritingDynamic = false;
		}
		 
		public static void WriteProperties(TextWriter writer, object value)
		{
			if (typeof(TSerializer) == typeof(JsonTypeSerializer) && JsState.WritingKeyCount > 0)
				writer.Write(JsWriter.QuoteChar);

			writer.Write(JsWriter.MapStartChar);

			var i = 0;
			if (WriteTypeInfo != null || JsState.IsWritingDynamic)
			{
				if (JsConfig.PreferInterfaces && TryWriteSelfType(writer)) i++;
				else if (TryWriteTypeInfo(writer, value)) i++;
			}

			if (PropertyWriters != null)
			{
				var len = PropertyWriters.Length;
				for (int index = 0; index < len; index++)
				{
					var propertyWriter = PropertyWriters[index];
					var propertyValue = value != null 
						? propertyWriter.GetterFn((T)value)
						: null;

					if ((propertyValue == null
					     || (propertyWriter.DefaultValue != null && propertyWriter.DefaultValue.Equals(propertyValue)))
                        && !Serializer.IncludeNullValues) continue;

					if (i++ > 0)
						writer.Write(JsWriter.ItemSeperator);

					Serializer.WritePropertyName(writer, propertyWriter.PropertyName);
					writer.Write(JsWriter.MapKeySeperator);

					if (typeof (TSerializer) == typeof (JsonTypeSerializer)) JsState.IsWritingValue = true;
					if (propertyValue == null)
					{
						writer.Write(JsonUtils.Null);
					}
					else
					{
						propertyWriter.WriteFn(writer, propertyValue);
					}
					if (typeof(TSerializer) == typeof(JsonTypeSerializer)) JsState.IsWritingValue = false;
				}
			}

			writer.Write(JsWriter.MapEndChar);

			if (typeof(TSerializer) == typeof(JsonTypeSerializer) && JsState.WritingKeyCount > 0)
				writer.Write(JsWriter.QuoteChar);
		}

		public static void WriteQueryString(TextWriter writer, object value)
		{
			var i = 0;
			foreach (var propertyWriter in PropertyWriters)
			{
				var propertyValue = propertyWriter.GetterFn((T)value);
				if (propertyValue == null) continue;
				var propertyValueString = propertyValue as string;
				if (propertyValueString != null)
				{
					propertyValue = propertyValueString.UrlEncode();
				}

				if (i++ > 0)
					writer.Write('&');

				Serializer.WritePropertyName(writer, propertyWriter.PropertyName);
				writer.Write('=');
				propertyWriter.WriteFn(writer, propertyValue);
			}
		}
	}
}

#endregion Common_WriteType.cs

/// ********   File: \Controller\CommandProcessor.cs
#region Controller_CommandProcessor.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Controller
{
	public class CommandProcessor 
	{
		private object[] Controllers { get; set; }

		private readonly Dictionary<string, object> contextMap;

		public CommandProcessor(object[] controllers)
		{
			this.Controllers = controllers;

			this.contextMap = new Dictionary<string, object>();
			controllers.ToList().ForEach(x => contextMap[x.GetType().Name] = x);
		}

		public void Invoke(string commandUri)
		{
			var actionParts = commandUri.Split(new[] { "://" }, StringSplitOptions.None);

			var controllerName = actionParts[0];

			var pathInfo = PathInfo.Parse(actionParts[1]);

			object context;
			if (!this.contextMap.TryGetValue(controllerName, out context))
			{
				throw new Exception("UnknownContext: " + controllerName);
			}

			var methodName = pathInfo.ActionName;

			var method = context.GetType().GetMethods().First(
				c => c.Name == methodName && c.GetParameters().Count() == pathInfo.Arguments.Count);

			var methodParamTypes = method.GetParameters().Select(x => x.ParameterType);

			var methodArgs = ConvertValuesToTypes(pathInfo.Arguments, methodParamTypes.ToList());

			try
			{
				method.Invoke(context, methodArgs);
			}
			catch (Exception ex)
			{
				throw new Exception("InvalidCommand", ex);
			}
		}

		private static object[] ConvertValuesToTypes(IList<string> values, IList<Type> types)
		{
			var convertedValues = new object[types.Count];
			for (var i = 0; i < types.Count; i++)
			{
				var propertyValueType = types[i];
				var propertyValueString = values[i];
				var argValue = TypeSerializer.DeserializeFromString(propertyValueString, propertyValueType);
				convertedValues[i] = argValue;
			}
			return convertedValues;
		}
	}
}
#endregion Controller_CommandProcessor.cs

/// ********   File: \Controller\PathInfo.cs
#region Controller_PathInfo.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Controller
{
	/// <summary>
	/// Class to hold  
	/// </summary>
	public class PathInfo
	{
		public string ControllerName { get; private set; }

		public string ActionName { get; private set; }

		public List<string> Arguments { get; private set; }

		public Dictionary<string, string> Options { get; private set; }

		public PathInfo(string actionName, params string[] arguments)
			: this(actionName, arguments.ToList(), null)
		{
		}

		public PathInfo(string actionName, List<string> arguments, Dictionary<string, string> options)
		{
			ActionName = actionName;
			Arguments = arguments ?? new List<string>();
			Options = options ?? new Dictionary<string, string>();
		}

		public string FirstArgument
		{
			get
			{
				return this.Arguments.Count > 0 ? this.Arguments[0] : null;
			}
		}

		public T GetArgumentValue<T>(int index)
		{
			return TypeSerializer.DeserializeFromString<T>(this.Arguments[index]);
		}

		/// <summary>
		/// Parses the specified path info.
		/// e.g.
		///		MusicPage/arg1/0/true?debug&showFlows=3 => PathInfo
		///			.ActionName = 'MusicPage'
		///			.Arguments = ['arg1','0','true']
		///			.Options = { debug:'True', showFlows:'3' }
		/// </summary>
		/// <param name="pathUri">The path url.</param>
		/// <returns></returns>
		public static PathInfo Parse(string pathUri)
		{
			var actionParts = pathUri.Split(new[] { "://" }, StringSplitOptions.None);
			var controllerName = actionParts.Length == 2
									? actionParts[0]
									: null;

			var pathInfo = actionParts[actionParts.Length - 1];

			var optionMap = new Dictionary<string, string>();

			var optionsPos = pathInfo.LastIndexOf('?');
			if (optionsPos != -1)
			{
				var options = pathInfo.Substring(optionsPos + 1).Split('&');
				foreach (var option in options)
				{
					var keyValuePair = option.Split('=');

					optionMap[keyValuePair[0]] = keyValuePair.Length == 1
													? true.ToString()
													: keyValuePair[1].UrlDecode();
				}
				pathInfo = pathInfo.Substring(0, optionsPos);
			}

			var args = pathInfo.Split('/');
			var pageName = args[0];

			return new PathInfo(pageName, args.Skip(1).ToList(), optionMap) {
				ControllerName = controllerName
			};
		}
	}
}
#endregion Controller_PathInfo.cs

/// ********   File: \Json\JsonReader.Generic.cs
#region Json_JsonReader.Generic.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Json
{
	internal static class JsonReader
	{
		public static readonly JsReader<JsonTypeSerializer> Instance = new JsReader<JsonTypeSerializer>();

		private static Dictionary<Type, ParseFactoryDelegate> ParseFnCache = new Dictionary<Type, ParseFactoryDelegate>();
        
		public static ParseStringDelegate GetParseFn(Type type)
		{
			ParseFactoryDelegate parseFactoryFn;
            ParseFnCache.TryGetValue(type, out parseFactoryFn);

            if (parseFactoryFn != null) return parseFactoryFn();

            var genericType = typeof(JsonReader<>).MakeGenericType(type);
            var mi = genericType.GetMethod("GetParseFn", BindingFlags.Public | BindingFlags.Static);
            parseFactoryFn = (ParseFactoryDelegate)Delegate.CreateDelegate(typeof(ParseFactoryDelegate), mi);

            Dictionary<Type, ParseFactoryDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseFnCache;
                newCache = new Dictionary<Type, ParseFactoryDelegate>(ParseFnCache);
                newCache[type] = parseFactoryFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseFnCache, newCache, snapshot), snapshot));
            
            return parseFactoryFn();
		}
	}

	public static class JsonReader<T>
	{
		private static readonly ParseStringDelegate ReadFn;

		static JsonReader()
		{
			ReadFn = JsonReader.Instance.GetParseFn<T>();
		}
		
		public static ParseStringDelegate GetParseFn()
		{
			return ReadFn ?? Parse;
		}

		public static object Parse(string value)
		{
			if (ReadFn == null)
			{
                if (typeof(T).IsAbstract || typeof(T).IsInterface)
				{
					if (string.IsNullOrEmpty(value)) return null;
					var concreteType = DeserializeType<JsonTypeSerializer>.ExtractType(value);
					if (concreteType != null)
					{
						return JsonReader.GetParseFn(concreteType)(value);
					}
					throw new NotSupportedException("Can not deserialize interface type: "
						+ typeof(T).Name);
				}
			}
			return value == null 
			       	? null 
			       	: ReadFn(value);
		}
	}
}
#endregion Json_JsonReader.Generic.cs

/// ********   File: \Json\JsonTypeSerializer.cs
#region Json_JsonTypeSerializer.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Json
{
    internal class JsonTypeSerializer
        : ITypeSerializer
    {
        public static ITypeSerializer Instance = new JsonTypeSerializer();

        public bool IncludeNullValues
        {
            get { return JsConfig.IncludeNullValues; }
        }

        public string TypeAttrInObject
        {
            get { return JsConfig.JsonTypeAttrInObject; }
        }

        internal static string GetTypeAttrInObject(string typeAttr)
        {
            return string.Format("{{\"{0}\":", typeAttr);
        }

        public static readonly bool[] WhiteSpaceFlags = new bool[' ' + 1];

        static JsonTypeSerializer()
        {
            WhiteSpaceFlags[' '] = true;
            WhiteSpaceFlags['\t'] = true;
            WhiteSpaceFlags['\r'] = true;
            WhiteSpaceFlags['\n'] = true;
        }

        public WriteObjectDelegate GetWriteFn<T>()
        {
            return JsonWriter<T>.WriteFn();
		}

        public WriteObjectDelegate GetWriteFn(Type type)
        {
            return JsonWriter.GetWriteFn(type);
        }

        public TypeInfo GetTypeInfo(Type type)
        {
            return JsonWriter.GetTypeInfo(type);
        }

        /// <summary>
        /// Shortcut escape when we're sure value doesn't contain any escaped chars
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public void WriteRawString(TextWriter writer, string value)
        {
            writer.Write(JsWriter.QuoteChar);
            writer.Write(value);
            writer.Write(JsWriter.QuoteChar);
        }

        public void WritePropertyName(TextWriter writer, string value)
        {
            if (JsState.WritingKeyCount > 0)
            {
                writer.Write(JsWriter.EscapedQuoteString);
                writer.Write(value);
                writer.Write(JsWriter.EscapedQuoteString);
            }
            else
            {
                WriteRawString(writer, value);
            }
        }

        public void WriteString(TextWriter writer, string value)
        {
            JsonUtils.WriteString(writer, value);
        }

        public void WriteBuiltIn(TextWriter writer, object value)
        {
            if (JsState.WritingKeyCount > 0 && !JsState.IsWritingValue) writer.Write(JsonUtils.QuoteChar);

            WriteRawString(writer, value.ToString());

            if (JsState.WritingKeyCount > 0 && !JsState.IsWritingValue) writer.Write(JsonUtils.QuoteChar);
        }

        public void WriteObjectString(TextWriter writer, object value)
        {
            JsonUtils.WriteString(writer, value != null ? value.ToString() : null);
        }

        public void WriteException(TextWriter writer, object value)
        {
            WriteString(writer, ((Exception)value).Message);
        }

        public void WriteDateTime(TextWriter writer, object oDateTime)
        {
            WriteRawString(writer, DateTimeSerializer.ToWcfJsonDate((DateTime)oDateTime));
        }

        public void WriteNullableDateTime(TextWriter writer, object dateTime)
        {
            if (dateTime == null)
                writer.Write(JsonUtils.Null);
            else
                WriteDateTime(writer, dateTime);
        }

        public void WriteDateTimeOffset(TextWriter writer, object oDateTimeOffset)
        {
            WriteRawString(writer, DateTimeSerializer.ToWcfJsonDateTimeOffset((DateTimeOffset)oDateTimeOffset));
        }

        public void WriteNullableDateTimeOffset(TextWriter writer, object dateTimeOffset)
        {
            if (dateTimeOffset == null)
                writer.Write(JsonUtils.Null);
            else
                WriteDateTimeOffset(writer, dateTimeOffset);
        }

        public void WriteTimeSpan(TextWriter writer, object oTimeSpan)
        {
            var stringValue = JsConfig.TimeSpanHandler == JsonTimeSpanHandler.StandardFormat
                ? oTimeSpan.ToString()
                : DateTimeSerializer.ToXsdTimeSpanString((TimeSpan)oTimeSpan);
            WriteRawString(writer, stringValue);
        }

        public void WriteNullableTimeSpan(TextWriter writer, object oTimeSpan)
        {

            if (oTimeSpan == null) return;
            WriteTimeSpan(writer, ((TimeSpan?)oTimeSpan).Value);
        }

        public void WriteGuid(TextWriter writer, object oValue)
        {
            WriteRawString(writer, ((Guid)oValue).ToString("N"));
        }

        public void WriteNullableGuid(TextWriter writer, object oValue)
        {
            if (oValue == null) return;
            WriteRawString(writer, ((Guid)oValue).ToString("N"));
        }

        public void WriteBytes(TextWriter writer, object oByteValue)
        {
            if (oByteValue == null) return;
            WriteRawString(writer, Convert.ToBase64String((byte[])oByteValue));
        }

        public void WriteChar(TextWriter writer, object charValue)
        {
            if (charValue == null)
                writer.Write(JsonUtils.Null);
            else
                WriteRawString(writer, ((char)charValue).ToString(CultureInfo.InvariantCulture));
        }

        public void WriteByte(TextWriter writer, object byteValue)
        {
            if (byteValue == null)
                writer.Write(JsonUtils.Null);
            else
                writer.Write((byte)byteValue);
        }

        public void WriteInt16(TextWriter writer, object intValue)
        {
            if (intValue == null)
                writer.Write(JsonUtils.Null);
            else
                writer.Write((short)intValue);
        }

        public void WriteUInt16(TextWriter writer, object intValue)
        {
            if (intValue == null)
                writer.Write(JsonUtils.Null);
            else
                writer.Write((ushort)intValue);
        }

        public void WriteInt32(TextWriter writer, object intValue)
        {
            if (intValue == null)
                writer.Write(JsonUtils.Null);
            else
                writer.Write((int)intValue);
        }

        public void WriteUInt32(TextWriter writer, object uintValue)
        {
            if (uintValue == null)
                writer.Write(JsonUtils.Null);
            else
                writer.Write((uint)uintValue);
        }

        public void WriteInt64(TextWriter writer, object integerValue)
        {
            if (integerValue == null)
                writer.Write(JsonUtils.Null);
            else
                writer.Write((long)integerValue);
        }

        public void WriteUInt64(TextWriter writer, object ulongValue)
        {
            if (ulongValue == null)
            {
                writer.Write(JsonUtils.Null);
            }
            else
                writer.Write((ulong)ulongValue);
        }

        public void WriteBool(TextWriter writer, object boolValue)
        {
            if (boolValue == null)
                writer.Write(JsonUtils.Null);
            else
                writer.Write(((bool)boolValue) ? JsonUtils.True : JsonUtils.False);
        }

        public void WriteFloat(TextWriter writer, object floatValue)
        {
            if (floatValue == null)
                writer.Write(JsonUtils.Null);
            else
            {
                var floatVal = (float)floatValue;
                if (Equals(floatVal, float.MaxValue) || Equals(floatVal, float.MinValue))
                    writer.Write(floatVal.ToString("r", CultureInfo.InvariantCulture));
                else
                    writer.Write(floatVal.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void WriteDouble(TextWriter writer, object doubleValue)
        {
            if (doubleValue == null)
                writer.Write(JsonUtils.Null);
            else
            {
                var doubleVal = (double)doubleValue;
                if (Equals(doubleVal, double.MaxValue) || Equals(doubleVal, double.MinValue))
                    writer.Write(doubleVal.ToString("r", CultureInfo.InvariantCulture));
                else
                    writer.Write(doubleVal.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void WriteDecimal(TextWriter writer, object decimalValue)
        {
            if (decimalValue == null)
                writer.Write(JsonUtils.Null);
            else
                writer.Write(((decimal)decimalValue).ToString(CultureInfo.InvariantCulture));
        }

        public void WriteEnum(TextWriter writer, object enumValue)
        {
            if (enumValue == null) return;
			if (JsConfig.TreatEnumAsInteger)
				JsWriter.WriteEnumFlags(writer, enumValue);
			else
				WriteRawString(writer, enumValue.ToString());
        }

        public void WriteEnumFlags(TextWriter writer, object enumFlagValue)
        {
			JsWriter.WriteEnumFlags(writer, enumFlagValue);
        }

        public void WriteLinqBinary(TextWriter writer, object linqBinaryValue)
        {
#if !MONOTOUCH && !SILVERLIGHT && !XBOX  && !ANDROID
            WriteRawString(writer, Convert.ToBase64String(((System.Data.Linq.Binary)linqBinaryValue).ToArray()));
#endif
        }

        public ParseStringDelegate GetParseFn<T>()
        {
            return JsonReader.Instance.GetParseFn<T>();
        }

        public ParseStringDelegate GetParseFn(Type type)
        {
            return JsonReader.GetParseFn(type);
        }

        public string ParseRawString(string value)
        {
            return value;
        }

        public string ParseString(string value)
        {
            return string.IsNullOrEmpty(value) ? value : ParseRawString(value);
        }

        internal static bool IsEmptyMap(string value)
        {
            var i = 1;
            for (; i < value.Length; i++) { var c = value[i]; if (c >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[c]) break; } //Whitespace inline
            if (value.Length == i) return true;
            return value[i++] == JsWriter.MapEndChar;
        }

        internal static string ParseString(string json, ref int index)
        {
            var jsonLength = json.Length;
            if (json[index] != JsonUtils.QuoteChar)
                throw new Exception("Invalid unquoted string starting with: " + json.SafeSubstring(50));

        	var startIndex = ++index;
            do
            {
                char c = json[index];
                if (c == JsonUtils.QuoteChar) break;
                if (c != JsonUtils.EscapeChar) continue;
                c = json[index++];
                if (c == 'u')
                {
                    index += 4;
                }
            } while (index++ < jsonLength);
            index++;
            return json.Substring(startIndex, Math.Min(index, jsonLength) - startIndex - 1);
        }

        public string UnescapeString(string value)
        {
            var i = 0;
            return UnEscapeJsonString(value, ref i);
        }

        public string UnescapeSafeString(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value[0] == JsonUtils.QuoteChar && value[value.Length - 1] == JsonUtils.QuoteChar
                ? value.Substring(1, value.Length - 2)
                : value;

            //if (value[0] != JsonUtils.QuoteChar)
            //    throw new Exception("Invalid unquoted string starting with: " + value.SafeSubstring(50));

            //return value.Substring(1, value.Length - 2);
        }

        static readonly char[] IsSafeJsonChars = new[] { JsonUtils.QuoteChar, JsonUtils.EscapeChar };

        internal static string ParseJsonString(string json, ref int index)
        {
            for (; index < json.Length; index++) { var ch = json[index]; if (ch >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[ch]) break; } //Whitespace inline

            return UnEscapeJsonString(json, ref index);
        }

        private static string UnEscapeJsonString(string json, ref int index)
        {
            if (string.IsNullOrEmpty(json)) return json;
            var jsonLength = json.Length;
            var firstChar = json[index];
            if (firstChar == JsonUtils.QuoteChar)
            {
                index++;

                //MicroOp: See if we can short-circuit evaluation (to avoid StringBuilder)
                var strEndPos = json.IndexOfAny(IsSafeJsonChars, index);
                if (strEndPos == -1) return json.Substring(index, jsonLength - index);
                if (json[strEndPos] == JsonUtils.QuoteChar)
                {
                    var potentialValue = json.Substring(index, strEndPos - index);
                    index = strEndPos + 1;
                    return potentialValue;
                }
            }

            var sb = new StringBuilder(jsonLength);

        	while (true)
            {
                if (index == jsonLength) break;

                char c = json[index++];
                if (c == JsonUtils.QuoteChar) break;

                if (c == JsonUtils.EscapeChar)
                {
                    if (index == jsonLength)
                    {
                        break;
                    }
                    c = json[index++];
                    switch (c)
                    {
                        case '"':
                            sb.Append('"');
                            break;
                        case '\\':
                            sb.Append('\\');
                            break;
                        case '/':
                            sb.Append('/');
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'u':
                            var remainingLength = jsonLength - index;
                            if (remainingLength >= 4)
                            {
                                var unicodeString = json.Substring(index, 4);
                                var unicodeIntVal = UInt32.Parse(unicodeString, NumberStyles.HexNumber);
                                sb.Append(ConvertFromUtf32((int) unicodeIntVal));
                                index += 4;
                            }
                            else
                            {
                                break;
                            }
                            break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Since Silverlight doesn't have char.ConvertFromUtf32() so putting Mono's implemenation inline.
        /// </summary>
        /// <param name="utf32"></param>
        /// <returns></returns>
        private static string ConvertFromUtf32(int utf32)
        {
            if (utf32 < 0 || utf32 > 0x10FFFF)
                throw new ArgumentOutOfRangeException("utf32", "The argument must be from 0 to 0x10FFFF.");
            if (0xD800 <= utf32 && utf32 <= 0xDFFF)
                throw new ArgumentOutOfRangeException("utf32", "The argument must not be in surrogate pair range.");
            if (utf32 < 0x10000)
                return new string((char)utf32, 1);
            utf32 -= 0x10000;
            return new string(new[] {(char) ((utf32 >> 10) + 0xD800),
                                (char) (utf32 % 0x0400 + 0xDC00)});
        }

    	public string EatTypeValue(string value, ref int i)
        {
            return EatValue(value, ref i);
        }

        public bool EatMapStartChar(string value, ref int i)
        {
            for (; i < value.Length; i++) { var c = value[i]; if (c >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[c]) break; } //Whitespace inline
            return value[i++] == JsWriter.MapStartChar;
        }

        public string EatMapKey(string value, ref int i)
        {
            return ParseJsonString(value, ref i);
        }

        public bool EatMapKeySeperator(string value, ref int i)
        {
            for (; i < value.Length; i++) { var c = value[i]; if (c >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[c]) break; } //Whitespace inline
            if (value.Length == i) return false;
            return value[i++] == JsWriter.MapKeySeperator;
        }

        public bool EatItemSeperatorOrMapEndChar(string value, ref int i)
        {
            for (; i < value.Length; i++) { var c = value[i]; if (c >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[c]) break; } //Whitespace inline

            if (i == value.Length) return false;

            var success = value[i] == JsWriter.ItemSeperator
                || value[i] == JsWriter.MapEndChar;

            i++;

            if (success)
            {
                for (; i < value.Length; i++) { var c = value[i]; if (c >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[c]) break; } //Whitespace inline
            }

            return success;
        }

        public void EatWhitespace(string value, ref int i)
        {
            for (; i < value.Length; i++) { var c = value[i]; if (c >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[c]) break; } //Whitespace inline
        }

        public string EatValue(string value, ref int i)
        {
            var valueLength = value.Length;
            if (i == valueLength) return null;

            for (; i < value.Length; i++) { var c = value[i]; if (c >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[c]) break; } //Whitespace inline
            if (i == valueLength) return null;

            var tokenStartPos = i;
            var valueChar = value[i];
            var withinQuotes = false;
            var endsToEat = 1;

            switch (valueChar)
            {
                //If we are at the end, return.
                case JsWriter.ItemSeperator:
                case JsWriter.MapEndChar:
                    return null;

                //Is Within Quotes, i.e. "..."
                case JsWriter.QuoteChar:
                    return ParseString(value, ref i);

                //Is Type/Map, i.e. {...}
                case JsWriter.MapStartChar:
                    while (++i < valueLength && endsToEat > 0)
                    {
                        valueChar = value[i];

                        if (valueChar == JsonUtils.EscapeChar)
                        {
                            i++;
                            continue;
                        }

                        if (valueChar == JsWriter.QuoteChar)
                            withinQuotes = !withinQuotes;

                        if (withinQuotes)
                            continue;

                        if (valueChar == JsWriter.MapStartChar)
                            endsToEat++;

                        if (valueChar == JsWriter.MapEndChar)
                            endsToEat--;
                    }
                    return value.Substring(tokenStartPos, i - tokenStartPos);

                //Is List, i.e. [...]
                case JsWriter.ListStartChar:
                    while (++i < valueLength && endsToEat > 0)
                    {
                        valueChar = value[i];

                        if (valueChar == JsonUtils.EscapeChar)
                        {
                            i++;
                            continue;
                        }

                        if (valueChar == JsWriter.QuoteChar)
                            withinQuotes = !withinQuotes;

                        if (withinQuotes)
                            continue;

                        if (valueChar == JsWriter.ListStartChar)
                            endsToEat++;

                        if (valueChar == JsWriter.ListEndChar)
                            endsToEat--;
                    }
                    return value.Substring(tokenStartPos, i - tokenStartPos);
            }

            //Is Value
            while (++i < valueLength)
            {
                valueChar = value[i];

                if (valueChar == JsWriter.ItemSeperator
                    || valueChar == JsWriter.MapEndChar
                    //If it doesn't have quotes it's either a keyword or number so also has a ws boundary
                    || (valueChar < WhiteSpaceFlags.Length && WhiteSpaceFlags[valueChar])
                )
                {
                    break;
                }
            }

            var strValue = value.Substring(tokenStartPos, i - tokenStartPos);
            return strValue == JsonUtils.Null ? null : strValue;
        }
    }

}
#endregion Json_JsonTypeSerializer.cs

/// ********   File: \Json\JsonUtils.cs
#region Json_JsonUtils.cs

namespace ServiceStack.Text.Json
{
	public static class JsonUtils
	{
		public const char EscapeChar = '\\';
		public const char QuoteChar = '"';
		public const string Null = "null";
		public const string True = "true";
		public const string False = "false";

		static readonly char[] EscapeChars = new[]
			{
				QuoteChar, '\n', '\r', '\t', '"', '\\', '\f', '\b',
			};

		private const int LengthFromLargestChar = '\\' + 1;
		private static readonly bool[] EscapeCharFlags = new bool[LengthFromLargestChar];

		static JsonUtils()
		{
			foreach (var escapeChar in EscapeChars)
			{
				EscapeCharFlags[escapeChar] = true;
			}
		}

		public static void WriteString(TextWriter writer, string value)
		{
			if (value == null)
			{
				writer.Write(JsonUtils.Null);
				return;
			}
			if (!HasAnyEscapeChars(value))
			{
				writer.Write(QuoteChar);
				writer.Write(value);
				writer.Write(QuoteChar);
				return;
			}

			var hexSeqBuffer = new char[4];
			writer.Write(QuoteChar);

			var len = value.Length;
			for (var i = 0; i < len; i++)
			{
				switch (value[i])
				{
					case '\n':
						writer.Write("\\n");
						continue;

					case '\r':
						writer.Write("\\r");
						continue;

					case '\t':
						writer.Write("\\t");
						continue;

					case '"':
					case '\\':
						writer.Write('\\');
						writer.Write(value[i]);
						continue;

					case '\f':
						writer.Write("\\f");
						continue;

					case '\b':
						writer.Write("\\b");
						continue;
				}

				//Is printable char?
				if (value[i] >= 32 && value[i] <= 126)
				{
					writer.Write(value[i]);
					continue;
				}

				var isValidSequence = value[i] < 0xD800 || value[i] > 0xDFFF;
				if (isValidSequence)
				{
					// Default, turn into a \uXXXX sequence
					IntToHex(value[i], hexSeqBuffer);
					writer.Write("\\u");
					writer.Write(hexSeqBuffer);
				}
			}

			writer.Write(QuoteChar);
		}

		/// <summary>
		/// micro optimizations: using flags instead of value.IndexOfAny(EscapeChars)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool HasAnyEscapeChars(string value)
		{
			var len = value.Length;
			for (var i = 0; i < len; i++)
			{
				var c = value[i];
				if (c >= LengthFromLargestChar || !EscapeCharFlags[c]) continue;
				return true;
			}
			return false;
		}

		public static void IntToHex(int intValue, char[] hex)
		{
			for (var i = 0; i < 4; i++)
			{
				var num = intValue % 16;

				if (num < 10)
					hex[3 - i] = (char)('0' + num);
				else
					hex[3 - i] = (char)('A' + (num - 10));

				intValue >>= 4;
			}
		}

		public static bool IsJsObject(string value)
		{
			return !string.IsNullOrEmpty(value)
				&& value[0] == '{'
				&& value[value.Length - 1] == '}';
		}

		public static bool IsJsArray(string value)
		{
			return !string.IsNullOrEmpty(value)
				&& value[0] == '['
				&& value[value.Length - 1] == ']';
		}
	}

}
#endregion Json_JsonUtils.cs

/// ********   File: \Json\JsonWriter.Generic.cs
#region Json_JsonWriter.Generic.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Json
{
	internal static class JsonWriter
	{
		public static readonly JsWriter<JsonTypeSerializer> Instance = new JsWriter<JsonTypeSerializer>();

		private static Dictionary<Type, WriteObjectDelegate> WriteFnCache = new Dictionary<Type, WriteObjectDelegate>();

		public static WriteObjectDelegate GetWriteFn(Type type)
		{
			try
			{
				WriteObjectDelegate writeFn;
				if (WriteFnCache.TryGetValue(type, out writeFn)) return writeFn;

				var genericType = typeof(JsonWriter<>).MakeGenericType(type);
				var mi = genericType.GetMethod("WriteFn", BindingFlags.Public | BindingFlags.Static);
				var writeFactoryFn = (Func<WriteObjectDelegate>)Delegate.CreateDelegate(typeof(Func<WriteObjectDelegate>), mi);
				writeFn = writeFactoryFn();

				Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
				do
				{
					snapshot = WriteFnCache;
					newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache);
					newCache[type] = writeFn;

				} while (!ReferenceEquals(
					Interlocked.CompareExchange(ref WriteFnCache, newCache, snapshot), snapshot));

				return writeFn;
			}
			catch (Exception ex)
			{
				Tracer.Instance.WriteError(ex);
				throw;
			}
		}

		private static Dictionary<Type, TypeInfo> JsonTypeInfoCache = new Dictionary<Type, TypeInfo>();

		public static TypeInfo GetTypeInfo(Type type)
		{
			try
			{
				TypeInfo writeFn;
				if (JsonTypeInfoCache.TryGetValue(type, out writeFn)) return writeFn;

				var genericType = typeof(JsonWriter<>).MakeGenericType(type);
				var mi = genericType.GetMethod("GetTypeInfo", BindingFlags.Public | BindingFlags.Static);
				var writeFactoryFn = (Func<TypeInfo>)Delegate.CreateDelegate(typeof(Func<TypeInfo>), mi);
				writeFn = writeFactoryFn();

				Dictionary<Type, TypeInfo> snapshot, newCache;
				do
				{
					snapshot = JsonTypeInfoCache;
					newCache = new Dictionary<Type, TypeInfo>(JsonTypeInfoCache);
					newCache[type] = writeFn;

				} while (!ReferenceEquals(
					Interlocked.CompareExchange(ref JsonTypeInfoCache, newCache, snapshot), snapshot));

				return writeFn;
			}
			catch (Exception ex)
			{
				Tracer.Instance.WriteError(ex);
				throw;
			}
		}

		public static void WriteLateBoundObject(TextWriter writer, object value)
		{
			if (value == null)
			{
				writer.Write(JsonUtils.Null);
				return;
			}

			var type = value.GetType();
			var writeFn = type == typeof(object)
				? WriteType<object, JsonTypeSerializer>.WriteEmptyType
				: GetWriteFn(type);

			var prevState = JsState.IsWritingDynamic;
			JsState.IsWritingDynamic = true;
			writeFn(writer, value);
			JsState.IsWritingDynamic = prevState;
		}

		public static WriteObjectDelegate GetValueTypeToStringMethod(Type type)
		{
			return Instance.GetValueTypeToStringMethod(type);
		}
	}

	internal class TypeInfo
	{
		internal bool EncodeMapKey;
	}

	/// <summary>
	/// Implement the serializer using a more static approach
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal static class JsonWriter<T>
	{
		internal static TypeInfo TypeInfo;
		private static readonly WriteObjectDelegate CacheFn;

		public static WriteObjectDelegate WriteFn()
		{
			return CacheFn ?? WriteObject;
		}

		public static TypeInfo GetTypeInfo()
		{
			return TypeInfo;
		}

		static JsonWriter()
		{
			TypeInfo = new TypeInfo {
				EncodeMapKey = typeof(T) == typeof(bool) || typeof(T).IsNumericType()
			};

            CacheFn = typeof(T) == typeof(object) 
                ? JsonWriter.WriteLateBoundObject 
                : JsonWriter.Instance.GetWriteFn<T>();
		}

	    public static void WriteObject(TextWriter writer, object value)
		{
#if MONOTOUCH
			if (writer == null) return;
#endif
			CacheFn(writer, value);
		}
	}

}
#endregion Json_JsonWriter.Generic.cs

/// ********   File: \Jsv\JsvDeserializeType.cs
#region Jsv_JsvDeserializeType.cs

namespace ServiceStack.Text.Jsv
{
	public static class JsvDeserializeType
	{
		public static SetPropertyDelegate GetSetPropertyMethod(Type type, PropertyInfo propertyInfo)
		{
			return TypeAccessor.GetSetPropertyMethod(type, propertyInfo);
		}
	}
}
#endregion Jsv_JsvDeserializeType.cs

/// ********   File: \Jsv\JsvReader.Generic.cs
#region Jsv_JsvReader.Generic.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Jsv
{
	public static class JsvReader
	{ 
		internal static readonly JsReader<JsvTypeSerializer> Instance = new JsReader<JsvTypeSerializer>();

        private static Dictionary<Type, ParseFactoryDelegate> ParseFnCache = new Dictionary<Type, ParseFactoryDelegate>();

		public static ParseStringDelegate GetParseFn(Type type)
		{
			ParseFactoryDelegate parseFactoryFn;
            ParseFnCache.TryGetValue(type, out parseFactoryFn);

            if (parseFactoryFn != null) return parseFactoryFn();

            var genericType = typeof(JsvReader<>).MakeGenericType(type);
            var mi = genericType.GetMethod("GetParseFn", BindingFlags.Public | BindingFlags.Static);
            parseFactoryFn = (ParseFactoryDelegate)Delegate.CreateDelegate(typeof(ParseFactoryDelegate), mi);

            Dictionary<Type, ParseFactoryDelegate> snapshot, newCache;
            do
            {
                snapshot = ParseFnCache;
                newCache = new Dictionary<Type, ParseFactoryDelegate>(ParseFnCache);
                newCache[type] = parseFactoryFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ParseFnCache, newCache, snapshot), snapshot));
            
            return parseFactoryFn();
		}
	}

	public static class JsvReader<T>
	{
		private static readonly ParseStringDelegate ReadFn;

		static JsvReader()
		{
			ReadFn = JsvReader.Instance.GetParseFn<T>();
		}
		
		public static ParseStringDelegate GetParseFn()
		{
			return ReadFn ?? Parse;
		}

		public static object Parse(string value)
		{
			if (ReadFn == null)
			{
				if (typeof(T).IsInterface)
				{
					throw new NotSupportedException("Can not deserialize interface type: "
						+ typeof(T).Name);
				}
			}
			return value == null 
			       	? null 
			       	: ReadFn(value);
		}
	}
}
#endregion Jsv_JsvReader.Generic.cs

/// ********   File: \Jsv\JsvSerializer.Generic.cs
#region Jsv_JsvSerializer.Generic.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Jsv
{
	public class JsvSerializer<T>
	{
		Dictionary<Type, ParseStringDelegate> DeserializerCache = new Dictionary<Type, ParseStringDelegate>();

		public T DeserializeFromString(string value, Type type)
		{
			ParseStringDelegate parseFn;
            if (DeserializerCache.TryGetValue(type, out parseFn)) return (T)parseFn(value);

            var genericType = typeof(T).MakeGenericType(type);
            var mi = genericType.GetMethod("DeserializeFromString", new[] { typeof(string) });
            parseFn = (ParseStringDelegate)Delegate.CreateDelegate(typeof(ParseStringDelegate), mi);

            Dictionary<Type, ParseStringDelegate> snapshot, newCache;
            do
            {
                snapshot = DeserializerCache;
                newCache = new Dictionary<Type, ParseStringDelegate>(DeserializerCache);
                newCache[type] = parseFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref DeserializerCache, newCache, snapshot), snapshot));
            
            return (T)parseFn(value);
		}

		public T DeserializeFromString(string value)
		{
			if (typeof(T) == typeof(string)) return (T)(object)value;

			return (T)JsvReader<T>.Parse(value);
		}

		public void SerializeToWriter(T value, TextWriter writer)
		{
			JsvWriter<T>.WriteObject(writer, value);
		}

		public string SerializeToString(T value)
		{
			if (value == null) return null;
			if (value is string) return value as string;

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				JsvWriter<T>.WriteObject(writer, value);
			}
			return sb.ToString();
		}
	}
}
#endregion Jsv_JsvSerializer.Generic.cs

/// ********   File: \Jsv\JsvTypeSerializer.cs
#region Jsv_JsvTypeSerializer.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Jsv
{
	internal class JsvTypeSerializer
		: ITypeSerializer
	{
		public static ITypeSerializer Instance = new JsvTypeSerializer();

	    public bool IncludeNullValues
	    {
            get { return false; } //Doesn't support null values, treated as "null" string literal
	    }

        public string TypeAttrInObject
        {
            get { return JsConfig.JsvTypeAttrInObject; }
        }

        internal static string GetTypeAttrInObject(string typeAttr)
        {
            return string.Format("{{{0}:", typeAttr);
        }

		public WriteObjectDelegate GetWriteFn<T>()
		{
			return JsvWriter<T>.WriteFn();
		}

		public WriteObjectDelegate GetWriteFn(Type type)
		{
			return JsvWriter.GetWriteFn(type);
		}

		static readonly ServiceStack.Text.Json.TypeInfo DefaultTypeInfo = new ServiceStack.Text.Json.TypeInfo { EncodeMapKey = false };

		public ServiceStack.Text.Json.TypeInfo GetTypeInfo(Type type)
		{
			return DefaultTypeInfo;
		}

		public void WriteRawString(TextWriter writer, string value)
		{
			writer.Write(value.EncodeJsv());
		}

		public void WritePropertyName(TextWriter writer, string value)
		{
			writer.Write(value);
		}

		public void WriteBuiltIn(TextWriter writer, object value)
		{
			writer.Write(value);
		}

		public void WriteObjectString(TextWriter writer, object value)
		{
			if (value != null)
			{
				writer.Write(value.ToString().EncodeJsv());
			}
		}

		public void WriteException(TextWriter writer, object value)
		{
			writer.Write(((Exception)value).Message.EncodeJsv());
		}

		public void WriteString(TextWriter writer, string value)
		{
			writer.Write(value.EncodeJsv());
		}

		public void WriteDateTime(TextWriter writer, object oDateTime)
		{
			writer.Write(DateTimeSerializer.ToShortestXsdDateTimeString((DateTime)oDateTime));
		}

		public void WriteNullableDateTime(TextWriter writer, object dateTime)
		{
			if (dateTime == null) return;
			writer.Write(DateTimeSerializer.ToShortestXsdDateTimeString((DateTime)dateTime));
		}

		public void WriteDateTimeOffset(TextWriter writer, object oDateTimeOffset)
		{
			writer.Write(((DateTimeOffset) oDateTimeOffset).ToString("o"));
		}

		public void WriteNullableDateTimeOffset(TextWriter writer, object dateTimeOffset)
		{
			if (dateTimeOffset == null) return;
			this.WriteDateTimeOffset(writer, dateTimeOffset);
		}

        public void WriteTimeSpan(TextWriter writer, object oTimeSpan)
        {
            writer.Write(DateTimeSerializer.ToXsdTimeSpanString((TimeSpan)oTimeSpan));
        }

        public void WriteNullableTimeSpan(TextWriter writer, object oTimeSpan)
        {
            if (oTimeSpan == null) return;
            writer.Write(DateTimeSerializer.ToXsdTimeSpanString((TimeSpan?)oTimeSpan));
        }

		public void WriteGuid(TextWriter writer, object oValue)
		{
			writer.Write(((Guid)oValue).ToString("N"));
		}

		public void WriteNullableGuid(TextWriter writer, object oValue)
		{
			if (oValue == null) return;
			writer.Write(((Guid)oValue).ToString("N"));
		}

		public void WriteBytes(TextWriter writer, object oByteValue)
		{
			if (oByteValue == null) return;
			writer.Write(Convert.ToBase64String((byte[])oByteValue));
		}

		public void WriteChar(TextWriter writer, object charValue)
		{
			if (charValue == null) return;
			writer.Write((char)charValue);
		}

		public void WriteByte(TextWriter writer, object byteValue)
		{
			if (byteValue == null) return;
			writer.Write((byte)byteValue);
		}

		public void WriteInt16(TextWriter writer, object intValue)
		{
			if (intValue == null) return;
			writer.Write((short)intValue);
		}

		public void WriteUInt16(TextWriter writer, object intValue)
		{
			if (intValue == null) return;
			writer.Write((ushort)intValue);
		}

		public void WriteInt32(TextWriter writer, object intValue)
		{
			if (intValue == null) return;
			writer.Write((int)intValue);
		}

		public void WriteUInt32(TextWriter writer, object uintValue)
		{
			if (uintValue == null) return;
			writer.Write((uint)uintValue);
		}

		public void WriteUInt64(TextWriter writer, object ulongValue)
		{
			if (ulongValue == null) return;
			writer.Write((ulong)ulongValue);
		}

		public void WriteInt64(TextWriter writer, object longValue)
		{
			if (longValue == null) return;
			writer.Write((long)longValue);
		}

		public void WriteBool(TextWriter writer, object boolValue)
		{
			if (boolValue == null) return;
			writer.Write((bool)boolValue);
		}

		public void WriteFloat(TextWriter writer, object floatValue)
		{
			if (floatValue == null) return;
			var floatVal = (float)floatValue;
			if (Equals(floatVal, float.MaxValue) || Equals(floatVal, float.MinValue))
				writer.Write(floatVal.ToString("r", CultureInfo.InvariantCulture));
			else
				writer.Write(floatVal.ToString(CultureInfo.InvariantCulture));
		}

		public void WriteDouble(TextWriter writer, object doubleValue)
		{
			if (doubleValue == null) return;
			var doubleVal = (double)doubleValue;
			if (Equals(doubleVal, double.MaxValue) || Equals(doubleVal, double.MinValue))
				writer.Write(doubleVal.ToString("r", CultureInfo.InvariantCulture));
			else
				writer.Write(doubleVal.ToString(CultureInfo.InvariantCulture));
		}

		public void WriteDecimal(TextWriter writer, object decimalValue)
		{
			if (decimalValue == null) return;
			writer.Write(((decimal)decimalValue).ToString(CultureInfo.InvariantCulture));
		}

		public void WriteEnum(TextWriter writer, object enumValue)
		{
			if (enumValue == null) return;
			if (JsConfig.TreatEnumAsInteger)
				JsWriter.WriteEnumFlags(writer, enumValue);
			else
				writer.Write(enumValue.ToString());
		}

        public void WriteEnumFlags(TextWriter writer, object enumFlagValue)
        {
			JsWriter.WriteEnumFlags(writer, enumFlagValue);
        }

		public void WriteLinqBinary(TextWriter writer, object linqBinaryValue)
        {
#if !MONOTOUCH && !SILVERLIGHT && !XBOX  && !ANDROID
			WriteRawString(writer, Convert.ToBase64String(((System.Data.Linq.Binary)linqBinaryValue).ToArray()));
#endif
        }

		public object EncodeMapKey(object value)
		{
			return value;
		}

		public ParseStringDelegate GetParseFn<T>()
		{
			return JsvReader.Instance.GetParseFn<T>();
		}

		public ParseStringDelegate GetParseFn(Type type)
		{
			return JsvReader.GetParseFn(type);
		}

        public string UnescapeSafeString(string value)
        {
            return value.FromCsvField();
        }

		public string ParseRawString(string value)
		{
			return value;
		}

		public string ParseString(string value)
		{
			return value.FromCsvField();
		}

	    public string UnescapeString(string value)
	    {
            return value.FromCsvField();
        }

	    public string EatTypeValue(string value, ref int i)
		{
			return EatValue(value, ref i);
		}

		public bool EatMapStartChar(string value, ref int i)
		{
			var success = value[i] == JsWriter.MapStartChar;
			if (success) i++;
			return success;
		}

		public string EatMapKey(string value, ref int i)
		{
			var tokenStartPos = i;

			var valueLength = value.Length;

			var valueChar = value[tokenStartPos];

			switch (valueChar)
			{
				case JsWriter.QuoteChar:
					while (++i < valueLength)
					{
						valueChar = value[i];

						if (valueChar != JsWriter.QuoteChar) continue;

						var isLiteralQuote = i + 1 < valueLength && value[i + 1] == JsWriter.QuoteChar;

						i++; //skip quote
						if (!isLiteralQuote)
							break;
					}
					return value.Substring(tokenStartPos, i - tokenStartPos);

				//Is Type/Map, i.e. {...}
				case JsWriter.MapStartChar:
					var endsToEat = 1;
					var withinQuotes = false;
					while (++i < valueLength && endsToEat > 0)
					{
						valueChar = value[i];

						if (valueChar == JsWriter.QuoteChar)
							withinQuotes = !withinQuotes;

						if (withinQuotes)
							continue;

						if (valueChar == JsWriter.MapStartChar)
							endsToEat++;

						if (valueChar == JsWriter.MapEndChar)
							endsToEat--;
					}
					return value.Substring(tokenStartPos, i - tokenStartPos);
			}

			while (value[++i] != JsWriter.MapKeySeperator) { }
			return value.Substring(tokenStartPos, i - tokenStartPos);
		}

		public bool EatMapKeySeperator(string value, ref int i)
		{
			return value[i++] == JsWriter.MapKeySeperator;
		}

		public bool EatItemSeperatorOrMapEndChar(string value, ref int i)
		{
			if (i == value.Length) return false;

			var success = value[i] == JsWriter.ItemSeperator
				|| value[i] == JsWriter.MapEndChar;
			i++;
			return success;
		}

        public void EatWhitespace(string value, ref int i)
        {
        }

		public string EatValue(string value, ref int i)
		{
			var tokenStartPos = i;
			var valueLength = value.Length;
			if (i == valueLength) return null;

			var valueChar = value[i];
			var withinQuotes = false;
			var endsToEat = 1;

			switch (valueChar)
			{
				//If we are at the end, return.
				case JsWriter.ItemSeperator:
				case JsWriter.MapEndChar:
					return null;

				//Is Within Quotes, i.e. "..."
				case JsWriter.QuoteChar:
					while (++i < valueLength)
					{
						valueChar = value[i];

						if (valueChar != JsWriter.QuoteChar) continue;

						var isLiteralQuote = i + 1 < valueLength && value[i + 1] == JsWriter.QuoteChar;

						i++; //skip quote
						if (!isLiteralQuote)
							break;
					}
					return value.Substring(tokenStartPos, i - tokenStartPos);

				//Is Type/Map, i.e. {...}
				case JsWriter.MapStartChar:
					while (++i < valueLength && endsToEat > 0)
					{
						valueChar = value[i];

						if (valueChar == JsWriter.QuoteChar)
							withinQuotes = !withinQuotes;

						if (withinQuotes)
							continue;

						if (valueChar == JsWriter.MapStartChar)
							endsToEat++;

						if (valueChar == JsWriter.MapEndChar)
							endsToEat--;
					}
					return value.Substring(tokenStartPos, i - tokenStartPos);

				//Is List, i.e. [...]
				case JsWriter.ListStartChar:
					while (++i < valueLength && endsToEat > 0)
					{
						valueChar = value[i];

						if (valueChar == JsWriter.QuoteChar)
							withinQuotes = !withinQuotes;

						if (withinQuotes)
							continue;

						if (valueChar == JsWriter.ListStartChar)
							endsToEat++;

						if (valueChar == JsWriter.ListEndChar)
							endsToEat--;
					}
					return value.Substring(tokenStartPos, i - tokenStartPos);
			}

			//Is Value
			while (++i < valueLength)
			{
				valueChar = value[i];

				if (valueChar == JsWriter.ItemSeperator
					|| valueChar == JsWriter.MapEndChar)
				{
					break;
				}
			}

			return value.Substring(tokenStartPos, i - tokenStartPos);
		}
	}
}
#endregion Jsv_JsvTypeSerializer.cs

/// ********   File: \Jsv\JsvWriter.Generic.cs
#region Jsv_JsvWriter.Generic.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Jsv
{
	internal static class JsvWriter
	{
		public static readonly JsWriter<JsvTypeSerializer> Instance = new JsWriter<JsvTypeSerializer>();

        private static Dictionary<Type, WriteObjectDelegate> WriteFnCache = new Dictionary<Type, WriteObjectDelegate>();

		public static WriteObjectDelegate GetWriteFn(Type type)
		{
			try
			{
                WriteObjectDelegate writeFn;
                if (WriteFnCache.TryGetValue(type, out writeFn)) return writeFn;

                var genericType = typeof(JsvWriter<>).MakeGenericType(type);
                var mi = genericType.GetMethod("WriteFn", BindingFlags.Public | BindingFlags.Static);
                var writeFactoryFn = (Func<WriteObjectDelegate>)Delegate.CreateDelegate(typeof(Func<WriteObjectDelegate>), mi);
                writeFn = writeFactoryFn();

                Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
                do
                {
                    snapshot = WriteFnCache;
                    newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache);
                    newCache[type] = writeFn;

                } while (!ReferenceEquals(
                    Interlocked.CompareExchange(ref WriteFnCache, newCache, snapshot), snapshot));

                return writeFn;
			}
			catch (Exception ex)
			{
				Tracer.Instance.WriteError(ex);
				throw;
			}
		}

		public static void WriteLateBoundObject(TextWriter writer, object value)
		{
			if (value == null) return;
			var type = value.GetType();
			var writeFn = type == typeof(object)
                ? WriteType<object, JsvTypeSerializer>.WriteEmptyType
				: GetWriteFn(type);

			var prevState = JsState.IsWritingDynamic;
			JsState.IsWritingDynamic = true;
			writeFn(writer, value);
			JsState.IsWritingDynamic = prevState;
		}

		public static WriteObjectDelegate GetValueTypeToStringMethod(Type type)
		{
			return Instance.GetValueTypeToStringMethod(type);
		}
	}

	/// <summary>
	/// Implement the serializer using a more static approach
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal static class JsvWriter<T>
	{
		private static readonly WriteObjectDelegate CacheFn;

		public static WriteObjectDelegate WriteFn()
		{
			return CacheFn ?? WriteObject;
		}

		static JsvWriter()
		{
		    CacheFn = typeof(T) == typeof(object) 
                ? JsvWriter.WriteLateBoundObject 
                : JsvWriter.Instance.GetWriteFn<T>();
		}

	    public static void WriteObject(TextWriter writer, object value)
		{
#if MONOTOUCH
			if (writer == null) return;
#endif
			CacheFn(writer, value);
		}

	}
}
#endregion Jsv_JsvWriter.Generic.cs

/// ********   File: \Marc\Link.cs
#region Marc_Link.cs

//Not using it here, but @marcgravell's stuff is too good not to include
namespace ServiceStack.Text.Marc
{
	/// <summary>
	/// Pretty Thread-Safe cache class from:
	/// http://code.google.com/p/dapper-dot-net/source/browse/Dapper/SqlMapper.cs
	/// 
	/// This is a micro-cache; suitable when the number of terms is controllable (a few hundred, for example),
	/// and strictly append-only; you cannot change existing values. All key matches are on **REFERENCE**
	/// equality. The type is fully thread-safe.
	/// </summary>
	class Link<TKey, TValue> where TKey : class
	{
		public static bool TryGet(Link<TKey, TValue> link, TKey key, out TValue value)
		{
			while (link != null)
			{
				if ((object)key == (object)link.Key)
				{
					value = link.Value;
					return true;
				}
				link = link.Tail;
			}
			value = default(TValue);
			return false;
		}

		public static bool TryAdd(ref Link<TKey, TValue> head, TKey key, ref TValue value)
		{
			bool tryAgain;
			do
			{
				var snapshot = Interlocked.CompareExchange(ref head, null, null);
				TValue found;
				if (TryGet(snapshot, key, out found))
				{ // existing match; report the existing value instead
					value = found;
					return false;
				}
				var newNode = new Link<TKey, TValue>(key, value, snapshot);
				// did somebody move our cheese?
				tryAgain = Interlocked.CompareExchange(ref head, newNode, snapshot) != snapshot;
			} while (tryAgain);
			return true;
		}

		private Link(TKey key, TValue value, Link<TKey, TValue> tail)
		{
			Key = key;
			Value = value;
			Tail = tail;
		}
		
		public TKey Key { get; private set; }
		public TValue Value { get; private set; }
		public Link<TKey, TValue> Tail { get; private set; }
	}
}
#endregion Marc_Link.cs

/// ********   File: \Marc\ObjectAccessor.cs
#region Marc_ObjectAccessor.cs
//using System.Dynamic;

//Not using it here, but @marcgravell's stuff is too good not to include
#if !SILVERLIGHT && !MONOTOUCH && !XBOX
namespace ServiceStack.Text.FastMember
{
    /// <summary>
    /// Represents an individual object, allowing access to members by-name
    /// </summary>
    public abstract class ObjectAccessor
    {
        /// <summary>
        /// Get or Set the value of a named member for the underlying object
        /// </summary>
        public abstract object this[string name] { get; set; }
        /// <summary>
        /// The object represented by this instance
        /// </summary>
        public abstract object Target { get; }
        /// <summary>
        /// Use the target types definition of equality
        /// </summary>
        public override bool Equals(object obj)
        {
            return Target.Equals(obj);
        }
        /// <summary>
        /// Obtain the hash of the target object
        /// </summary>
        public override int GetHashCode()
        {
            return Target.GetHashCode();
        }
        /// <summary>
        /// Use the target's definition of a string representation
        /// </summary>
        public override string ToString()
        {
            return Target.ToString();
        }

        /// <summary>
        /// Wraps an individual object, allowing by-name access to that instance
        /// </summary>
        public static ObjectAccessor Create(object target)
        {
            if (target == null) throw new ArgumentNullException("target");
            //IDynamicMetaObjectProvider dlr = target as IDynamicMetaObjectProvider;
            //if (dlr != null) return new DynamicWrapper(dlr); // use the DLR
            return new TypeAccessorWrapper(target, TypeAccessor.Create(target.GetType()));
        }

        sealed class TypeAccessorWrapper : ObjectAccessor
        {
            private readonly object target;
            private readonly TypeAccessor accessor;
            public TypeAccessorWrapper(object target, TypeAccessor accessor)
            {
                this.target = target;
                this.accessor = accessor;
            }
            public override object this[string name]
            {
                get { return accessor[target, name.ToUpperInvariant()]; }
				set { accessor[target, name.ToUpperInvariant()] = value; }
            }
            public override object Target
            {
                get { return target; }
            }
        }

		//sealed class DynamicWrapper : ObjectAccessor
		//{
		//    private readonly IDynamicMetaObjectProvider target;
		//    public override object Target
		//    {
		//        get { return target; }
		//    }
		//    public DynamicWrapper(IDynamicMetaObjectProvider target)
		//    {
		//        this.target = target;
		//    }
		//    public override object this[string name]
		//    {
		//        get { return CallSiteCache.GetValue(name, target); }
		//        set { CallSiteCache.SetValue(name, target, value); }
		//    }
        //}
    }

}

#endif
#endregion Marc_ObjectAccessor.cs

/// ********   File: \Marc\TypeAccessor.cs
#region Marc_TypeAccessor.cs
//using System.Dynamic;

//Not using it here, but @marcgravell's stuff is too good not to include
// http://code.google.com/p/fast-member/ Apache License 2.0
#if !SILVERLIGHT && !MONOTOUCH && !XBOX
namespace ServiceStack.Text.FastMember
{
    /// <summary>
    /// Provides by-name member-access to objects of a given type
    /// </summary>
    public abstract class TypeAccessor
    {
        // hash-table has better read-without-locking semantics than dictionary
        private static readonly Hashtable typeLookyp = new Hashtable();

        /// <summary>
        /// Does this type support new instances via a parameterless constructor?
        /// </summary>
        public virtual bool CreateNewSupported { get { return false; } }
        /// <summary>
        /// Create a new instance of this type
        /// </summary>
        public virtual object CreateNew() { throw new NotSupportedException();}

        /// <summary>
        /// Provides a type-specific accessor, allowing by-name access for all objects of that type
        /// </summary>
        /// <remarks>The accessor is cached internally; a pre-existing accessor may be returned</remarks>
        public static TypeAccessor Create(Type type)
        {
            if(type == null) throw new ArgumentNullException("type");
            TypeAccessor obj = (TypeAccessor)typeLookyp[type];
            if (obj != null) return obj;

            lock(typeLookyp)
            {
                // double-check
                obj = (TypeAccessor)typeLookyp[type];
                if (obj != null) return obj;

                obj = CreateNew(type);

                typeLookyp[type] = obj;
                return obj;
            }
        }

		//sealed class DynamicAccessor : TypeAccessor
		//{
		//    public static readonly DynamicAccessor Singleton = new DynamicAccessor();
		//    private DynamicAccessor(){}
		//    public override object this[object target, string name]
		//    {
		//        get { return CallSiteCache.GetValue(name, target); }
		//        set { CallSiteCache.SetValue(name, target, value); }
		//    }
		//}

        private static AssemblyBuilder assembly;
        private static ModuleBuilder module;
        private static int counter;

        private static void WriteGetter(ILGenerator il, Type type, PropertyInfo[] props, FieldInfo[] fields, bool isStatic)
        {
            LocalBuilder loc = type.IsValueType ? il.DeclareLocal(type) : null;
            OpCode propName = isStatic ? OpCodes.Ldarg_1 : OpCodes.Ldarg_2, target = isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1;
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetIndexParameters().Length != 0 || !prop.CanRead) continue;

                Label next = il.DefineLabel();
                il.Emit(propName);
                il.Emit(OpCodes.Ldstr, prop.Name);
                il.EmitCall(OpCodes.Call, strinqEquals, null);
                il.Emit(OpCodes.Brfalse_S, next);
                // match:
                il.Emit(target);
                Cast(il, type, loc);
                il.EmitCall(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, prop.GetGetMethod(), null);
                if (prop.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Box, prop.PropertyType);
                }
                il.Emit(OpCodes.Ret);
                // not match:
                il.MarkLabel(next);
            }
            foreach (FieldInfo field in fields)
            {
                Label next = il.DefineLabel();
                il.Emit(propName);
                il.Emit(OpCodes.Ldstr, field.Name);
                il.EmitCall(OpCodes.Call, strinqEquals, null);
                il.Emit(OpCodes.Brfalse_S, next);
                // match:
                il.Emit(target);
                Cast(il, type, loc);
                il.Emit(OpCodes.Ldfld, field);
                if (field.FieldType.IsValueType)
                {
                    il.Emit(OpCodes.Box, field.FieldType);
                }
                il.Emit(OpCodes.Ret);
                // not match:
                il.MarkLabel(next);
            }
            il.Emit(OpCodes.Ldstr, "name");
            il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Throw);
        }
        private static void WriteSetter(ILGenerator il, Type type, PropertyInfo[] props, FieldInfo[] fields, bool isStatic)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldstr, "Write is not supported for structs");
                il.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(new Type[] { typeof(string) }));
                il.Emit(OpCodes.Throw);
            }
            else
            {
                OpCode propName = isStatic ? OpCodes.Ldarg_1 : OpCodes.Ldarg_2,
                       target = isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1,
                       value = isStatic ? OpCodes.Ldarg_2 : OpCodes.Ldarg_3;
                LocalBuilder loc = type.IsValueType ? il.DeclareLocal(type) : null;
                foreach (PropertyInfo prop in props)
                {
                    if (prop.GetIndexParameters().Length != 0 || !prop.CanWrite) continue;

                    Label next = il.DefineLabel();
                    il.Emit(propName);
                    il.Emit(OpCodes.Ldstr, prop.Name);
                    il.EmitCall(OpCodes.Call, strinqEquals, null);
                    il.Emit(OpCodes.Brfalse_S, next);
                    // match:
                    il.Emit(target);
                    Cast(il, type, loc);
                    il.Emit(value);
                    Cast(il, prop.PropertyType, null);
                    il.EmitCall(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, prop.GetSetMethod(), null);
                    il.Emit(OpCodes.Ret);
                    // not match:
                    il.MarkLabel(next);
                }
                foreach (FieldInfo field in fields)
                {
                    Label next = il.DefineLabel();
                    il.Emit(propName);
                    il.Emit(OpCodes.Ldstr, field.Name);
                    il.EmitCall(OpCodes.Call, strinqEquals, null);
                    il.Emit(OpCodes.Brfalse_S, next);
                    // match:
                    il.Emit(target);
                    Cast(il, type, loc);
                    il.Emit(value);
                    Cast(il, field.FieldType, null);
                    il.Emit(OpCodes.Stfld, field);
                    il.Emit(OpCodes.Ret);
                    // not match:
                    il.MarkLabel(next);
                }
                il.Emit(OpCodes.Ldstr, "name");
                il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
                il.Emit(OpCodes.Throw);
            }
        }
        private static readonly MethodInfo strinqEquals = typeof(string).GetMethod("op_Equality", new Type[] { typeof(string), typeof(string) });

        sealed class DelegateAccessor : TypeAccessor
        {
            private readonly Func<object, string, object> getter;
            private readonly Action<object, string, object> setter;
            private readonly Func<object> ctor;
            public DelegateAccessor(Func<object, string, object> getter, Action<object, string, object> setter, Func<object> ctor)
            {
                this.getter = getter;
                this.setter = setter;
                this.ctor = ctor;
            }
            public override bool CreateNewSupported { get { return ctor != null; } }
            public override object CreateNew()
            {
                return ctor != null ? ctor() : base.CreateNew();
            }
            public override object this[object target, string name]
            {
                get { return getter(target, name); }
                set { setter(target, name, value); }
            }
        }
        private static bool IsFullyPublic(Type type)
        {
            while (type.IsNestedPublic) type = type.DeclaringType;
            return type.IsPublic;
        }
        static TypeAccessor CreateNew(Type type)
        {
			//if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type))
			//{
			//    return DynamicAccessor.Singleton;
			//}

            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            ConstructorInfo ctor = null;
            if(type.IsClass && !type.IsAbstract)
            {
                ctor = type.GetConstructor(Type.EmptyTypes);
            }
            ILGenerator il;
            if(!IsFullyPublic(type))
            {
                DynamicMethod dynGetter = new DynamicMethod(type.FullName + "_get", typeof(object), new Type[] { typeof(object), typeof(string) }, type, true),
                              dynSetter = new DynamicMethod(type.FullName + "_set", null, new Type[] { typeof(object), typeof(string), typeof(object) }, type, true);
                WriteGetter(dynGetter.GetILGenerator(), type, props, fields, true);
                WriteSetter(dynSetter.GetILGenerator(), type, props, fields, true);
                DynamicMethod dynCtor = null;
                if(ctor != null)
                {
                    dynCtor = new DynamicMethod(type.FullName + "_ctor", typeof(object), Type.EmptyTypes, type, true);
                    il = dynCtor.GetILGenerator();
                    il.Emit(OpCodes.Newobj, ctor);
                    il.Emit(OpCodes.Ret);
                }
                return new DelegateAccessor(
                    (Func<object,string,object>)dynGetter.CreateDelegate(typeof(Func<object,string,object>)),
                    (Action<object,string,object>)dynSetter.CreateDelegate(typeof(Action<object,string,object>)),
                    dynCtor == null ? null : (Func<object>)dynCtor.CreateDelegate(typeof(Func<object>)));
            }

            // note this region is synchronized; only one is being created at a time so we don't need to stress about the builders
            if(assembly == null)
            {
                AssemblyName name = new AssemblyName("FastMember_dynamic");
                assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                module = assembly.DefineDynamicModule(name.Name);
            }
            TypeBuilder tb = module.DefineType("FastMember_dynamic." + type.Name + "_" + Interlocked.Increment(ref counter),
                (typeof(TypeAccessor).Attributes | TypeAttributes.Sealed) & ~TypeAttributes.Abstract, typeof(TypeAccessor) );

            tb.DefineDefaultConstructor(MethodAttributes.Public);
            PropertyInfo indexer = typeof (TypeAccessor).GetProperty("Item");
            MethodInfo baseGetter = indexer.GetGetMethod(), baseSetter = indexer.GetSetMethod();
            MethodBuilder body = tb.DefineMethod(baseGetter.Name, baseGetter.Attributes & ~MethodAttributes.Abstract, typeof(object), new Type[] {typeof(object), typeof(string)});
            il = body.GetILGenerator();
            WriteGetter(il, type, props, fields, false);
            tb.DefineMethodOverride(body, baseGetter);

            body = tb.DefineMethod(baseSetter.Name, baseSetter.Attributes & ~MethodAttributes.Abstract, null, new Type[] { typeof(object), typeof(string), typeof(object) });
            il = body.GetILGenerator();
            WriteSetter(il, type, props, fields, false);
            tb.DefineMethodOverride(body, baseSetter);

            if(ctor != null)
            {
                MethodInfo baseMethod = typeof (TypeAccessor).GetProperty("CreateNewSupported").GetGetMethod();
                body = tb.DefineMethod(baseMethod.Name, baseMethod.Attributes, typeof (bool), Type.EmptyTypes);
                il = body.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Ret);
                tb.DefineMethodOverride(body, baseMethod);

                baseMethod = typeof (TypeAccessor).GetMethod("CreateNew");
                body = tb.DefineMethod(baseMethod.Name, baseMethod.Attributes, typeof (object), Type.EmptyTypes);
                il = body.GetILGenerator();
                il.Emit(OpCodes.Newobj, ctor);
                il.Emit(OpCodes.Ret);
                tb.DefineMethodOverride(body, baseMethod);
            }

            return (TypeAccessor)Activator.CreateInstance(tb.CreateType());
        }

        private static void Cast(ILGenerator il, Type type, LocalBuilder addr)
        {
            if(type == typeof(object)) {}
            else if(type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
                if (addr != null)
                {
                    il.Emit(OpCodes.Stloc, addr);
                    il.Emit(OpCodes.Ldloca_S, addr);
                }
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        /// Get or set the value of a named member on the target instance
        /// </summary>
        public abstract object this[object target, string name]
        {
            get; set;
        }
    }
}

#endif

#endregion Marc_TypeAccessor.cs


/// ********   File: \Reflection\StaticAccessors.cs
#region Reflection_StaticAccessors.cs

namespace ServiceStack.Text.Reflection
{
    public static class StaticAccessors
    {
        public static Func<object, object> GetValueGetter(this PropertyInfo propertyInfo, Type type)
        {
#if (SILVERLIGHT && !WINDOWS_PHONE) || MONOTOUCH || XBOX
			var getMethodInfo = propertyInfo.GetGetMethod();
			if (getMethodInfo == null) return null;
			return x => getMethodInfo.Invoke(x, new object[0]);
#else

            var instance = Expression.Parameter(typeof(object), "i");
            var convertInstance = Expression.TypeAs(instance, propertyInfo.DeclaringType);
            var property = Expression.Property(convertInstance, propertyInfo);
            var convertProperty = Expression.TypeAs(property, typeof(object));
            return Expression.Lambda<Func<object, object>>(convertProperty, instance).Compile();
#endif
        }

        public static Func<T, object> GetValueGetter<T>(this PropertyInfo propertyInfo)
        {
#if (SILVERLIGHT && !WINDOWS_PHONE) || MONOTOUCH || XBOX
			var getMethodInfo = propertyInfo.GetGetMethod();
			if (getMethodInfo == null) return null;
			return x => getMethodInfo.Invoke(x, new object[0]);
#else
            var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
            var property = Expression.Property(instance, propertyInfo);
            var convert = Expression.TypeAs(property, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, instance).Compile();
#endif
        }

#if !XBOX
        public static Action<T, object> GetValueSetter<T>(this PropertyInfo propertyInfo)
        {
            if (typeof(T) != propertyInfo.DeclaringType)
            {
                throw new ArgumentException();
            }

            var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
            var argument = Expression.Parameter(typeof(object), "a");
            var setterCall = Expression.Call(
                instance,
                propertyInfo.GetSetMethod(),
                Expression.Convert(argument, propertyInfo.PropertyType));

            return Expression.Lambda<Action<T, object>>
            (
                setterCall, instance, argument
            ).Compile();
        }
#endif

    }
}

#endregion Reflection_StaticAccessors.cs

/// ********   File: \Support\AssemblyTypeDefinition.cs
#region Support_AssemblyTypeDefinition.cs

namespace ServiceStack.Common.Support
{
	internal class AssemblyTypeDefinition
	{
		private const char TypeDefinitionSeperator = ',';
		private const int TypeNameIndex = 0;
		private const int AssemblyNameIndex = 1;

		public AssemblyTypeDefinition(string typeDefinition)
		{
			if (string.IsNullOrEmpty(typeDefinition))
			{
				throw new ArgumentNullException();
			}
			var parts = typeDefinition.Split(TypeDefinitionSeperator);
			TypeName = parts[TypeNameIndex].Trim();
			AssemblyName = (parts.Length > AssemblyNameIndex) ? parts[AssemblyNameIndex].Trim() : null;
		}

		public string TypeName { get; set; }

		public string AssemblyName { get; set; }
	}
}
#endregion Support_AssemblyTypeDefinition.cs

/// ********   File: \Support\DoubleConverter.cs
#region Support_DoubleConverter.cs
namespace ServiceStack.Text.Support
{
	using System;
	using System.Globalization;

	/// <summary>
	/// A class to allow the conversion of doubles to string representations of
	/// their exact decimal values. The implementation aims for readability over
	/// efficiency.
	/// 
	/// Courtesy of @JonSkeet
	/// http://www.yoda.arachsys.com/csharp/DoubleConverter.cs
	/// </summary>
	public class DoubleConverter
	{
		/// <summary>
		/// Converts the given double to a string representation of its
		/// exact decimal value.
		/// </summary>
		/// <param name="d">The double to convert.</param>
		/// <returns>A string representation of the double's exact decimal value.</return>
		public static string ToExactString(double d)
		{
#if XBOX
			return BitConverter.ToString( BitConverter.GetBytes( d ) ) ;
#else
			if (double.IsPositiveInfinity(d))
				return "+Infinity";
			if (double.IsNegativeInfinity(d))
				return "-Infinity";
			if (double.IsNaN(d))
				return "NaN";

			// Translate the double into sign, exponent and mantissa.
			long bits = BitConverter.DoubleToInt64Bits(d);
			// Note that the shift is sign-extended, hence the test against -1 not 1
			bool negative = (bits < 0);
			int exponent = (int)((bits >> 52) & 0x7ffL);
			long mantissa = bits & 0xfffffffffffffL;

			// Subnormal numbers; exponent is effectively one higher,
			// but there's no extra normalisation bit in the mantissa
			if (exponent == 0)
			{
				exponent++;
			}
			// Normal numbers; leave exponent as it is but add extra
			// bit to the front of the mantissa
			else
			{
				mantissa = mantissa | (1L << 52);
			}

			// Bias the exponent. It's actually biased by 1023, but we're
			// treating the mantissa as m.0 rather than 0.m, so we need
			// to subtract another 52 from it.
			exponent -= 1075;

			if (mantissa == 0)
			{
				return "0";
			}

			/* Normalize */
			while ((mantissa & 1) == 0)
			{    /*  i.e., Mantissa is even */
				mantissa >>= 1;
				exponent++;
			}

			/// Construct a new decimal expansion with the mantissa
			ArbitraryDecimal ad = new ArbitraryDecimal(mantissa);

			// If the exponent is less than 0, we need to repeatedly
			// divide by 2 - which is the equivalent of multiplying
			// by 5 and dividing by 10.
			if (exponent < 0)
			{
				for (int i = 0; i < -exponent; i++)
					ad.MultiplyBy(5);
				ad.Shift(-exponent);
			}
			// Otherwise, we need to repeatedly multiply by 2
			else
			{
				for (int i = 0; i < exponent; i++)
					ad.MultiplyBy(2);
			}

			// Finally, return the string with an appropriate sign
			if (negative)
				return "-" + ad.ToString();
			else
				return ad.ToString();
#endif
		}

		/// <summary>Private class used for manipulating
		class ArbitraryDecimal
		{
			/// <summary>Digits in the decimal expansion, one byte per digit
			byte[] digits;
			/// <summary> 
			/// How many digits are *after* the decimal point
			/// </summary>
			int decimalPoint = 0;

			/// <summary> 
			/// Constructs an arbitrary decimal expansion from the given long.
			/// The long must not be negative.
			/// </summary>
			internal ArbitraryDecimal(long x)
			{
				string tmp = x.ToString(CultureInfo.InvariantCulture);
				digits = new byte[tmp.Length];
				for (int i = 0; i < tmp.Length; i++)
					digits[i] = (byte)(tmp[i] - '0');
				Normalize();
			}

			/// <summary>
			/// Multiplies the current expansion by the given amount, which should
			/// only be 2 or 5.
			/// </summary>
			internal void MultiplyBy(int amount)
			{
				byte[] result = new byte[digits.Length + 1];
				for (int i = digits.Length - 1; i >= 0; i--)
				{
					int resultDigit = digits[i] * amount + result[i + 1];
					result[i] = (byte)(resultDigit / 10);
					result[i + 1] = (byte)(resultDigit % 10);
				}
				if (result[0] != 0)
				{
					digits = result;
				}
				else
				{
					Array.Copy(result, 1, digits, 0, digits.Length);
				}
				Normalize();
			}

			/// <summary>
			/// Shifts the decimal point; a negative value makes
			/// the decimal expansion bigger (as fewer digits come after the
			/// decimal place) and a positive value makes the decimal
			/// expansion smaller.
			/// </summary>
			internal void Shift(int amount)
			{
				decimalPoint += amount;
			}

			/// <summary>
			/// Removes leading/trailing zeroes from the expansion.
			/// </summary>
			internal void Normalize()
			{
				int first;
				for (first = 0; first < digits.Length; first++)
					if (digits[first] != 0)
						break;
				int last;
				for (last = digits.Length - 1; last >= 0; last--)
					if (digits[last] != 0)
						break;

				if (first == 0 && last == digits.Length - 1)
					return;

				byte[] tmp = new byte[last - first + 1];
				for (int i = 0; i < tmp.Length; i++)
					tmp[i] = digits[i + first];

				decimalPoint -= digits.Length - (last + 1);
				digits = tmp;
			}

			/// <summary>
			/// Converts the value to a proper decimal string representation.
			/// </summary>
			public override String ToString()
			{
				char[] digitString = new char[digits.Length];
				for (int i = 0; i < digits.Length; i++)
					digitString[i] = (char)(digits[i] + '0');

				// Simplest case - nothing after the decimal point,
				// and last real digit is non-zero, eg value=35
				if (decimalPoint == 0)
				{
					return new string(digitString);
				}

				// Fairly simple case - nothing after the decimal
				// point, but some 0s to add, eg value=350
				if (decimalPoint < 0)
				{
					return new string(digitString) +
						   new string('0', -decimalPoint);
				}

				// Nothing before the decimal point, eg 0.035
				if (decimalPoint >= digitString.Length)
				{
					return "0." +
						new string('0', (decimalPoint - digitString.Length)) +
						new string(digitString);
				}

				// Most complicated case - part of the string comes
				// before the decimal point, part comes after it,
				// eg 3.5
				return new string(digitString, 0,
								   digitString.Length - decimalPoint) +
					"." +
					new string(digitString,
								digitString.Length - decimalPoint,
								decimalPoint);
			}
		}
	}

}
#endregion Support_DoubleConverter.cs

/// ********   File: \Support\TypePair.cs
#region Support_TypePair.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.Support
{
	public class TypePair
	{
		public Type[] Args1 { get; set; }
		public Type[] Arg2 { get; set; }

		public TypePair(Type[] arg1, Type[] arg2)
		{
			Args1 = arg1;
			Arg2 = arg2;
		}

		public bool Equals(TypePair other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Args1, Args1) && Equals(other.Arg2, Arg2);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (TypePair)) return false;
			return Equals((TypePair) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Args1 != null ? Args1.GetHashCode() : 0)*397) ^ (Arg2 != null ? Arg2.GetHashCode() : 0);
			}
		}
	}
}
#endregion Support_TypePair.cs

/// ********   File: \AssemblyUtils.cs
#region AssemblyUtils.cs
#if SILVERLIGHT

#endif

namespace ServiceStack.Text
{
    /// <summary>
    /// Utils to load types
    /// </summary>
    public static class AssemblyUtils
    {
        private const string FileUri = "file:///";
        private const string DllExt = "dll";
        private const string ExeExt = "dll";
        private const char UriSeperator = '/';

#if !XBOX
        /// <summary>
        /// Find the type from the name supplied
        /// </summary>
        /// <param name="typeName">[typeName] or [typeName, assemblyName]</param>
        /// <returns></returns>
        public static Type FindType(string typeName)
        {
#if !SILVERLIGHT
            var type = Type.GetType(typeName);
            if (type != null) return type;
#endif
            var typeDef = new AssemblyTypeDefinition(typeName);
            if (!String.IsNullOrEmpty(typeDef.AssemblyName))
            {
                return FindType(typeDef.TypeName, typeDef.AssemblyName);
            }
            else
            {
                return FindTypeFromLoadedAssemblies(typeDef.TypeName);
            }
        }
#endif

#if !XBOX

		
		/// <summary>
		/// The top-most interface of the given type, if any.
		/// </summary>
    	public static Type MainInterface<T>() {
			var t = typeof(T);
    		if (t.BaseType == typeof(object)) {
				// on Windows, this can be just "t.GetInterfaces()" but Mono doesn't return in order.
				var interfaceType = t.GetInterfaces().FirstOrDefault(i => !t.GetInterfaces().Any(i2 => i2.GetInterfaces().Contains(i)));
				if (interfaceType != null) return interfaceType;
			}
			return t; // not safe to use interface, as it might be a superclass's one.
		}

        /// <summary>
        /// Find type if it exists
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="assemblyName"></param>
        /// <returns>The type if it exists</returns>
        public static Type FindType(string typeName, string assemblyName)
        {
            var type = FindTypeFromLoadedAssemblies(typeName);
            if (type != null)
            {
                return type;
            }
            var binPath = GetAssemblyBinPath(Assembly.GetExecutingAssembly());
            Assembly assembly = null;
            var assemblyDllPath = binPath + String.Format("{0}.{1}", assemblyName, DllExt);
            if (File.Exists(assemblyDllPath))
            {
                assembly = LoadAssembly(assemblyDllPath);
            }
            var assemblyExePath = binPath + String.Format("{0}.{1}", assemblyName, ExeExt);
            if (File.Exists(assemblyExePath))
            {
                assembly = LoadAssembly(assemblyExePath);
            }
            return assembly != null ? assembly.GetType(typeName) : null;
        }
#endif

#if !XBOX
        public static Type FindTypeFromLoadedAssemblies(string typeName)
        {
#if SILVERLIGHT4
        	var assemblies = ((dynamic) AppDomain.CurrentDomain).GetAssemblies() as Assembly[];
#else
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
#endif
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }
#endif

#if !SILVERLIGHT
private static Assembly LoadAssembly(string assemblyPath)
		{
			return Assembly.LoadFrom(assemblyPath);
		}
#elif WINDOWS_PHONE
        private static Assembly LoadAssembly(string assemblyPath)
        {
            return Assembly.LoadFrom(assemblyPath);
        }
#else
        private static Assembly LoadAssembly(string assemblyPath)
        {
            var sri = System.Windows.Application.GetResourceStream(new Uri(assemblyPath, UriKind.Relative));
            var myPart = new System.Windows.AssemblyPart();
            var assembly = myPart.Load(sri.Stream);
            return assembly;
        }
#endif

#if !XBOX
        public static string GetAssemblyBinPath(Assembly assembly)
        {
#if WINDOWS_PHONE
            var codeBase = assembly.GetName().CodeBase;
#else
            var codeBase = assembly.CodeBase;
#endif

            var binPathPos = codeBase.LastIndexOf(UriSeperator);
            var assemblyPath = codeBase.Substring(0, binPathPos + 1);
            if (assemblyPath.StartsWith(FileUri))
            {
                assemblyPath = assemblyPath.Remove(0, FileUri.Length);
            }
            return assemblyPath;
        }
#endif

#if !SILVERLIGHT
		static readonly Regex versionRegEx = new Regex(", Version=[^\\]]+", RegexOptions.Compiled);
#else
        static readonly Regex versionRegEx = new Regex(", Version=[^\\]]+");
#endif
        public static string ToTypeString(this Type type)
        {
            return versionRegEx.Replace(type.AssemblyQualifiedName, "");
        }

        public static string WriteType(Type type)
        {
            return type.ToTypeString();
        }
    }
}
#endregion AssemblyUtils.cs

/// ********   File: \CollectionExtensions.cs
#region CollectionExtensions.cs

namespace ServiceStack.Text
{
    public static class CollectionExtensions
    {
        public static ICollection<T> CreateAndPopulate<T>(Type ofCollectionType, T[] withItems)
        {
            if (ofCollectionType == null) return new List<T>(withItems);

            var genericType = ofCollectionType.GetGenericType();
            var genericTypeDefinition = genericType != null
                ? genericType.GetGenericTypeDefinition()
                : null;
#if !XBOX
            if (genericTypeDefinition == typeof(ServiceStack.Text.WP.HashSet<T>))
                return new ServiceStack.Text.WP.HashSet<T>(withItems);
#endif
            if (genericTypeDefinition == typeof(LinkedList<T>))
                return new LinkedList<T>(withItems);

            var collection = (ICollection<T>)ofCollectionType.CreateInstance();
            foreach (var item in withItems)
            {
                collection.Add(item);
            }
            return collection;
        }

        public static T[] ToArray<T>(this ICollection<T> collection)
        {
            var to = new T[collection.Count];
            collection.CopyTo(to, 0);
            return to;
        }

        public static object Convert<T>(object objCollection, Type toCollectionType)
        {
            var collection = (ICollection<T>) objCollection;
            var to = new T[collection.Count];
            collection.CopyTo(to, 0);
            return CreateAndPopulate(toCollectionType, to);
        }
    }
}
#endregion CollectionExtensions.cs

/// ********   File: \CsvAttribute.cs
#region CsvAttribute.cs

namespace ServiceStack.Text
{
    public enum CsvBehavior
    {
        FirstEnumerable
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CsvAttribute : Attribute
    {
        public CsvBehavior CsvBehavior { get; set; }
        public CsvAttribute(CsvBehavior csvBehavior)
        {
            this.CsvBehavior = csvBehavior;
        }
    }
}
#endregion CsvAttribute.cs

/// ********   File: \CsvConfig.cs
#region CsvConfig.cs

namespace ServiceStack.Text
{
    public static class CsvConfig
    {
		static CsvConfig()
		{
            Reset();
		}

		[ThreadStatic]
		private static string tsItemSeperatorString;
		private static string sItemSeperatorString;
		public static string ItemSeperatorString
		{
			get
			{
				return tsItemSeperatorString ?? sItemSeperatorString ?? JsWriter.ItemSeperatorString;
			}
			set
			{
				tsItemSeperatorString = value;
				if (sItemSeperatorString == null) sItemSeperatorString = value;
                ResetEscapeStrings();
			}
		}

		[ThreadStatic]
		private static string tsItemDelimiterString;
		private static string sItemDelimiterString;
		public static string ItemDelimiterString
		{
			get
			{
				return tsItemDelimiterString ?? sItemDelimiterString ?? JsWriter.QuoteString;
			}
			set
			{
				tsItemDelimiterString = value;
				if (sItemDelimiterString == null) sItemDelimiterString = value;
			    EscapedItemDelimiterString = value + value;
			    ResetEscapeStrings();
			}
		}

        private const string DefaultEscapedItemDelimiterString = JsWriter.QuoteString + JsWriter.QuoteString;

        [ThreadStatic]
		private static string tsEscapedItemDelimiterString;
		private static string sEscapedItemDelimiterString;
		internal static string EscapedItemDelimiterString
		{
			get
			{
				return tsEscapedItemDelimiterString ?? sEscapedItemDelimiterString ?? DefaultEscapedItemDelimiterString;
			}
			set
			{
				tsEscapedItemDelimiterString = value;
				if (sEscapedItemDelimiterString == null) sEscapedItemDelimiterString = value;
			}
		}

        private static readonly string[] defaultEscapeStrings = GetEscapeStrings();

		[ThreadStatic]
		private static string[] tsEscapeStrings;
		private static string[] sEscapeStrings;
		public static string[] EscapeStrings
		{
			get
			{
				return tsEscapeStrings ?? sEscapeStrings ?? defaultEscapeStrings;
			}
            private set
            {
				tsEscapeStrings = value;
				if (sEscapeStrings == null) sEscapeStrings = value;
            }
		}

        private static string[] GetEscapeStrings()
        {
            return new[] {ItemDelimiterString, ItemSeperatorString, RowSeparatorString, "\r", "\n"};
        }

        private static void ResetEscapeStrings()
        {
            EscapeStrings = GetEscapeStrings();
        }

		[ThreadStatic]
		private static string tsRowSeparatorString;
		private static string sRowSeparatorString;
		public static string RowSeparatorString
		{
			get
			{
			    return tsRowSeparatorString ?? sRowSeparatorString ?? Environment.NewLine;
			}
		    set
			{
				tsRowSeparatorString = value;
				if (sRowSeparatorString == null) sRowSeparatorString = value;
                ResetEscapeStrings();
			}
		}
       
		public static void Reset()
		{
			tsItemSeperatorString = sItemSeperatorString = null;
			tsItemDelimiterString = sItemDelimiterString = null;
			tsEscapedItemDelimiterString = sEscapedItemDelimiterString = null;
			tsRowSeparatorString = sRowSeparatorString = null;
		    tsEscapeStrings = sEscapeStrings = null;
		}

    }

	public static class CsvConfig<T>
	{
		public static bool OmitHeaders { get; set; }

		private static Dictionary<string, string> customHeadersMap;
		public static Dictionary<string, string> CustomHeadersMap
		{
			get
			{
				return customHeadersMap;
			}
			set
			{
				customHeadersMap = value;
				if (value == null) return;
				CsvWriter<T>.ConfigureCustomHeaders(customHeadersMap);
			}
		}

		public static object CustomHeaders
		{
			set
			{
				if (value == null) return;
				if (value.GetType().IsValueType)
					throw new ArgumentException("CustomHeaders is a ValueType");

				var propertyInfos = value.GetType().GetProperties();
				if (propertyInfos.Length == 0) return;

				customHeadersMap = new Dictionary<string, string>();
				foreach (var pi in propertyInfos)
				{
					var getMethod = pi.GetGetMethod();
					if (getMethod == null) continue;

					var oValue = getMethod.Invoke(value, new object[0]);
					if (oValue == null) continue;
					customHeadersMap[pi.Name] = oValue.ToString();
				}
				CsvWriter<T>.ConfigureCustomHeaders(customHeadersMap);
			}
		}

		public static void Reset()
		{
			OmitHeaders = false;
			CsvWriter<T>.Reset();
		}
	}
}
#endregion CsvConfig.cs

/// ********   File: \CsvSerializer.cs
#region CsvSerializer.cs

namespace ServiceStack.Text
{
	public class CsvSerializer
	{
		private static readonly UTF8Encoding UTF8EncodingWithoutBom = new UTF8Encoding(false);

		private static Dictionary<Type, WriteObjectDelegate> WriteFnCache = new Dictionary<Type, WriteObjectDelegate>();

		internal static WriteObjectDelegate GetWriteFn(Type type)
		{
			try
			{
				WriteObjectDelegate writeFn;
                if (WriteFnCache.TryGetValue(type, out writeFn)) return writeFn;

                var genericType = typeof(CsvSerializer<>).MakeGenericType(type);
                var mi = genericType.GetMethod("WriteFn", BindingFlags.Public | BindingFlags.Static);
                var writeFactoryFn = (Func<WriteObjectDelegate>)Delegate.CreateDelegate(
                    typeof(Func<WriteObjectDelegate>), mi);
                writeFn = writeFactoryFn();

                Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
                do
                {
                    snapshot = WriteFnCache;
                    newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache);
                    newCache[type] = writeFn;

                } while (!ReferenceEquals(
                    Interlocked.CompareExchange(ref WriteFnCache, newCache, snapshot), snapshot));
                
                return writeFn;
			}
			catch (Exception ex)
			{
				Tracer.Instance.WriteError(ex);
				throw;
			}
		}

		public static string SerializeToCsv<T>(IEnumerable<T> records)
		{
			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				writer.WriteCsv(records);
				return sb.ToString();
			}
		}

		public static string SerializeToString<T>(T value)
		{
			if (value == null) return null;
			if (typeof(T) == typeof(string)) return value as string;

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				CsvSerializer<T>.WriteObject(writer, value);
			}
			return sb.ToString();
		}

		public static void SerializeToWriter<T>(T value, TextWriter writer)
		{
			if (value == null) return;
			if (typeof(T) == typeof(string))
			{
				writer.Write(value);
				return;
			}
			CsvSerializer<T>.WriteObject(writer, value);
		}

		public static void SerializeToStream<T>(T value, Stream stream)
		{
			if (value == null) return;
		    var writer = new StreamWriter(stream, UTF8EncodingWithoutBom);
			CsvSerializer<T>.WriteObject(writer, value);
            writer.Flush();
		}

		public static void SerializeToStream(object obj, Stream stream)
		{
			if (obj == null) return;
		    var writer = new StreamWriter(stream, UTF8EncodingWithoutBom);
            var writeFn = GetWriteFn(obj.GetType());
            writeFn(writer, obj);
            writer.Flush();
        }

		public static T DeserializeFromStream<T>(Stream stream)
		{
            throw new NotImplementedException();
		}

		public static object DeserializeFromStream(Type type, Stream stream)
		{
            throw new NotImplementedException();
		}

		public static void WriteLateBoundObject(TextWriter writer, object value)
		{
			if (value == null) return;
			var writeFn = GetWriteFn(value.GetType());
			writeFn(writer, value);
		}
	}

	internal static class CsvSerializer<T>
	{
		private static readonly WriteObjectDelegate CacheFn;

		public static WriteObjectDelegate WriteFn()
		{
			return CacheFn;
		}

		private const string IgnoreResponseStatus = "ResponseStatus";

		private static Func<object, object> valueGetter = null;
		private static WriteObjectDelegate writeElementFn = null;

		private static WriteObjectDelegate GetWriteFn()
		{
			PropertyInfo firstCandidate = null;
			Type bestCandidateEnumerableType = null;
			PropertyInfo bestCandidate = null;

			if (typeof(T).IsValueType)
			{
				return JsvWriter<T>.WriteObject;
			}

			//If type is an enumerable property itself write that
			bestCandidateEnumerableType = typeof(T).GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));
			if (bestCandidateEnumerableType != null)
			{
				var elementType = bestCandidateEnumerableType.GetGenericArguments()[0];
				writeElementFn = CreateWriteFn(elementType);

				return WriteEnumerableType;
			}

			//Look for best candidate property if DTO
			if (typeof(T).IsDto() || typeof(T).HasAttr<CsvAttribute>())
			{
				var properties = TypeConfig<T>.Properties;
				foreach (var propertyInfo in properties)
				{
					if (propertyInfo.Name == IgnoreResponseStatus) continue;

					if (propertyInfo.PropertyType == typeof(string)
						|| propertyInfo.PropertyType.IsValueType
						|| propertyInfo.PropertyType == typeof(byte[])) continue;

					if (firstCandidate == null)
					{
						firstCandidate = propertyInfo;
					}

					var enumProperty = propertyInfo.PropertyType
						.GetTypeWithGenericTypeDefinitionOf(typeof(IEnumerable<>));

					if (enumProperty != null)
					{
						bestCandidateEnumerableType = enumProperty;
						bestCandidate = propertyInfo;
						break;
					}
				}
			}

			//If is not DTO or no candidates exist, write self
			var noCandidatesExist = bestCandidate == null && firstCandidate == null;
			if (noCandidatesExist)
			{
				return WriteSelf;
			}

			//If is DTO and has an enumerable property serialize that
			if (bestCandidateEnumerableType != null)
			{
				valueGetter = bestCandidate.GetValueGetter(typeof(T));

				var elementType = bestCandidateEnumerableType.GetGenericArguments()[0];
				writeElementFn = CreateWriteFn(elementType);

				return WriteEnumerableProperty;
			}

			//If is DTO and has non-enumerable, reference type property serialize that
			valueGetter = firstCandidate.GetValueGetter(typeof(T));
			writeElementFn = CreateWriteRowFn(firstCandidate.PropertyType);

			return WriteNonEnumerableType;
		}

		private static WriteObjectDelegate CreateWriteFn(Type elementType)
		{
			return CreateCsvWriterFn(elementType, "WriteObject");
		}

		private static WriteObjectDelegate CreateWriteRowFn(Type elementType)
		{
			return CreateCsvWriterFn(elementType, "WriteObjectRow");
		}

		private static WriteObjectDelegate CreateCsvWriterFn(Type elementType, string methodName)
		{
			var genericType = typeof(CsvWriter<>).MakeGenericType(elementType);
			var mi = genericType.GetMethod(methodName, 
				BindingFlags.Static | BindingFlags.Public);

			var writeFn = (WriteObjectDelegate)Delegate.CreateDelegate(typeof(WriteObjectDelegate), mi);

			return writeFn;
		}

		public static void WriteEnumerableType(TextWriter writer, object obj)
		{
			writeElementFn(writer, obj);
		}

		public static void WriteSelf(TextWriter writer, object obj)
		{
			CsvWriter<T>.WriteRow(writer, (T)obj);
		}

		public static void WriteEnumerableProperty(TextWriter writer, object obj)
		{
			if (obj == null) return; //AOT

			var enumerableProperty = valueGetter(obj);
			writeElementFn(writer, enumerableProperty);
		}

		public static void WriteNonEnumerableType(TextWriter writer, object obj)
		{
			var nonEnumerableType = valueGetter(obj);
			writeElementFn(writer, nonEnumerableType);
		}

		static CsvSerializer()
		{
			if (typeof(T) == typeof(object))
			{
				CacheFn = CsvSerializer.WriteLateBoundObject;
			}
			else
			{
				CacheFn = GetWriteFn();
			}
		}

		public static void WriteObject(TextWriter writer, object value)
		{
			CacheFn(writer, value);
		}
	}
}
#endregion CsvSerializer.cs

/// ********   File: \CsvStreamExtensions.cs
#region CsvStreamExtensions.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public static class CsvStreamExtensions
	{
		public static void WriteCsv<T>(this Stream outputStream, IEnumerable<T> records)
		{
			using (var textWriter = new StreamWriter(outputStream))
			{
				textWriter.WriteCsv(records);
			}
		}

		public static void WriteCsv<T>(this TextWriter writer, IEnumerable<T> records)
		{
			CsvWriter<T>.Write(writer, records);
		}

	}
}
#endregion CsvStreamExtensions.cs

/// ********   File: \CsvWriter.cs
#region CsvWriter.cs
#if WINDOWS_PHONE
#endif

namespace ServiceStack.Text
{
    internal class CsvDictionaryWriter
    {
		public static void WriteRow(TextWriter writer, IEnumerable<string> row)
		{
			var ranOnce = false;
			foreach (var field in row)
			{
				CsvWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

				writer.Write(field.ToCsvField());
			}
			writer.Write(CsvConfig.RowSeparatorString);
		}

		public static void Write(TextWriter writer, IEnumerable<Dictionary<string, string>> records)
		{
			if (records == null) return; //AOT

            var allKeys = new ServiceStack.Text.WP.HashSet<string>();
		    var cachedRecords = new List<IDictionary<string, string>>();

			foreach (var record in records) {
                foreach (var key in record.Keys) {
                    if (!allKeys.Contains(key)) {
                        allKeys.Add(key);
                    }
                }
                cachedRecords.Add(record);
			}

		    var headers = allKeys.OrderBy(key => key).ToList();
            if (!CsvConfig<Dictionary<string, string>>.OmitHeaders) {
                WriteRow(writer, headers);
            }
		    foreach (var cachedRecord in cachedRecords) {
                var fullRecord = new List<string>();
                foreach (var header in headers) {
                    fullRecord.Add(cachedRecord.ContainsKey(header)
                                        ? cachedRecord[header]
                                        : null);
                }
                WriteRow(writer, fullRecord);
		    }
		}
    }

    public static class CsvWriter
    {
        public static bool HasAnyEscapeChars(string value)
        {
            return CsvConfig.EscapeStrings.Any(value.Contains);
        }

        internal static void WriteItemSeperatorIfRanOnce(TextWriter writer, ref bool ranOnce)
        {
            if (ranOnce)
                writer.Write(CsvConfig.ItemSeperatorString);
            else
                ranOnce = true;
        }
    }

	internal class CsvWriter<T>
	{
		public const char DelimiterChar = ',';

		public static List<string> Headers { get; set; }

		internal static List<Func<T, object>> PropertyGetters;

		private static readonly WriteObjectDelegate OptimizedWriter;

		static CsvWriter()
		{
			if (typeof(T) == typeof(string))
			{
				OptimizedWriter = (w, o) => WriteRow(w, (IEnumerable<string>)o);
				return;
			}

			Reset();
		}

		internal static void Reset()
		{
			Headers = new List<string>();

			PropertyGetters = new List<Func<T, object>>();
		    var isDataContract = typeof(T).IsDto();
			foreach (var propertyInfo in TypeConfig<T>.Properties)
			{
				if (!propertyInfo.CanRead || propertyInfo.GetGetMethod() == null) continue;
				if (!TypeSerializer.CanCreateFromString(propertyInfo.PropertyType)) continue;

				PropertyGetters.Add(propertyInfo.GetValueGetter<T>());
                var propertyName = propertyInfo.Name;
                if (isDataContract)
                {
                    var dcsDataMember = propertyInfo.GetDataMember();
                    if (dcsDataMember != null && dcsDataMember.Name != null)
                    {
                        propertyName = dcsDataMember.Name;
                    }
                }
                Headers.Add(propertyName);
			}
		}

		internal static void ConfigureCustomHeaders(Dictionary<string, string> customHeadersMap)
		{
			Reset();

			for (var i = Headers.Count - 1; i >= 0; i--)
			{
				var oldHeader = Headers[i];
				string newHeaderValue;
				if (!customHeadersMap.TryGetValue(oldHeader, out newHeaderValue))
				{
					Headers.RemoveAt(i);
					PropertyGetters.RemoveAt(i);
				}
				else
				{
					Headers[i] = newHeaderValue.EncodeJsv();
				}
			}
		}

		private static List<string> GetSingleRow(IEnumerable<T> records, Type recordType)
		{
			var row = new List<string>();
			foreach (var value in records)
			{
				var strValue = recordType == typeof(string)
				   ? value as string
				   : TypeSerializer.SerializeToString(value);

				row.Add(strValue);
			}
			return row;
		}

		public static List<List<string>> GetRows(IEnumerable<T> records)
		{
			var rows = new List<List<string>>();

			if (records == null) return rows;

			if (typeof(T).IsValueType || typeof(T) == typeof(string))
			{
				rows.Add(GetSingleRow(records, typeof(T)));
				return rows;
			}

			foreach (var record in records)
			{
				var row = new List<string>();
				foreach (var propertyGetter in PropertyGetters)
				{
					var value = propertyGetter(record) ?? "";

					var strValue = value.GetType() == typeof(string)
						? (string)value
						: TypeSerializer.SerializeToString(value);

					row.Add(strValue);
				}
				rows.Add(row);
			}

			return rows;
		}

		public static void WriteObject(TextWriter writer, object records)
		{
			Write(writer, (IEnumerable<T>)records);
		}

		public static void WriteObjectRow(TextWriter writer, object record)
		{
			WriteRow(writer, (T)record);
		}

		public static void Write(TextWriter writer, IEnumerable<T> records)
		{
			if (writer == null) return; //AOT

            if (typeof (T) == typeof(Dictionary<string, string>)) {
                CsvDictionaryWriter.Write(writer, (IEnumerable<Dictionary<string, string>>)records);
                return;
            }

			if (OptimizedWriter != null)
			{
				OptimizedWriter(writer, records);
				return;
			}

			if (!CsvConfig<T>.OmitHeaders && Headers.Count > 0)
			{
				var ranOnce = false;
				foreach (var header in Headers)
				{
					CsvWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

					writer.Write(header);
				}
				writer.Write(CsvConfig.RowSeparatorString);
			}

			if (records == null) return;

			if (typeof(T).IsValueType || typeof(T) == typeof(string))
			{
				var singleRow = GetSingleRow(records, typeof(T));
				WriteRow(writer, singleRow);
				return;
			}

			var row = new string[Headers.Count];
			foreach (var record in records)
			{
				for (var i = 0; i < PropertyGetters.Count; i++)
				{
					var propertyGetter = PropertyGetters[i];
					var value = propertyGetter(record) ?? "";

					var strValue = value.GetType() == typeof(string)
					   ? (string)value
					   : TypeSerializer.SerializeToString(value);

					row[i] = strValue;
				}
				WriteRow(writer, row);
			}
		}

		public static void WriteRow(TextWriter writer, T row)
		{
			if (writer == null) return; //AOT

			Write(writer, new[] { row });
		}

		public static void WriteRow(TextWriter writer, IEnumerable<string> row)
		{
			var ranOnce = false;
			foreach (var field in row)
			{
				CsvWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

				writer.Write(field.ToCsvField());
			}
			writer.Write(CsvConfig.RowSeparatorString);
		}

		public static void Write(TextWriter writer, IEnumerable<List<string>> rows)
		{
			if (Headers.Count > 0)
			{
				var ranOnce = false;
				foreach (var header in Headers)
				{
					CsvWriter.WriteItemSeperatorIfRanOnce(writer, ref ranOnce);

					writer.Write(header);
				}
				writer.Write(CsvConfig.RowSeparatorString);
			}

			foreach (var row in rows)
			{
				WriteRow(writer, row);
			}
		}
	}

}
#endregion CsvWriter.cs

/// ********   File: \DateTimeExtensions.cs
#region DateTimeExtensions.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	/// <summary>
	/// A fast, standards-based, serialization-issue free DateTime serailizer.
	/// </summary>
	public static class DateTimeExtensions
	{
		public const long UnixEpoch = 621355968000000000L;
		private static readonly DateTime UnixEpochDateTimeUtc = new DateTime(UnixEpoch, DateTimeKind.Utc);
		private static readonly DateTime UnixEpochDateTimeUnspecified = new DateTime(UnixEpoch, DateTimeKind.Unspecified);

		public static long ToUnixTime(this DateTime dateTime)
		{
			return (dateTime.ToStableUniversalTime().Ticks - UnixEpoch) / TimeSpan.TicksPerSecond;
		}

		public static DateTime FromUnixTime(this double unixTime)
		{
			return UnixEpochDateTimeUtc + TimeSpan.FromSeconds(unixTime);
		}

		public static long ToUnixTimeMs(this DateTime dateTime)
		{
			return (dateTime.ToStableUniversalTime().Ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
		}

        public static long ToUnixTimeMs(this long ticks)
        {
            return (ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
        }

		public static DateTime FromUnixTimeMs(this double msSince1970)
		{
			return UnixEpochDateTimeUtc + TimeSpan.FromMilliseconds(msSince1970);
		}

		public static DateTime FromUnixTimeMs(this long msSince1970)
		{
			return UnixEpochDateTimeUtc + TimeSpan.FromMilliseconds(msSince1970);
		}

		public static DateTime FromUnixTimeMs(this long msSince1970, TimeSpan offset)
		{
			return UnixEpochDateTimeUnspecified + TimeSpan.FromMilliseconds(msSince1970) + offset;
		}

		public static DateTime FromUnixTimeMs(this double msSince1970, TimeSpan offset)
		{
			return UnixEpochDateTimeUnspecified + TimeSpan.FromMilliseconds(msSince1970) + offset;
		}

		public static DateTime FromUnixTimeMs(string msSince1970)
		{
			long ms;
			if (long.TryParse(msSince1970, out ms)) return ms.FromUnixTimeMs();

			// Do we really need to support fractional unix time ms time strings??
			return double.Parse(msSince1970).FromUnixTimeMs();
		}

		public static DateTime FromUnixTimeMs(string msSince1970, TimeSpan offset)
		{
			long ms;
			if (long.TryParse(msSince1970, out ms)) return ms.FromUnixTimeMs(offset);

			// Do we really need to support fractional unix time ms time strings??
			return double.Parse(msSince1970).FromUnixTimeMs(offset);
		}

		public static DateTime RoundToMs(this DateTime dateTime)
		{
			return new DateTime((dateTime.Ticks / TimeSpan.TicksPerMillisecond) * TimeSpan.TicksPerMillisecond);
		}

		public static DateTime RoundToSecond(this DateTime dateTime)
		{
			return new DateTime((dateTime.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);
		}

		public static string ToShortestXsdDateTimeString(this DateTime dateTime)
		{
			return DateTimeSerializer.ToShortestXsdDateTimeString(dateTime);
		}

		public static DateTime FromShortestXsdDateTimeString(this string xsdDateTime)
		{
			return DateTimeSerializer.ParseShortestXsdDateTime(xsdDateTime);
		}

		public static bool IsEqualToTheSecond(this DateTime dateTime, DateTime otherDateTime)
		{
			return dateTime.ToStableUniversalTime().RoundToSecond().Equals(otherDateTime.ToStableUniversalTime().RoundToSecond());
		}

		public static string ToTimeOffsetString(this TimeSpan offset, bool includeColon = false)
		{
			var sign = offset < TimeSpan.Zero ? "-" : "+";
			var hours = Math.Abs(offset.Hours);
			var minutes = Math.Abs(offset.Minutes);
			var separator = includeColon ? ":" : "";
			return string.Format("{0}{1:00}{2}{3:00}", sign, hours, separator, minutes);
		}

		public static TimeSpan FromTimeOffsetString(this string offsetString)
		{
			if (!offsetString.Contains(":"))
				offsetString = offsetString.Insert(offsetString.Length - 2, ":");

			offsetString = offsetString.TrimStart('+');

			return TimeSpan.Parse(offsetString);
		}

		public static DateTime ToStableUniversalTime(this DateTime dateTime)
		{
#if SILVERLIGHT
			// Silverlight 3, 4 and 5 all work ok with DateTime.ToUniversalTime, but have no TimeZoneInfo.ConverTimeToUtc implementation.
			return dateTime.ToUniversalTime();
#else
			// .Net 2.0 - 3.5 has an issue with DateTime.ToUniversalTime, but works ok with TimeZoneInfo.ConvertTimeToUtc.
			// .Net 4.0+ does this under the hood anyway.
			return TimeZoneInfo.ConvertTimeToUtc(dateTime);
#endif
		}

        public static string FmtSortableDate(this DateTime from)
        {
            return from.ToString("yyyy-MM-dd");
        }

        public static string FmtSortableDateTime(this DateTime from)
        {
            return from.ToString("u");
        }

        public static DateTime LastMonday(this DateTime from)
        {
            var modayOfWeekBefore = from.Date.AddDays(-(int)from.DayOfWeek - 6);
            return modayOfWeekBefore;
        }

        public static DateTime StartOfLastMonth(this DateTime from)
        {
            return new DateTime(from.Date.Year, from.Date.Month, 1).AddMonths(-1);
        }

        public static DateTime EndOfLastMonth(this DateTime from)
        {
            return new DateTime(from.Date.Year, from.Date.Month, 1).AddDays(-1); 
        }
    }
}
#endregion DateTimeExtensions.cs

/// ********   File: \DynamicJson.cs
#region DynamicJson.cs
#if NET40

namespace ServiceStack.Text
{
    public class DynamicJson : DynamicObject
    {
        private readonly IDictionary<string, object> _hash = new Dictionary<string, object>();

        public static string Serialize(dynamic instance)
        {
            var json = JsonSerializer.SerializeToString(instance);
            return json;
        }

        public static dynamic Deserialize(string json)
        {
            // Support arbitrary nesting by using JsonObject
            var deserialized = JsonSerializer.DeserializeFromString<JsonObject>(json);
            var hash = deserialized.ToDictionary<KeyValuePair<string, string>, string, object>(entry => entry.Key, entry => entry.Value);
            return new DynamicJson(hash);
        }

        public DynamicJson(IEnumerable<KeyValuePair<string, object>> hash)
        {
            _hash.Clear();
            foreach (var entry in hash)
            {
                _hash.Add(Underscored(entry.Key), entry.Value);
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var name = Underscored(binder.Name);
            _hash[name] = value;
            return _hash[name] == value;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = Underscored(binder.Name);
            return YieldMember(name, out result);
        }

        public override string ToString()
        {
            return JsonSerializer.SerializeToString(_hash);
        }

        private bool YieldMember(string name, out object result)
        {
            if (_hash.ContainsKey(name))
            {
                var json = _hash[name].ToString();
                if (json.TrimStart(' ').StartsWith("{"))
                {
                    result = Deserialize(json);
                    return true;
                }
                result = json;
                return _hash[name] == result;
            }
            result = null;
            return false;
        }

        internal static string Underscored(IEnumerable<char> pascalCase)
        {
            var sb = new StringBuilder();
            var i = 0;
            foreach (var c in pascalCase)
            {
                if (char.IsUpper(c) && i > 0)
                {
                    sb.Append("_");
                }
                sb.Append(c);
                i++;
            }
            return sb.ToString().ToLowerInvariant();
        }
    }
}
#endif

#endregion DynamicJson.cs

/// ********   File: \DynamicProxy.cs
#region DynamicProxy.cs
#if !SILVERLIGHT && !MONOTOUCH

namespace ServiceStack.Text {
	public static class DynamicProxy {
		public static T GetInstanceFor<T> () {
			return (T)GetInstanceFor(typeof(T));
		}

		static readonly ModuleBuilder ModuleBuilder;
		static readonly AssemblyBuilder DynamicAssembly;

		public static object GetInstanceFor (Type targetType) {
			lock (DynamicAssembly)
			{
				var constructedType = DynamicAssembly.GetType(ProxyName(targetType)) ?? GetConstructedType(targetType);
				var instance = Activator.CreateInstance(constructedType);
				return instance;
			}
		}

		static string ProxyName(Type targetType)
		{
			return targetType.Name + "Proxy";
		}

		static DynamicProxy () {
			var assemblyName = new AssemblyName("DynImpl");
			DynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder = DynamicAssembly.DefineDynamicModule("DynImplModule");
		}

		static Type GetConstructedType (Type targetType) {
			var typeBuilder = ModuleBuilder.DefineType(targetType.Name + "Proxy", TypeAttributes.Public);

			var ctorBuilder = typeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				new Type[] { });
			var ilGenerator = ctorBuilder.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ret);

			IncludeType(targetType, typeBuilder);

			foreach (var face in targetType.GetInterfaces())
				IncludeType(face, typeBuilder);

			return typeBuilder.CreateType();
		}

		static void IncludeType (Type typeOfT, TypeBuilder typeBuilder) {
			var methodInfos = typeOfT.GetMethods();
			foreach (var methodInfo in methodInfos) {
				if (methodInfo.Name.StartsWith("set_")) continue; // we always add a set for a get.

				if (methodInfo.Name.StartsWith("get_")) {
					BindProperty(typeBuilder, methodInfo);
				} else {
					BindMethod(typeBuilder, methodInfo);
				}
			}

			typeBuilder.AddInterfaceImplementation(typeOfT);
		}

		static void BindMethod (TypeBuilder typeBuilder, MethodInfo methodInfo) {
			var methodBuilder = typeBuilder.DefineMethod(
				methodInfo.Name,
				MethodAttributes.Public | MethodAttributes.Virtual,
				methodInfo.ReturnType,
				methodInfo.GetParameters().Select(p => p.GetType()).ToArray()
				);
			var methodILGen = methodBuilder.GetILGenerator();
			if (methodInfo.ReturnType == typeof(void)) {
				methodILGen.Emit(OpCodes.Ret);
			} else {
				if (methodInfo.ReturnType.IsValueType || methodInfo.ReturnType.IsEnum) {
					MethodInfo getMethod = typeof(Activator).GetMethod("CreateInstance",
																	   new[] { typeof(Type) });
					LocalBuilder lb = methodILGen.DeclareLocal(methodInfo.ReturnType);
					methodILGen.Emit(OpCodes.Ldtoken, lb.LocalType);
					methodILGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
					methodILGen.Emit(OpCodes.Callvirt, getMethod);
					methodILGen.Emit(OpCodes.Unbox_Any, lb.LocalType);
				} else {
					methodILGen.Emit(OpCodes.Ldnull);
				}
				methodILGen.Emit(OpCodes.Ret);
			}
			typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
		}

		public static void BindProperty (TypeBuilder typeBuilder, MethodInfo methodInfo) {
			// Backing Field
			string propertyName = methodInfo.Name.Replace("get_", "");
			Type propertyType = methodInfo.ReturnType;
			FieldBuilder backingField = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

			//Getter
			MethodBuilder backingGet = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public |
				MethodAttributes.SpecialName | MethodAttributes.Virtual |
				MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
			ILGenerator getIl = backingGet.GetILGenerator();

			getIl.Emit(OpCodes.Ldarg_0);
			getIl.Emit(OpCodes.Ldfld, backingField);
			getIl.Emit(OpCodes.Ret);

			//Setter
			MethodBuilder backingSet = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public |
				MethodAttributes.SpecialName | MethodAttributes.Virtual |
				MethodAttributes.HideBySig, null, new[] { propertyType });

			ILGenerator setIl = backingSet.GetILGenerator();

			setIl.Emit(OpCodes.Ldarg_0);
			setIl.Emit(OpCodes.Ldarg_1);
			setIl.Emit(OpCodes.Stfld, backingField);
			setIl.Emit(OpCodes.Ret);

			// Property
			PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
			propertyBuilder.SetGetMethod(backingGet);
			propertyBuilder.SetSetMethod(backingSet);
		}
	}
}
#endif

#endregion DynamicProxy.cs

/// ********   File: \Env.cs
#region Env.cs

namespace ServiceStack.Text
{
	public static class Env
	{
		static Env()
		{
			var platform = (int)Environment.OSVersion.Platform;
			IsUnix = (platform == 4) || (platform == 6) || (platform == 128);

			IsMono = Type.GetType("Mono.Runtime") != null;

			IsMonoTouch = Type.GetType("MonoTouch.Foundation.NSObject") != null;

			SupportsExpressions = SupportsEmit = !IsMonoTouch;

			ServerUserAgent = "ServiceStack/" +
				ServiceStackVersion + " "
				+ Environment.OSVersion.Platform
				+ (IsMono ? "/Mono" : "/.NET")
				+ (IsMonoTouch ? " MonoTouch" : "");
		}

		public static decimal ServiceStackVersion = 3.928m;

		public static bool IsUnix { get; set; }

		public static bool IsMono { get; set; }

		public static bool IsMonoTouch { get; set; }

		public static bool SupportsExpressions { get; set; }

		public static bool SupportsEmit { get; set; }

		public static string ServerUserAgent { get; set; }
	}
}
#endregion Env.cs

/// ********   File: \HashSet.cs
#region HashSet.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//	 Mijail Cisneros (cisneros@mijail.ru)
//
// Copyright 2012 Liquidbit Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text.WP
{
    ///<summary>
    /// A hashset implementation that uses an IDictionary
    ///</summary>
    public class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private readonly Dictionary<T, short> _dict;

        public HashSet()
        {
            _dict = new Dictionary<T, short>();
        }

        public HashSet(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            _dict =  new Dictionary<T, short>(collection.Count());
            foreach (T item in collection)
                Add(item);
        }

        public void Add(T item)
        {
            _dict.Add(item, 0);
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Contains(T item)
        {
            return _dict.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _dict.Keys.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _dict.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        public int Count
        {
            get { return _dict.Keys.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}

#endregion HashSet.cs

/// ********   File: \ITracer.cs
#region ITracer.cs

namespace ServiceStack.Text
{
	public interface ITracer
	{
        void WriteDebug(string error);
        void WriteDebug(string format, params object[] args);
        void WriteWarning(string warning);
        void WriteWarning(string format, params object[] args);
		
		void WriteError(Exception ex);
		void WriteError(string error);
		void WriteError(string format, params object[] args);
	}
}
#endregion ITracer.cs

/// ********   File: \ITypeSerializer.Generic.cs
#region ITypeSerializer.Generic.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public interface ITypeSerializer<T>
	{
		/// <summary>
		/// Determines whether this serializer can create the specified type from a string.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can create from string] the specified type; otherwise, <c>false</c>.
		/// </returns>
		bool CanCreateFromString(Type type);

		/// <summary>
		/// Parses the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		T DeserializeFromString(string value);

		/// <summary>
		/// Deserializes from reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		T DeserializeFromReader(TextReader reader);

		/// <summary>
		/// Serializes to string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		string SerializeToString(T value);

		/// <summary>
		/// Serializes to writer.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="writer">The writer.</param>
		void SerializeToWriter(T value, TextWriter writer);
	}
}
#endregion ITypeSerializer.Generic.cs

/// ********   File: \JsConfig.cs
#region JsConfig.cs

#if WINDOWS_PHONE
#endif

namespace ServiceStack.Text
{
    public static class
        JsConfig
    {
        static JsConfig()
        {
            //In-built default serialization, to Deserialize Color struct do:
            //JsConfig<System.Drawing.Color>.SerializeFn = c => c.ToString().Replace("Color ", "").Replace("[", "").Replace("]", "");
            //JsConfig<System.Drawing.Color>.DeSerializeFn = System.Drawing.Color.FromName;
            Reset();
        }

        [ThreadStatic]
        private static bool? tsConvertObjectTypesIntoStringDictionary;
        private static bool? sConvertObjectTypesIntoStringDictionary;
        public static bool ConvertObjectTypesIntoStringDictionary
        {
            get
            {
                return tsConvertObjectTypesIntoStringDictionary ?? sConvertObjectTypesIntoStringDictionary ?? false;
            }
            set
            {
                tsConvertObjectTypesIntoStringDictionary = value;
                if (!sConvertObjectTypesIntoStringDictionary.HasValue) sConvertObjectTypesIntoStringDictionary = value;
            }
        }

        [ThreadStatic]
        private static bool? tsTryToParsePrimitiveTypeValues;
        private static bool? sTryToParsePrimitiveTypeValues;
        public static bool TryToParsePrimitiveTypeValues
        {
            get
            {
                return tsTryToParsePrimitiveTypeValues ?? sTryToParsePrimitiveTypeValues ?? false;
            }
            set
            {
                tsTryToParsePrimitiveTypeValues = value;
                if (!sTryToParsePrimitiveTypeValues.HasValue) sTryToParsePrimitiveTypeValues = value;
            }
        }

        [ThreadStatic]
        private static bool? tsIncludeNullValues;
        private static bool? sIncludeNullValues;
        public static bool IncludeNullValues
        {
            get
            {
                return tsIncludeNullValues ?? sIncludeNullValues ?? false;
            }
            set
            {
                tsIncludeNullValues = value;
                if (!sIncludeNullValues.HasValue) sIncludeNullValues = value;
            }
        }

        [ThreadStatic]
        private static bool? tsTreatEnumAsInteger;
        private static bool? sTreatEnumAsInteger;
        public static bool TreatEnumAsInteger
        {
            get
            {
                return tsTreatEnumAsInteger ?? sTreatEnumAsInteger ?? false;
            }
            set
            {
                tsTreatEnumAsInteger = value;
                if (!sTreatEnumAsInteger.HasValue) sTreatEnumAsInteger = value;
            }
        }

        [ThreadStatic]
        private static bool? tsExcludeTypeInfo;
        private static bool? sExcludeTypeInfo;
        public static bool ExcludeTypeInfo
        {
            get
            {
                return tsExcludeTypeInfo ?? sExcludeTypeInfo ?? false;
            }
            set
            {
                tsExcludeTypeInfo = value;
                if (!sExcludeTypeInfo.HasValue) sExcludeTypeInfo = value;
            }
        }

        [ThreadStatic]
        private static bool? tsForceTypeInfo;
        private static bool? sForceTypeInfo;
        public static bool IncludeTypeInfo
        {
            get
            {
                return tsForceTypeInfo ?? sForceTypeInfo ?? false;
            }
            set
            {
                if (!tsForceTypeInfo.HasValue) tsForceTypeInfo = value;
                if (!sForceTypeInfo.HasValue) sForceTypeInfo = value;
            }
        }

        [ThreadStatic]
        private static string tsTypeAttr;
        private static string sTypeAttr;
        public static string TypeAttr
        {
            get
            {
                return tsTypeAttr ?? sTypeAttr ?? JsWriter.TypeAttr;
            }
            set
            {
                tsTypeAttr = value;
                if (sTypeAttr == null) sTypeAttr = value;
                JsonTypeAttrInObject = JsonTypeSerializer.GetTypeAttrInObject(value);
                JsvTypeAttrInObject = JsvTypeSerializer.GetTypeAttrInObject(value);
            }
        }

        [ThreadStatic]
        private static string tsJsonTypeAttrInObject;
        private static string sJsonTypeAttrInObject;
        private static readonly string defaultJsonTypeAttrInObject = JsonTypeSerializer.GetTypeAttrInObject(TypeAttr);
        internal static string JsonTypeAttrInObject
        {
            get
            {
                return tsJsonTypeAttrInObject ?? sJsonTypeAttrInObject ?? defaultJsonTypeAttrInObject;
            }
            set
            {
                tsJsonTypeAttrInObject = value;
                if (sJsonTypeAttrInObject == null) sJsonTypeAttrInObject = value;
            }
        }

        [ThreadStatic]
        private static string tsJsvTypeAttrInObject;
        private static string sJsvTypeAttrInObject;
        private static readonly string defaultJsvTypeAttrInObject = JsvTypeSerializer.GetTypeAttrInObject(TypeAttr);
        internal static string JsvTypeAttrInObject
        {
            get
            {
                return tsJsvTypeAttrInObject ?? sJsvTypeAttrInObject ?? defaultJsvTypeAttrInObject;
            }
            set
            {
                tsJsvTypeAttrInObject = value;
                if (sJsvTypeAttrInObject == null) sJsvTypeAttrInObject = value;
            }
        }

        [ThreadStatic]
        private static Func<Type, string> tsTypeWriter;
        private static Func<Type, string> sTypeWriter;
        public static Func<Type, string> TypeWriter
        {
            get
            {
                return tsTypeWriter ?? sTypeWriter ?? AssemblyUtils.WriteType;
            }
            set
            {
                tsTypeWriter = value;
                if (sTypeWriter == null) sTypeWriter = value;
            }
        }

        [ThreadStatic]
        private static Func<string, Type> tsTypeFinder;
        private static Func<string, Type> sTypeFinder;
        public static Func<string, Type> TypeFinder
        {
            get
            {
                return tsTypeFinder ?? sTypeFinder ?? AssemblyUtils.FindType;
            }
            set
            {
                tsTypeFinder = value;
                if (sTypeFinder == null) sTypeFinder = value;
            }
        }

        [ThreadStatic]
        private static JsonDateHandler? tsDateHandler;
        private static JsonDateHandler? sDateHandler;
        public static JsonDateHandler DateHandler
        {
            get
            {
                return tsDateHandler ?? sDateHandler ?? JsonDateHandler.TimestampOffset;
            }
            set
            {
                tsDateHandler = value;
                if (!sDateHandler.HasValue) sDateHandler = value;
            }
        }

        /// <summary>
        /// Sets which format to use when serializing TimeSpans
        /// </summary>
        public static JsonTimeSpanHandler TimeSpanHandler { get; set; }

        /// <summary>
        /// <see langword="true"/> if the <see cref="ITypeSerializer"/> is configured
        /// to take advantage of <see cref="CLSCompliantAttribute"/> specification,
        /// to support user-friendly serialized formats, ie emitting camelCasing for JSON
        /// and parsing member names and enum values in a case-insensitive manner.
        /// </summary>
        [ThreadStatic]
        private static bool? tsEmitCamelCaseNames;
        private static bool? sEmitCamelCaseNames;
        public static bool EmitCamelCaseNames
        {
            // obeying the use of ThreadStatic, but allowing for setting JsConfig once as is the normal case
            get
            {
                return tsEmitCamelCaseNames ?? sEmitCamelCaseNames ?? false;
            }
            set
            {
                tsEmitCamelCaseNames = value;
                if (!sEmitCamelCaseNames.HasValue) sEmitCamelCaseNames = value;
            }
        }

        /// <summary>
        /// <see langword="true"/> if the <see cref="ITypeSerializer"/> is configured
        /// to support web-friendly serialized formats, ie emitting lowercase_underscore_casing for JSON
        /// </summary>
        [ThreadStatic]
        private static bool? tsEmitLowercaseUnderscoreNames;
        private static bool? sEmitLowercaseUnderscoreNames;
        public static bool EmitLowercaseUnderscoreNames
        {
            // obeying the use of ThreadStatic, but allowing for setting JsConfig once as is the normal case
            get
            {
                return tsEmitLowercaseUnderscoreNames ?? sEmitLowercaseUnderscoreNames ?? false;
            }
            set
            {
                tsEmitLowercaseUnderscoreNames = value;
                if (!sEmitLowercaseUnderscoreNames.HasValue) sEmitLowercaseUnderscoreNames = value;
            }
        }

        /// <summary>
        /// Define how property names are mapped during deserialization
        /// </summary>
        private static JsonPropertyConvention propertyConvention;
        public static JsonPropertyConvention PropertyConvention
        {
            get { return propertyConvention; }
            set
            {
                propertyConvention = value;
                switch (propertyConvention)
                {
                    case JsonPropertyConvention.ExactMatch:
                        DeserializeTypeRefJson.PropertyNameResolver = DeserializeTypeRefJson.DefaultPropertyNameResolver;
                        break;
                    case JsonPropertyConvention.Lenient:
                        DeserializeTypeRefJson.PropertyNameResolver = DeserializeTypeRefJson.LenientPropertyNameResolver;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the framework should throw serialization exceptions
        /// or continue regardless of deserialization errors. If <see langword="true"/>  the framework
        /// will throw; otherwise, it will parse as many fields as possible. The default is <see langword="false"/>.
        /// </summary>
        [ThreadStatic]
        private static bool? tsThrowOnDeserializationError;
        private static bool? sThrowOnDeserializationError;
        public static bool ThrowOnDeserializationError
        {
            // obeying the use of ThreadStatic, but allowing for setting JsConfig once as is the normal case
            get
            {
                return tsThrowOnDeserializationError ?? sThrowOnDeserializationError ?? false;
            }
            set
            {
                tsThrowOnDeserializationError = value;
                if (!sThrowOnDeserializationError.HasValue) sThrowOnDeserializationError = value;
            }
        }

        internal static ServiceStack.Text.WP.HashSet<Type> HasSerializeFn = new ServiceStack.Text.WP.HashSet<Type>();

        internal static ServiceStack.Text.WP.HashSet<Type> TreatValueAsRefTypes = new ServiceStack.Text.WP.HashSet<Type>();

        [ThreadStatic]
        private static bool? tsPreferInterfaces;
        private static bool? sPreferInterfaces;
        /// <summary>
        /// If set to true, Interface types will be prefered over concrete types when serializing.
        /// </summary>
        public static bool PreferInterfaces
        {
            get
            {
                return tsPreferInterfaces ?? sPreferInterfaces ?? false;
            }
            set
            {
                tsPreferInterfaces = value;
                if (!sPreferInterfaces.HasValue) sPreferInterfaces = value;
            }
        }

        internal static bool TreatAsRefType(Type valueType)
        {
            return TreatValueAsRefTypes.Contains(valueType.IsGenericType ? valueType.GetGenericTypeDefinition() : valueType);
        }

        public static void Reset()
        {
            ModelFactory = ReflectionExtensions.GetConstructorMethodToCache;
            tsTryToParsePrimitiveTypeValues = sTryToParsePrimitiveTypeValues = null;
            tsConvertObjectTypesIntoStringDictionary = sConvertObjectTypesIntoStringDictionary = null;
            tsIncludeNullValues = sIncludeNullValues = null;
            tsExcludeTypeInfo = sExcludeTypeInfo = null;
            tsEmitCamelCaseNames = sEmitCamelCaseNames = null;
            tsEmitLowercaseUnderscoreNames = sEmitLowercaseUnderscoreNames = null;
            tsDateHandler = sDateHandler = null;
            tsPreferInterfaces = sPreferInterfaces = null;
            tsThrowOnDeserializationError = sThrowOnDeserializationError = null;
            tsTypeAttr = sTypeAttr = null;
            tsJsonTypeAttrInObject = sJsonTypeAttrInObject = null;
            tsJsvTypeAttrInObject = sJsvTypeAttrInObject = null;
            tsTypeWriter = sTypeWriter = null;
            tsTypeFinder = sTypeFinder = null;
			tsTreatEnumAsInteger = sTreatEnumAsInteger = null;
            HasSerializeFn = new ServiceStack.Text.WP.HashSet<Type>();
            TreatValueAsRefTypes = new ServiceStack.Text.WP.HashSet<Type> { typeof(KeyValuePair<,>) };
            PropertyConvention = JsonPropertyConvention.ExactMatch;
        }

#if MONOTOUCH
        /// <summary>
        /// Provide hint to MonoTouch AOT compiler to pre-compile generic classes for all your DTOs.
        /// Just needs to be called once in a static constructor.
        /// </summary>
        [MonoTouch.Foundation.Preserve]
		public static void InitForAot() { 
		}

        [MonoTouch.Foundation.Preserve]
        public static void RegisterForAot()
        {
			RegisterTypeForAot<Poco>();

            RegisterElement<Poco, string>();

            RegisterElement<Poco, bool>();
            RegisterElement<Poco, char>();
            RegisterElement<Poco, byte>();
            RegisterElement<Poco, sbyte>();
            RegisterElement<Poco, short>();
            RegisterElement<Poco, ushort>();
            RegisterElement<Poco, int>();
            RegisterElement<Poco, uint>();

			RegisterElement<Poco, long>();
            RegisterElement<Poco, ulong>();
            RegisterElement<Poco, float>();
            RegisterElement<Poco, double>();
            RegisterElement<Poco, decimal>();

            RegisterElement<Poco, bool?>();
            RegisterElement<Poco, char?>();
            RegisterElement<Poco, byte?>();
            RegisterElement<Poco, sbyte?>();
            RegisterElement<Poco, short?>();
            RegisterElement<Poco, ushort?>();
            RegisterElement<Poco, int?>();
            RegisterElement<Poco, uint?>();
            RegisterElement<Poco, long?>();
            RegisterElement<Poco, ulong?>();
            RegisterElement<Poco, float?>();
            RegisterElement<Poco, double?>();
            RegisterElement<Poco, decimal?>();

			//RegisterElement<Poco, JsonValue>();

			RegisterTypeForAot<DayOfWeek>(); // used by DateTime

			// register built in structs
			RegisterTypeForAot<Guid>();
			RegisterTypeForAot<TimeSpan>();
			RegisterTypeForAot<DateTime>();
			RegisterTypeForAot<DateTime?>();
			RegisterTypeForAot<TimeSpan?>();
			RegisterTypeForAot<Guid?>();
        }

		[MonoTouch.Foundation.Preserve]
		public static void RegisterTypeForAot<T>()
		{
			AotConfig.RegisterSerializers<T>();
		}

        [MonoTouch.Foundation.Preserve]
        static void RegisterQueryStringWriter()
        {
            var i = 0;
            if (QueryStringWriter<Poco>.WriteFn() != null) i++;
        }
		        
        [MonoTouch.Foundation.Preserve]
		internal static int RegisterElement<T, TElement>()
        {
			var i = 0;
			i += AotConfig.RegisterSerializers<TElement>();
			AotConfig.RegisterElement<T, TElement, JsonTypeSerializer>();
			AotConfig.RegisterElement<T, TElement, JsvTypeSerializer>();
			return i;
		}

		///<summary>
		/// Class contains Ahead-of-Time (AOT) explicit class declarations which is used only to workaround "-aot-only" exceptions occured on device only. 
		/// </summary>			
		[MonoTouch.Foundation.Preserve(AllMembers=true)]
		internal class AotConfig
		{
			internal static JsReader<JsonTypeSerializer> jsonReader;
			internal static JsWriter<JsonTypeSerializer> jsonWriter;
			internal static JsReader<JsvTypeSerializer> jsvReader;
			internal static JsWriter<JsvTypeSerializer> jsvWriter;
			internal static JsonTypeSerializer jsonSerializer;
			internal static JsvTypeSerializer jsvSerializer;

			static AotConfig()
			{
				jsonSerializer = new JsonTypeSerializer();
				jsvSerializer = new JsvTypeSerializer();
				jsonReader = new JsReader<JsonTypeSerializer>();
				jsonWriter = new JsWriter<JsonTypeSerializer>();
				jsvReader = new JsReader<JsvTypeSerializer>();
				jsvWriter = new JsWriter<JsvTypeSerializer>();
			}

			internal static int RegisterSerializers<T>()
			{
				var i = 0;
				i += Register<T, JsonTypeSerializer>();
				if (jsonSerializer.GetParseFn<T>() != null) i++;
				if (jsonSerializer.GetWriteFn<T>() != null) i++;
				if (jsonReader.GetParseFn<T>() != null) i++;
				if (jsonWriter.GetWriteFn<T>() != null) i++;

				i += Register<T, JsvTypeSerializer>();
				if (jsvSerializer.GetParseFn<T>() != null) i++;
				if (jsvSerializer.GetWriteFn<T>() != null) i++;
				if (jsvReader.GetParseFn<T>() != null) i++;
				if (jsvWriter.GetWriteFn<T>() != null) i++;

				//RegisterCsvSerializer<T>();
				RegisterQueryStringWriter();
				return i;
			}

			internal static void RegisterCsvSerializer<T>()
			{
				CsvSerializer<T>.WriteFn();
				CsvSerializer<T>.WriteObject(null, null);
				CsvWriter<T>.Write(null, default(IEnumerable<T>));
				CsvWriter<T>.WriteRow(null, default(T));
			}

			public static ParseStringDelegate GetParseFn(Type type)
			{
				var parseFn = JsonTypeSerializer.Instance.GetParseFn(type);
				return parseFn;
			}

			internal static int Register<T, TSerializer>() where TSerializer : ITypeSerializer 
			{
				var i = 0;

				if (JsonWriter<T>.WriteFn() != null) i++;
				if (JsonWriter.Instance.GetWriteFn<T>() != null) i++;
				if (JsonReader.Instance.GetParseFn<T>() != null) i++;
				if (JsonReader<T>.Parse(null) != null) i++;
				if (JsonReader<T>.GetParseFn() != null) i++;
				//if (JsWriter.GetTypeSerializer<JsonTypeSerializer>().GetWriteFn<T>() != null) i++;
				if (new List<T>() != null) i++;
				if (new T[0] != null) i++;

				JsConfig<T>.ExcludeTypeInfo = false;
				
				if (JsConfig<T>.OnDeserializedFn != null) i++;
				if (JsConfig<T>.HasDeserializeFn) i++;
				if (JsConfig<T>.SerializeFn != null) i++;
				if (JsConfig<T>.DeSerializeFn != null) i++;
				//JsConfig<T>.SerializeFn = arg => "";
				//JsConfig<T>.DeSerializeFn = arg => default(T);
				if (TypeConfig<T>.Properties != null) i++;

/*
				if (WriteType<T, TSerializer>.Write != null) i++;
				if (WriteType<object, TSerializer>.Write != null) i++;
				
				if (DeserializeBuiltin<T>.Parse != null) i++;
				if (DeserializeArray<T[], TSerializer>.Parse != null) i++;
				DeserializeType<TSerializer>.ExtractType(null);
				DeserializeArrayWithElements<T, TSerializer>.ParseGenericArray(null, null);
				DeserializeCollection<TSerializer>.ParseCollection<T>(null, null, null);
				DeserializeListWithElements<T, TSerializer>.ParseGenericList(null, null, null);

				SpecializedQueueElements<T>.ConvertToQueue(null);
				SpecializedQueueElements<T>.ConvertToStack(null);
*/

				WriteListsOfElements<T, TSerializer>.WriteList(null, null);
				WriteListsOfElements<T, TSerializer>.WriteIList(null, null);
				WriteListsOfElements<T, TSerializer>.WriteEnumerable(null, null);
				WriteListsOfElements<T, TSerializer>.WriteListValueType(null, null);
				WriteListsOfElements<T, TSerializer>.WriteIListValueType(null, null);
				WriteListsOfElements<T, TSerializer>.WriteGenericArrayValueType(null, null);
				WriteListsOfElements<T, TSerializer>.WriteArray(null, null);

				TranslateListWithElements<T>.LateBoundTranslateToGenericICollection(null, null);
				TranslateListWithConvertibleElements<T, T>.LateBoundTranslateToGenericICollection(null, null);
				
				QueryStringWriter<T>.WriteObject(null, null);
				return i;
			}

			internal static void RegisterElement<T, TElement, TSerializer>() where TSerializer : ITypeSerializer
			{
				DeserializeDictionary<TSerializer>.ParseDictionary<T, TElement>(null, null, null, null);
				DeserializeDictionary<TSerializer>.ParseDictionary<TElement, T>(null, null, null, null);
				
				ToStringDictionaryMethods<T, TElement, TSerializer>.WriteIDictionary(null, null, null, null);
				ToStringDictionaryMethods<TElement, T, TSerializer>.WriteIDictionary(null, null, null, null);
				
				// Include List deserialisations from the Register<> method above.  This solves issue where List<Guid> properties on responses deserialise to null.
				// No idea why this is happening because there is no visible exception raised.  Suspect MonoTouch is swallowing an AOT exception somewhere.
				DeserializeArrayWithElements<TElement, TSerializer>.ParseGenericArray(null, null);
				DeserializeListWithElements<TElement, TSerializer>.ParseGenericList(null, null, null);
				
				// Cannot use the line below for some unknown reason - when trying to compile to run on device, mtouch bombs during native code compile.
				// Something about this line or its inner workings is offensive to mtouch. Luckily this was not needed for my List<Guide> issue.
				// DeserializeCollection<JsonTypeSerializer>.ParseCollection<TElement>(null, null, null);
				
				TranslateListWithElements<TElement>.LateBoundTranslateToGenericICollection(null, typeof(List<TElement>));
				TranslateListWithConvertibleElements<TElement, TElement>.LateBoundTranslateToGenericICollection(null, typeof(List<TElement>));
			}
		}

#endif

        /// <summary>
        /// Set this to enable your own type construction provider.
        /// This is helpful for integration with IoC containers where you need to call the container constructor.
        /// Return null if you don't know how to construct the type and the parameterless constructor will be used.
        /// </summary>
        public static EmptyCtorFactoryDelegate ModelFactory { get; set; }
    }

#if MONOTOUCH
    [MonoTouch.Foundation.Preserve(AllMembers=true)]
    internal class Poco
    {
        public string Dummy { get; set; }
    }
#endif

    public class JsConfig<T>
    {
        /// <summary>
        /// Always emit type info for this type.  Takes precedence over ExcludeTypeInfo
        /// </summary>
        public static bool IncludeTypeInfo = false;

        /// <summary>
        /// Never emit type info for this type
        /// </summary>
        public static bool ExcludeTypeInfo = false;

        /// <summary>
        /// <see langword="true"/> if the <see cref="ITypeSerializer"/> is configured
        /// to take advantage of <see cref="CLSCompliantAttribute"/> specification,
        /// to support user-friendly serialized formats, ie emitting camelCasing for JSON
        /// and parsing member names and enum values in a case-insensitive manner.
        /// </summary>
        public static bool EmitCamelCaseNames = false;

        /// <summary>
        /// Define custom serialization fn for BCL Structs
        /// </summary>
        private static Func<T, string> serializeFn;
        public static Func<T, string> SerializeFn
        {
            get { return serializeFn; }
            set
            {
                serializeFn = value;
                if (value != null)
                    JsConfig.HasSerializeFn.Add(typeof(T));
                else
                    JsConfig.HasSerializeFn.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Opt-in flag to set some Value Types to be treated as a Ref Type
        /// </summary>
        public bool TreatValueAsRefTypes
        {
            get { return JsConfig.TreatValueAsRefTypes.Contains(typeof(T)); }
            set
            {
                if (value)
                    JsConfig.TreatValueAsRefTypes.Add(typeof(T));
                else
                    JsConfig.TreatValueAsRefTypes.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Whether there is a fn (raw or otherwise)
        /// </summary>
        public static bool HasSerializeFn
        {
            get { return serializeFn != null || rawSerializeFn != null; }
        }

        /// <summary>
        /// Define custom raw serialization fn
        /// </summary>
        private static Func<T, string> rawSerializeFn;
        public static Func<T, string> RawSerializeFn
        {
            get { return rawSerializeFn; }
            set
            {
                rawSerializeFn = value;
                if (value != null)
                    JsConfig.HasSerializeFn.Add(typeof(T));
                else
                    JsConfig.HasSerializeFn.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Define custom serialization hook
        /// </summary>
        private static Func<T, T> onSerializingFn;
        public static Func<T, T> OnSerializingFn
        {
            get { return onSerializingFn; }
            set { onSerializingFn = value; }
        }

        /// <summary>
        /// Define custom deserialization fn for BCL Structs
        /// </summary>
        public static Func<string, T> DeSerializeFn;

        /// <summary>
        /// Define custom raw deserialization fn for objects
        /// </summary>
        public static Func<string, T> RawDeserializeFn;

        public static bool HasDeserializeFn
        {
            get { return DeSerializeFn != null || RawDeserializeFn != null; }
        }

        private static Func<T, T> onDeserializedFn;
        public static Func<T, T> OnDeserializedFn
        {
            get { return onDeserializedFn; }
            set { onDeserializedFn = value; }
        }

        /// <summary>
        /// Exclude specific properties of this type from being serialized
        /// </summary>
        public static string[] ExcludePropertyNames;

        public static void WriteFn<TSerializer>(TextWriter writer, object obj)
        {
            if (RawSerializeFn != null)
            {
                writer.Write(RawSerializeFn((T)obj));
            }
            else
            {
                var serializer = JsWriter.GetTypeSerializer<TSerializer>();
                serializer.WriteString(writer, SerializeFn((T)obj));
            }
        }

        public static object ParseFn(string str)
        {
            return DeSerializeFn(str);
        }

        internal static object ParseFn(ITypeSerializer serializer, string str)
        {
            if (RawDeserializeFn != null)
            {
                return RawDeserializeFn(str);
            }
            else
            {
                return DeSerializeFn(serializer.UnescapeString(str));
            }
        }
    }

    public enum JsonPropertyConvention
    {
        /// <summary>
        /// The property names on target types must match property names in the JSON source
        /// </summary>
        ExactMatch,
        /// <summary>
        /// The property names on target types may not match the property names in the JSON source
        /// </summary>
        Lenient
    }

    public enum JsonDateHandler
    {
        TimestampOffset,
        DCJSCompatible,
        ISO8601
    }

    public enum JsonTimeSpanHandler
    {
        /// <summary>
        /// Uses the xsd format like PT15H10M20S
        /// </summary>
        DurationFormat,
        /// <summary>
        /// Uses the standard .net ToString method of the TimeSpan class
        /// </summary>
        StandardFormat
    }
}

#endregion JsConfig.cs

/// ********   File: \JsonObject.cs
#region JsonObject.cs

namespace ServiceStack.Text
{
	public static class JsonExtensions
	{
		public static T JsonTo<T>(this Dictionary<string, string> map, string key)
		{
			return Get<T>(map, key);
		}

        /// <summary>
        /// Get JSON string value converted to T
        /// </summary>
        public static T Get<T>(this Dictionary<string, string> map, string key)
		{
			string strVal;
			return map.TryGetValue(key, out strVal) ? JsonSerializer.DeserializeFromString<T>(strVal) : default(T);
		}

        /// <summary>
        /// Get JSON string value
        /// </summary>
        public static string Get(this Dictionary<string, string> map, string key)
		{
			string strVal;
            return map.TryGetValue(key, out strVal) ? JsonTypeSerializer.Instance.UnescapeString(strVal) : null;
		}

		public static JsonArrayObjects ArrayObjects(this string json)
		{
			return Text.JsonArrayObjects.Parse(json);
		}

		public static List<T> ConvertAll<T>(this JsonArrayObjects jsonArrayObjects, Func<JsonObject, T> converter)
		{
			var results = new List<T>();

			foreach (var jsonObject in jsonArrayObjects)
			{
				results.Add(converter(jsonObject));
			}

			return results;
		}

		public static T ConvertTo<T>(this JsonObject jsonObject, Func<JsonObject, T> converFn)
		{
			return jsonObject == null 
				? default(T) 
				: converFn(jsonObject);
		}

		public static Dictionary<string, string> ToDictionary(this JsonObject jsonObject)
		{
			return jsonObject == null 
				? new Dictionary<string, string>() 
				: new Dictionary<string, string>(jsonObject);
		}
	}

	public class JsonObject : Dictionary<string, string>
	{
        /// <summary>
        /// Get JSON string value
        /// </summary>
        public new string this[string key]
        {
            get { return this.Get(key); }
            set { base[key] = value; }
        }

		public static JsonObject Parse(string json)
		{
			return JsonSerializer.DeserializeFromString<JsonObject>(json);
		}

		public JsonArrayObjects ArrayObjects(string propertyName)
		{
			string strValue;
			return this.TryGetValue(propertyName, out strValue)
				? JsonArrayObjects.Parse(strValue)
				: null;
		}

		public JsonObject Object(string propertyName)
		{
			string strValue;
			return this.TryGetValue(propertyName, out strValue)
				? Parse(strValue)
				: null;
		}

        /// <summary>
        /// Get unescaped string value
        /// </summary>
        public string GetUnescaped(string key)
        {
            return base[key];
        }

        /// <summary>
        /// Get unescaped string value
        /// </summary>
        public string Child(string key)
        {
            return base[key];
        }
#if !SILVERLIGHT && !MONOTOUCH
        static readonly Regex NumberRegEx = new Regex(@"^[0-9]*(?:\.[0-9]*)?$", RegexOptions.Compiled);
#else
        static readonly Regex NumberRegEx = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
#endif
        /// <summary>
        /// Write JSON Array, Object, bool or number values as raw string
        /// </summary>
        public static void WriteValue(TextWriter writer, object value)
        {
            var strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                var firstChar = strValue[0];
                var lastChar = strValue[strValue.Length - 1];
                if ((firstChar == JsWriter.MapStartChar && lastChar == JsWriter.MapEndChar)
                    || (firstChar == JsWriter.ListStartChar && lastChar == JsWriter.ListEndChar) 
                    || JsonUtils.True == strValue
                    || JsonUtils.False == strValue
                    || NumberRegEx.IsMatch(strValue))
                {
                    writer.Write(strValue);
                    return;
                }
            }
            JsonUtils.WriteString(writer, strValue);
        }
    }

	public class JsonArrayObjects : List<JsonObject>
	{
		public static JsonArrayObjects Parse(string json)
		{
			return JsonSerializer.DeserializeFromString<JsonArrayObjects>(json);
		}
	}

    public struct JsonValue
    {
        private readonly string json;

        public JsonValue(string json)
        {
            this.json = json;
        }

        public T As<T>()
        {
            return JsonSerializer.DeserializeFromString<T>(json);
        }
        
        public override string ToString()
        {
            return json;
        }
    }

}
#endregion JsonObject.cs

/// ********   File: \JsonSerializer.cs
#region JsonSerializer.cs

//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	/// <summary>
	/// Creates an instance of a Type from a string value
	/// </summary>
	public static class JsonSerializer
	{
		private static readonly UTF8Encoding UTF8EncodingWithoutBom = new UTF8Encoding(false);

		public static T DeserializeFromString<T>(string value)
		{
			if (string.IsNullOrEmpty(value)) return default(T);
			return (T)JsonReader<T>.Parse(value);
		}

		public static T DeserializeFromReader<T>(TextReader reader)
		{
			return DeserializeFromString<T>(reader.ReadToEnd());
		}

		public static object DeserializeFromString(string value, Type type)
		{
            return string.IsNullOrEmpty(value)
					? null
					: JsonReader.GetParseFn(type)(value);
		}

		public static object DeserializeFromReader(TextReader reader, Type type)
		{
			return DeserializeFromString(reader.ReadToEnd(), type);
		}

		public static string SerializeToString<T>(T value)
		{
			if (value == null) return null;
            if (typeof(T) == typeof(object) || typeof(T).IsAbstract || typeof(T).IsInterface)
            {
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = true;
                var result = SerializeToString(value, value.GetType());
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = false;
                return result;
            }

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				if (typeof(T) == typeof(string))
				{
					JsonUtils.WriteString(writer, value as string);
				}
				else
				{
					JsonWriter<T>.WriteObject(writer, value);
				}
			}
			return sb.ToString();
		}

		public static string SerializeToString(object value, Type type)
		{
			if (value == null) return null;

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				if (type == typeof(string))
				{
					JsonUtils.WriteString(writer, value as string);
				}
				else
				{
					JsonWriter.GetWriteFn(type)(writer, value);
				}
			}
			return sb.ToString();
		}

		public static void SerializeToWriter<T>(T value, TextWriter writer)
		{
			if (value == null) return;
			if (typeof(T) == typeof(string))
			{
				writer.Write(value);
				return;
			}
            if (typeof(T) == typeof(object) || typeof(T).IsAbstract || typeof(T).IsInterface)
			{
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = true;
                SerializeToWriter(value, value.GetType(), writer);
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = false;
                return;
			}

			JsonWriter<T>.WriteObject(writer, value);
		}

		public static void SerializeToWriter(object value, Type type, TextWriter writer)
		{
			if (value == null) return;
			if (type == typeof(string))
			{
				writer.Write(value);
				return;
			}

			JsonWriter.GetWriteFn(type)(writer, value);
		}

		public static void SerializeToStream<T>(T value, Stream stream)
		{
			if (value == null) return;
			if (typeof(T) == typeof(object) || typeof(T).IsAbstract || typeof(T).IsInterface)
			{
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = true;
                SerializeToStream(value, value.GetType(), stream);
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = false;
				return;
			}

			var writer = new StreamWriter(stream, UTF8EncodingWithoutBom);
			JsonWriter<T>.WriteObject(writer, value);
			writer.Flush();
		}

		public static void SerializeToStream(object value, Type type, Stream stream)
		{
			var writer = new StreamWriter(stream, UTF8EncodingWithoutBom);
			JsonWriter.GetWriteFn(type)(writer, value);
			writer.Flush();
		}

		public static T DeserializeFromStream<T>(Stream stream)
		{
			using (var reader = new StreamReader(stream, UTF8EncodingWithoutBom))
			{
				return DeserializeFromString<T>(reader.ReadToEnd());
			}
		}

		public static object DeserializeFromStream(Type type, Stream stream)
		{
			using (var reader = new StreamReader(stream, UTF8EncodingWithoutBom))
			{
				return DeserializeFromString(reader.ReadToEnd(), type);
			}
		}
	}
}
#endregion JsonSerializer.cs

/// ********   File: \JsonSerializer.Generic.cs
#region JsonSerializer.Generic.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public class JsonSerializer<T> : ITypeSerializer<T>
	{
		public bool CanCreateFromString(Type type)
		{
			return JsonReader.GetParseFn(type) != null;
		}

		/// <summary>
		/// Parses the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public T DeserializeFromString(string value)
		{
			if (string.IsNullOrEmpty(value)) return default(T);
			return (T)JsonReader<T>.Parse(value);
		}

		public T DeserializeFromReader(TextReader reader)
		{
			return DeserializeFromString(reader.ReadToEnd());
		}

		public string SerializeToString(T value)
		{
			if (value == null) return null;
			if (typeof(T) == typeof(string)) return value as string;
            if (typeof(T) == typeof(object) || typeof(T).IsAbstract || typeof(T).IsInterface)
            {
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = true;
                var result = JsonSerializer.SerializeToString(value, value.GetType());
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = false;
                return result;
            }

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				JsonWriter<T>.WriteObject(writer, value);
			}
			return sb.ToString();
		}

		public void SerializeToWriter(T value, TextWriter writer)
		{
			if (value == null) return;
			if (typeof(T) == typeof(string))
			{
				writer.Write(value);
				return;
			}
            if (typeof(T) == typeof(object) || typeof(T).IsAbstract || typeof(T).IsInterface)
            {
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = true;
                JsonSerializer.SerializeToWriter(value, value.GetType(), writer);
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = false;
                return;
            }
           
            JsonWriter<T>.WriteObject(writer, value);
		}
	}
}
#endregion JsonSerializer.Generic.cs

/// ********   File: \JsvFormatter.cs
#region JsvFormatter.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//	 Peter Townsend (townsend.pete@gmail.com)
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public static class JsvFormatter
	{
		public static string Format(string serializedText)
		{
			if (string.IsNullOrEmpty(serializedText)) return null;

			var tabCount = 0;
			var sb = new StringBuilder();
			var firstKeySeparator = true;

			for (var i = 0; i < serializedText.Length; i++)
			{
				var current = serializedText[i];
				var previous = i - 1 >= 0 ? serializedText[i - 1] : 0;
				var next = i < serializedText.Length - 1 ? serializedText[i + 1] : 0;

				if (current == JsWriter.MapStartChar || current == JsWriter.ListStartChar)
				{
					if (previous == JsWriter.MapKeySeperator)
					{
						if (next == JsWriter.MapEndChar || next == JsWriter.ListEndChar)
						{
							sb.Append(current);
							sb.Append(serializedText[++i]); //eat next
							continue;
						}

						AppendTabLine(sb, tabCount);
					}

					sb.Append(current);
					AppendTabLine(sb, ++tabCount);
					firstKeySeparator = true;
					continue;
				}

				if (current == JsWriter.MapEndChar || current == JsWriter.ListEndChar)
				{
					AppendTabLine(sb, --tabCount);
					sb.Append(current);
					firstKeySeparator = true;
					continue;
				}

				if (current == JsWriter.ItemSeperator)
				{
					sb.Append(current);
					AppendTabLine(sb, tabCount);
					firstKeySeparator = true;
					continue;
				}

				sb.Append(current);

				if (current == JsWriter.MapKeySeperator && firstKeySeparator)
				{
					sb.Append(" ");
					firstKeySeparator = false;
				}
			}

			return sb.ToString();
		}

		private static void AppendTabLine(StringBuilder sb, int tabCount)
		{
			sb.AppendLine();

			if (tabCount > 0)
			{
				sb.Append(new string('\t', tabCount));
			}
		}
	}
}
#endregion JsvFormatter.cs

/// ********   File: \ListExtensions.cs
#region ListExtensions.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public static class ListExtensions
	{
		public static string Join<T>(this IEnumerable<T> values)
		{
			return Join(values, JsWriter.ItemSeperatorString);
		}

		public static string Join<T>(this IEnumerable<T> values, string seperator)
		{
			var sb = new StringBuilder();
			foreach (var value in values)
			{
				if (sb.Length > 0)
					sb.Append(seperator);
				sb.Append(value);
			}
			return sb.ToString();
		}

		public static bool IsNullOrEmpty<T>(this List<T> list)
		{
			return list == null || list.Count == 0;
		}

		//TODO: make it work
		public static IEnumerable<TFrom> SafeWhere<TFrom>(this List<TFrom> list, Func<TFrom, bool> predicate)
		{
			return list.Where(predicate);
		}

		public static int NullableCount<T>(this List<T> list)
		{
			return list == null ? 0 : list.Count;
		}

		public static void AddIfNotExists<T>(this List<T> list, T item)
		{
			if (!list.Contains(item))
				list.Add(item);
		}
	}
}
#endregion ListExtensions.cs

/// ********   File: \MapExtensions.cs
#region MapExtensions.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public static class MapExtensions
	{
		public static string Join<K, V>(this Dictionary<K, V> values)
		{
			return Join(values, JsWriter.ItemSeperatorString, JsWriter.MapKeySeperatorString);
		}

		public static string Join<K, V>(this Dictionary<K, V> values, string itemSeperator, string keySeperator)
		{
			var sb = new StringBuilder();
			foreach (var entry in values)
			{
				if (sb.Length > 0)
					sb.Append(itemSeperator);

				sb.Append(entry.Key).Append(keySeperator).Append(entry.Value);
			}
			return sb.ToString();
		}
	}
}
#endregion MapExtensions.cs

/// ********   File: \QueryStringSerializer.cs
#region QueryStringSerializer.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public static class QueryStringSerializer
	{
		internal static readonly JsWriter<JsvTypeSerializer> Instance = new JsWriter<JsvTypeSerializer>();

		private static Dictionary<Type, WriteObjectDelegate> WriteFnCache = new Dictionary<Type, WriteObjectDelegate>();

		internal static WriteObjectDelegate GetWriteFn(Type type)
		{
			try
			{
				WriteObjectDelegate writeFn;
                if (WriteFnCache.TryGetValue(type, out writeFn)) return writeFn;

                var genericType = typeof(QueryStringWriter<>).MakeGenericType(type);
                var mi = genericType.GetMethod("WriteFn", BindingFlags.NonPublic | BindingFlags.Static);
                var writeFactoryFn = (Func<WriteObjectDelegate>)Delegate.CreateDelegate(
                    typeof(Func<WriteObjectDelegate>), mi);
                writeFn = writeFactoryFn();

                Dictionary<Type, WriteObjectDelegate> snapshot, newCache;
                do
                {
                    snapshot = WriteFnCache;
                    newCache = new Dictionary<Type, WriteObjectDelegate>(WriteFnCache);
                    newCache[type] = writeFn;

                } while (!ReferenceEquals(
                    Interlocked.CompareExchange(ref WriteFnCache, newCache, snapshot), snapshot));
                
                return writeFn;
			}
			catch (Exception ex)
			{
				Tracer.Instance.WriteError(ex);
				throw;
			}
		}

		public static void WriteLateBoundObject(TextWriter writer, object value)
		{
			if (value == null) return;
			var writeFn = GetWriteFn(value.GetType());
			writeFn(writer, value);
		}

		internal static WriteObjectDelegate GetValueTypeToStringMethod(Type type)
		{
			return Instance.GetValueTypeToStringMethod(type);
		}

		public static string SerializeToString<T>(T value)
		{
			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				GetWriteFn(value.GetType())(writer, value);
			}
			return sb.ToString();
		}
	}

	/// <summary>
	/// Implement the serializer using a more static approach
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class QueryStringWriter<T>
	{
		private static readonly WriteObjectDelegate CacheFn;

		internal static WriteObjectDelegate WriteFn()
		{
			return CacheFn;
		}

		static QueryStringWriter()
		{
			if (typeof(T) == typeof(object))
			{
				CacheFn = QueryStringSerializer.WriteLateBoundObject;
			}
			else
			{
				if (typeof(T).IsClass || typeof(T).IsInterface)
				{
					var canWriteType = WriteType<T, JsvTypeSerializer>.Write;
					if (canWriteType != null)
					{
						CacheFn = WriteType<T, JsvTypeSerializer>.WriteQueryString;
						return;
					}
				}

				CacheFn = QueryStringSerializer.Instance.GetWriteFn<T>();
			}
		}

		public static void WriteObject(TextWriter writer, object value)
		{
			if (writer == null) return;
			CacheFn(writer, value);
		}
	}
	
}
#endregion QueryStringSerializer.cs

/// ********   File: \ReflectionExtensions.cs
#region ReflectionExtensions.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

#if WINDOWS_PHONE
#endif

namespace ServiceStack.Text
{
    public delegate EmptyCtorDelegate EmptyCtorFactoryDelegate(Type type);
    public delegate object EmptyCtorDelegate();

    public static class ReflectionExtensions
    {
        private static Dictionary<Type, object> DefaultValueTypes = new Dictionary<Type, object>();

        public static object GetDefaultValue(this Type type)
        {
            if (!type.IsValueType) return null;

            object defaultValue;
            if (DefaultValueTypes.TryGetValue(type, out defaultValue)) return defaultValue;

            defaultValue = Activator.CreateInstance(type);

            Dictionary<Type, object> snapshot, newCache;
            do
            {
                snapshot = DefaultValueTypes;
                newCache = new Dictionary<Type, object>(DefaultValueTypes);
                newCache[type] = defaultValue;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref DefaultValueTypes, newCache, snapshot), snapshot));

            return defaultValue;
        }

        public static bool IsInstanceOf(this Type type, Type thisOrBaseType)
        {
            while (type != null)
            {
                if (type == thisOrBaseType)
                    return true;

                type = type.BaseType;
            }
            return false;
        }

        public static bool IsGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                    return true;

                type = type.BaseType;
            }
            return false;
        }

        public static Type GetGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                    return type;

                type = type.BaseType;
            }
            return null;
        }

        public static bool IsOrHasGenericInterfaceTypeOf(this Type type, Type genericTypeDefinition)
        {
            return (type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition) != null)
                || (type == genericTypeDefinition);
        }

        public static Type GetTypeWithGenericTypeDefinitionOf(this Type type, Type genericTypeDefinition)
        {
            foreach (var t in type.GetInterfaces())
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return t;
                }
            }

            var genericType = type.GetGenericType();
            if (genericType != null && genericType.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return genericType;
            }

            return null;
        }

        public static Type GetTypeWithInterfaceOf(this Type type, Type interfaceType)
        {
            if (type == interfaceType) return interfaceType;

            foreach (var t in type.GetInterfaces())
            {
                if (t == interfaceType)
                    return t;
            }

            return null;
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            foreach (var t in type.GetInterfaces())
            {
                if (t == interfaceType)
                    return true;
            }
            return false;
        }

        public static bool AllHaveInterfacesOfType(
            this Type assignableFromType, params Type[] types)
        {
            foreach (var type in types)
            {
                if (assignableFromType.GetTypeWithInterfaceOf(type) == null) return false;
            }
            return true;
        }

        public static bool IsNumericType(this Type type)
        {
            if (!type.IsValueType) return false;
            return type.IsIntegerType() || type.IsRealNumberType();
        }

        public static bool IsIntegerType(this Type type)
        {
            if (!type.IsValueType) return false;
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType == typeof(byte)
               || underlyingType == typeof(sbyte)
               || underlyingType == typeof(short)
               || underlyingType == typeof(ushort)
               || underlyingType == typeof(int)
               || underlyingType == typeof(uint)
               || underlyingType == typeof(long)
               || underlyingType == typeof(ulong);
        }

        public static bool IsRealNumberType(this Type type)
        {
            if (!type.IsValueType) return false;
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType == typeof(float)
               || underlyingType == typeof(double)
               || underlyingType == typeof(decimal);
        }

        public static Type GetTypeWithGenericInterfaceOf(this Type type, Type genericInterfaceType)
        {
            foreach (var t in type.GetInterfaces())
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == genericInterfaceType) return t;
            }

            if (!type.IsGenericType) return null;

            var genericType = type.GetGenericType();
            return genericType.GetGenericTypeDefinition() == genericInterfaceType
                    ? genericType
                    : null;
        }

        public static bool HasAnyTypeDefinitionsOf(this Type genericType, params Type[] theseGenericTypes)
        {
            if (!genericType.IsGenericType) return false;
            var genericTypeDefinition = genericType.GetGenericTypeDefinition();

            foreach (var thisGenericType in theseGenericTypes)
            {
                if (genericTypeDefinition == thisGenericType)
                    return true;
            }

            return false;
        }

        public static Type[] GetGenericArgumentsIfBothHaveSameGenericDefinitionTypeAndArguments(
            this Type assignableFromType, Type typeA, Type typeB)
        {
            var typeAInterface = typeA.GetTypeWithGenericInterfaceOf(assignableFromType);
            if (typeAInterface == null) return null;

            var typeBInterface = typeB.GetTypeWithGenericInterfaceOf(assignableFromType);
            if (typeBInterface == null) return null;

            var typeAGenericArgs = typeAInterface.GetGenericArguments();
            var typeBGenericArgs = typeBInterface.GetGenericArguments();
            if (typeAGenericArgs.Length != typeBGenericArgs.Length) return null;

            for (var i = 0; i < typeBGenericArgs.Length; i++)
            {
                if (typeAGenericArgs[i] != typeBGenericArgs[i])
                {
                    return null;
                }
            }

            return typeAGenericArgs;
        }

        public static TypePair GetGenericArgumentsIfBothHaveConvertibleGenericDefinitionTypeAndArguments(
            this Type assignableFromType, Type typeA, Type typeB)
        {
            var typeAInterface = typeA.GetTypeWithGenericInterfaceOf(assignableFromType);
            if (typeAInterface == null) return null;

            var typeBInterface = typeB.GetTypeWithGenericInterfaceOf(assignableFromType);
            if (typeBInterface == null) return null;

            var typeAGenericArgs = typeAInterface.GetGenericArguments();
            var typeBGenericArgs = typeBInterface.GetGenericArguments();
            if (typeAGenericArgs.Length != typeBGenericArgs.Length) return null;

            for (var i = 0; i < typeBGenericArgs.Length; i++)
            {
                if (!AreAllStringOrValueTypes(typeAGenericArgs[i], typeBGenericArgs[i]))
                {
                    return null;
                }
            }

            return new TypePair(typeAGenericArgs, typeBGenericArgs);
        }

        public static bool AreAllStringOrValueTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                if (!(type == typeof(string) || type.IsValueType)) return false;
            }
            return true;
        }

        static Dictionary<Type, EmptyCtorDelegate> ConstructorMethods = new Dictionary<Type, EmptyCtorDelegate>();
        public static EmptyCtorDelegate GetConstructorMethod(Type type)
        {
            EmptyCtorDelegate emptyCtorFn;
            if (ConstructorMethods.TryGetValue(type, out emptyCtorFn)) return emptyCtorFn;

            emptyCtorFn = GetConstructorMethodToCache(type);

            Dictionary<Type, EmptyCtorDelegate> snapshot, newCache;
            do
            {
                snapshot = ConstructorMethods;
                newCache = new Dictionary<Type, EmptyCtorDelegate>(ConstructorMethods);
                newCache[type] = emptyCtorFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ConstructorMethods, newCache, snapshot), snapshot));

            return emptyCtorFn;
        }

        static Dictionary<string, EmptyCtorDelegate> TypeNamesMap = new Dictionary<string, EmptyCtorDelegate>();
        public static EmptyCtorDelegate GetConstructorMethod(string typeName)
        {
            EmptyCtorDelegate emptyCtorFn;
            if (TypeNamesMap.TryGetValue(typeName, out emptyCtorFn)) return emptyCtorFn;

            var type = JsConfig.TypeFinder.Invoke(typeName);
            if (type == null) return null;
            emptyCtorFn = GetConstructorMethodToCache(type);

            Dictionary<string, EmptyCtorDelegate> snapshot, newCache;
            do
            {
                snapshot = TypeNamesMap;
                newCache = new Dictionary<string, EmptyCtorDelegate>(TypeNamesMap);
                newCache[typeName] = emptyCtorFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref TypeNamesMap, newCache, snapshot), snapshot));

            return emptyCtorFn;
        }

        public static EmptyCtorDelegate GetConstructorMethodToCache(Type type)
        {
            var emptyCtor = type.GetConstructor(Type.EmptyTypes);
            if (emptyCtor != null)
            {

#if MONOTOUCH || c|| XBOX
				return () => Activator.CreateInstance(type);
                
#elif WINDOWS_PHONE
                return Expression.Lambda<EmptyCtorDelegate>(Expression.New(type)).Compile();
#else
#if SILVERLIGHT
                var dm = new System.Reflection.Emit.DynamicMethod("MyCtor", type, Type.EmptyTypes);
#else
                var dm = new System.Reflection.Emit.DynamicMethod("MyCtor", type, Type.EmptyTypes, typeof(ReflectionExtensions).Module, true);
#endif
                var ilgen = dm.GetILGenerator();
                ilgen.Emit(System.Reflection.Emit.OpCodes.Nop);
                ilgen.Emit(System.Reflection.Emit.OpCodes.Newobj, emptyCtor);
                ilgen.Emit(System.Reflection.Emit.OpCodes.Ret);

                return (EmptyCtorDelegate)dm.CreateDelegate(typeof(EmptyCtorDelegate));
#endif
            }

#if (SILVERLIGHT && !WINDOWS_PHONE) || XBOX
            return () => Activator.CreateInstance(type);
#elif WINDOWS_PHONE
            return Expression.Lambda<EmptyCtorDelegate>(Expression.New(type)).Compile();
#else
            //Anonymous types don't have empty constructors
            return () => FormatterServices.GetUninitializedObject(type);
#endif
        }

        private static class TypeMeta<T>
        {
            public static readonly EmptyCtorDelegate EmptyCtorFn;
            static TypeMeta()
            {
                EmptyCtorFn = GetConstructorMethodToCache(typeof(T));
            }
        }

        public static object CreateInstance<T>()
        {
            return TypeMeta<T>.EmptyCtorFn();
        }

        public static object CreateInstance(this Type type)
        {
            var ctorFn = GetConstructorMethod(type);
            return ctorFn();
        }

        public static object CreateInstance(string typeName)
        {
            var ctorFn = GetConstructorMethod(typeName);
            return ctorFn();
        }

        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
                .Where(t => t.GetIndexParameters().Length == 0) // ignore indexed properties
                .ToArray();
        }

        const string DataContract = "DataContractAttribute";
        const string DataMember = "DataMemberAttribute";
        const string IgnoreDataMember = "IgnoreDataMemberAttribute";

        public static PropertyInfo[] GetSerializableProperties(this Type type)
        {
            var publicProperties = GetPublicProperties(type);
            var publicReadableProperties = publicProperties.Where(x => x.GetGetMethod(false) != null);

            if (type.IsDto())
            {
                return !Env.IsMono
                    ? publicReadableProperties.Where(attr => 
                        attr.IsDefined(typeof(DataMemberAttribute), false)).ToArray()
                    : publicReadableProperties.Where(attr => 
                        attr.GetCustomAttributes(false).Any(x => x.GetType().Name == DataMember)).ToArray();
            }

            // else return those properties that are not decorated with IgnoreDataMember
            return publicReadableProperties.Where(prop => !prop.GetCustomAttributes(false).Any(attr => attr.GetType().Name == IgnoreDataMember)).ToArray();
        }

        public static bool IsDto(this Type type)
        {
            return !Env.IsMono
                ? type.IsDefined(typeof(DataContractAttribute), false)
                : type.GetCustomAttributes(true).Any(x => x.GetType().Name == DataContract);
        }

        public static bool HasAttr<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(true).Any(x => x.GetType() == typeof(T));
        }

#if !SILVERLIGHT && !MONOTOUCH 
        static readonly Dictionary<Type, FastMember.TypeAccessor> typeAccessorMap 
            = new Dictionary<Type, FastMember.TypeAccessor>();
#endif

        public static DataContractAttribute GetDataContract(this Type type)
        {
            var dataContract = type.GetCustomAttributes(typeof(DataContractAttribute), true)
                .FirstOrDefault() as DataContractAttribute;

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
            if (dataContract == null && Env.IsMono)
                return type.GetWeakDataContract();
#endif
            return dataContract;
        }

        public static DataMemberAttribute GetDataMember(this PropertyInfo pi)
        {
            var dataMember = pi.GetCustomAttributes(typeof(DataMemberAttribute), false)
                .FirstOrDefault() as DataMemberAttribute;

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
            if (dataMember == null && Env.IsMono)
                return pi.GetWeakDataMember();
#endif
            return dataMember;
        }

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
        public static DataContractAttribute GetWeakDataContract(this Type type)
        {
            var attr = type.GetCustomAttributes(true).FirstOrDefault(x => x.GetType().Name == DataContract);
            if (attr != null)
            {
                var attrType = attr.GetType();

                FastMember.TypeAccessor accessor;
                lock (typeAccessorMap)
                {
                    if (!typeAccessorMap.TryGetValue(attrType, out accessor))
                        typeAccessorMap[attrType] = accessor = FastMember.TypeAccessor.Create(attr.GetType());
                }

                return new DataContractAttribute {
                    Name = (string)accessor[attr, "Name"],
                    Namespace = (string)accessor[attr, "Namespace"],
                };
            }
            return null;
        }

        public static DataMemberAttribute GetWeakDataMember(this PropertyInfo pi)
        {
            var attr = pi.GetCustomAttributes(true).FirstOrDefault(x => x.GetType().Name == DataMember);
            if (attr != null)
            {
                var attrType = attr.GetType();

                FastMember.TypeAccessor accessor;
                lock (typeAccessorMap)
                {
                    if (!typeAccessorMap.TryGetValue(attrType, out accessor))
                        typeAccessorMap[attrType] = accessor = FastMember.TypeAccessor.Create(attr.GetType());
                }

                var newAttr = new DataMemberAttribute {
                    Name = (string) accessor[attr, "Name"],
                    EmitDefaultValue = (bool)accessor[attr, "EmitDefaultValue"],
                    IsRequired = (bool)accessor[attr, "IsRequired"],
                };

                var order = (int)accessor[attr, "Order"];
                if (order >= 0)
                    newAttr.Order = order; //Throws Exception if set to -1

                return newAttr;
            }
            return null;
        }
#endif

    }

}
#endregion ReflectionExtensions.cs

/// ********   File: \StreamExtensions.cs
#region StreamExtensions.cs

namespace ServiceStack.Text
{
	public static class StreamExtensions
	{
		public static void WriteTo(this Stream inStream, Stream outStream)
		{
			var memoryStream = inStream as MemoryStream;
			if (memoryStream != null)
			{
				memoryStream.WriteTo(outStream);
				return;
			}

			var data = new byte[4096];
			int bytesRead;

			while ((bytesRead = inStream.Read(data, 0, data.Length)) > 0)
			{
				outStream.Write(data, 0, bytesRead);
			}
		}

		public static IEnumerable<string> ReadLines(this StreamReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			string line;
			while ((line = reader.ReadLine()) != null)
			{
				yield return line;
			}
		}

		/// <summary>
		/// @jonskeet: Collection of utility methods which operate on streams.
		/// r285, February 26th 2009: http://www.yoda.arachsys.com/csharp/miscutil/
		/// </summary>
		const int DefaultBufferSize = 8 * 1024;

		/// <summary>
		/// Reads the given stream up to the end, returning the data as a byte
		/// array.
		/// </summary>
		public static byte[] ReadFully(this Stream input)
		{
			return ReadFully(input, DefaultBufferSize);
		}

		/// <summary>
		/// Reads the given stream up to the end, returning the data as a byte
		/// array, using the given buffer size.
		/// </summary>
		public static byte[] ReadFully(this Stream input, int bufferSize)
		{
			if (bufferSize < 1)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			return ReadFully(input, new byte[bufferSize]);
		}

		/// <summary>
		/// Reads the given stream up to the end, returning the data as a byte
		/// array, using the given buffer for transferring data. Note that the
		/// current contents of the buffer is ignored, so the buffer needn't
		/// be cleared beforehand.
		/// </summary>
		public static byte[] ReadFully(this Stream input, byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (buffer.Length == 0)
			{
				throw new ArgumentException("Buffer has length of 0");
			}
			// We could do all our own work here, but using MemoryStream is easier
			// and likely to be just as efficient.
			using (var tempStream = new MemoryStream())
			{
				CopyTo(input, tempStream, buffer);
				// No need to copy the buffer if it's the right size
				if (tempStream.Length == tempStream.GetBuffer().Length)
				{
					return tempStream.GetBuffer();
				}
				// Okay, make a copy that's the right size
				return tempStream.ToArray();
			}
		}

		/// <summary>
		/// Copies all the data from one stream into another.
		/// </summary>
		public static void CopyTo(this Stream input, Stream output)
		{
			CopyTo(input, output, DefaultBufferSize);
		}

		/// <summary>
		/// Copies all the data from one stream into another, using a buffer
		/// of the given size.
		/// </summary>
		public static void CopyTo(this Stream input, Stream output, int bufferSize)
		{
			if (bufferSize < 1)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			CopyTo(input, output, new byte[bufferSize]);
		}

		/// <summary>
		/// Copies all the data from one stream into another, using the given 
		/// buffer for transferring data. Note that the current contents of 
		/// the buffer is ignored, so the buffer needn't be cleared beforehand.
		/// </summary>
		public static void CopyTo(this Stream input, Stream output, byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (buffer.Length == 0)
			{
				throw new ArgumentException("Buffer has length of 0");
			}
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, read);
			}
		}

		/// <summary>
		/// Reads exactly the given number of bytes from the specified stream.
		/// If the end of the stream is reached before the specified amount
		/// of data is read, an exception is thrown.
		/// </summary>
		public static byte[] ReadExactly(this Stream input, int bytesToRead)
		{
			return ReadExactly(input, new byte[bytesToRead]);
		}

		/// <summary>
		/// Reads into a buffer, filling it completely.
		/// </summary>
		public static byte[] ReadExactly(this Stream input, byte[] buffer)
		{
			return ReadExactly(input, buffer, buffer.Length);
		}

		/// <summary>
		/// Reads exactly the given number of bytes from the specified stream,
		/// into the given buffer, starting at position 0 of the array.
		/// </summary>
		public static byte[] ReadExactly(this Stream input, byte[] buffer, int bytesToRead)
		{
			return ReadExactly(input, buffer, 0, bytesToRead);
		}

		/// <summary>
		/// Reads exactly the given number of bytes from the specified stream,
		/// into the given buffer, starting at position 0 of the array.
		/// </summary>
		public static byte[] ReadExactly(this Stream input, byte[] buffer, int startIndex, int bytesToRead)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}

			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}

			if (startIndex < 0 || startIndex >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}

			if (bytesToRead < 1 || startIndex + bytesToRead > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("bytesToRead");
			}

			return ReadExactlyFast(input, buffer, startIndex, bytesToRead);
		}

		/// <summary>
		/// Same as ReadExactly, but without the argument checks.
		/// </summary>
		private static byte[] ReadExactlyFast(Stream fromStream, byte[] intoBuffer, int startAtIndex, int bytesToRead)
		{
			var index = 0;
			while (index < bytesToRead)
			{
				var read = fromStream.Read(intoBuffer, startAtIndex + index, bytesToRead - index);
				if (read == 0)
				{
					throw new EndOfStreamException
						(String.Format("End of stream reached with {0} byte{1} left to read.",
						               bytesToRead - index,
						               bytesToRead - index == 1 ? "s" : ""));
				}
				index += read;
			}
			return intoBuffer;
		}
	}
}
#endregion StreamExtensions.cs

/// ********   File: \StringExtensions.cs
#region StringExtensions.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

#if WINDOWS_PHONE

#endif

namespace ServiceStack.Text
{
    public static class StringExtensions
    {
        public static T To<T>(this string value)
        {
            return TypeSerializer.DeserializeFromString<T>(value);
        }

        public static T To<T>(this string value, T defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : TypeSerializer.DeserializeFromString<T>(value);
        }

        public static T ToOrDefaultValue<T>(this string value)
        {
            return string.IsNullOrEmpty(value) ? default(T) : TypeSerializer.DeserializeFromString<T>(value);
        }

        public static object To(this string value, Type type)
        {
            return TypeSerializer.DeserializeFromString(value, type);
        }

        /// <summary>
        /// Converts from base: 0 - 62
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public static string BaseConvert(this string source, int from, int to)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = "";
            var length = source.Length;
            var number = new int[length];

            for (var i = 0; i < length; i++)
            {
                number[i] = chars.IndexOf(source[i]);
            }

            int newlen;

            do
            {
                var divide = 0;
                newlen = 0;

                for (var i = 0; i < length; i++)
                {
                    divide = divide * from + number[i];

                    if (divide >= to)
                    {
                        number[newlen++] = divide / to;
                        divide = divide % to;
                    }
                    else if (newlen > 0)
                    {
                        number[newlen++] = 0;
                    }
                }

                length = newlen;
                result = chars[divide] + result;
            }
            while (newlen != 0);

            return result;
        }

        public static string EncodeXml(this string value)
        {
            return value.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
        }

        public static string EncodeJson(this string value)
        {
            return string.Concat
            ("\"",
                value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n"),
                "\""
            );
        }

        public static string EncodeJsv(this string value)
        {
			return string.IsNullOrEmpty(value) || !JsWriter.HasAnyEscapeChars(value)
		       	? value
		       	: string.Concat
		       	  	(
						JsWriter.QuoteString,
						value.Replace(JsWriter.QuoteString, TypeSerializer.DoubleQuoteString),
						JsWriter.QuoteString
		       	  	);
        }

        public static string DecodeJsv(this string value)
        {
			const int startingQuotePos = 1;
			const int endingQuotePos = 2;
			return string.IsNullOrEmpty(value) || value[0] != JsWriter.QuoteChar
			       	? value
					: value.Substring(startingQuotePos, value.Length - endingQuotePos)
						.Replace(TypeSerializer.DoubleQuoteString, JsWriter.QuoteString);
        }

        public static string UrlEncode(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var sb = new StringBuilder();

            foreach (var charCode in Encoding.UTF8.GetBytes(text))
            {

                if (
                    charCode >= 65 && charCode <= 90        // A-Z
                    || charCode >= 97 && charCode <= 122    // a-z
                    || charCode >= 48 && charCode <= 57     // 0-9
                    || charCode >= 44 && charCode <= 46     // ,-.
                    )
                {
                    sb.Append((char)charCode);
                }
                else
                {
                    sb.Append('%' + charCode.ToString("x2"));
                }
            }

            return sb.ToString();
        }

        public static string UrlDecode(this string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var bytes = new List<byte>();

            var textLength = text.Length;
            for (var i = 0; i < textLength; i++)
            {
                var c = text[i];
                if (c == '+')
                {
                    bytes.Add(32);
                }
                else if (c == '%')
                {
                    var hexNo = Convert.ToByte(text.Substring(i + 1, 2), 16);
                    bytes.Add(hexNo);
                    i += 2;
                }
                else
                {
                    bytes.Add((byte)c);
                }
            }
#if SILVERLIGHT
            byte[] byteArray = bytes.ToArray();
            return Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
#else
            return Encoding.UTF8.GetString(bytes.ToArray());
#endif
        }

#if !XBOX
        public static string HexEscape(this string text, params char[] anyCharOf)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (anyCharOf == null || anyCharOf.Length == 0) return text;

            var encodeCharMap = new ServiceStack.Text.WP.HashSet<char>(anyCharOf);

            var sb = new StringBuilder();
            var textLength = text.Length;
            for (var i = 0; i < textLength; i++)
            {
                var c = text[i];
                if (encodeCharMap.Contains(c))
                {
                    sb.Append('%' + ((int)c).ToString("x"));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
#endif
        public static string HexUnescape(this string text, params char[] anyCharOf)
        {
            if (string.IsNullOrEmpty(text)) return null;
            if (anyCharOf == null || anyCharOf.Length == 0) return text;

            var sb = new StringBuilder();

            var textLength = text.Length;
            for (var i = 0; i < textLength; i++)
            {
                var c = text.Substring(i, 1);
                if (c == "%")
                {
                    var hexNo = Convert.ToInt32(text.Substring(i + 1, 2), 16);
                    sb.Append((char)hexNo);
                    i += 2;
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string UrlFormat(this string url, params string[] urlComponents)
        {
            var encodedUrlComponents = new string[urlComponents.Length];
            for (var i = 0; i < urlComponents.Length; i++)
            {
                var x = urlComponents[i];
                encodedUrlComponents[i] = x.UrlEncode();
            }

            return string.Format(url, encodedUrlComponents);
        }

        public static string ToRot13(this string value)
        {
            var array = value.ToCharArray();
            for (var i = 0; i < array.Length; i++)
            {
                var number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                    number += (number > 'm') ? -13 : 13;

                else if (number >= 'A' && number <= 'Z')
                    number += (number > 'M') ? -13 : 13;

                array[i] = (char)number;
            }
            return new string(array);
        }

        public static string WithTrailingSlash(this string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (path[path.Length - 1] != '/')
            {
                return path + "/";
            }
            return path;
        }

        public static string AppendPath(this string uri, params string[] uriComponents)
        {
            return AppendUrlPaths(uri, uriComponents);
        }

        public static string AppendUrlPaths(this string uri, params string[] uriComponents)
        {
            var sb = new StringBuilder(uri.WithTrailingSlash());
            var i = 0;
            foreach (var uriComponent in uriComponents)
            {
                if (i++ > 0) sb.Append('/');
                sb.Append(uriComponent.UrlEncode());
            }
            return sb.ToString();
        }

        public static string FromUtf8Bytes(this byte[] bytes)
        {
            return bytes == null ? null
                : Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static byte[] ToUtf8Bytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static byte[] ToUtf8Bytes(this int intVal)
        {
            return FastToUtf8Bytes(intVal.ToString());
        }

        public static byte[] ToUtf8Bytes(this long longVal)
        {
            return FastToUtf8Bytes(longVal.ToString());
        }

        public static byte[] ToUtf8Bytes(this double doubleVal)
        {
            var doubleStr = doubleVal.ToString(CultureInfo.InvariantCulture.NumberFormat);

            if (doubleStr.IndexOf('E') != -1 || doubleStr.IndexOf('e') != -1)
                doubleStr = DoubleConverter.ToExactString(doubleVal);

            return FastToUtf8Bytes(doubleStr);
        }

        /// <summary>
        /// Skip the encoding process for 'safe strings' 
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns></returns>
        private static byte[] FastToUtf8Bytes(string strVal)
        {
            var bytes = new byte[strVal.Length];
            for (var i = 0; i < strVal.Length; i++)
                bytes[i] = (byte)strVal[i];

            return bytes;
        }

        public static string[] SplitOnFirst(this string strVal, char needle)
        {
            if (strVal == null) return new string[0];
            var pos = strVal.IndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }

        public static string[] SplitOnFirst(this string strVal, string needle)
        {
            if (strVal == null) return new string[0];
            var pos = strVal.IndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }

        public static string[] SplitOnLast(this string strVal, char needle)
        {
            if (strVal == null) return new string[0];
            var pos = strVal.LastIndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }

        public static string[] SplitOnLast(this string strVal, string needle)
        {
            if (strVal == null) return new string[0];
            var pos = strVal.LastIndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }

        public static string WithoutExtension(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            var extPos = filePath.LastIndexOf('.');
            if (extPos == -1) return filePath;

            var dirPos = filePath.LastIndexOfAny(DirSeps);
            return extPos > dirPos ? filePath.Substring(0, extPos) : filePath;
        }

        private static readonly char DirSep = Path.DirectorySeparatorChar;
        private static readonly char AltDirSep = Path.DirectorySeparatorChar == '/' ? '\\' : '/';
        static readonly char[] DirSeps = new[] { '\\', '/' };

        public static string ParentDirectory(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            var dirSep = filePath.IndexOf(DirSep) != -1
                         ? DirSep
                         : filePath.IndexOf(AltDirSep) != -1 ? AltDirSep : (char)0;

            return dirSep == 0 ? null : filePath.TrimEnd(dirSep).SplitOnLast(dirSep)[0];
        }

        public static string ToJsv<T>(this T obj)
        {
            return TypeSerializer.SerializeToString(obj);
        }

        public static T FromJsv<T>(this string jsv)
        {
            return TypeSerializer.DeserializeFromString<T>(jsv);
        }

        public static string ToJson<T>(this T obj) {
        	return JsConfig.PreferInterfaces
				? JsonSerializer.SerializeToString(obj, AssemblyUtils.MainInterface<T>())
				: JsonSerializer.SerializeToString(obj);
        }

    	public static T FromJson<T>(this string json)
        {
            return JsonSerializer.DeserializeFromString<T>(json);
        }

#if !XBOX && !SILVERLIGHT && !MONOTOUCH
        public static string ToXml<T>(this T obj)
        {
            return XmlSerializer.SerializeToString(obj);
        }
#endif

#if !XBOX && !SILVERLIGHT && !MONOTOUCH
        public static T FromXml<T>(this string json)
        {
            return XmlSerializer.DeserializeFromString<T>(json);
        }
#endif
        public static string FormatWith(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static string Fmt(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static bool StartsWithIgnoreCase(this string text, string startsWith)
        {
            return text != null
                && text.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string ReadAllText(this string filePath)
        {
#if XBOX && !SILVERLIGHT
			using( var fileStream = new FileStream( filePath, FileMode.Open, FileAccess.Read ) )
			{
				return new StreamReader( fileStream ).ReadToEnd( ) ;
			}

#elif WINDOWS_PHONE
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var fileStream = isoStore.OpenFile(filePath, FileMode.Open))
                {
                    return new StreamReader(fileStream).ReadToEnd();
                }
            }
#else
            return File.ReadAllText(filePath);
#endif

        }

        public static int IndexOfAny(this string text, params string[] needles)
        {
            return IndexOfAny(text, 0, needles);
        }

        public static int IndexOfAny(this string text, int startIndex, params string[] needles)
        {
            if (text == null) return -1;

            var firstPos = -1;
            foreach (var needle in needles)
            {
                var pos = text.IndexOf(needle);
                if (firstPos == -1 || pos < firstPos) firstPos = pos;
            }
            return firstPos;
        }

        public static string ExtractContents(this string fromText, string startAfter, string endAt)
        {
            return ExtractContents(fromText, startAfter, startAfter, endAt);
        }

        public static string ExtractContents(this string fromText, string uniqueMarker, string startAfter, string endAt)
        {
            if (string.IsNullOrEmpty(uniqueMarker))
                throw new ArgumentNullException("uniqueMarker");
            if (string.IsNullOrEmpty(startAfter))
                throw new ArgumentNullException("startAfter");
            if (string.IsNullOrEmpty(endAt))
                throw new ArgumentNullException("endAt");

            if (string.IsNullOrEmpty(fromText)) return null;

            var markerPos = fromText.IndexOf(uniqueMarker);
            if (markerPos == -1) return null;

            var startPos = fromText.IndexOf(startAfter, markerPos);
            if (startPos == -1) return null;
            startPos += startAfter.Length;

            var endPos = fromText.IndexOf(endAt, startPos);
            if (endPos == -1) endPos = fromText.Length;

            return fromText.Substring(startPos, endPos - startPos);
        }

#if XBOX && !SILVERLIGHT
		static readonly Regex StripHtmlRegEx = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);
#else
        static readonly Regex StripHtmlRegEx = new Regex(@"<(.|\n)*?>");
#endif
        public static string StripHtml(this string html)
        {
            return string.IsNullOrEmpty(html) ? null : StripHtmlRegEx.Replace(html, "");
        }

#if XBOX && !SILVERLIGHT
		static readonly Regex StripBracketsRegEx = new Regex(@"\[(.|\n)*?\]", RegexOptions.Compiled);
		static readonly Regex StripBracesRegEx = new Regex(@"\((.|\n)*?\)", RegexOptions.Compiled);
#else
        static readonly Regex StripBracketsRegEx = new Regex(@"\[(.|\n)*?\]");
        static readonly Regex StripBracesRegEx = new Regex(@"\((.|\n)*?\)");
#endif
        public static string StripMarkdownMarkup(this string markdown)
        {
            if (string.IsNullOrEmpty(markdown)) return null;
            markdown = StripBracketsRegEx.Replace(markdown, "");
            markdown = StripBracesRegEx.Replace(markdown, "");
            markdown = markdown
                .Replace("*", "")
                .Replace("!", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("#", "");

            return markdown;
        }

        private const int LowerCaseOffset = 'a' - 'A';
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            var len = value.Length;
            var newValue = new char[len];
            var firstPart = true;

            for (var i = 0; i < len; ++i) {
                var c0 = value[i];
                var c1 = i < len - 1 ? value[i + 1] : 'A';
                var c0isUpper = c0 >= 'A' && c0 <= 'Z';
                var c1isUpper = c1 >= 'A' && c1 <= 'Z';

                if (firstPart && c0isUpper && (c1isUpper || i == 0))
                    c0 = (char)(c0 + LowerCaseOffset);
                else
                    firstPart = false;

                newValue[i] = c0;
            }

            return new string(newValue);
        }

        private static readonly TextInfo TextInfo = CultureInfo.InvariantCulture.TextInfo;
        public static string ToTitleCase(this string value)
        {
#if SILVERLIGHT || __MonoCS__
            string[] words = value.Split('_');

            for (int i = 0; i <= words.Length - 1; i++)
            {
                if ((!object.ReferenceEquals(words[i], string.Empty)))
                {
                    string firstLetter = words[i].Substring(0, 1);
                    string rest = words[i].Substring(1);
                    string result = firstLetter.ToUpper() + rest.ToLower();
                    words[i] = result;
                }
            }
            return String.Join("", words);
#else
            return TextInfo.ToTitleCase(value).Replace("_", string.Empty);
#endif
        }

        public static string ToLowercaseUnderscore(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            value = value.ToCamelCase();
            
            var sb = new StringBuilder(value.Length);
            foreach (var t in value)
            {
                if (char.IsLower(t))
                {
                    sb.Append(t);
                }
                else
                {
                    sb.Append("_");
                    sb.Append(char.ToLower(t));
                }
            }
            return sb.ToString();
        }

        public static string SafeSubstring(this string value, int length)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : value.Substring(Math.Min(length, value.Length));
        }

        public static string SafeSubstring(this string value, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Length >= (startIndex + length))
                return value.Substring(startIndex, length);

            return value.Length > startIndex ? value.Substring(startIndex) : string.Empty;
        }

        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}
#endregion StringExtensions.cs

/// ********   File: \SystemTime.cs
#region SystemTime.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//   Damian Hickey (dhickey@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public static class SystemTime
	{
		public static Func<DateTime> UtcDateTimeResolver;

		public static DateTime Now
		{
			get
			{
				var temp = UtcDateTimeResolver;
				return temp == null ? DateTime.Now : temp().ToLocalTime();
			}
		}

		public static DateTime UtcNow
		{
			get
			{
				var temp = UtcDateTimeResolver;
				return temp == null ? DateTime.UtcNow : temp();
			}
		}
	}
}

#endregion SystemTime.cs

/// ********   File: \TextExtensions.cs
#region TextExtensions.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public static class TextExtensions
	{
		public static string ToCsvField(this string text)
		{
			return string.IsNullOrEmpty(text) || !CsvWriter.HasAnyEscapeChars(text)
		       	? text
		       	: string.Concat
		       	  	(
                        CsvConfig.ItemDelimiterString,
						text.Replace(CsvConfig.ItemDelimiterString, CsvConfig.EscapedItemDelimiterString),
						CsvConfig.ItemDelimiterString
		       	  	);
		}

		public static string FromCsvField(this string text)
		{
			return string.IsNullOrEmpty(text) || !text.StartsWith(CsvConfig.ItemDelimiterString)
			       	? text
					: text.Substring(CsvConfig.ItemDelimiterString.Length, text.Length - CsvConfig.EscapedItemDelimiterString.Length)
						.Replace(CsvConfig.EscapedItemDelimiterString, CsvConfig.ItemDelimiterString);
		}

		public static List<string> FromCsvFields(this IEnumerable<string> texts)
		{
			var safeTexts = new List<string>();
			foreach (var text in texts)
			{
				safeTexts.Add(FromCsvField(text));
			}
			return safeTexts;
		}

		public static string[] FromCsvFields(params string[] texts)
		{
			var textsLen = texts.Length;
			var safeTexts = new string[textsLen];
			for (var i = 0; i < textsLen; i++)
			{
				safeTexts[i] = FromCsvField(texts[i]);
			}
			return safeTexts;
		}

		public static string SerializeToString<T>(this T value)
		{
			return JsonSerializer.SerializeToString(value);
		}
	}
}
#endregion TextExtensions.cs

/// ********   File: \Tracer.cs
#region Tracer.cs

namespace ServiceStack.Text
{
	public class Tracer
	{
		public static ITracer Instance = new NullTracer();

		public class NullTracer : ITracer
		{
			public void WriteDebug(string error) { }

			public void WriteDebug(string format, params object[] args) { }
		    
            public void WriteWarning(string warning) { }

		    public void WriteWarning(string format, params object[] args) { }

		    public void WriteError(Exception ex) { }

			public void WriteError(string error) { }

			public void WriteError(string format, params object[] args) { }

		}

		public class ConsoleTracer : ITracer
		{
			public void WriteDebug(string error)
			{
				Console.WriteLine(error);
			}

			public void WriteDebug(string format, params object[] args)
			{
				Console.WriteLine(format, args);
			}

		    public void WriteWarning(string warning)
		    {
                Console.WriteLine(warning);                
		    }

		    public void WriteWarning(string format, params object[] args)
		    {
                Console.WriteLine(format, args);
            }

		    public void WriteError(Exception ex)
			{
				Console.WriteLine(ex);
			}

			public void WriteError(string error)
			{
				Console.WriteLine(error);
			}

			public void WriteError(string format, params object[] args)
			{
				Console.WriteLine(format, args);
			}
		}
	}
}
#endregion Tracer.cs

/// ********   File: \TranslateListWithElements.cs
#region TranslateListWithElements.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public static class TranslateListWithElements
	{
        private static Dictionary<Type, ConvertInstanceDelegate> TranslateICollectionCache
            = new Dictionary<Type, ConvertInstanceDelegate>();

		public static object TranslateToGenericICollectionCache(object from, Type toInstanceOfType, Type elementType)
		{
            ConvertInstanceDelegate translateToFn;
            if (TranslateICollectionCache.TryGetValue(toInstanceOfType, out translateToFn))
                return translateToFn(from, toInstanceOfType);

            var genericType = typeof(TranslateListWithElements<>).MakeGenericType(elementType);
            var mi = genericType.GetMethod("LateBoundTranslateToGenericICollection", BindingFlags.Static | BindingFlags.Public);
            translateToFn = (ConvertInstanceDelegate)Delegate.CreateDelegate(typeof(ConvertInstanceDelegate), mi);

            Dictionary<Type, ConvertInstanceDelegate> snapshot, newCache;
            do
            {
                snapshot = TranslateICollectionCache;
                newCache = new Dictionary<Type, ConvertInstanceDelegate>(TranslateICollectionCache);
                newCache[elementType] = translateToFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref TranslateICollectionCache, newCache, snapshot), snapshot));

			return translateToFn(from, toInstanceOfType);
		}

        private static Dictionary<ConvertibleTypeKey, ConvertInstanceDelegate> TranslateConvertibleICollectionCache
            = new Dictionary<ConvertibleTypeKey, ConvertInstanceDelegate>();

		public static object TranslateToConvertibleGenericICollectionCache(
			object from, Type toInstanceOfType, Type fromElementType)
		{
			var typeKey = new ConvertibleTypeKey(toInstanceOfType, fromElementType);
            ConvertInstanceDelegate translateToFn;
            if (TranslateConvertibleICollectionCache.TryGetValue(typeKey, out translateToFn)) return translateToFn(from, toInstanceOfType);

            var toElementType = toInstanceOfType.GetGenericType().GetGenericArguments()[0];
            var genericType = typeof(TranslateListWithConvertibleElements<,>).MakeGenericType(fromElementType, toElementType);
            var mi = genericType.GetMethod("LateBoundTranslateToGenericICollection", BindingFlags.Static | BindingFlags.Public);
            translateToFn = (ConvertInstanceDelegate)Delegate.CreateDelegate(typeof(ConvertInstanceDelegate), mi);

            Dictionary<ConvertibleTypeKey, ConvertInstanceDelegate> snapshot, newCache;
            do
            {
                snapshot = TranslateConvertibleICollectionCache;
                newCache = new Dictionary<ConvertibleTypeKey, ConvertInstanceDelegate>(TranslateConvertibleICollectionCache);
                newCache[typeKey] = translateToFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref TranslateConvertibleICollectionCache, newCache, snapshot), snapshot));
            
            return translateToFn(from, toInstanceOfType);
		}

		public static object TryTranslateToGenericICollection(Type fromPropertyType, Type toPropertyType, object fromValue)
		{
			var args = typeof(ICollection<>).GetGenericArgumentsIfBothHaveSameGenericDefinitionTypeAndArguments(
				fromPropertyType, toPropertyType);

			if (args != null)
			{
				return TranslateToGenericICollectionCache(
					fromValue, toPropertyType, args[0]);
			}

			var varArgs = typeof(ICollection<>).GetGenericArgumentsIfBothHaveConvertibleGenericDefinitionTypeAndArguments(
			fromPropertyType, toPropertyType);

			if (varArgs != null)
			{
				return TranslateToConvertibleGenericICollectionCache(
					fromValue, toPropertyType, varArgs.Args1[0]);
			}

			return null;
		}

	}

	public class ConvertibleTypeKey
	{
		public Type ToInstanceType { get; set; }
		public Type FromElemenetType { get; set; }

		public ConvertibleTypeKey()
		{
		}

		public ConvertibleTypeKey(Type toInstanceType, Type fromElemenetType)
		{
			ToInstanceType = toInstanceType;
			FromElemenetType = fromElemenetType;
		}

		public bool Equals(ConvertibleTypeKey other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ToInstanceType, ToInstanceType) && Equals(other.FromElemenetType, FromElemenetType);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ConvertibleTypeKey)) return false;
			return Equals((ConvertibleTypeKey)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((ToInstanceType != null ? ToInstanceType.GetHashCode() : 0) * 397)
					^ (FromElemenetType != null ? FromElemenetType.GetHashCode() : 0);
			}
		}
	}

	public class TranslateListWithElements<T>
	{
		public static object CreateInstance(Type toInstanceOfType)
		{
			if (toInstanceOfType.IsGenericType)
			{
				if (toInstanceOfType.HasAnyTypeDefinitionsOf(
					typeof(ICollection<>), typeof(IList<>)))
				{
					return ReflectionExtensions.CreateInstance(typeof(List<T>));
				}
			}

			return ReflectionExtensions.CreateInstance(toInstanceOfType);
		}

		public static IList TranslateToIList(IList fromList, Type toInstanceOfType)
		{
			var to = (IList)ReflectionExtensions.CreateInstance(toInstanceOfType);
			foreach (var item in fromList)
			{
				to.Add(item);
			}
			return to;
		}

		public static object LateBoundTranslateToGenericICollection(
			object fromList, Type toInstanceOfType)
		{
			if (fromList == null) return null; //AOT

			return TranslateToGenericICollection(
				(ICollection<T>)fromList, toInstanceOfType);
		}

		public static ICollection<T> TranslateToGenericICollection(
			ICollection<T> fromList, Type toInstanceOfType)
		{
			var to = (ICollection<T>)CreateInstance(toInstanceOfType);
			foreach (var item in fromList)
			{
				to.Add(item);
			}
			return to;
		}
	}

	public class TranslateListWithConvertibleElements<TFrom, TTo>
	{
		private static readonly Func<TFrom, TTo> ConvertFn;

		static TranslateListWithConvertibleElements()
		{
			ConvertFn = GetConvertFn();
		}

		public static object LateBoundTranslateToGenericICollection(
			object fromList, Type toInstanceOfType)
		{
			return TranslateToGenericICollection(
				(ICollection<TFrom>)fromList, toInstanceOfType);
		}

		public static ICollection<TTo> TranslateToGenericICollection(
			ICollection<TFrom> fromList, Type toInstanceOfType)
		{
			if (fromList == null) return null; //AOT

			var to = (ICollection<TTo>)TranslateListWithElements<TTo>.CreateInstance(toInstanceOfType);

			foreach (var item in fromList)
			{
				var toItem = ConvertFn(item);
				to.Add(toItem);
			}
			return to;
		}

		private static Func<TFrom, TTo> GetConvertFn()
		{
			if (typeof(TTo) == typeof(string))
			{
				return x => (TTo)(object)TypeSerializer.SerializeToString(x);
			}
			if (typeof(TFrom) == typeof(string))
			{
				return x => TypeSerializer.DeserializeFromString<TTo>((string)(object)x);
			}
			return x => TypeSerializer.DeserializeFromString<TTo>(TypeSerializer.SerializeToString(x));
		}
	}
}

#endregion TranslateListWithElements.cs

/// ********   File: \TypeConfig.cs
#region TypeConfig.cs

namespace ServiceStack.Text
{
	internal class TypeConfig
	{
		internal readonly Type Type;
		internal bool EnableAnonymousFieldSetterses;
		internal PropertyInfo[] Properties;

		internal TypeConfig(Type type)
		{
			Type = type;
			EnableAnonymousFieldSetterses = false;
			Properties = new PropertyInfo[0];
		}
	}

	public static class TypeConfig<T>
	{
		private static readonly TypeConfig config;

		public static PropertyInfo[] Properties
		{
			get { return config.Properties; }
			set { config.Properties = value; }
		}

		public static bool EnableAnonymousFieldSetters
		{
			get { return config.EnableAnonymousFieldSetterses; }
			set { config.EnableAnonymousFieldSetterses = value; }
		}

		static TypeConfig()
		{
			config = new TypeConfig(typeof(T));
			
			var excludedProperties = JsConfig<T>.ExcludePropertyNames ?? new string[0];
			var properties = excludedProperties.Any() 
				? config.Type.GetSerializableProperties().Where(x => !excludedProperties.Contains(x.Name))
				: config.Type.GetSerializableProperties();
			Properties = properties.Where(x => x.GetIndexParameters().Length == 0).ToArray();
		}

		internal static TypeConfig GetState()
		{
			return config;
		}
	}
}
#endregion TypeConfig.cs

/// ********   File: \TypeSerializer.cs
#region TypeSerializer.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	/// <summary>
	/// Creates an instance of a Type from a string value
	/// </summary>
	public static class TypeSerializer
	{
		private static readonly UTF8Encoding UTF8EncodingWithoutBom = new UTF8Encoding(false);

		public const string DoubleQuoteString = "\"\"";

		/// <summary>
		/// Determines whether the specified type is convertible from string.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> if the specified type is convertible from string; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanCreateFromString(Type type)
		{
			return JsvReader.GetParseFn(type) != null;
		}

		/// <summary>
		/// Parses the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static T DeserializeFromString<T>(string value)
		{
			if (string.IsNullOrEmpty(value)) return default(T);
			return (T)JsvReader<T>.Parse(value);
		}

		public static T DeserializeFromReader<T>(TextReader reader)
		{
			return DeserializeFromString<T>(reader.ReadToEnd());
		}

		/// <summary>
		/// Parses the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static object DeserializeFromString(string value, Type type)
		{
			return value == null 
			       	? null 
			       	: JsvReader.GetParseFn(type)(value);
		}

		public static object DeserializeFromReader(TextReader reader, Type type)
		{
			return DeserializeFromString(reader.ReadToEnd(), type);
		}

		public static string SerializeToString<T>(T value)
		{
			if (value == null) return null;
			if (typeof(T) == typeof(string)) return value as string;
            if (typeof(T) == typeof(object) || typeof(T).IsAbstract || typeof(T).IsInterface)
            {
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = true;
                var result = SerializeToString(value, value.GetType());
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = false;
                return result;
            }

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				JsvWriter<T>.WriteObject(writer, value);
			}
			return sb.ToString();
		}

		public static string SerializeToString(object value, Type type)
		{
			if (value == null) return null;
			if (type == typeof(string)) return value as string;

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
			{
				JsvWriter.GetWriteFn(type)(writer, value);
			}
			return sb.ToString();
		}

		public static void SerializeToWriter<T>(T value, TextWriter writer)
		{
			if (value == null) return;
			if (typeof(T) == typeof(string))
			{
				writer.Write(value);
				return;
			}
			if (typeof(T) == typeof(object))
			{
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = true;
                SerializeToWriter(value, value.GetType(), writer);
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = false;
                return;
			}

			JsvWriter<T>.WriteObject(writer, value);
		}

		public static void SerializeToWriter(object value, Type type, TextWriter writer)
		{
			if (value == null) return;
			if (type == typeof(string))
			{
				writer.Write(value);
				return;
			}

			JsvWriter.GetWriteFn(type)(writer, value);
		}

		public static void SerializeToStream<T>(T value, Stream stream)
		{
			if (value == null) return;
			if (typeof(T) == typeof(object))
			{
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = true;
                SerializeToStream(value, value.GetType(), stream);
                if (typeof(T).IsAbstract || typeof(T).IsInterface) JsState.IsWritingDynamic = false;
                return;
			}

			var writer = new StreamWriter(stream, UTF8EncodingWithoutBom);
			JsvWriter<T>.WriteObject(writer, value);
			writer.Flush();
		}

		public static void SerializeToStream(object value, Type type, Stream stream)
		{
			var writer = new StreamWriter(stream, UTF8EncodingWithoutBom);
			JsvWriter.GetWriteFn(type)(writer, value);
			writer.Flush();
		}

		public static T Clone<T>(T value)
		{
			var serializedValue = SerializeToString(value);
			var cloneObj = DeserializeFromString<T>(serializedValue);
			return cloneObj;
		}

		public static T DeserializeFromStream<T>(Stream stream)
		{
			using (var reader = new StreamReader(stream, UTF8EncodingWithoutBom))
			{
				return DeserializeFromString<T>(reader.ReadToEnd());
			}
		}

		public static object DeserializeFromStream(Type type, Stream stream)
		{
			using (var reader = new StreamReader(stream, UTF8EncodingWithoutBom))
			{
				return DeserializeFromString(reader.ReadToEnd(), type);
			}
		}

		/// <summary>
		/// Useful extension method to get the Dictionary[string,string] representation of any POCO type.
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, string> ToStringDictionary<T>(this T obj)
			where T : class
		{
			var jsv = SerializeToString(obj);
			var map = DeserializeFromString<Dictionary<string, string>>(jsv);
			return map;
		}

        /// <summary>
        /// Recursively prints the contents of any POCO object in a human-friendly, readable format
        /// </summary>
        /// <returns></returns>
        public static string Dump<T>(this T instance)
        {
            return SerializeAndFormat(instance);
        }

        /// <summary>
        /// Print Dump to Console.WriteLine
        /// </summary>
        public static void PrintDump<T>(this T instance)
        {
            Console.WriteLine(SerializeAndFormat(instance));
        }

        /// <summary>
        /// Print string.Format to Console.WriteLine
        /// </summary>
        public static void Print(this string text, params object[] args)
        {
            if (args.Length > 0)
                Console.WriteLine(text, args);
            else
                Console.WriteLine(text);
        }

		public static string SerializeAndFormat<T>(this T instance)
		{
			var dtoStr = SerializeToString(instance);
			var formatStr = JsvFormatter.Format(dtoStr);
			return formatStr;
		}
	}
}
#endregion TypeSerializer.cs

/// ********   File: \TypeSerializer.Generic.cs
#region TypeSerializer.Generic.cs
//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

namespace ServiceStack.Text
{
	public class TypeSerializer<T> : ITypeSerializer<T>
	{
		public bool CanCreateFromString(Type type)
		{
			return JsvReader.GetParseFn(type) != null;
		}

		/// <summary>
		/// Parses the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public T DeserializeFromString(string value)
		{
			if (string.IsNullOrEmpty(value)) return default(T);
			return (T)JsvReader<T>.Parse(value);
		}

		public T DeserializeFromReader(TextReader reader)
		{
			return DeserializeFromString(reader.ReadToEnd());
		}

		public string SerializeToString(T value)
		{
			if (value == null) return null;
			if (typeof(T) == typeof(string)) return value as string;

			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				JsvWriter<T>.WriteObject(writer, value);
			}
			return sb.ToString();
		}

		public void SerializeToWriter(T value, TextWriter writer)
		{
			if (value == null) return;
			if (typeof(T) == typeof(string))
			{
				writer.Write(value);
				return;
			}

			JsvWriter<T>.WriteObject(writer, value);
		}
	}
}
#endregion TypeSerializer.Generic.cs

/// ********   File: \WebRequestExtensions.cs
#region WebRequestExtensions.cs

namespace ServiceStack.Text
{
    public static class WebRequestExtensions
    {
        public static string GetJsonFromUrl(this string url, Action<HttpWebResponse> responseFilter = null)
        {
            return url.GetStringFromUrl("application/json", responseFilter);
        }

        public static string GetStringFromUrl(this string url, string acceptContentType = "*/*", Action<HttpWebResponse> responseFilter = null)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            webReq.Accept = acceptContentType;
            webReq.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (var webRes = webReq.GetResponse())
            using (var stream = webRes.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                if (responseFilter != null)
                {
                    responseFilter((HttpWebResponse)webRes);
                }
                return reader.ReadToEnd();
            }
        }

        public static bool Is404(this Exception ex)
        {
            return HasStatus(ex as WebException, HttpStatusCode.NotFound);
        }

        public static HttpStatusCode? GetResponseStatus(this string url)
        {
            try
            {
                var webReq = (HttpWebRequest)WebRequest.Create(url);
                using (var webRes = webReq.GetResponse())
                {
                    var httpRes = webRes as HttpWebResponse;
                    return httpRes != null ? httpRes.StatusCode : (HttpStatusCode?)null;
                }
            }
            catch (Exception ex)
            {
                return ex.GetStatus();
            }
        }

        public static HttpStatusCode? GetStatus(this Exception ex)
        {
            return GetStatus(ex as WebException);
        }

        public static HttpStatusCode? GetStatus(this WebException webEx)
        {
            if (webEx == null) return null;
            var httpRes = webEx.Response as HttpWebResponse;
            return httpRes != null ? httpRes.StatusCode : (HttpStatusCode?)null;
        }

        public static bool HasStatus(this WebException webEx, HttpStatusCode statusCode)
        {
            return GetStatus(webEx) == statusCode;
        }
    }
}
#endregion WebRequestExtensions.cs

/// ********   File: \XmlSerializer.cs
#region XmlSerializer.cs

#if !XBOX360 && !SILVERLIGHT && !WINDOWS_PHONE && !MONOTOUCH
#endif

namespace ServiceStack.Text
{
#if !XBOX
    public class XmlSerializer
    {
        private readonly XmlDictionaryReaderQuotas quotas;
        private static readonly XmlWriterSettings XSettings = new XmlWriterSettings();

        public static XmlSerializer Instance
            = new XmlSerializer(
#if !SILVERLIGHT && !WINDOWS_PHONE && !MONOTOUCH
                new XmlDictionaryReaderQuotas { MaxStringContentLength = 1024 * 1024, }
#endif
);

        public XmlSerializer(XmlDictionaryReaderQuotas quotas=null, bool omitXmlDeclaration = false)
        {
            this.quotas = quotas;
            XSettings.Encoding = new UTF8Encoding(false);
            XSettings.OmitXmlDeclaration = omitXmlDeclaration;
        }

        private static object Deserialize(string xml, Type type, XmlDictionaryReaderQuotas quotas)
        {
            try
            {
#if WINDOWS_PHONE
                using (var reader = XmlDictionaryReader.Create(xml))
                {
                    var serializer = new DataContractSerializer(type);
                    return serializer.ReadObject(reader);
                }
#else
                var bytes = Encoding.UTF8.GetBytes(xml);
                using (var reader = XmlDictionaryReader.CreateTextReader(bytes, quotas))
                {
                    var serializer = new DataContractSerializer(type);
                    return serializer.ReadObject(reader);
                }
#endif
            }
            catch (Exception ex)
            {
                throw new SerializationException("DeserializeDataContract: Error converting type: " + ex.Message, ex);
            }
        }

        public static object DeserializeFromString(string xml, Type type)
        {
            return Deserialize(xml, type, Instance.quotas);
        }

        public static T DeserializeFromString<T>(string xml)
        {
            var type = typeof(T);
            return (T)Deserialize(xml, type, Instance.quotas);
        }

        public static T DeserializeFromReader<T>(TextReader reader)
        {
            return DeserializeFromString<T>(reader.ReadToEnd());
        }

        public static T DeserializeFromStream<T>(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(T));

            return (T)serializer.ReadObject(stream);
        }

        public static object DeserializeFromStream(Type type, Stream stream)
        {
            var serializer = new DataContractSerializer(type);
            return serializer.ReadObject(stream);
        }

        public static string SerializeToString<T>(T from)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var xw = XmlWriter.Create(ms, XSettings))
                    {
                        var serializer = new DataContractSerializer(from.GetType());
                        serializer.WriteObject(xw, from);
                        xw.Flush();
                        ms.Seek(0, SeekOrigin.Begin);
                        var reader = new StreamReader(ms);
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Error serializing object of type {0}", from.GetType().FullName), ex);
            }
        }

        public static void SerializeToWriter<T>(T value, TextWriter writer)
        {
            try
            {
#if !SILVERLIGHT
				using (var xw = new XmlTextWriter(writer))
#else
                using (var xw = XmlWriter.Create(writer))
#endif
                {
                    var serializer = new DataContractSerializer(value.GetType());
                    serializer.WriteObject(xw, value);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Error serializing object of type {0}", value.GetType().FullName), ex);
            }
        }

        public static void SerializeToStream(object obj, Stream stream)
        {
#if !SILVERLIGHT
            using (var xw = new XmlTextWriter(stream, Encoding.UTF8))
#else
            using (var xw = XmlWriter.Create(stream))
#endif
            {
                var serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(xw, obj);
            }
        }

#if !SILVERLIGHT && !MONOTOUCH
        public static void CompressToStream<TXmlDto>(TXmlDto from, Stream stream)
        {
            using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress))
            using (var xw = new XmlTextWriter(deflateStream, Encoding.UTF8))
            {
                var serializer = new DataContractSerializer(from.GetType());
                serializer.WriteObject(xw, from);
                xw.Flush();
            }
        }

        public static byte[] Compress<TXmlDto>(TXmlDto from)
        {
            using (var ms = new MemoryStream())
            {
                CompressToStream(from, ms);

                return ms.ToArray();
            }
        }
#endif

    }
#endif
}

#endregion XmlSerializer.cs


