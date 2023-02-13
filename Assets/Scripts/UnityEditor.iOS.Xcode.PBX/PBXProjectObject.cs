using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXProjectObject : PBXObject
	{
		private static PropertyCommentChecker checkerData = new PropertyCommentChecker(new string[5]
		{
			"buildConfigurationList/*",
			"mainGroup/*",
			"projectReferences/*/ProductGroup/*",
			"projectReferences/*/ProjectRef/*",
			"targets/*"
		});

		public List<ProjectReference> projectReferences = new List<ProjectReference>();

		public string buildConfigList;

		internal override PropertyCommentChecker checker
		{
			get
			{
				return checkerData;
			}
		}

		public string mainGroup
		{
			get
			{
				return GetPropertyString("mainGroup");
			}
		}

		public List<string> targets
		{
			get
			{
				return GetPropertyList("targets");
			}
		}

		public void AddReference(string productGroup, string projectRef)
		{
			projectReferences.Add(ProjectReference.Create(productGroup, projectRef));
		}

		public override void UpdateProps()
		{
			m_Properties.values.Remove("projectReferences");
			if (projectReferences.Count > 0)
			{
				PBXElementArray pBXElementArray = m_Properties.CreateArray("projectReferences");
				foreach (ProjectReference projectReference in projectReferences)
				{
					PBXElementDict pBXElementDict = pBXElementArray.AddDict();
					pBXElementDict.SetString("ProductGroup", projectReference.group);
					pBXElementDict.SetString("ProjectRef", projectReference.projectRef);
				}
			}
			SetPropertyString("buildConfigurationList", buildConfigList);
		}

		public override void UpdateVars()
		{
			projectReferences = new List<ProjectReference>();
			if (m_Properties.Contains("projectReferences"))
			{
				PBXElementArray pBXElementArray = m_Properties["projectReferences"].AsArray();
				foreach (PBXElement value in pBXElementArray.values)
				{
					PBXElementDict pBXElementDict = value.AsDict();
					if (pBXElementDict.Contains("ProductGroup") && pBXElementDict.Contains("ProjectRef"))
					{
						string group = pBXElementDict["ProductGroup"].AsString();
						string projectRef = pBXElementDict["ProjectRef"].AsString();
						projectReferences.Add(ProjectReference.Create(group, projectRef));
					}
				}
			}
			buildConfigList = GetPropertyString("buildConfigurationList");
		}
	}
}
