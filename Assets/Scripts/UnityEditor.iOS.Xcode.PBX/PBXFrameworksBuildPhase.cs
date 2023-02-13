using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXFrameworksBuildPhase : FileGUIDListBase
	{
		public static PBXFrameworksBuildPhase Create()
		{
			PBXFrameworksBuildPhase pBXFrameworksBuildPhase = new PBXFrameworksBuildPhase();
			pBXFrameworksBuildPhase.guid = PBXGUID.Generate();
			pBXFrameworksBuildPhase.SetPropertyString("isa", "PBXFrameworksBuildPhase");
			pBXFrameworksBuildPhase.SetPropertyString("buildActionMask", "2147483647");
			pBXFrameworksBuildPhase.files = new List<string>();
			pBXFrameworksBuildPhase.SetPropertyString("runOnlyForDeploymentPostprocessing", "0");
			return pBXFrameworksBuildPhase;
		}
	}
}
