using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using mgModel;

namespace mgCustom
{
	/// <summary>
	/// Generic class to hold values for DropDowns, ListBoxes, and ComboBoxes
	/// </summary>
	public class GenericListItem
	{
		private string _key = string.Empty;
		private string _value = string.Empty;
		private string _extValue = string.Empty;

		public GenericListItem(string Key, string Value)
		{
			_key = Key;
			_value = Value;
		}

		public GenericListItem(string Key, string Value, string ExtendedValue)
		{
			_key = Key;
			_value = Value;
			_extValue = ExtendedValue;
		}

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public string ExtendedValue
		{
			get { return _extValue; }
			set { _extValue = value; }
		}

		public override string ToString()
		{
			return (!String.IsNullOrEmpty(_value) ? _value : "");
		}
	}

	public class GenericListItemCollection : List<GenericListItem>
	{
		//public void Add(GenericListItem element)
		//{
		//    List.Add(element);
		//}

		//public GenericListItem this[int index]
		//{
		//    get { return (GenericListItem)List[index];	}
		//}

		public GenericListItem this[string key]
		{
			get
			{
				GenericListItem val = null;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].Key.ToLower() == key.ToLower())
					{
						val = this[i];
						break;
					}
				}
				return val;
			}
		}

		/// <summary>
		/// Return the value at index in the list
		/// </summary>
		/// <param name="index">The index to find</param>
		/// <returns>The string in value</returns>
		public string Get(int index)
		{
			string val = string.Empty;
			if (index < this.Count) { val = this[index].Value; }
			return val;
		}

		/// <summary>
		/// Return the value with the key = to string in the list
		/// </summary>
		/// <param name="index">The index to find</param>
		/// <returns>The string in value</returns>
		public string Get(string index)
		{
			string val = string.Empty;
			if (this.KeyExists(index)) { val = this[index].Value; }
			return val;
		}

		//public void Remove(GenericListItem element)
		//{
		//    List.Remove(element);
		//}

		//public void AddRange(GenericListItem[] elementArray)
		//{
		//    for (int i = 0; i < elementArray.Length; i++)
		//    {
		//        List.Add(elementArray[i]);
		//    }
		//}	

		public GenericListItem[] ToItemArray()
		{
			GenericListItem[] list = new GenericListItem[this.Count];
			for (int i = 0; i < this.Count; i++)
			{
				list[i] = this[i];
			}
			return list;
		}

		/// <summary>
		/// Get the collection sorted by the value
		/// </summary>
		/// <returns>A copy of the collection sorted by value</returns>
		public SortedList GetSortedByValue()
		{
			SortedList coll = new SortedList();
			for (int i = 0; i < this.Count; i++)
			{
				coll.Add(this[i].Value + i.ToString(), this[i]);
			}
			return coll;
		}

		/// <summary>
		/// Get the collection sorted by the key
		/// </summary>
		/// <returns>A copy of the collection sorted by key</returns>
		public SortedList GetSortedByKey()
		{
			SortedList coll = new SortedList();
			for (int i = 0; i < this.Count; i++)
			{
				coll.Add(this[i].Key + i.ToString(), this[i]);
			}
			return coll;
		}

		/// <summary>
		/// Return whether or not the object exists in the collection
		/// </summary>
		/// <param name="key">The key to find in the collection</param>
		/// <returns>Whether or not the key exists</returns>
		public bool KeyExists(string key)
		{
			bool exists = false;
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Key.ToLower() == key.ToLower())
				{
					exists = true;
					break;
				}
			}
			return exists;
		}

		/// <summary>
		/// Remove the element from the collection where the key exists
		/// </summary>
		/// <param name="key">The key to find</param>
		public void RemoveByKey(string key)
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				if (this[i].Key.ToLower() == key.ToLower())
				{
					this.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Insert the item at the selected index position in the collection
		/// </summary>
		/// <param name="item">The item to insert</param>
		/// <param name="index">The index to insert the item at</param>
		public void InsertAt(GenericListItem item, int index)
		{
			this.Insert(index, item);
			//List.Insert(index, item);
		}

		/// <summary>
		/// Set the value in the collection based on the key
		/// Creates the object in the collection if it doesn't already exist
		/// </summary>
		/// <param name="key">The key to update the value for</param>
		/// <param name="newValue">The value of the new key</param>
		public void SetByKey(string key, string newValue)
		{
			if (!this.KeyExists(key))
			{
				// Create the key
				this.Add(new GenericListItem(key, newValue));
			}
			else
			{
				this[key].Value = newValue;
			}
		}

		/// <summary>
		/// Sorts the collection in memory.
		/// </summary>
		public GenericListItemCollection Sort(string orderBy)
		{
			GenericListItemCollection list = this;
			DynamicComparer<GenericListItem> comparer = new DynamicComparer<GenericListItem>(orderBy);
			list.Sort(comparer.Compare);

			return list;
		}
	}

	public class GenericListObjectCollection : List<GenericListObject>
	{

	}

	public class GenericListObject
	{
		private object _key = null;
		private object _value = null;
		private object _tag = null;
		private string _stringRepresentation = string.Empty;

		public GenericListObject(object Key, object Value)
		{
			_key = Key;
			_value = Value;
		}

		public GenericListObject(object Key, object Value, object Tag)
		{
			_key = Key;
			_value = Value;
			_tag = Tag;
		}

		public GenericListObject(object Key, object Value, object Tag, string stringRepresentation)
		{
			_key = Key;
			_value = Value;
			_tag = Tag;
			_stringRepresentation = stringRepresentation;
		}

		public GenericListObject(object Key, object Value, string stringRepresentation)
		{
			_key = Key;
			_value = Value;
			_stringRepresentation = stringRepresentation;
		}

		public object Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		public string StringRepresentation
		{
			get { return _stringRepresentation; }
			set { _stringRepresentation = value; }
		}

		public override string ToString()
		{
			return (!String.IsNullOrEmpty(_stringRepresentation) ? _stringRepresentation : _value.ToString());
		}
	}
}
