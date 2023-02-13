namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXShellScriptBuildPhase : PBXObject
	{
		public GUIDList files;

		private static PropertyCommentChecker checkerData = new PropertyCommentChecker(new string[1]
		{
			"files/*"
		});

		internal override PropertyCommentChecker checker
		{
			get
			{
				return checkerData;
			}
		}

		public override void UpdateProps()
		{
			SetPropertyList("files", files);
		}

		public override void UpdateVars()
		{
			files = GetPropertyList("files");
		}
	}
}
