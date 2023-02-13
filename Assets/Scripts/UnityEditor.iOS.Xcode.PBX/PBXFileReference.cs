using System.IO;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXFileReference : PBXObject
	{
		public string path;

		public string name;

		public PBXSourceTree tree;

		internal override bool shouldCompact
		{
			get
			{
				return true;
			}
		}

		public static PBXFileReference CreateFromFile(string path, string projectFileName, PBXSourceTree tree)
		{
			string guid = PBXGUID.Generate();
			PBXFileReference pBXFileReference = new PBXFileReference();
			pBXFileReference.SetPropertyString("isa", "PBXFileReference");
			pBXFileReference.guid = guid;
			pBXFileReference.path = path;
			pBXFileReference.name = projectFileName;
			pBXFileReference.tree = tree;
			return pBXFileReference;
		}

		public override void UpdateProps()
		{
			string text = null;
			if (name != null)
			{
				text = Path.GetExtension(name);
			}
			else if (path != null)
			{
				text = Path.GetExtension(path);
			}
			if (text != null)
			{
				if (FileTypeUtils.IsFileTypeExplicit(text))
				{
					SetPropertyString("explicitFileType", FileTypeUtils.GetTypeName(text));
				}
				else
				{
					SetPropertyString("lastKnownFileType", FileTypeUtils.GetTypeName(text));
				}
			}
			if (path == name)
			{
				SetPropertyString("name", null);
			}
			else
			{
				SetPropertyString("name", name);
			}
			if (path == null)
			{
				SetPropertyString("path", string.Empty);
			}
			else
			{
				SetPropertyString("path", path);
			}
			SetPropertyString("sourceTree", FileTypeUtils.SourceTreeDesc(tree));
		}

		public override void UpdateVars()
		{
			name = GetPropertyString("name");
			path = GetPropertyString("path");
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
