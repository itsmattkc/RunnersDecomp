using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class FileTypeUtils
	{
		internal class FileTypeDesc
		{
			public string name;

			public PBXFileType type;

			public bool isExplicit;

			public FileTypeDesc(string typeName, PBXFileType type)
			{
				name = typeName;
				this.type = type;
				isExplicit = false;
			}

			public FileTypeDesc(string typeName, PBXFileType type, bool isExplicit)
			{
				name = typeName;
				this.type = type;
				this.isExplicit = isExplicit;
			}
		}

		private static readonly Dictionary<string, FileTypeDesc> types = new Dictionary<string, FileTypeDesc>
		{
			{
				".a",
				new FileTypeDesc("archive.ar", PBXFileType.Framework)
			},
			{
				".app",
				new FileTypeDesc("wrapper.application", PBXFileType.NotBuildable, true)
			},
			{
				".appex",
				new FileTypeDesc("wrapper.app-extension", PBXFileType.CopyFile)
			},
			{
				".s",
				new FileTypeDesc("sourcecode.asm", PBXFileType.Source)
			},
			{
				".c",
				new FileTypeDesc("sourcecode.c.c", PBXFileType.Source)
			},
			{
				".cc",
				new FileTypeDesc("sourcecode.cpp.cpp", PBXFileType.Source)
			},
			{
				".cpp",
				new FileTypeDesc("sourcecode.cpp.cpp", PBXFileType.Source)
			},
			{
				".swift",
				new FileTypeDesc("sourcecode.swift", PBXFileType.Source)
			},
			{
				".dll",
				new FileTypeDesc("file", PBXFileType.NotBuildable)
			},
			{
				".framework",
				new FileTypeDesc("wrapper.framework", PBXFileType.Framework)
			},
			{
				".h",
				new FileTypeDesc("sourcecode.c.h", PBXFileType.NotBuildable)
			},
			{
				".pch",
				new FileTypeDesc("sourcecode.c.h", PBXFileType.NotBuildable)
			},
			{
				".icns",
				new FileTypeDesc("image.icns", PBXFileType.Resource)
			},
			{
				".xcassets",
				new FileTypeDesc("folder.assetcatalog", PBXFileType.Resource)
			},
			{
				".inc",
				new FileTypeDesc("sourcecode.inc", PBXFileType.NotBuildable)
			},
			{
				".m",
				new FileTypeDesc("sourcecode.c.objc", PBXFileType.Source)
			},
			{
				".mm",
				new FileTypeDesc("sourcecode.cpp.objcpp", PBXFileType.Source)
			},
			{
				".nib",
				new FileTypeDesc("wrapper.nib", PBXFileType.Resource)
			},
			{
				".plist",
				new FileTypeDesc("text.plist.xml", PBXFileType.Resource)
			},
			{
				".png",
				new FileTypeDesc("image.png", PBXFileType.Resource)
			},
			{
				".rtf",
				new FileTypeDesc("text.rtf", PBXFileType.Resource)
			},
			{
				".tiff",
				new FileTypeDesc("image.tiff", PBXFileType.Resource)
			},
			{
				".txt",
				new FileTypeDesc("text", PBXFileType.Resource)
			},
			{
				".json",
				new FileTypeDesc("text.json", PBXFileType.Resource)
			},
			{
				".xcodeproj",
				new FileTypeDesc("wrapper.pb-project", PBXFileType.NotBuildable)
			},
			{
				".xib",
				new FileTypeDesc("file.xib", PBXFileType.Resource)
			},
			{
				".strings",
				new FileTypeDesc("text.plist.strings", PBXFileType.Resource)
			},
			{
				".storyboard",
				new FileTypeDesc("file.storyboard", PBXFileType.Resource)
			},
			{
				".bundle",
				new FileTypeDesc("wrapper.plug-in", PBXFileType.Resource)
			},
			{
				".dylib",
				new FileTypeDesc("compiled.mach-o.dylib", PBXFileType.Framework)
			},
			{
				".db",
				new FileTypeDesc("template.db", PBXFileType.Resource)
			}
		};

		private static readonly Dictionary<PBXSourceTree, string> sourceTree = new Dictionary<PBXSourceTree, string>
		{
			{
				PBXSourceTree.Absolute,
				"<absolute>"
			},
			{
				PBXSourceTree.Group,
				"<group>"
			},
			{
				PBXSourceTree.Build,
				"BUILT_PRODUCTS_DIR"
			},
			{
				PBXSourceTree.Developer,
				"DEVELOPER_DIR"
			},
			{
				PBXSourceTree.Sdk,
				"SDKROOT"
			},
			{
				PBXSourceTree.Source,
				"SOURCE_ROOT"
			}
		};

		private static readonly Dictionary<string, PBXSourceTree> stringToSourceTreeMap = new Dictionary<string, PBXSourceTree>
		{
			{
				"<absolute>",
				PBXSourceTree.Absolute
			},
			{
				"<group>",
				PBXSourceTree.Group
			},
			{
				"BUILT_PRODUCTS_DIR",
				PBXSourceTree.Build
			},
			{
				"DEVELOPER_DIR",
				PBXSourceTree.Developer
			},
			{
				"SDKROOT",
				PBXSourceTree.Sdk
			},
			{
				"SOURCE_ROOT",
				PBXSourceTree.Source
			}
		};

		public static bool IsKnownExtension(string ext)
		{
			return types.ContainsKey(ext);
		}

		internal static bool IsFileTypeExplicit(string ext)
		{
			if (types.ContainsKey(ext))
			{
				return types[ext].isExplicit;
			}
			return false;
		}

		public static PBXFileType GetFileType(string ext)
		{
			if (types.ContainsKey(ext))
			{
				return types[ext].type;
			}
			return PBXFileType.NotBuildable;
		}

		public static string GetTypeName(string ext)
		{
			if (types.ContainsKey(ext))
			{
				return types[ext].name;
			}
			return "text";
		}

		public static bool IsBuildable(string ext)
		{
			if (types.ContainsKey(ext) && types[ext].type != 0)
			{
				return true;
			}
			return false;
		}

		internal static string SourceTreeDesc(PBXSourceTree tree)
		{
			return sourceTree[tree];
		}

		internal static PBXSourceTree ParseSourceTree(string tree)
		{
			if (stringToSourceTreeMap.ContainsKey(tree))
			{
				return stringToSourceTreeMap[tree];
			}
			return PBXSourceTree.Source;
		}

		internal static List<PBXSourceTree> AllAbsoluteSourceTrees()
		{
			List<PBXSourceTree> list = new List<PBXSourceTree>();
			list.Add(PBXSourceTree.Absolute);
			list.Add(PBXSourceTree.Build);
			list.Add(PBXSourceTree.Developer);
			list.Add(PBXSourceTree.Sdk);
			list.Add(PBXSourceTree.Source);
			return list;
		}
	}
}
