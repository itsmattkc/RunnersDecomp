namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXReferenceProxy : PBXObject
	{
		private static PropertyCommentChecker checkerData = new PropertyCommentChecker(new string[1]
		{
			"remoteRef/*"
		});

		internal override PropertyCommentChecker checker
		{
			get
			{
				return checkerData;
			}
		}

		public string path
		{
			get
			{
				return GetPropertyString("path");
			}
		}

		public static PBXReferenceProxy Create(string path, string fileType, string remoteRef, string sourceTree)
		{
			PBXReferenceProxy pBXReferenceProxy = new PBXReferenceProxy();
			pBXReferenceProxy.guid = PBXGUID.Generate();
			pBXReferenceProxy.SetPropertyString("isa", "PBXReferenceProxy");
			pBXReferenceProxy.SetPropertyString("path", path);
			pBXReferenceProxy.SetPropertyString("fileType", fileType);
			pBXReferenceProxy.SetPropertyString("remoteRef", remoteRef);
			pBXReferenceProxy.SetPropertyString("sourceTree", sourceTree);
			return pBXReferenceProxy;
		}
	}
}
