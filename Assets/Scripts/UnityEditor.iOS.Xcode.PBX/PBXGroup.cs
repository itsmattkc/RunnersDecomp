using System;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXGroup : PBXObject
	{
		public GUIDList children;

		public PBXSourceTree tree;

		public string name;

		public string path;

		private static PropertyCommentChecker checkerData = new PropertyCommentChecker(new string[1]
		{
			"children/*"
		});

		internal override PropertyCommentChecker checker
		{
			get
			{
				return checkerData;
			}
		}

		public static PBXGroup Create(string name, string path, PBXSourceTree tree)
		{
			if (name.Contains("/"))
			{
				throw new Exception("Group name must not contain '/'");
			}
			PBXGroup pBXGroup = new PBXGroup();
			pBXGroup.guid = PBXGUID.Generate();
			pBXGroup.SetPropertyString("isa", "PBXGroup");
			pBXGroup.name = name;
			pBXGroup.path = path;
			pBXGroup.tree = PBXSourceTree.Group;
			pBXGroup.children = new GUIDList();
			return pBXGroup;
		}

		public static PBXGroup CreateRelative(string name)
		{
			return Create(name, name, PBXSourceTree.Group);
		}

		public override void UpdateProps()
		{
			SetPropertyList("children", children);
			if (name == path)
			{
				SetPropertyString("name", null);
			}
			else
			{
				SetPropertyString("name", name);
			}
			if (path == string.Empty)
			{
				SetPropertyString("path", null);
			}
			else
			{
				SetPropertyString("path", path);
			}
			SetPropertyString("sourceTree", FileTypeUtils.SourceTreeDesc(tree));
		}

		public override void UpdateVars()
		{
			children = GetPropertyList("children");
			path = GetPropertyString("path");
			name = GetPropertyString("name");
			if (name == null)
			{
				name = path;
			}
			if (path == null)
			{
				path = string.Empty;
			}
			tree = FileTypeUtils.ParseSourceTree(GetPropertyString("sourceTree"));
		}
	}
}
