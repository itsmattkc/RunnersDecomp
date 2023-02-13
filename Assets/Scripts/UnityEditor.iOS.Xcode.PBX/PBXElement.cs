namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXElement
	{
		public PBXElement this[string key]
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

		protected PBXElement()
		{
		}

		public string AsString()
		{
			return ((PBXElementString)this).value;
		}

		public PBXElementArray AsArray()
		{
			return (PBXElementArray)this;
		}

		public PBXElementDict AsDict()
		{
			return (PBXElementDict)this;
		}
	}
}
