using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class BuildConfigEntry
	{
		public string name;

		public List<string> val = new List<string>();

		public static string ExtractValue(string src)
		{
			return PBXStream.UnquoteString(src.Trim().TrimEnd(','));
		}

		public void AddValue(string value)
		{
			if (!val.Contains(value))
			{
				val.Add(value);
			}
		}

		public static BuildConfigEntry FromNameValue(string name, string value)
		{
			BuildConfigEntry buildConfigEntry = new BuildConfigEntry();
			buildConfigEntry.name = name;
			buildConfigEntry.AddValue(value);
			return buildConfigEntry;
		}
	}
}
