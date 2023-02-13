namespace UnityEditor.iOS.Xcode.PBX
{
	internal class XCConfigurationList : PBXObject
	{
		public GUIDList buildConfigs;

		private static PropertyCommentChecker checkerData = new PropertyCommentChecker(new string[1]
		{
			"buildConfigurations/*"
		});

		internal override PropertyCommentChecker checker
		{
			get
			{
				return checkerData;
			}
		}

		public static XCConfigurationList Create()
		{
			XCConfigurationList xCConfigurationList = new XCConfigurationList();
			xCConfigurationList.guid = PBXGUID.Generate();
			xCConfigurationList.SetPropertyString("isa", "XCConfigurationList");
			xCConfigurationList.buildConfigs = new GUIDList();
			xCConfigurationList.SetPropertyString("defaultConfigurationIsVisible", "0");
			return xCConfigurationList;
		}

		public override void UpdateProps()
		{
			SetPropertyList("buildConfigurations", buildConfigs);
		}

		public override void UpdateVars()
		{
			buildConfigs = GetPropertyList("buildConfigurations");
		}
	}
}
