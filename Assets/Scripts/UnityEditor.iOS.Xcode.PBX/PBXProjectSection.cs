using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXProjectSection : KnownSectionBase<PBXProjectObject>
	{
		public PBXProjectObject project
		{
			get
			{
				using (SortedDictionary<string, PBXProjectObject>.Enumerator enumerator = entries.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current.Value;
					}
				}
				return null;
			}
		}

		public PBXProjectSection()
			: base("PBXProject")
		{
		}
	}
}
