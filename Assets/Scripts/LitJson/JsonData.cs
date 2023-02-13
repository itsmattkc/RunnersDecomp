using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace LitJson
{
	public class JsonData : IList, ICollection, IDictionary, IEnumerable, IOrderedDictionary, IEquatable<JsonData>, IJsonWrapper
	{
		private IList<JsonData> inst_array;

		private bool inst_boolean;

		private double inst_double;

		private int inst_int;

		private long inst_long;

		private IDictionary<string, JsonData> inst_object;

		private string inst_string;

		private string json;

		private JsonType type;

		private IList<KeyValuePair<string, JsonData>> object_list;

		int ICollection.Count
		{
			get
			{
				return Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return EnsureCollection().IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return EnsureCollection().SyncRoot;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return EnsureDictionary().IsFixedSize;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return EnsureDictionary().IsReadOnly;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				EnsureDictionary();
				IList<string> list = new List<string>();
				foreach (KeyValuePair<string, JsonData> item in object_list)
				{
					list.Add(item.Key);
				}
				return (ICollection)list;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				EnsureDictionary();
				IList<JsonData> list = new List<JsonData>();
				foreach (KeyValuePair<string, JsonData> item in object_list)
				{
					list.Add(item.Value);
				}
				return (ICollection)list;
			}
		}

		bool IJsonWrapper.IsArray
		{
			get
			{
				return IsArray;
			}
		}

		bool IJsonWrapper.IsBoolean
		{
			get
			{
				return IsBoolean;
			}
		}

		bool IJsonWrapper.IsDouble
		{
			get
			{
				return IsDouble;
			}
		}

		bool IJsonWrapper.IsInt
		{
			get
			{
				return IsInt;
			}
		}

		bool IJsonWrapper.IsLong
		{
			get
			{
				return IsLong;
			}
		}

		bool IJsonWrapper.IsObject
		{
			get
			{
				return IsObject;
			}
		}

		bool IJsonWrapper.IsString
		{
			get
			{
				return IsString;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return EnsureList().IsFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return EnsureList().IsReadOnly;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return EnsureDictionary()[key];
			}
			set
			{
				if (!(key is string))
				{
					throw new ArgumentException("The key has to be a string");
				}
				JsonData value2 = ToJsonData(value);
				this[(string)key] = value2;
			}
		}

		object IOrderedDictionary.this[int idx]
		{
			get
			{
				EnsureDictionary();
				return object_list[idx].Value;
			}
			set
			{
				EnsureDictionary();
				JsonData value2 = ToJsonData(value);
				KeyValuePair<string, JsonData> keyValuePair = object_list[idx];
				inst_object[keyValuePair.Key] = value2;
				KeyValuePair<string, JsonData> value3 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value2);
				object_list[idx] = value3;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return EnsureList()[index];
			}
			set
			{
				EnsureList();
				JsonData jsonData2 = this[index] = ToJsonData(value);
			}
		}

		public int Count
		{
			get
			{
				return EnsureCollection().Count;
			}
		}

		public bool IsArray
		{
			get
			{
				return type == JsonType.Array;
			}
		}

		public bool IsBoolean
		{
			get
			{
				return type == JsonType.Boolean;
			}
		}

		public bool IsDouble
		{
			get
			{
				return type == JsonType.Double;
			}
		}

		public bool IsInt
		{
			get
			{
				return type == JsonType.Int;
			}
		}

		public bool IsLong
		{
			get
			{
				return type == JsonType.Long;
			}
		}

		public bool IsObject
		{
			get
			{
				return type == JsonType.Object;
			}
		}

		public bool IsString
		{
			get
			{
				return type == JsonType.String;
			}
		}

		public JsonData this[string prop_name]
		{
			get
			{
				EnsureDictionary();
				return inst_object[prop_name];
			}
			set
			{
				EnsureDictionary();
				KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(prop_name, value);
				if (inst_object.ContainsKey(prop_name))
				{
					for (int i = 0; i < object_list.Count; i++)
					{
						if (object_list[i].Key == prop_name)
						{
							object_list[i] = keyValuePair;
							break;
						}
					}
				}
				else
				{
					object_list.Add(keyValuePair);
				}
				inst_object[prop_name] = value;
				json = null;
			}
		}

		public JsonData this[int index]
		{
			get
			{
				EnsureCollection();
				if (type == JsonType.Array)
				{
					return inst_array[index];
				}
				return object_list[index].Value;
			}
			set
			{
				EnsureCollection();
				if (type == JsonType.Array)
				{
					inst_array[index] = value;
				}
				else
				{
					KeyValuePair<string, JsonData> keyValuePair = object_list[index];
					KeyValuePair<string, JsonData> value2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value);
					object_list[index] = value2;
					inst_object[keyValuePair.Key] = value;
				}
				json = null;
			}
		}

		public JsonData()
		{
		}

		public JsonData(bool boolean)
		{
			type = JsonType.Boolean;
			inst_boolean = boolean;
		}

		public JsonData(double number)
		{
			type = JsonType.Double;
			inst_double = number;
		}

		public JsonData(int number)
		{
			type = JsonType.Int;
			inst_int = number;
		}

		public JsonData(long number)
		{
			type = JsonType.Long;
			inst_long = number;
		}

		public JsonData(object obj)
		{
			if (obj is bool)
			{
				type = JsonType.Boolean;
				inst_boolean = (bool)obj;
				return;
			}
			if (obj is double)
			{
				type = JsonType.Double;
				inst_double = (double)obj;
				return;
			}
			if (obj is int)
			{
				type = JsonType.Int;
				inst_int = (int)obj;
				return;
			}
			if (obj is long)
			{
				type = JsonType.Long;
				inst_long = (long)obj;
				return;
			}
			if (obj is string)
			{
				type = JsonType.String;
				inst_string = (string)obj;
				return;
			}
			throw new ArgumentException("Unable to wrap the given object with JsonData");
		}

		public JsonData(string str)
		{
			type = JsonType.String;
			inst_string = str;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			EnsureCollection().CopyTo(array, index);
		}

		void IDictionary.Add(object key, object value)
		{
			JsonData value2 = ToJsonData(value);
			EnsureDictionary().Add(key, value2);
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>((string)key, value2);
			object_list.Add(item);
			json = null;
		}

		void IDictionary.Clear()
		{
			EnsureDictionary().Clear();
			object_list.Clear();
			json = null;
		}

		bool IDictionary.Contains(object key)
		{
			return EnsureDictionary().Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IOrderedDictionary)this).GetEnumerator();
		}

		void IDictionary.Remove(object key)
		{
			EnsureDictionary().Remove(key);
			for (int i = 0; i < object_list.Count; i++)
			{
				if (object_list[i].Key == (string)key)
				{
					object_list.RemoveAt(i);
					break;
				}
			}
			json = null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return EnsureCollection().GetEnumerator();
		}

		bool IJsonWrapper.GetBoolean()
		{
			if (type != JsonType.Boolean)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
			}
			return inst_boolean;
		}

		double IJsonWrapper.GetDouble()
		{
			if (type != JsonType.Double)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a double");
			}
			return inst_double;
		}

		int IJsonWrapper.GetInt()
		{
			if (type != JsonType.Int)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold an int");
			}
			return inst_int;
		}

		long IJsonWrapper.GetLong()
		{
			if (type != JsonType.Long)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a long");
			}
			return inst_long;
		}

		string IJsonWrapper.GetString()
		{
			if (type != JsonType.String)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a string");
			}
			return inst_string;
		}

		void IJsonWrapper.SetBoolean(bool val)
		{
			type = JsonType.Boolean;
			inst_boolean = val;
			json = null;
		}

		void IJsonWrapper.SetDouble(double val)
		{
			type = JsonType.Double;
			inst_double = val;
			json = null;
		}

		void IJsonWrapper.SetInt(int val)
		{
			type = JsonType.Int;
			inst_int = val;
			json = null;
		}

		void IJsonWrapper.SetLong(long val)
		{
			type = JsonType.Long;
			inst_long = val;
			json = null;
		}

		void IJsonWrapper.SetString(string val)
		{
			type = JsonType.String;
			inst_string = val;
			json = null;
		}

		string IJsonWrapper.ToJson()
		{
			return ToJson();
		}

		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			ToJson(writer);
		}

		int IList.Add(object value)
		{
			return Add(value);
		}

		void IList.Clear()
		{
			EnsureList().Clear();
			json = null;
		}

		bool IList.Contains(object value)
		{
			return EnsureList().Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return EnsureList().IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			EnsureList().Insert(index, value);
			json = null;
		}

		void IList.Remove(object value)
		{
			EnsureList().Remove(value);
			json = null;
		}

		void IList.RemoveAt(int index)
		{
			EnsureList().RemoveAt(index);
			json = null;
		}

		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			EnsureDictionary();
			return new OrderedDictionaryEnumerator(object_list.GetEnumerator());
		}

		void IOrderedDictionary.Insert(int idx, object key, object value)
		{
			string text = (string)key;
			JsonData value2 = this[text] = ToJsonData(value);
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>(text, value2);
			object_list.Insert(idx, item);
		}

		void IOrderedDictionary.RemoveAt(int idx)
		{
			EnsureDictionary();
			inst_object.Remove(object_list[idx].Key);
			object_list.RemoveAt(idx);
		}

		public string GetKey(int index)
		{
			EnsureCollection();
			if (type == JsonType.Array)
			{
				return inst_array[index].GetKey(0);
			}
			return object_list[index].Key;
		}

		public bool ContainsKey(string prop_name)
		{
			return inst_object.ContainsKey(prop_name);
		}

		private ICollection EnsureCollection()
		{
			if (type == JsonType.Array)
			{
				return (ICollection)inst_array;
			}
			if (type == JsonType.Object)
			{
				return (ICollection)inst_object;
			}
			throw new InvalidOperationException("The JsonData instance has to be initialized first");
		}

		private IDictionary EnsureDictionary()
		{
			if (type == JsonType.Object)
			{
				return (IDictionary)inst_object;
			}
			if (type != 0)
			{
				throw new InvalidOperationException("Instance of JsonData is not a dictionary");
			}
			type = JsonType.Object;
			inst_object = new Dictionary<string, JsonData>();
			object_list = new List<KeyValuePair<string, JsonData>>();
			return (IDictionary)inst_object;
		}

		private IList EnsureList()
		{
			if (type == JsonType.Array)
			{
				return (IList)inst_array;
			}
			if (type != 0)
			{
				throw new InvalidOperationException("Instance of JsonData is not a list");
			}
			type = JsonType.Array;
			inst_array = new List<JsonData>();
			return (IList)inst_array;
		}

		private JsonData ToJsonData(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is JsonData)
			{
				return (JsonData)obj;
			}
			return new JsonData(obj);
		}

		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj.IsString)
			{
				writer.Write(obj.GetString());
			}
			else if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
			}
			else if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
			}
			else if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
			}
			else if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
			}
			else if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (object item in (IEnumerable)obj)
				{
					WriteJson((JsonData)item, writer);
				}
				writer.WriteArrayEnd();
			}
			else
			{
				if (!obj.IsObject)
				{
					return;
				}
				writer.WriteObjectStart();
				foreach (DictionaryEntry item2 in (IDictionary)obj)
				{
					writer.WritePropertyName((string)item2.Key);
					WriteJson((JsonData)item2.Value, writer);
				}
				writer.WriteObjectEnd();
			}
		}

		public int Add(object value)
		{
			JsonData value2 = ToJsonData(value);
			json = null;
			return EnsureList().Add(value2);
		}

		public void Clear()
		{
			if (IsObject)
			{
				((IDictionary)this).Clear();
			}
			else if (IsArray)
			{
				((IList)this).Clear();
			}
		}

		public bool Equals(JsonData x)
		{
			if (x == null)
			{
				return false;
			}
			if (x.type != type)
			{
				return false;
			}
			switch (type)
			{
			case JsonType.None:
				return true;
			case JsonType.Object:
				return inst_object.Equals(x.inst_object);
			case JsonType.Array:
				return inst_array.Equals(x.inst_array);
			case JsonType.String:
				return inst_string.Equals(x.inst_string);
			case JsonType.Int:
				return inst_int.Equals(x.inst_int);
			case JsonType.Long:
				return inst_long.Equals(x.inst_long);
			case JsonType.Double:
				return inst_double.Equals(x.inst_double);
			case JsonType.Boolean:
				return inst_boolean.Equals(x.inst_boolean);
			default:
				return false;
			}
		}

		public JsonType GetJsonType()
		{
			return type;
		}

		public void SetJsonType(JsonType type)
		{
			if (this.type != type)
			{
				switch (type)
				{
				case JsonType.Object:
					inst_object = new Dictionary<string, JsonData>();
					object_list = new List<KeyValuePair<string, JsonData>>();
					break;
				case JsonType.Array:
					inst_array = new List<JsonData>();
					break;
				case JsonType.String:
					inst_string = null;
					break;
				case JsonType.Int:
					inst_int = 0;
					break;
				case JsonType.Long:
					inst_long = 0L;
					break;
				case JsonType.Double:
					inst_double = 0.0;
					break;
				case JsonType.Boolean:
					inst_boolean = false;
					break;
				}
				this.type = type;
			}
		}

		public string ToJson()
		{
			if (json != null)
			{
				return json;
			}
			StringWriter stringWriter = new StringWriter();
			JsonWriter jsonWriter = new JsonWriter(stringWriter);
			jsonWriter.Validate = false;
			WriteJson(this, jsonWriter);
			json = stringWriter.ToString();
			return json;
		}

		public void ToJson(JsonWriter writer)
		{
			bool validate = writer.Validate;
			writer.Validate = false;
			WriteJson(this, writer);
			writer.Validate = validate;
		}

		public override string ToString()
		{
			switch (type)
			{
			case JsonType.Array:
				return "JsonData array";
			case JsonType.Boolean:
				return inst_boolean.ToString();
			case JsonType.Double:
				return inst_double.ToString();
			case JsonType.Int:
				return inst_int.ToString();
			case JsonType.Long:
				return inst_long.ToString();
			case JsonType.Object:
				return "JsonData object";
			case JsonType.String:
				return inst_string;
			default:
				return "Uninitialized JsonData";
			}
		}

		public static implicit operator JsonData(bool data)
		{
			return new JsonData(data);
		}

		public static implicit operator JsonData(double data)
		{
			return new JsonData(data);
		}

		public static implicit operator JsonData(int data)
		{
			return new JsonData(data);
		}

		public static implicit operator JsonData(long data)
		{
			return new JsonData(data);
		}

		public static implicit operator JsonData(string data)
		{
			return new JsonData(data);
		}

		public static explicit operator bool(JsonData data)
		{
			if (data.type != JsonType.Boolean)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_boolean;
		}

		public static explicit operator double(JsonData data)
		{
			if (data.type != JsonType.Double)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_double;
		}

		public static explicit operator int(JsonData data)
		{
			if (data.type != JsonType.Int)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_int;
		}

		public static explicit operator long(JsonData data)
		{
			if (data.type != JsonType.Long)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_long;
		}

		public static explicit operator string(JsonData data)
		{
			if (data.type != JsonType.String)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a string");
			}
			return data.inst_string;
		}
	}
}
