using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXElementArray : PBXElement
	{
		public List<PBXElement> values = new List<PBXElement>();

		public void AddString(string val)
		{
			values.Add(new PBXElementString(val));
		}

		public PBXElementArray AddArray()
		{
			PBXElementArray pBXElementArray = new PBXElementArray();
			values.Add(pBXElementArray);
			return pBXElementArray;
		}

		public PBXElementDict AddDict()
		{
			PBXElementDict pBXElementDict = new PBXElementDict();
			values.Add(pBXElementDict);
			return pBXElementDict;
		}
	}
}
