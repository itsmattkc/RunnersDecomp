namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXBuildFile : PBXObject
	{
		public string fileRef;

		public string compileFlags;

		public bool weak;

		private static PropertyCommentChecker checkerData = new PropertyCommentChecker(new string[1]
		{
			"fileRef/*"
		});

		internal override PropertyCommentChecker checker
		{
			get
			{
				return checkerData;
			}
		}

		internal override bool shouldCompact
		{
			get
			{
				return true;
			}
		}

		public static PBXBuildFile CreateFromFile(string fileRefGUID, bool weak, string compileFlags)
		{
			PBXBuildFile pBXBuildFile = new PBXBuildFile();
			pBXBuildFile.guid = PBXGUID.Generate();
			pBXBuildFile.SetPropertyString("isa", "PBXBuildFile");
			pBXBuildFile.fileRef = fileRefGUID;
			pBXBuildFile.compileFlags = compileFlags;
			pBXBuildFile.weak = weak;
			return pBXBuildFile;
		}

		private PBXElementDict GetSettingsDict()
		{
			if (m_Properties.Contains("settings"))
			{
				return m_Properties["settings"].AsDict();
			}
			return m_Properties.CreateDict("settings");
		}

		public override void UpdateProps()
		{
			SetPropertyString("fileRef", fileRef);
			if (compileFlags != null && compileFlags != string.Empty)
			{
				GetSettingsDict().SetString("COMPILER_FLAGS", compileFlags);
			}
			if (!weak)
			{
				return;
			}
			PBXElementDict settingsDict = GetSettingsDict();
			PBXElementArray pBXElementArray = null;
			pBXElementArray = ((!settingsDict.Contains("ATTRIBUTES")) ? settingsDict.CreateArray("ATTRIBUTES") : settingsDict["ATTRIBUTES"].AsArray());
			bool flag = false;
			foreach (PBXElement value in pBXElementArray.values)
			{
				if (value is PBXElementString && value.AsString() == "Weak")
				{
					flag = true;
				}
			}
			if (!flag)
			{
				pBXElementArray.AddString("Weak");
			}
		}

		public override void UpdateVars()
		{
			fileRef = GetPropertyString("fileRef");
			compileFlags = null;
			weak = false;
			if (!m_Properties.Contains("settings"))
			{
				return;
			}
			PBXElementDict pBXElementDict = m_Properties["settings"].AsDict();
			if (pBXElementDict.Contains("COMPILER_FLAGS"))
			{
				compileFlags = pBXElementDict["COMPILER_FLAGS"].AsString();
			}
			if (!pBXElementDict.Contains("ATTRIBUTES"))
			{
				return;
			}
			PBXElementArray pBXElementArray = pBXElementDict["ATTRIBUTES"].AsArray();
			foreach (PBXElement value in pBXElementArray.values)
			{
				if (value is PBXElementString && value.AsString() == "Weak")
				{
					weak = true;
				}
			}
		}
	}
}
