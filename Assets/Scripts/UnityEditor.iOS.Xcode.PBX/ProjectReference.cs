namespace UnityEditor.iOS.Xcode.PBX
{
	internal class ProjectReference
	{
		public string group;

		public string projectRef;

		public static ProjectReference Create(string group, string projectRef)
		{
			ProjectReference projectReference = new ProjectReference();
			projectReference.group = group;
			projectReference.projectRef = projectRef;
			return projectReference;
		}
	}
}
