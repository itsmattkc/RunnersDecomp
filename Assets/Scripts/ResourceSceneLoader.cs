using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSceneLoader : MonoBehaviour
{
	public class ResourceInfo
	{
		public bool m_isAssetBundle;

		public bool m_onlyDownload;

		public bool m_isloaded;

		public string m_scenename;

		public ResourceCategory m_category = ResourceCategory.UNKNOWN;

		public bool m_dontDestroyOnChangeScene;

		public string m_rootObjectName;

		public bool m_isAsycnLoad;

		public ResourceInfo()
		{
		}

		public ResourceInfo(ResourceCategory cate, string name, bool assetbundle, bool onlyDownload, bool dontdestroyOnScene, string rootObjectName = null, bool isAsyncLoad = false)
		{
			m_scenename = name;
			m_category = cate;
			m_dontDestroyOnChangeScene = (!onlyDownload && dontdestroyOnScene);
			m_isAssetBundle = assetbundle;
			m_onlyDownload = onlyDownload;
			m_rootObjectName = ((rootObjectName != null) ? rootObjectName : m_scenename);
			m_isAsycnLoad = isAsyncLoad;
			m_isloaded = false;
		}
	}

	private bool m_checkTime = true;

	private List<ResourceInfo> m_loadInfos = new List<ResourceInfo>();

	private List<ResourceInfo> m_assetLoadInfos = new List<ResourceInfo>();

	private bool m_isloaded;

	private bool m_pause;

	private int m_loadEndCount;

	public int LoadEndCount
	{
		get
		{
			return m_loadEndCount;
		}
		private set
		{
		}
	}

	public int RequestedLoadCount
	{
		get
		{
			return m_loadInfos.Count;
		}
		private set
		{
		}
	}

	public bool Loaded
	{
		get
		{
			return m_isloaded;
		}
	}

	private void Start()
	{
		m_checkTime = false;
		StartAssetBundleLoad();
		StartCoroutine(LoadScene());
	}

	private void Update()
	{
		foreach (ResourceInfo loadInfo in m_loadInfos)
		{
			if (!loadInfo.m_isloaded)
			{
				m_isloaded = false;
				return;
			}
		}
		foreach (ResourceInfo assetLoadInfo in m_assetLoadInfos)
		{
			if (!assetLoadInfo.m_isloaded)
			{
				m_isloaded = false;
				return;
			}
		}
		m_isloaded = true;
	}

	public void Pause(bool value)
	{
		m_pause = value;
		base.enabled = !m_pause;
	}

	public bool AddLoad(string scenename, bool onAssetBundle, bool onlyDownload)
	{
		if ((bool)ResourceManager.Instance && ResourceManager.Instance.IsExistContainer(scenename))
		{
			return false;
		}
		ResourceInfo resourceInfo = new ResourceInfo();
		resourceInfo.m_scenename = scenename;
		resourceInfo.m_isAssetBundle = onAssetBundle;
		resourceInfo.m_isloaded = false;
		resourceInfo.m_category = ResourceCategory.UNKNOWN;
		resourceInfo.m_onlyDownload = onlyDownload;
		if (onAssetBundle && AssetBundleLoader.Instance != null)
		{
			if (onlyDownload && AssetBundleLoader.Instance.IsDownloaded(scenename))
			{
				return false;
			}
			bool flag = true;
			foreach (ResourceInfo assetLoadInfo in m_assetLoadInfos)
			{
				if (assetLoadInfo.m_scenename == scenename)
				{
					flag = false;
				}
			}
			if (flag)
			{
				m_assetLoadInfos.Add(resourceInfo);
			}
		}
		else
		{
			if (onlyDownload)
			{
				return false;
			}
			bool flag2 = true;
			foreach (ResourceInfo assetLoadInfo2 in m_assetLoadInfos)
			{
				if (assetLoadInfo2.m_scenename == scenename)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				m_loadInfos.Add(resourceInfo);
			}
		}
		return true;
	}

	public bool AddLoadAndResourceManager(string scenename, bool onAssetBundle, ResourceCategory category, bool dontDestroyOnChangeScene, bool onlyDownload, string rootObjectName)
	{
		if ((bool)ResourceManager.Instance && ResourceManager.Instance.IsExistContainer(scenename))
		{
			return false;
		}
		ResourceInfo resourceInfo = new ResourceInfo();
		resourceInfo.m_scenename = scenename;
		resourceInfo.m_isAssetBundle = onAssetBundle;
		resourceInfo.m_isloaded = false;
		resourceInfo.m_category = category;
		resourceInfo.m_dontDestroyOnChangeScene = dontDestroyOnChangeScene;
		resourceInfo.m_rootObjectName = ((rootObjectName == null) ? scenename : rootObjectName);
		resourceInfo.m_onlyDownload = onlyDownload;
		AddLoadAndResourceManager(resourceInfo);
		return true;
	}

	public bool AddLoadAndResourceManager(ResourceInfo info)
	{
		if ((bool)ResourceManager.Instance && ResourceManager.Instance.IsExistContainer(info.m_scenename))
		{
			return false;
		}
		bool isAssetBundle = info.m_isAssetBundle;
		bool onlyDownload = info.m_onlyDownload;
		if (isAssetBundle && AssetBundleLoader.Instance != null)
		{
			if (onlyDownload && AssetBundleLoader.Instance.IsDownloaded(info.m_scenename))
			{
				return false;
			}
			m_assetLoadInfos.Add(info);
		}
		else
		{
			if (onlyDownload)
			{
				return false;
			}
			m_loadInfos.Add(info);
		}
		return true;
	}

	private IEnumerator LoadScene()
	{
		m_loadEndCount = 0;
		float loadStartTime = Time.realtimeSinceStartup;
		foreach (ResourceInfo loadInfo in m_loadInfos)
		{
			if (loadInfo.m_scenename != null)
			{
				float oldTime = Time.realtimeSinceStartup;
				if (m_checkTime)
				{
					Debug.Log("start load scene " + loadInfo.m_scenename);
				}
				if (loadInfo.m_isAsycnLoad)
				{
					AsyncOperation operation = Application.LoadLevelAdditiveAsync(loadInfo.m_scenename);
					yield return StartCoroutine(WaitLoard(operation));
				}
				else
				{
					Application.LoadLevelAdditive(loadInfo.m_scenename);
					yield return StartCoroutine(WaitLoard());
				}
				RegisterResourceManager(loadInfo);
				loadInfo.m_isloaded = true;
				m_loadEndCount++;
				if (m_checkTime)
				{
					Debug.Log(string.Concat(str3: (Time.realtimeSinceStartup - oldTime).ToString(), str0: "LS:Load File ", str1: loadInfo.m_scenename, str2: " Time is "));
				}
			}
		}
		if (m_checkTime)
		{
			float loadEndTime = Time.realtimeSinceStartup;
			Debug.Log("LS:All Loading Time is " + (loadEndTime - loadStartTime));
		}
	}

	private IEnumerator LoadSceneAssetBundle(ResourceInfo loadInfo, AssetBundleResult result)
	{
		float oldTime = Time.realtimeSinceStartup;
		if (m_checkTime)
		{
			Debug.Log("start load scene " + loadInfo.m_scenename);
		}
		if (loadInfo.m_isAsycnLoad)
		{
			AsyncOperation operation = Application.LoadLevelAdditiveAsync(loadInfo.m_scenename);
			yield return StartCoroutine(WaitLoard(operation));
		}
		else
		{
			Application.LoadLevelAdditive(loadInfo.m_scenename);
			yield return StartCoroutine(WaitLoard());
		}
		RegisterResourceManager(loadInfo);
		loadInfo.m_isloaded = true;
		m_loadEndCount++;
		if (m_checkTime)
		{
			Debug.Log(string.Concat(str3: (Time.realtimeSinceStartup - oldTime).ToString(), str0: "LS:Load File ", str1: loadInfo.m_scenename, str2: " Time is "));
		}
		if (result != null && (bool)AssetBundleManager.Instance)
		{
			AssetBundleManager.Instance.RequestUnload(result.Path);
		}
	}

	private void StartAssetBundleLoad()
	{
		foreach (ResourceInfo assetLoadInfo in m_assetLoadInfos)
		{
			AssetBundleLoader.Instance.RequestLoadScene(assetLoadInfo.m_scenename, true, base.gameObject);
		}
	}

	private void RegisterResourceManager(ResourceInfo loadInfo)
	{
		if (loadInfo.m_category != ResourceCategory.UNKNOWN && ResourceManager.Instance != null)
		{
			GameObject gameObject = GameObject.Find(loadInfo.m_rootObjectName);
			if (gameObject != null)
			{
				ResourceManager.Instance.AddCategorySceneObjects(loadInfo.m_category, loadInfo.m_scenename, gameObject, loadInfo.m_dontDestroyOnChangeScene);
			}
		}
	}

	private IEnumerator WaitLoard(AsyncOperation async)
	{
		while (async.progress < 0.9f)
		{
			yield return null;
		}
	}

	private IEnumerator WaitLoard()
	{
		yield return null;
	}

	private void AssetBundleResponseSucceed(MsgAssetBundleResponseSucceed msg)
	{
		string fileName = msg.m_request.FileName;
		AssetBundleResult result = msg.m_result;
		foreach (ResourceInfo assetLoadInfo in m_assetLoadInfos)
		{
			if (!assetLoadInfo.m_scenename.Equals(fileName))
			{
				continue;
			}
			if (assetLoadInfo.m_onlyDownload)
			{
				assetLoadInfo.m_isloaded = true;
				m_loadEndCount++;
				if (result != null && (bool)AssetBundleManager.Instance)
				{
					AssetBundleManager.Instance.RequestUnload(result.Path);
				}
			}
			else
			{
				StartCoroutine(LoadSceneAssetBundle(assetLoadInfo, result));
			}
			break;
		}
	}
}
