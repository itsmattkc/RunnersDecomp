using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXObject
	{
		public string guid;

		protected PBXElementDict m_Properties = new PBXElementDict();

		private static PropertyCommentChecker checkerData = new PropertyCommentChecker();

		internal virtual PropertyCommentChecker checker
		{
			get
			{
				return checkerData;
			}
		}

		internal virtual bool shouldCompact
		{
			get
			{
				return false;
			}
		}

		internal void SetPropertiesWhenSerializing(PBXElementDict props)
		{
			m_Properties = props;
		}

		internal PBXElementDict GetPropertiesWhenSerializing()
		{
			return m_Properties;
		}

		protected string GetPropertyString(string name)
		{
			PBXElement pBXElement = m_Properties[name];
			if (pBXElement == null)
			{
				return null;
			}
			return pBXElement.AsString();
		}

		protected void SetPropertyString(string name, string value)
		{
			if (value == null)
			{
				m_Properties.Remove(name);
			}
			else
			{
				m_Properties.SetString(name, value);
			}
		}

		protected List<string> GetPropertyList(string name)
		{
			PBXElement pBXElement = m_Properties[name];
			if (pBXElement == null)
			{
				return null;
			}
			List<string> list = new List<string>();
			foreach (PBXElement value in pBXElement.AsArray().values)
			{
				list.Add(value.AsString());
			}
			return list;
		}

		protected void SetPropertyList(string name, List<string> value)
		{
			if (value == null)
			{
				m_Properties.Remove(name);
				return;
			}
			PBXElementArray pBXElementArray = m_Properties.CreateArray(name);
			foreach (string item in value)
			{
				pBXElementArray.AddString(item);
			}
		}

		public virtual void UpdateProps()
		{
		}

		public virtual void UpdateVars()
		{
		}
	}
}
