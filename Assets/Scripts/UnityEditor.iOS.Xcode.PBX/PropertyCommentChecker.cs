using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PropertyCommentChecker
	{
		private int m_Level;

		private bool m_All;

		private List<List<string>> m_Props;

		protected PropertyCommentChecker(int level, List<List<string>> props)
		{
			m_Level = level;
			m_All = false;
			m_Props = props;
		}

		public PropertyCommentChecker()
		{
			m_Level = 0;
			m_All = false;
			m_Props = new List<List<string>>();
		}

		public PropertyCommentChecker(IEnumerable<string> props)
		{
			m_Level = 0;
			m_All = false;
			m_Props = new List<List<string>>();
			foreach (string prop in props)
			{
				m_Props.Add(new List<string>(prop.Split('/')));
			}
		}

		private bool CheckContained(string prop)
		{
			if (m_All)
			{
				return true;
			}
			foreach (List<string> prop2 in m_Props)
			{
				if (prop2.Count == m_Level + 1)
				{
					if (prop2[m_Level] == prop)
					{
						return true;
					}
					if (prop2[m_Level] == "*")
					{
						m_All = true;
						return true;
					}
				}
			}
			return false;
		}

		public bool CheckStringValueInArray(string value)
		{
			return CheckContained(value);
		}

		public bool CheckKeyInDict(string key)
		{
			return CheckContained(key);
		}

		public bool CheckStringValueInDict(string key, string value)
		{
			foreach (List<string> prop in m_Props)
			{
				if (prop.Count == m_Level + 2 && (((prop[m_Level] == "*" || prop[m_Level] == key) && prop[m_Level + 1] == "*") || prop[m_Level + 1] == value))
				{
					return true;
				}
			}
			return false;
		}

		public PropertyCommentChecker NextLevel(string prop)
		{
			List<List<string>> list = new List<List<string>>();
			foreach (List<string> prop2 in m_Props)
			{
				if (prop2.Count > m_Level + 1 && (prop2[m_Level] == "*" || prop2[m_Level] == prop))
				{
					list.Add(prop2);
				}
			}
			return new PropertyCommentChecker(m_Level + 1, list);
		}
	}
}
