using System.Collections.Generic;
using System.Text;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class KnownSectionBase<T> : SectionBase where T : PBXObject, new()
	{
		public SortedDictionary<string, T> entries = new SortedDictionary<string, T>();

		private string m_Name;

		public T this[string guid]
		{
			get
			{
				if (entries.ContainsKey(guid))
				{
					return entries[guid];
				}
				return (T)null;
			}
		}

		public KnownSectionBase(string sectionName)
		{
			m_Name = sectionName;
		}

		public override void AddObject(string key, PBXElementDict value)
		{
			T val = new T();
			val.guid = key;
			val.SetPropertiesWhenSerializing(value);
			val.UpdateVars();
			entries[val.guid] = val;
		}

		public override void WriteSection(StringBuilder sb, GUIDToCommentMap comments)
		{
			if (entries.Count == 0)
			{
				return;
			}
			sb.AppendFormat("\n\n/* Begin {0} section */", m_Name);
			foreach (T value in entries.Values)
			{
				T current = value;
				current.UpdateProps();
				sb.AppendFormat("\n\t\t{0} = ", comments.Write(current.guid));
				Serializer.WriteDict(sb, current.GetPropertiesWhenSerializing(), 2, current.shouldCompact, current.checker, comments);
				sb.Append(";");
			}
			sb.AppendFormat("\n/* End {0} section */", m_Name);
		}

		public void AddEntry(T obj)
		{
			entries[obj.guid] = obj;
		}

		public void RemoveEntry(string guid)
		{
			if (entries.ContainsKey(guid))
			{
				entries.Remove(guid);
			}
		}
	}
}
