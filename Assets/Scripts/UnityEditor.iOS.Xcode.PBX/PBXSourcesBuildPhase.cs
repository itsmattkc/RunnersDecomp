using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXSourcesBuildPhase : FileGUIDListBase
	{
		public static PBXSourcesBuildPhase Create()
		{
			PBXSourcesBuildPhase pBXSourcesBuildPhase = new PBXSourcesBuildPhase();
			pBXSourcesBuildPhase.guid = PBXGUID.Generate();
			pBXSourcesBuildPhase.SetPropertyString("isa", "PBXSourcesBuildPhase");
			pBXSourcesBuildPhase.SetPropertyString("buildActionMask", "2147483647");
			pBXSourcesBuildPhase.files = new List<string>();
			pBXSourcesBuildPhase.SetPropertyString("runOnlyForDeploymentPostprocessing", "0");
			return pBXSourcesBuildPhase;
		}
	}
}
