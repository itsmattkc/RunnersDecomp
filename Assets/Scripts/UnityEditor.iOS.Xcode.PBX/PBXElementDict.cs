using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXElementDict : PBXElement
	{
		private SortedDictionary<string, PBXElement> m_PrivateValue = new SortedDictionary<string, PBXElement>();

		public IDictionary<string, PBXElement> values
		{
			get
			{
				return m_PrivateValue;
			}
		}

		public new PBXElement this[string key]
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

		public bool Contains(string key)
		{
			return values.ContainsKey(key);
		}

		public void Remove(string key)
		{
			values.Remove(key);
		}

		public void SetString(string key, string val)
		{
			values[key] = new PBXElementString(val);
		}

		public PBXElementArray CreateArray(string key)
		{
			PBXElementArray pBXElementArray = new PBXElementArray();
			values[key] = pBXElementArray;
			return pBXElementArray;
		}

		public PBXElementDict CreateDict(string key)
		{
			PBXElementDict pBXElementDict = new PBXElementDict();
			values[key] = pBXElementDict;
			return pBXElementDict;
		}
	}
}
