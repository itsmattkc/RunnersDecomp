using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXResourcesBuildPhase : FileGUIDListBase
	{
		public static PBXResourcesBuildPhase Create()
		{
			PBXResourcesBuildPhase pBXResourcesBuildPhase = new PBXResourcesBuildPhase();
			pBXResourcesBuildPhase.guid = PBXGUID.Generate();
			pBXResourcesBuildPhase.SetPropertyString("isa", "PBXResourcesBuildPhase");
			pBXResourcesBuildPhase.SetPropertyString("buildActionMask", "2147483647");
			pBXResourcesBuildPhase.files = new List<string>();
			pBXResourcesBuildPhase.SetPropertyString("runOnlyForDeploymentPostprocessing", "0");
			return pBXResourcesBuildPhase;
		}
	}
}
