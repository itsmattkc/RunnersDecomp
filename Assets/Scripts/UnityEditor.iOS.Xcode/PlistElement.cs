namespace UnityEditor.iOS.Xcode
{
	public class PlistElement
	{
		public PlistElement this[string key]
		{
			get
			{
				return AsDict()[key];
			}
			set
			{
				AsDict()[key] = value;
			}
		}

		protected PlistElement()
		{
		}

		public string AsString()
		{
			return ((PlistElementString)this).value;
		}

		public int AsInteger()
		{
			return ((PlistElementInteger)this).value;
		}

		public bool AsBoolean()
		{
			return ((PlistElementBoolean)this).value;
		}

		public PlistElementArray AsArray()
		{
			return (PlistElementArray)this;
		}

		public PlistElementDict AsDict()
		{
			return (PlistElementDict)this;
		}
	}
}
