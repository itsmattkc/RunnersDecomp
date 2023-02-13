using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXNativeTarget : PBXObject
	{
		public GUIDList phases;

		public string buildConfigList;

		public string name;

		public GUIDList dependencies;

		private static PropertyCommentChecker checkerData = new PropertyCommentChecker(new string[5]
		{
			"buildPhases/*",
			"buildRules/*",
			"dependencies/*",
			"productReference/*",
			"buildConfigurationList/*"
		});

		internal override PropertyCommentChecker checker
		{
			get
			{
				return checkerData;
			}
		}

		public static PBXNativeTarget Create(string name, string productRef, string productType, string buildConfigList)
		{
			PBXNativeTarget pBXNativeTarget = new PBXNativeTarget();
			pBXNativeTarget.guid = PBXGUID.Generate();
			pBXNativeTarget.SetPropertyString("isa", "PBXNativeTarget");
			pBXNativeTarget.buildConfigList = buildConfigList;
			pBXNativeTarget.phases = new GUIDList();
			pBXNativeTarget.SetPropertyList("buildRules", new List<string>());
			pBXNativeTarget.dependencies = new GUIDList();
			pBXNativeTarget.name = name;
			pBXNativeTarget.SetPropertyString("productName", name);
			pBXNativeTarget.SetPropertyString("productReference", productRef);
			pBXNativeTarget.SetPropertyString("productType", productType);
			return pBXNativeTarget;
		}

		public override void UpdateProps()
		{
			SetPropertyString("buildConfigurationList", buildConfigList);
			SetPropertyString("name", name);
			SetPropertyList("buildPhases", phases);
			SetPropertyList("dependencies", dependencies);
		}

		public override void UpdateVars()
		{
			buildConfigList = GetPropertyString("buildConfigurationList");
			name = GetPropertyString("name");
			phases = GetPropertyList("buildPhases");
			dependencies = GetPropertyList("dependencies");
		}
	}
}
