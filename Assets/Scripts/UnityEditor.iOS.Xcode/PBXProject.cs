using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.iOS.Xcode.PBX;

namespace UnityEditor.iOS.Xcode
{
	public class PBXProject
	{
		private Dictionary<string, SectionBase> m_Section;

		private PBXElementDict m_RootElements;

		private PBXElementDict m_UnknownObjects;

		private string m_ObjectVersion;

		private List<string> m_SectionOrder;

		private Dictionary<string, KnownSectionBase<PBXObject>> m_UnknownSections;

		private KnownSectionBase<PBXBuildFile> buildFiles;

		private KnownSectionBase<PBXFileReference> fileRefs;

		private KnownSectionBase<PBXGroup> groups;

		private KnownSectionBase<PBXContainerItemProxy> containerItems;

		private KnownSectionBase<PBXReferenceProxy> references;

		private KnownSectionBase<PBXSourcesBuildPhase> sources;

		private KnownSectionBase<PBXFrameworksBuildPhase> frameworks;

		private KnownSectionBase<PBXResourcesBuildPhase> resources;

		private KnownSectionBase<PBXCopyFilesBuildPhase> copyFiles;

		private KnownSectionBase<PBXShellScriptBuildPhase> shellScripts;

		private KnownSectionBase<PBXNativeTarget> nativeTargets;

		private KnownSectionBase<PBXTargetDependency> targetDependencies;

		private KnownSectionBase<PBXVariantGroup> variantGroups;

		private KnownSectionBase<XCBuildConfiguration> buildConfigs;

		private KnownSectionBase<XCConfigurationList> configs;

		private PBXProjectSection project;

		private Dictionary<string, Dictionary<string, PBXBuildFile>> m_FileGuidToBuildFileMap;

		private Dictionary<string, PBXFileReference> m_ProjectPathToFileRefMap;

		private Dictionary<string, string> m_FileRefGuidToProjectPathMap;

		private Dictionary<PBXSourceTree, Dictionary<string, PBXFileReference>> m_RealPathToFileRefMap;

		private Dictionary<string, PBXGroup> m_ProjectPathToGroupMap;

		private Dictionary<string, string> m_GroupGuidToProjectPathMap;

		private Dictionary<string, PBXGroup> m_GuidToParentGroupMap;

		private void BuildFilesAdd(string targetGuid, PBXBuildFile buildFile)
		{
			if (!m_FileGuidToBuildFileMap.ContainsKey(targetGuid))
			{
				m_FileGuidToBuildFileMap[targetGuid] = new Dictionary<string, PBXBuildFile>();
			}
			m_FileGuidToBuildFileMap[targetGuid][buildFile.fileRef] = buildFile;
			buildFiles.AddEntry(buildFile);
		}

		private void BuildFilesRemove(string targetGuid, string fileGuid)
		{
			PBXBuildFile buildFileForFileGuid = GetBuildFileForFileGuid(targetGuid, fileGuid);
			if (buildFileForFileGuid != null)
			{
				m_FileGuidToBuildFileMap[targetGuid].Remove(buildFileForFileGuid.fileRef);
				buildFiles.RemoveEntry(buildFileForFileGuid.guid);
			}
		}

		private PBXBuildFile GetBuildFileForFileGuid(string targetGuid, string fileGuid)
		{
			if (!m_FileGuidToBuildFileMap.ContainsKey(targetGuid))
			{
				return null;
			}
			if (!m_FileGuidToBuildFileMap[targetGuid].ContainsKey(fileGuid))
			{
				return null;
			}
			return m_FileGuidToBuildFileMap[targetGuid][fileGuid];
		}

		private void FileRefsAdd(string realPath, string projectPath, PBXGroup parent, PBXFileReference fileRef)
		{
			fileRefs.AddEntry(fileRef);
			m_ProjectPathToFileRefMap.Add(projectPath, fileRef);
			m_FileRefGuidToProjectPathMap.Add(fileRef.guid, projectPath);
			m_RealPathToFileRefMap[fileRef.tree].Add(realPath, fileRef);
			m_GuidToParentGroupMap.Add(fileRef.guid, parent);
		}

		private void FileRefsRemove(string guid)
		{
			PBXFileReference pBXFileReference = fileRefs[guid];
			fileRefs.RemoveEntry(guid);
			m_ProjectPathToFileRefMap.Remove(m_FileRefGuidToProjectPathMap[guid]);
			m_FileRefGuidToProjectPathMap.Remove(guid);
			foreach (PBXSourceTree item in FileTypeUtils.AllAbsoluteSourceTrees())
			{
				m_RealPathToFileRefMap[item].Remove(pBXFileReference.path);
			}
			m_GuidToParentGroupMap.Remove(guid);
		}

		private void GroupsAdd(string projectPath, PBXGroup parent, PBXGroup gr)
		{
			m_ProjectPathToGroupMap.Add(projectPath, gr);
			m_GroupGuidToProjectPathMap.Add(gr.guid, projectPath);
			m_GuidToParentGroupMap.Add(gr.guid, parent);
			groups.AddEntry(gr);
		}

		private void GroupsRemove(string guid)
		{
			m_ProjectPathToGroupMap.Remove(m_GroupGuidToProjectPathMap[guid]);
			m_GroupGuidToProjectPathMap.Remove(guid);
			m_GuidToParentGroupMap.Remove(guid);
			groups.RemoveEntry(guid);
		}

		private void RefreshBuildFilesMapForBuildFileGuidList(Dictionary<string, PBXBuildFile> mapForTarget, FileGUIDListBase list)
		{
			foreach (string item in (IEnumerable<string>)list.files)
			{
				PBXBuildFile pBXBuildFile = buildFiles[item];
				mapForTarget[pBXBuildFile.fileRef] = pBXBuildFile;
			}
		}

		private void CombinePaths(string path1, PBXSourceTree tree1, string path2, PBXSourceTree tree2, out string resPath, out PBXSourceTree resTree)
		{
			if (tree2 == PBXSourceTree.Group)
			{
				resPath = Path.Combine(path1, path2);
				resTree = tree1;
			}
			else
			{
				resPath = path2;
				resTree = tree2;
			}
		}

		private void RefreshMapsForGroupChildren(string projectPath, string realPath, PBXSourceTree realPathTree, PBXGroup parent)
		{
			List<string> list = new List<string>(parent.children);
			foreach (string item in list)
			{
				PBXFileReference pBXFileReference = fileRefs[item];
				string resPath;
				PBXSourceTree resTree;
				if (pBXFileReference != null)
				{
					string text = Path.Combine(projectPath, pBXFileReference.name);
					CombinePaths(realPath, realPathTree, pBXFileReference.path, pBXFileReference.tree, out resPath, out resTree);
					m_ProjectPathToFileRefMap.Add(text, pBXFileReference);
					m_FileRefGuidToProjectPathMap.Add(pBXFileReference.guid, text);
					m_RealPathToFileRefMap[resTree].Add(resPath, pBXFileReference);
					m_GuidToParentGroupMap.Add(item, parent);
					continue;
				}
				PBXGroup pBXGroup = groups[item];
				if (pBXGroup != null)
				{
					string text = Path.Combine(projectPath, pBXGroup.name);
					CombinePaths(realPath, realPathTree, pBXGroup.path, pBXGroup.tree, out resPath, out resTree);
					m_ProjectPathToGroupMap.Add(text, pBXGroup);
					m_GroupGuidToProjectPathMap.Add(pBXGroup.guid, text);
					m_GuidToParentGroupMap.Add(item, parent);
					RefreshMapsForGroupChildren(text, resPath, resTree, pBXGroup);
				}
			}
		}

		private void RefreshAuxMaps()
		{
			foreach (KeyValuePair<string, PBXNativeTarget> entry in nativeTargets.entries)
			{
				Dictionary<string, PBXBuildFile> dictionary = new Dictionary<string, PBXBuildFile>();
				foreach (string item in (IEnumerable<string>)entry.Value.phases)
				{
					if (frameworks.entries.ContainsKey(item))
					{
						RefreshBuildFilesMapForBuildFileGuidList(dictionary, frameworks.entries[item]);
					}
					if (resources.entries.ContainsKey(item))
					{
						RefreshBuildFilesMapForBuildFileGuidList(dictionary, resources.entries[item]);
					}
					if (sources.entries.ContainsKey(item))
					{
						RefreshBuildFilesMapForBuildFileGuidList(dictionary, sources.entries[item]);
					}
					if (copyFiles.entries.ContainsKey(item))
					{
						RefreshBuildFilesMapForBuildFileGuidList(dictionary, copyFiles.entries[item]);
					}
				}
				m_FileGuidToBuildFileMap[entry.Key] = dictionary;
			}
			RefreshMapsForGroupChildren(string.Empty, string.Empty, PBXSourceTree.Source, groups[project.project.mainGroup]);
		}

		private void Clear()
		{
			buildFiles = new KnownSectionBase<PBXBuildFile>("PBXBuildFile");
			fileRefs = new KnownSectionBase<PBXFileReference>("PBXFileReference");
			groups = new KnownSectionBase<PBXGroup>("PBXGroup");
			containerItems = new KnownSectionBase<PBXContainerItemProxy>("PBXContainerItemProxy");
			references = new KnownSectionBase<PBXReferenceProxy>("PBXReferenceProxy");
			sources = new KnownSectionBase<PBXSourcesBuildPhase>("PBXSourcesBuildPhase");
			frameworks = new KnownSectionBase<PBXFrameworksBuildPhase>("PBXFrameworksBuildPhase");
			resources = new KnownSectionBase<PBXResourcesBuildPhase>("PBXResourcesBuildPhase");
			copyFiles = new KnownSectionBase<PBXCopyFilesBuildPhase>("PBXCopyFilesBuildPhase");
			shellScripts = new KnownSectionBase<PBXShellScriptBuildPhase>("PBXShellScriptBuildPhase");
			nativeTargets = new KnownSectionBase<PBXNativeTarget>("PBXNativeTarget");
			targetDependencies = new KnownSectionBase<PBXTargetDependency>("PBXTargetDependency");
			variantGroups = new KnownSectionBase<PBXVariantGroup>("PBXVariantGroup");
			buildConfigs = new KnownSectionBase<XCBuildConfiguration>("XCBuildConfiguration");
			configs = new KnownSectionBase<XCConfigurationList>("XCConfigurationList");
			project = new PBXProjectSection();
			m_UnknownSections = new Dictionary<string, KnownSectionBase<PBXObject>>();
			m_Section = new Dictionary<string, SectionBase>
			{
				{
					"PBXBuildFile",
					buildFiles
				},
				{
					"PBXFileReference",
					fileRefs
				},
				{
					"PBXGroup",
					groups
				},
				{
					"PBXContainerItemProxy",
					containerItems
				},
				{
					"PBXReferenceProxy",
					references
				},
				{
					"PBXSourcesBuildPhase",
					sources
				},
				{
					"PBXFrameworksBuildPhase",
					frameworks
				},
				{
					"PBXResourcesBuildPhase",
					resources
				},
				{
					"PBXCopyFilesBuildPhase",
					copyFiles
				},
				{
					"PBXShellScriptBuildPhase",
					shellScripts
				},
				{
					"PBXNativeTarget",
					nativeTargets
				},
				{
					"PBXTargetDependency",
					targetDependencies
				},
				{
					"PBXVariantGroup",
					variantGroups
				},
				{
					"XCBuildConfiguration",
					buildConfigs
				},
				{
					"XCConfigurationList",
					configs
				},
				{
					"PBXProject",
					project
				}
			};
			m_RootElements = new PBXElementDict();
			m_UnknownObjects = new PBXElementDict();
			m_ObjectVersion = null;
			m_SectionOrder = new List<string>
			{
				"PBXBuildFile",
				"PBXContainerItemProxy",
				"PBXCopyFilesBuildPhase",
				"PBXFileReference",
				"PBXFrameworksBuildPhase",
				"PBXGroup",
				"PBXNativeTarget",
				"PBXProject",
				"PBXReferenceProxy",
				"PBXResourcesBuildPhase",
				"PBXShellScriptBuildPhase",
				"PBXSourcesBuildPhase",
				"PBXTargetDependency",
				"PBXVariantGroup",
				"XCBuildConfiguration",
				"XCConfigurationList"
			};
			m_FileGuidToBuildFileMap = new Dictionary<string, Dictionary<string, PBXBuildFile>>();
			m_ProjectPathToFileRefMap = new Dictionary<string, PBXFileReference>();
			m_FileRefGuidToProjectPathMap = new Dictionary<string, string>();
			m_RealPathToFileRefMap = new Dictionary<PBXSourceTree, Dictionary<string, PBXFileReference>>();
			foreach (PBXSourceTree item in FileTypeUtils.AllAbsoluteSourceTrees())
			{
				m_RealPathToFileRefMap.Add(item, new Dictionary<string, PBXFileReference>());
			}
			m_ProjectPathToGroupMap = new Dictionary<string, PBXGroup>();
			m_GroupGuidToProjectPathMap = new Dictionary<string, string>();
			m_GuidToParentGroupMap = new Dictionary<string, PBXGroup>();
		}

		public static string GetPBXProjectPath(string buildPath)
		{
			return Path.Combine(buildPath, "Unity-iPhone/project.pbxproj");
		}

		public static string GetUnityTargetName()
		{
			return "Unity-iPhone";
		}

		public static string GetUnityTestTargetName()
		{
			return "Unity-iPhone Tests";
		}

		public string TargetGuidByName(string name)
		{
			foreach (KeyValuePair<string, PBXNativeTarget> entry in nativeTargets.entries)
			{
				if (entry.Value.name == name)
				{
					return entry.Key;
				}
			}
			return null;
		}

		private FileGUIDListBase BuildSection(PBXNativeTarget target, string path)
		{
			string extension = Path.GetExtension(path);
			switch (FileTypeUtils.GetFileType(extension))
			{
			case PBXFileType.Framework:
				foreach (string item in (IEnumerable<string>)target.phases)
				{
					if (frameworks.entries.ContainsKey(item))
					{
						return frameworks.entries[item];
					}
				}
				break;
			case PBXFileType.Resource:
				foreach (string item2 in (IEnumerable<string>)target.phases)
				{
					if (resources.entries.ContainsKey(item2))
					{
						return resources.entries[item2];
					}
				}
				break;
			case PBXFileType.Source:
				foreach (string item3 in (IEnumerable<string>)target.phases)
				{
					if (sources.entries.ContainsKey(item3))
					{
						return sources.entries[item3];
					}
				}
				break;
			case PBXFileType.CopyFile:
				foreach (string item4 in (IEnumerable<string>)target.phases)
				{
					if (copyFiles.entries.ContainsKey(item4))
					{
						return copyFiles.entries[item4];
					}
				}
				break;
			}
			return null;
		}

		public static bool IsKnownExtension(string ext)
		{
			return FileTypeUtils.IsKnownExtension(ext);
		}

		public static bool IsBuildable(string ext)
		{
			return FileTypeUtils.IsBuildable(ext);
		}

		private string AddFileImpl(string path, string projectPath, PBXSourceTree tree)
		{
			path = FixSlashesInPath(path);
			projectPath = FixSlashesInPath(projectPath);
			string extension = Path.GetExtension(path);
			if (extension != Path.GetExtension(projectPath))
			{
				throw new Exception("Project and real path extensions do not match");
			}
			string text = FindFileGuidByProjectPath(projectPath);
			if (text == null)
			{
				text = FindFileGuidByRealPath(path);
			}
			if (text == null)
			{
				PBXFileReference pBXFileReference = PBXFileReference.CreateFromFile(path, GetFilenameFromPath(projectPath), tree);
				PBXGroup pBXGroup = CreateSourceGroup(GetDirectoryFromPath(projectPath));
				pBXGroup.children.AddGUID(pBXFileReference.guid);
				FileRefsAdd(path, projectPath, pBXGroup, pBXFileReference);
				text = pBXFileReference.guid;
			}
			return text;
		}

		public string AddFile(string path, string projectPath)
		{
			return AddFileImpl(path, projectPath, PBXSourceTree.Source);
		}

		public string AddFile(string path, string projectPath, PBXSourceTree sourceTree)
		{
			if (sourceTree == PBXSourceTree.Group)
			{
				throw new Exception("sourceTree must not be PBXSourceTree.Group");
			}
			return AddFileImpl(path, projectPath, sourceTree);
		}

		private void AddBuildFileImpl(string targetGuid, string fileGuid, bool weak, string compileFlags)
		{
			PBXNativeTarget target = nativeTargets[targetGuid];
			string extension = Path.GetExtension(fileRefs[fileGuid].path);
			if (FileTypeUtils.IsBuildable(extension) && GetBuildFileForFileGuid(targetGuid, fileGuid) == null)
			{
				PBXBuildFile pBXBuildFile = PBXBuildFile.CreateFromFile(fileGuid, weak, compileFlags);
				BuildFilesAdd(targetGuid, pBXBuildFile);
				BuildSection(target, extension).files.AddGUID(pBXBuildFile.guid);
			}
		}

		public void AddFileToBuild(string targetGuid, string fileGuid)
		{
			AddBuildFileImpl(targetGuid, fileGuid, false, null);
		}

		public void AddFileToBuildWithFlags(string targetGuid, string fileGuid, string compileFlags)
		{
			AddBuildFileImpl(targetGuid, fileGuid, false, compileFlags);
		}

		public List<string> GetCompileFlagsForFile(string targetGuid, string fileGuid)
		{
			PBXBuildFile buildFileForFileGuid = GetBuildFileForFileGuid(targetGuid, fileGuid);
			if (buildFileForFileGuid == null)
			{
				return null;
			}
			if (buildFileForFileGuid.compileFlags == null)
			{
				return new List<string>();
			}
			List<string> list = new List<string>();
			list.Add(buildFileForFileGuid.compileFlags);
			return list;
		}

		public void SetCompileFlagsForFile(string targetGuid, string fileGuid, List<string> compileFlags)
		{
			PBXBuildFile buildFileForFileGuid = GetBuildFileForFileGuid(targetGuid, fileGuid);
			if (buildFileForFileGuid != null)
			{
				buildFileForFileGuid.compileFlags = string.Join(" ", compileFlags.ToArray());
			}
		}

		public bool ContainsFileByRealPath(string path)
		{
			return FindFileGuidByRealPath(path) != null;
		}

		public bool ContainsFileByRealPath(string path, PBXSourceTree sourceTree)
		{
			if (sourceTree == PBXSourceTree.Group)
			{
				throw new Exception("sourceTree must not be PBXSourceTree.Group");
			}
			return FindFileGuidByRealPath(path, sourceTree) != null;
		}

		public bool ContainsFileByProjectPath(string path)
		{
			return FindFileGuidByProjectPath(path) != null;
		}

		public bool HasFramework(string framework)
		{
			return ContainsFileByRealPath("System/Library/Frameworks/" + framework);
		}

		public void AddFrameworkToProject(string targetGuid, string framework, bool weak)
		{
			string fileGuid = AddFile("System/Library/Frameworks/" + framework, "Frameworks/" + framework, PBXSourceTree.Sdk);
			AddBuildFileImpl(targetGuid, fileGuid, weak, null);
		}

		private string GetDirectoryFromPath(string path)
		{
			int num = path.LastIndexOf('/');
			if (num == -1)
			{
				return string.Empty;
			}
			return path.Substring(0, num);
		}

		private string GetFilenameFromPath(string path)
		{
			int num = path.LastIndexOf('/');
			if (num == -1)
			{
				return path;
			}
			return path.Substring(num + 1);
		}

		public string FindFileGuidByRealPath(string path, PBXSourceTree sourceTree)
		{
			if (sourceTree == PBXSourceTree.Group)
			{
				throw new Exception("sourceTree must not be PBXSourceTree.Group");
			}
			path = FixSlashesInPath(path);
			if (m_RealPathToFileRefMap[sourceTree].ContainsKey(path))
			{
				return m_RealPathToFileRefMap[sourceTree][path].guid;
			}
			return null;
		}

		public string FindFileGuidByRealPath(string path)
		{
			path = FixSlashesInPath(path);
			foreach (PBXSourceTree item in FileTypeUtils.AllAbsoluteSourceTrees())
			{
				string text = FindFileGuidByRealPath(path, item);
				if (text != null)
				{
					return text;
				}
			}
			return null;
		}

		public string FindFileGuidByProjectPath(string path)
		{
			path = FixSlashesInPath(path);
			if (m_ProjectPathToFileRefMap.ContainsKey(path))
			{
				return m_ProjectPathToFileRefMap[path].guid;
			}
			return null;
		}

		public void RemoveFileFromBuild(string targetGuid, string fileGuid)
		{
			PBXBuildFile buildFileForFileGuid = GetBuildFileForFileGuid(targetGuid, fileGuid);
			if (buildFileForFileGuid == null)
			{
				return;
			}
			BuildFilesRemove(targetGuid, fileGuid);
			string guid = buildFileForFileGuid.guid;
			if (guid == null)
			{
				return;
			}
			foreach (KeyValuePair<string, PBXSourcesBuildPhase> entry in sources.entries)
			{
				entry.Value.files.RemoveGUID(guid);
			}
			foreach (KeyValuePair<string, PBXResourcesBuildPhase> entry2 in resources.entries)
			{
				entry2.Value.files.RemoveGUID(guid);
			}
			foreach (KeyValuePair<string, PBXCopyFilesBuildPhase> entry3 in copyFiles.entries)
			{
				entry3.Value.files.RemoveGUID(guid);
			}
			foreach (KeyValuePair<string, PBXFrameworksBuildPhase> entry4 in frameworks.entries)
			{
				entry4.Value.files.RemoveGUID(guid);
			}
		}

		public void RemoveFile(string fileGuid)
		{
			if (fileGuid == null)
			{
				return;
			}
			PBXGroup pBXGroup = m_GuidToParentGroupMap[fileGuid];
			if (pBXGroup != null)
			{
				pBXGroup.children.RemoveGUID(fileGuid);
			}
			RemoveGroupIfEmpty(pBXGroup);
			foreach (KeyValuePair<string, PBXNativeTarget> entry in nativeTargets.entries)
			{
				RemoveFileFromBuild(entry.Value.guid, fileGuid);
			}
			FileRefsRemove(fileGuid);
		}

		private void RemoveGroupIfEmpty(PBXGroup gr)
		{
			if (gr.children.Count == 0 && gr.guid != project.project.mainGroup)
			{
				PBXGroup pBXGroup = m_GuidToParentGroupMap[gr.guid];
				pBXGroup.children.RemoveGUID(gr.guid);
				RemoveGroupIfEmpty(pBXGroup);
				GroupsRemove(gr.guid);
			}
		}

		private void RemoveGroupChildrenRecursive(PBXGroup parent)
		{
			List<string> list = new List<string>(parent.children);
			parent.children.Clear();
			foreach (string item in list)
			{
				PBXFileReference pBXFileReference = fileRefs[item];
				if (pBXFileReference != null)
				{
					foreach (KeyValuePair<string, PBXNativeTarget> entry in nativeTargets.entries)
					{
						RemoveFileFromBuild(entry.Value.guid, item);
					}
					FileRefsRemove(item);
					continue;
				}
				PBXGroup pBXGroup = groups[item];
				if (pBXGroup != null)
				{
					RemoveGroupChildrenRecursive(pBXGroup);
					GroupsRemove(parent.guid);
				}
			}
		}

		internal void RemoveFilesByProjectPathRecursive(string projectPath)
		{
			PBXGroup sourceGroup = GetSourceGroup(projectPath);
			if (sourceGroup != null)
			{
				RemoveGroupChildrenRecursive(sourceGroup);
				RemoveGroupIfEmpty(sourceGroup);
			}
		}

		private PBXGroup GetPBXGroupChildByName(PBXGroup group, string name)
		{
			foreach (string item in (IEnumerable<string>)group.children)
			{
				PBXGroup pBXGroup = groups[item];
				if (pBXGroup != null && pBXGroup.name == name)
				{
					return pBXGroup;
				}
			}
			return null;
		}

		private PBXGroup GetSourceGroup(string sourceGroup)
		{
			sourceGroup = FixSlashesInPath(sourceGroup);
			if (sourceGroup == null || sourceGroup == string.Empty)
			{
				return groups[project.project.mainGroup];
			}
			if (m_ProjectPathToGroupMap.ContainsKey(sourceGroup))
			{
				return m_ProjectPathToGroupMap[sourceGroup];
			}
			return null;
		}

		private PBXGroup CreateSourceGroup(string sourceGroup)
		{
			sourceGroup = FixSlashesInPath(sourceGroup);
			if (m_ProjectPathToGroupMap.ContainsKey(sourceGroup))
			{
				return m_ProjectPathToGroupMap[sourceGroup];
			}
			PBXGroup pBXGroup = groups[project.project.mainGroup];
			if (sourceGroup == null || sourceGroup == string.Empty)
			{
				return pBXGroup;
			}
			string[] array = sourceGroup.Trim('/').Split('/');
			string text = null;
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				text = ((text != null) ? (text + "/" + text2) : text2);
				PBXGroup pBXGroupChildByName = GetPBXGroupChildByName(pBXGroup, text2);
				if (pBXGroupChildByName != null)
				{
					pBXGroup = pBXGroupChildByName;
					continue;
				}
				PBXGroup pBXGroup2 = PBXGroup.Create(text2, text2, PBXSourceTree.Group);
				pBXGroup.children.AddGUID(pBXGroup2.guid);
				GroupsAdd(text, pBXGroup, pBXGroup2);
				pBXGroup = pBXGroup2;
			}
			return pBXGroup;
		}

		public void AddExternalProjectDependency(string path, string projectPath, PBXSourceTree sourceTree)
		{
			if (sourceTree == PBXSourceTree.Group)
			{
				throw new Exception("sourceTree must not be PBXSourceTree.Group");
			}
			path = FixSlashesInPath(path);
			projectPath = FixSlashesInPath(projectPath);
			PBXGroup pBXGroup = PBXGroup.CreateRelative("Products");
			groups.AddEntry(pBXGroup);
			PBXFileReference pBXFileReference = PBXFileReference.CreateFromFile(path, Path.GetFileName(projectPath), sourceTree);
			FileRefsAdd(path, projectPath, null, pBXFileReference);
			CreateSourceGroup(GetDirectoryFromPath(projectPath)).children.AddGUID(pBXFileReference.guid);
			project.project.AddReference(pBXGroup.guid, pBXFileReference.guid);
		}

		public void AddExternalLibraryDependency(string targetGuid, string filename, string remoteFileGuid, string projectPath, string remoteInfo)
		{
			PBXNativeTarget target = nativeTargets[targetGuid];
			filename = FixSlashesInPath(filename);
			projectPath = FixSlashesInPath(projectPath);
			string text = FindFileGuidByRealPath(projectPath);
			if (text == null)
			{
				throw new Exception("No such project");
			}
			string text2 = null;
			foreach (ProjectReference projectReference in project.project.projectReferences)
			{
				if (projectReference.projectRef == text)
				{
					text2 = projectReference.group;
					break;
				}
			}
			if (text2 == null)
			{
				throw new Exception("Malformed project: no project in project references");
			}
			PBXGroup pBXGroup = groups[text2];
			string extension = Path.GetExtension(filename);
			if (!FileTypeUtils.IsBuildable(extension))
			{
				throw new Exception("Wrong file extension");
			}
			PBXContainerItemProxy pBXContainerItemProxy = PBXContainerItemProxy.Create(text, "2", remoteFileGuid, remoteInfo);
			containerItems.AddEntry(pBXContainerItemProxy);
			string typeName = FileTypeUtils.GetTypeName(extension);
			PBXReferenceProxy pBXReferenceProxy = PBXReferenceProxy.Create(filename, typeName, pBXContainerItemProxy.guid, "BUILT_PRODUCTS_DIR");
			references.AddEntry(pBXReferenceProxy);
			PBXBuildFile pBXBuildFile = PBXBuildFile.CreateFromFile(pBXReferenceProxy.guid, false, null);
			BuildFilesAdd(targetGuid, pBXBuildFile);
			BuildSection(target, extension).files.AddGUID(pBXBuildFile.guid);
			pBXGroup.children.AddGUID(pBXReferenceProxy.guid);
		}

		private void SetDefaultAppExtensionReleaseBuildFlags(XCBuildConfiguration config, string infoPlistPath)
		{
			config.AddProperty("ALWAYS_SEARCH_USER_PATHS", "NO");
			config.AddProperty("CLANG_CXX_LANGUAGE_STANDARD", "gnu++0x");
			config.AddProperty("CLANG_CXX_LIBRARY", "libc++");
			config.AddProperty("CLANG_ENABLE_MODULES", "YES");
			config.AddProperty("CLANG_ENABLE_OBJC_ARC", "YES");
			config.AddProperty("CLANG_WARN_BOOL_CONVERSION", "YES");
			config.AddProperty("CLANG_WARN_CONSTANT_CONVERSION", "YES");
			config.AddProperty("CLANG_WARN_DIRECT_OBJC_ISA_USAGE", "YES_ERROR");
			config.AddProperty("CLANG_WARN_EMPTY_BODY", "YES");
			config.AddProperty("CLANG_WARN_ENUM_CONVERSION", "YES");
			config.AddProperty("CLANG_WARN_INT_CONVERSION", "YES");
			config.AddProperty("CLANG_WARN_OBJC_ROOT_CLASS", "YES_ERROR");
			config.AddProperty("CLANG_WARN_UNREACHABLE_CODE", "YES");
			config.AddProperty("CLANG_WARN__DUPLICATE_METHOD_MATCH", "YES");
			config.AddProperty("COPY_PHASE_STRIP", "YES");
			config.AddProperty("ENABLE_NS_ASSERTIONS", "NO");
			config.AddProperty("ENABLE_STRICT_OBJC_MSGSEND", "YES");
			config.AddProperty("GCC_C_LANGUAGE_STANDARD", "gnu99");
			config.AddProperty("GCC_WARN_64_TO_32_BIT_CONVERSION", "YES");
			config.AddProperty("GCC_WARN_ABOUT_RETURN_TYPE", "YES_ERROR");
			config.AddProperty("GCC_WARN_UNDECLARED_SELECTOR", "YES");
			config.AddProperty("GCC_WARN_UNINITIALIZED_AUTOS", "YES_AGGRESSIVE");
			config.AddProperty("GCC_WARN_UNUSED_FUNCTION", "YES");
			config.AddProperty("INFOPLIST_FILE", infoPlistPath);
			config.AddProperty("IPHONEOS_DEPLOYMENT_TARGET", "8.0");
			config.AddProperty("LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks @executable_path/../../Frameworks");
			config.AddProperty("MTL_ENABLE_DEBUG_INFO", "NO");
			config.AddProperty("PRODUCT_NAME", "$(TARGET_NAME)");
			config.AddProperty("SKIP_INSTALL", "YES");
			config.AddProperty("VALIDATE_PRODUCT", "YES");
		}

		private void SetDefaultAppExtensionDebugBuildFlags(XCBuildConfiguration config, string infoPlistPath)
		{
			config.AddProperty("ALWAYS_SEARCH_USER_PATHS", "NO");
			config.AddProperty("CLANG_CXX_LANGUAGE_STANDARD", "gnu++0x");
			config.AddProperty("CLANG_CXX_LIBRARY", "libc++");
			config.AddProperty("CLANG_ENABLE_MODULES", "YES");
			config.AddProperty("CLANG_ENABLE_OBJC_ARC", "YES");
			config.AddProperty("CLANG_WARN_BOOL_CONVERSION", "YES");
			config.AddProperty("CLANG_WARN_CONSTANT_CONVERSION", "YES");
			config.AddProperty("CLANG_WARN_DIRECT_OBJC_ISA_USAGE", "YES_ERROR");
			config.AddProperty("CLANG_WARN_EMPTY_BODY", "YES");
			config.AddProperty("CLANG_WARN_ENUM_CONVERSION", "YES");
			config.AddProperty("CLANG_WARN_INT_CONVERSION", "YES");
			config.AddProperty("CLANG_WARN_OBJC_ROOT_CLASS", "YES_ERROR");
			config.AddProperty("CLANG_WARN_UNREACHABLE_CODE", "YES");
			config.AddProperty("CLANG_WARN__DUPLICATE_METHOD_MATCH", "YES");
			config.AddProperty("COPY_PHASE_STRIP", "NO");
			config.AddProperty("ENABLE_STRICT_OBJC_MSGSEND", "YES");
			config.AddProperty("GCC_C_LANGUAGE_STANDARD", "gnu99");
			config.AddProperty("GCC_DYNAMIC_NO_PIC", "NO");
			config.AddProperty("GCC_OPTIMIZATION_LEVEL", "0");
			config.AddProperty("GCC_PREPROCESSOR_DEFINITIONS", "DEBUG=1");
			config.AddProperty("GCC_PREPROCESSOR_DEFINITIONS", "$(inherited)");
			config.AddProperty("GCC_SYMBOLS_PRIVATE_EXTERN", "NO");
			config.AddProperty("GCC_WARN_64_TO_32_BIT_CONVERSION", "YES");
			config.AddProperty("GCC_WARN_ABOUT_RETURN_TYPE", "YES_ERROR");
			config.AddProperty("GCC_WARN_UNDECLARED_SELECTOR", "YES");
			config.AddProperty("GCC_WARN_UNINITIALIZED_AUTOS", "YES_AGGRESSIVE");
			config.AddProperty("GCC_WARN_UNUSED_FUNCTION", "YES");
			config.AddProperty("INFOPLIST_FILE", infoPlistPath);
			config.AddProperty("IPHONEOS_DEPLOYMENT_TARGET", "8.0");
			config.AddProperty("LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
			config.AddProperty("LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
			config.AddProperty("LD_RUNPATH_SEARCH_PATHS", "@executable_path/../../Frameworks");
			config.AddProperty("MTL_ENABLE_DEBUG_INFO", "YES");
			config.AddProperty("ONLY_ACTIVE_ARCH", "YES");
			config.AddProperty("PRODUCT_NAME", "$(TARGET_NAME)");
			config.AddProperty("SKIP_INSTALL", "YES");
		}

		internal string AddAppExtension(string mainTarget, string name, string infoPlistPath)
		{
			string str = ".appex";
			string text = name + str;
			PBXFileReference pBXFileReference = PBXFileReference.CreateFromFile("Products/" + text, "Products/" + text, PBXSourceTree.Group);
			XCBuildConfiguration xCBuildConfiguration = XCBuildConfiguration.Create("Release");
			buildConfigs.AddEntry(xCBuildConfiguration);
			SetDefaultAppExtensionReleaseBuildFlags(xCBuildConfiguration, infoPlistPath);
			XCBuildConfiguration xCBuildConfiguration2 = XCBuildConfiguration.Create("Debug");
			buildConfigs.AddEntry(xCBuildConfiguration2);
			SetDefaultAppExtensionDebugBuildFlags(xCBuildConfiguration2, infoPlistPath);
			XCConfigurationList xCConfigurationList = XCConfigurationList.Create();
			configs.AddEntry(xCConfigurationList);
			xCConfigurationList.buildConfigs.AddGUID(xCBuildConfiguration.guid);
			xCConfigurationList.buildConfigs.AddGUID(xCBuildConfiguration2.guid);
			PBXNativeTarget pBXNativeTarget = PBXNativeTarget.Create(name, pBXFileReference.guid, "com.apple.product-type.app-extension", xCConfigurationList.guid);
			nativeTargets.AddEntry(pBXNativeTarget);
			project.project.targets.Add(pBXNativeTarget.guid);
			PBXSourcesBuildPhase pBXSourcesBuildPhase = PBXSourcesBuildPhase.Create();
			sources.AddEntry(pBXSourcesBuildPhase);
			pBXNativeTarget.phases.AddGUID(pBXSourcesBuildPhase.guid);
			PBXResourcesBuildPhase pBXResourcesBuildPhase = PBXResourcesBuildPhase.Create();
			resources.AddEntry(pBXResourcesBuildPhase);
			pBXNativeTarget.phases.AddGUID(pBXResourcesBuildPhase.guid);
			PBXFrameworksBuildPhase pBXFrameworksBuildPhase = PBXFrameworksBuildPhase.Create();
			frameworks.AddEntry(pBXFrameworksBuildPhase);
			pBXNativeTarget.phases.AddGUID(pBXFrameworksBuildPhase.guid);
			PBXCopyFilesBuildPhase pBXCopyFilesBuildPhase = PBXCopyFilesBuildPhase.Create("Embed App Extensions", "13");
			copyFiles.AddEntry(pBXCopyFilesBuildPhase);
			nativeTargets[mainTarget].phases.AddGUID(pBXCopyFilesBuildPhase.guid);
			PBXContainerItemProxy pBXContainerItemProxy = PBXContainerItemProxy.Create(project.project.guid, "1", pBXNativeTarget.guid, name);
			containerItems.AddEntry(pBXContainerItemProxy);
			PBXTargetDependency pBXTargetDependency = PBXTargetDependency.Create(pBXNativeTarget.guid, pBXContainerItemProxy.guid);
			targetDependencies.AddEntry(pBXTargetDependency);
			nativeTargets[mainTarget].dependencies.AddGUID(pBXTargetDependency.guid);
			AddFile(text, "Products/" + text, PBXSourceTree.Build);
			PBXBuildFile pBXBuildFile = PBXBuildFile.CreateFromFile(FindFileGuidByProjectPath("Products/" + text), false, string.Empty);
			BuildFilesAdd(mainTarget, pBXBuildFile);
			pBXCopyFilesBuildPhase.files.AddGUID(pBXBuildFile.guid);
			AddFile(infoPlistPath, name + "/Supporting Files/Info.plist", PBXSourceTree.Group);
			return pBXNativeTarget.guid;
		}

		public string BuildConfigByName(string targetGuid, string name)
		{
			PBXNativeTarget pBXNativeTarget = nativeTargets[targetGuid];
			foreach (string item in (IEnumerable<string>)configs[pBXNativeTarget.buildConfigList].buildConfigs)
			{
				XCBuildConfiguration xCBuildConfiguration = buildConfigs[item];
				if (xCBuildConfiguration != null && xCBuildConfiguration.name == name)
				{
					return xCBuildConfiguration.guid;
				}
			}
			return null;
		}

		public void AddBuildProperty(string targetGuid, string name, string value)
		{
			PBXNativeTarget pBXNativeTarget = nativeTargets[targetGuid];
			foreach (string item in (IEnumerable<string>)configs[pBXNativeTarget.buildConfigList].buildConfigs)
			{
				buildConfigs[item].AddProperty(name, value);
			}
		}

		public void AddBuildProperty(string[] targetGuids, string name, string value)
		{
			foreach (string targetGuid in targetGuids)
			{
				AddBuildProperty(targetGuid, name, value);
			}
		}

		public void AddBuildPropertyForConfig(string configGuid, string name, string value)
		{
			buildConfigs[configGuid].AddProperty(name, value);
		}

		public void AddBuildPropertyForConfig(string[] configGuids, string name, string value)
		{
			foreach (string configGuid in configGuids)
			{
				AddBuildPropertyForConfig(configGuid, name, value);
			}
		}

		public void SetBuildProperty(string targetGuid, string name, string value)
		{
			PBXNativeTarget pBXNativeTarget = nativeTargets[targetGuid];
			foreach (string item in (IEnumerable<string>)configs[pBXNativeTarget.buildConfigList].buildConfigs)
			{
				buildConfigs[item].SetProperty(name, value);
			}
		}

		public void SetBuildProperty(string[] targetGuids, string name, string value)
		{
			foreach (string targetGuid in targetGuids)
			{
				SetBuildProperty(targetGuid, name, value);
			}
		}

		public void SetBuildPropertyForConfig(string configGuid, string name, string value)
		{
			buildConfigs[configGuid].SetProperty(name, value);
		}

		public void SetBuildPropertyForConfig(string[] configGuids, string name, string value)
		{
			foreach (string configGuid in configGuids)
			{
				SetBuildPropertyForConfig(configGuid, name, value);
			}
		}

		public void UpdateBuildProperty(string targetGuid, string name, string[] addValues, string[] removeValues)
		{
			PBXNativeTarget pBXNativeTarget = nativeTargets[targetGuid];
			foreach (string item in (IEnumerable<string>)configs[pBXNativeTarget.buildConfigList].buildConfigs)
			{
				buildConfigs[item].UpdateProperties(name, addValues, removeValues);
			}
		}

		public void UpdateBuildProperty(string[] targetGuids, string name, string[] addValues, string[] removeValues)
		{
			foreach (string targetGuid in targetGuids)
			{
				UpdateBuildProperty(targetGuid, name, addValues, removeValues);
			}
		}

		public void UpdateBuildPropertyForConfig(string configGuid, string name, string[] addValues, string[] removeValues)
		{
			buildConfigs[configGuid].UpdateProperties(name, addValues, removeValues);
		}

		public void UpdateBuildPropertyForConfig(string[] configGuids, string name, string[] addValues, string[] removeValues)
		{
			foreach (string targetGuid in configGuids)
			{
				UpdateBuildProperty(targetGuid, name, addValues, removeValues);
			}
		}

		private static string FixSlashesInPath(string path)
		{
			if (path == null)
			{
				return null;
			}
			return path.Replace('\\', '/');
		}

		private void BuildCommentMapForBuildFiles(GUIDToCommentMap comments, List<string> guids, string sectName)
		{
			foreach (string guid in guids)
			{
				PBXBuildFile pBXBuildFile = buildFiles[guid];
				if (pBXBuildFile == null)
				{
					continue;
				}
				PBXFileReference pBXFileReference = fileRefs[pBXBuildFile.fileRef];
				if (pBXFileReference != null)
				{
					comments.Add(guid, string.Format("{0} in {1}", pBXFileReference.name, sectName));
					continue;
				}
				PBXReferenceProxy pBXReferenceProxy = references[pBXBuildFile.fileRef];
				if (pBXReferenceProxy != null)
				{
					comments.Add(guid, string.Format("{0} in {1}", pBXReferenceProxy.path, sectName));
				}
			}
		}

		private GUIDToCommentMap BuildCommentMap()
		{
			GUIDToCommentMap gUIDToCommentMap = new GUIDToCommentMap();
			foreach (PBXGroup value in groups.entries.Values)
			{
				gUIDToCommentMap.Add(value.guid, value.name);
			}
			foreach (PBXContainerItemProxy value2 in containerItems.entries.Values)
			{
				gUIDToCommentMap.Add(value2.guid, "PBXContainerItemProxy");
			}
			foreach (PBXReferenceProxy value3 in references.entries.Values)
			{
				gUIDToCommentMap.Add(value3.guid, value3.path);
			}
			foreach (PBXSourcesBuildPhase value4 in sources.entries.Values)
			{
				gUIDToCommentMap.Add(value4.guid, "Sources");
				BuildCommentMapForBuildFiles(gUIDToCommentMap, value4.files, "Sources");
			}
			foreach (PBXResourcesBuildPhase value5 in resources.entries.Values)
			{
				gUIDToCommentMap.Add(value5.guid, "Resources");
				BuildCommentMapForBuildFiles(gUIDToCommentMap, value5.files, "Resources");
			}
			foreach (PBXFrameworksBuildPhase value6 in frameworks.entries.Values)
			{
				gUIDToCommentMap.Add(value6.guid, "Frameworks");
				BuildCommentMapForBuildFiles(gUIDToCommentMap, value6.files, "Frameworks");
			}
			foreach (PBXCopyFilesBuildPhase value7 in copyFiles.entries.Values)
			{
				string text = value7.name;
				if (text == null)
				{
					text = "CopyFiles";
				}
				gUIDToCommentMap.Add(value7.guid, text);
				BuildCommentMapForBuildFiles(gUIDToCommentMap, value7.files, text);
			}
			foreach (PBXShellScriptBuildPhase value8 in shellScripts.entries.Values)
			{
				gUIDToCommentMap.Add(value8.guid, "ShellScript");
			}
			foreach (PBXTargetDependency value9 in targetDependencies.entries.Values)
			{
				gUIDToCommentMap.Add(value9.guid, "PBXTargetDependency");
			}
			foreach (PBXNativeTarget value10 in nativeTargets.entries.Values)
			{
				gUIDToCommentMap.Add(value10.guid, value10.name);
				gUIDToCommentMap.Add(value10.buildConfigList, string.Format("Build configuration list for PBXNativeTarget \"{0}\"", value10.name));
			}
			foreach (PBXVariantGroup value11 in variantGroups.entries.Values)
			{
				gUIDToCommentMap.Add(value11.guid, value11.name);
			}
			foreach (XCBuildConfiguration value12 in buildConfigs.entries.Values)
			{
				gUIDToCommentMap.Add(value12.guid, value12.name);
			}
			foreach (PBXProjectObject value13 in project.entries.Values)
			{
				gUIDToCommentMap.Add(value13.guid, "Project object");
				gUIDToCommentMap.Add(value13.buildConfigList, "Build configuration list for PBXProject \"Unity-iPhone\"");
			}
			foreach (PBXFileReference value14 in fileRefs.entries.Values)
			{
				gUIDToCommentMap.Add(value14.guid, value14.name);
			}
			if (m_RootElements.Contains("rootObject") && m_RootElements["rootObject"] is PBXElementString)
			{
				gUIDToCommentMap.Add(m_RootElements["rootObject"].AsString(), "Project object");
			}
			return gUIDToCommentMap;
		}

		public void ReadFromFile(string path)
		{
			ReadFromString(File.ReadAllText(path));
		}

		public void ReadFromString(string src)
		{
			TextReader sr = new StringReader(src);
			ReadFromStream(sr);
		}

		private static PBXElementDict ParseContent(string content)
		{
			TokenList tokens = Lexer.Tokenize(content);
			Parser parser = new Parser(tokens);
			TreeAST ast = parser.ParseTree();
			return Serializer.ParseTreeAST(ast, tokens, content);
		}

		public void ReadFromStream(TextReader sr)
		{
			Clear();
			m_RootElements = ParseContent(sr.ReadToEnd());
			if (!m_RootElements.Contains("objects"))
			{
				throw new Exception("Invalid PBX project file: no objects element");
			}
			PBXElementDict pBXElementDict = m_RootElements["objects"].AsDict();
			m_RootElements.Remove("objects");
			m_RootElements.SetString("objects", "OBJMARKER");
			if (m_RootElements.Contains("objectVersion"))
			{
				m_ObjectVersion = m_RootElements["objectVersion"].AsString();
				m_RootElements.Remove("objectVersion");
			}
			List<string> list = new List<string>();
			string prevSectionName = null;
			foreach (KeyValuePair<string, PBXElement> value2 in pBXElementDict.values)
			{
				list.Add(value2.Key);
				PBXElement value = value2.Value;
				if (!(value is PBXElementDict) || !value.AsDict().Contains("isa"))
				{
					m_UnknownObjects.values.Add(value2.Key, value);
					continue;
				}
				PBXElementDict pBXElementDict2 = value.AsDict();
				string text = pBXElementDict2["isa"].AsString();
				if (m_Section.ContainsKey(text))
				{
					SectionBase sectionBase = m_Section[text];
					sectionBase.AddObject(value2.Key, pBXElementDict2);
				}
				else
				{
					KnownSectionBase<PBXObject> knownSectionBase;
					if (m_UnknownSections.ContainsKey(text))
					{
						knownSectionBase = m_UnknownSections[text];
					}
					else
					{
						knownSectionBase = new KnownSectionBase<PBXObject>(text);
						m_UnknownSections.Add(text, knownSectionBase);
					}
					knownSectionBase.AddObject(value2.Key, pBXElementDict2);
					if (!m_SectionOrder.Contains(text))
					{
						int index = 0;
						if (prevSectionName != null)
						{
							index = m_SectionOrder.FindIndex((string x) => x == prevSectionName);
							index++;
						}
						m_SectionOrder.Insert(index, text);
					}
				}
				prevSectionName = text;
			}
			RepairStructure(list);
			RefreshAuxMaps();
		}

		public void WriteToFile(string path)
		{
			File.WriteAllText(path, WriteToString());
		}

		public void WriteToStream(TextWriter sw)
		{
			sw.Write(WriteToString());
		}

		public string WriteToString()
		{
			GUIDToCommentMap comments = BuildCommentMap();
			PropertyCommentChecker checker = new PropertyCommentChecker();
			GUIDToCommentMap comments2 = new GUIDToCommentMap();
			StringBuilder stringBuilder = new StringBuilder();
			if (m_ObjectVersion != null)
			{
				stringBuilder.AppendFormat("objectVersion = {0};\n\t", m_ObjectVersion);
			}
			stringBuilder.Append("objects = {");
			foreach (string item in m_SectionOrder)
			{
				if (m_Section.ContainsKey(item))
				{
					m_Section[item].WriteSection(stringBuilder, comments);
				}
				else if (m_UnknownSections.ContainsKey(item))
				{
					m_UnknownSections[item].WriteSection(stringBuilder, comments);
				}
			}
			foreach (KeyValuePair<string, PBXElement> value in m_UnknownObjects.values)
			{
				Serializer.WriteDictKeyValue(stringBuilder, value.Key, value.Value, 2, false, checker, comments2);
			}
			stringBuilder.Append("\n\t};");
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine("// !$*UTF8*$!");
			Serializer.WriteDict(stringBuilder2, m_RootElements, 0, false, new PropertyCommentChecker(new string[1]
			{
				"rootObject/*"
			}), comments);
			stringBuilder2.AppendLine();
			string text = stringBuilder2.ToString();
			return text.Replace("objects = OBJMARKER;", stringBuilder.ToString());
		}

		private void RepairStructure(List<string> allGuids)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (string allGuid in allGuids)
			{
				dictionary.Add(allGuid, false);
			}
			while (!RepairStructureImpl(dictionary))
			{
			}
		}

		private static void RepairStructureRemoveMissingGuids(GUIDList guidList, Dictionary<string, bool> allGuids)
		{
			List<string> list = null;
			foreach (string item in (IEnumerable<string>)guidList)
			{
				if (!allGuids.ContainsKey(item))
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(item);
				}
			}
			if (list == null)
			{
				return;
			}
			foreach (string item2 in list)
			{
				guidList.RemoveGUID(item2);
			}
		}

		private static void RepairStructureAnyType<T>(KnownSectionBase<T> section, Func<T, bool> checker, Dictionary<string, bool> allGuids, ref bool ok) where T : PBXObject, new()
		{
			List<string> list = null;
			foreach (KeyValuePair<string, T> entry in section.entries)
			{
				if (!checker(entry.Value))
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(entry.Key);
				}
			}
			if (list == null)
			{
				return;
			}
			ok = false;
			foreach (string item in list)
			{
				section.RemoveEntry(item);
				allGuids.Remove(item);
			}
		}

		private static void RepairStructureGuidList<T>(KnownSectionBase<T> section, Func<T, GUIDList> listRetrieveFunc, Dictionary<string, bool> allGuids, ref bool ok) where T : PBXObject, new()
		{
			Func<T, bool> checker = delegate(T obj)
			{
				GUIDList gUIDList = listRetrieveFunc(obj);
				if (gUIDList == null)
				{
					return false;
				}
				RepairStructureRemoveMissingGuids(gUIDList, allGuids);
				return true;
			};
			RepairStructureAnyType(section, checker, allGuids, ref ok);
		}

		private bool RepairStructureImpl(Dictionary<string, bool> allGuids)
		{
			bool ok = true;
			Func<PBXBuildFile, bool> checker = (PBXBuildFile obj) => (obj.fileRef != null && allGuids.ContainsKey(obj.fileRef)) ? true : false;
			RepairStructureAnyType(buildFiles, checker, allGuids, ref ok);
			RepairStructureGuidList(groups, (PBXGroup o) => o.children, allGuids, ref ok);
			RepairStructureGuidList(sources, (PBXSourcesBuildPhase o) => o.files, allGuids, ref ok);
			RepairStructureGuidList(frameworks, (PBXFrameworksBuildPhase o) => o.files, allGuids, ref ok);
			RepairStructureGuidList(resources, (PBXResourcesBuildPhase o) => o.files, allGuids, ref ok);
			RepairStructureGuidList(copyFiles, (PBXCopyFilesBuildPhase o) => o.files, allGuids, ref ok);
			RepairStructureGuidList(shellScripts, (PBXShellScriptBuildPhase o) => o.files, allGuids, ref ok);
			RepairStructureGuidList(nativeTargets, (PBXNativeTarget o) => o.phases, allGuids, ref ok);
			RepairStructureGuidList(variantGroups, (PBXVariantGroup o) => o.children, allGuids, ref ok);
			RepairStructureGuidList(configs, (XCConfigurationList o) => o.buildConfigs, allGuids, ref ok);
			return ok;
		}
	}
}
