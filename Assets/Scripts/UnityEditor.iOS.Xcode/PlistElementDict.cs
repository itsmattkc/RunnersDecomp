using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode
{
	public class PlistElementDict : PlistElement
	{
		private SortedDictionary<string, PlistElement> m_PrivateValue = new SortedDictionary<string, PlistElement>();

		public IDictionary<string, PlistElement> values
		{
			get
			{
				return m_PrivateValue;
			}
		}

		public new PlistElement this[string key]
		{
			get
			{
				if (values.ContainsKey(key))
				{
					return values[key];
				}
				return null;
			}
			set
			{
				values[key] = value;
			}
		}

		public void SetInteger(string key, int val)
		{
			values[key] = new PlistElementInteger(val);
		}

		public void SetString(string key, string val)
		{
			values[key] = new PlistElementString(val);
		}

		public void SetBoolean(string key, bool val)
		{
			values[key] = new PlistElementBoolean(val);
		}

		public PlistElementArray CreateArray(string key)
		{
			PlistElementArray plistElementArray = new PlistElementArray();
			values[key] = plistElementArray;
			return plistElementArray;
		}

		public PlistElementDict CreateDict(string key)
		{
			PlistElementDict plistElementDict = new PlistElementDict();
			values[key] = plistElementDict;
			return plistElementDict;
		}
	}
}
