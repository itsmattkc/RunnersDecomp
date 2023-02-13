using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class GUIDToCommentMap
	{
		private Dictionary<string, string> m_Dict = new Dictionary<string, string>();

		public string this[string guid]
		{
			get
			{
				if (m_Dict.ContainsKey(guid))
				{
					return m_Dict[guid];
				}
				return null;
			}
		}

		public void Add(string guid, string comment)
		{
			if (!m_Dict.ContainsKey(guid))
			{
				m_Dict.Add(guid, comment);
			}
		}

		public void Remove(string guid)
		{
			m_Dict.Remove(guid);
		}

		public string Write(string guid)
		{
			string text = this[guid];
			if (text == null)
			{
				return guid;
			}
			return string.Format("{0} /* {1} */", guid, text);
		}
	}
}
