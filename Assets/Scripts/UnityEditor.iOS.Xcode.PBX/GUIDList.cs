using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class GUIDList : IEnumerable<string>, IEnumerable
	{
		private List<string> m_List = new List<string>();

		public int Count
		{
			get
			{
				return m_List.Count;
			}
		}

		public GUIDList()
		{
		}

		public GUIDList(List<string> data)
		{
			m_List = data;
		}

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return m_List.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_List.GetEnumerator();
		}

		public void AddGUID(string guid)
		{
			m_List.Add(guid);
		}

		public void RemoveGUID(string guid)
		{
			m_List.RemoveAll((string x) => x == guid);
		}

		public bool Contains(string guid)
		{
			return m_List.Contains(guid);
		}

		public void Clear()
		{
			m_List.Clear();
		}

		public static implicit operator List<string>(GUIDList list)
		{
			return list.m_List;
		}

		public static implicit operator GUIDList(List<string> data)
		{
			return new GUIDList(data);
		}
	}
}
