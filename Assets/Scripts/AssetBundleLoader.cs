using App;
using Message;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour
{
	public class CachedFileInfo
	{
		public string name;

		public int version;

		public uint crc;

		public bool downloaded;
	}

	public class LoadingDataInfo
	{
		public string name;

		public string filePath;

		public string fullPath;

		public bool cashed;

		public CachedFileInfo cashedInfo;

		public GameObject returnObject;
	}

	private enum Mode
	{
		Non,
		DownloadList,
		EnableLoad
	}

	private Dictionary<string, CachedFileInfo> m_bundleInfoList;

	private Dictionary<string, LoadingDataInfo> m_loadingDataList = new Dictionary<string, LoadingDataInfo>();

	private List<AssetBundleRequest> m_retryRequest = new List<AssetBundleRequest>();

	private Mode m_mode;

	private bool m_isConnecting;

	private float m_connectUIDisplayTime;

	private static AssetBundleLoader instance;

	public static AssetBundleLoader Instance
	{
		get
		{
			return instance;
		}
	}

	public string[] GetChaoTextureList()
	{
		Dictionary<string, CachedFileInfo>.KeyCollection keys = m_bundleInfoList.Keys;
		List<string> list = new List<string>();
		foreach (string item in keys)
		{
			if (item.Contains("ui_tex_chao_"))
			{
				list.Add(item);
			}
			else if (item.Contains("ui_tex_player_"))
			{
				list.Add(item);
			}
			else if (item.Contains("ui_tex_mile_w"))
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	private void Start()
	{
		if (AssetBundleManager.Instance == null)
		{
			AssetBundleManager.Create();
		}
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
		AssetBundleManager assetBundleManager = AssetBundleManager.Instance;
		if (!(assetBundleManager != null))
		{
			return;
		}
		bool flag = assetBundleManager.Executing || assetBundleManager.RequestCount > 0;
		bool flag2 = m_retryRequest != null && m_retryRequest.Count > 0;
		if (flag || flag2)
		{
			return;
		}
		LoadingDataInfo loadingDataInfo = null;
		foreach (KeyValuePair<string, LoadingDataInfo> loadingData in m_loadingDataList)
		{
			loadingDataInfo = loadingData.Value;
		}
		if (loadingDataInfo != null)
		{
			Debug.Log("ExecuteRequest:" + loadingDataInfo.fullPath);
			ExecuteLoadScene(loadingDataInfo);
		}
	}

	public void Initialize()
	{
		if (Env.useAssetBundle)
		{
			string path = NetUtil.GetAssetBundleUrl() + "ablist.txt";
			AssetBundleManager.Instance.RequestNoCache(path, AssetBundleRequest.Type.TEXT, base.gameObject);
			if (NetMonitor.Instance != null)
			{
				m_connectUIDisplayTime = 0.01f;
				NetMonitor.Instance.StartMonitor(new AssetBundleRetryProcess(base.gameObject), m_connectUIDisplayTime, HudNetworkConnect.DisplayType.ALL);
			}
			m_mode = Mode.DownloadList;
		}
		else
		{
			m_mode = Mode.EnableLoad;
		}
	}

	public void ClearDownloadList()
	{
		m_mode = Mode.Non;
		m_bundleInfoList = null;
		m_loadingDataList.Clear();
		m_retryRequest.Clear();
	}

	public bool IsEnableDownlad()
	{
		return m_mode == Mode.EnableLoad;
	}

	public void RequestLoadScene(string filePath, bool cashed, GameObject returnObject)
	{
		if (m_mode != Mode.EnableLoad)
		{
			Debug.Log("AssetBundleLoader Not Initialized.");
		}
		else if (Env.useAssetBundle)
		{
			LoadingDataInfo loadingDataInfo = new LoadingDataInfo();
			loadingDataInfo.name = Path.GetFileNameWithoutExtension(filePath);
			loadingDataInfo.filePath = filePath;
			loadingDataInfo.fullPath = GetDownloadURL(filePath);
			loadingDataInfo.cashed = cashed;
			loadingDataInfo.returnObject = returnObject;
			if (!IsDownloaded(filePath))
			{
				m_isConnecting = true;
				if (NetMonitor.Instance != null && NetMonitor.Instance.IsIdle())
				{
					m_connectUIDisplayTime = -0.1f;
					NetMonitor.Instance.StartMonitor(new AssetBundleRetryProcess(base.gameObject), m_connectUIDisplayTime, HudNetworkConnect.DisplayType.ALL);
				}
			}
			m_loadingDataList.Add(loadingDataInfo.name, loadingDataInfo);
		}
		else
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
			Application.LoadLevelAdditive(fileNameWithoutExtension);
		}
	}

	public void RetryLoadScene(AssetBundleRetryProcess retryProcess)
	{
		if (NetMonitor.Instance != null)
		{
			NetMonitor.Instance.StartMonitor(new AssetBundleRetryProcess(base.gameObject), m_connectUIDisplayTime, HudNetworkConnect.DisplayType.ALL);
		}
		foreach (AssetBundleRequest item in m_retryRequest)
		{
			AssetBundleManager.Instance.ReRequest(item);
		}
		m_retryRequest.Clear();
	}

	public bool IsDownloaded(string fileName)
	{
		CachedFileInfo fileInfo = GetFileInfo(fileName);
		if (fileInfo != null)
		{
			return fileInfo.downloaded;
		}
		return false;
	}

	private void ExecuteLoadScene(LoadingDataInfo loadDataInfo)
	{
		string fullPath = loadDataInfo.fullPath;
		if (loadDataInfo.cashed)
		{
			string filePath = loadDataInfo.filePath;
			LoadSceneCache(fullPath, loadDataInfo.cashedInfo = GetFileInfo(filePath));
		}
		else
		{
			AssetBundleManager.Instance.RequestNoCache(fullPath, AssetBundleRequest.Type.SCENE, base.gameObject);
		}
	}

	private void LoadSceneCache(string fullPath, CachedFileInfo info)
	{
		int version = 0;
		uint crc = 0u;
		if (info != null)
		{
			version = info.version;
			crc = info.crc;
		}
		AssetBundleManager.Instance.Request(fullPath, version, crc, AssetBundleRequest.Type.SCENE, base.gameObject, true);
	}

	private void AssetBundleResponseSucceed(MsgAssetBundleResponseSucceed msg)
	{
		if (m_mode == Mode.DownloadList)
		{
			StartCoroutine(WaitCachingReady());
			string text = msg.m_result.Text;
			if (text != null)
			{
				ParseAssetBundleList(text);
			}
			AssetBundleManager.Instance.RequestUnload(msg.m_request.path);
			if (NetMonitor.Instance != null)
			{
				NetMonitor.Instance.EndMonitorForward(msg, base.gameObject, null);
				NetMonitor.Instance.EndMonitorBackward();
			}
			m_mode = Mode.EnableLoad;
			return;
		}
		AssetBundleResult result = msg.m_result;
		if (result == null)
		{
			return;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(result.Path);
		LoadingDataInfo value = null;
		if (m_loadingDataList.TryGetValue(fileNameWithoutExtension, out value))
		{
			if (value.cashedInfo != null)
			{
				value.cashedInfo.downloaded = true;
			}
			if (value.returnObject != null)
			{
				value.returnObject.SendMessage("AssetBundleResponseSucceed", msg, SendMessageOptions.DontRequireReceiver);
			}
			m_loadingDataList.Remove(fileNameWithoutExtension);
			if (NetMonitor.Instance != null && m_isConnecting && m_loadingDataList.Count <= 0)
			{
				m_isConnecting = false;
				NetMonitor.Instance.EndMonitorForward(msg, base.gameObject, null);
				NetMonitor.Instance.EndMonitorBackward();
			}
		}
	}

	private void AssetBundleResponseFailed(MsgAssetBundleResponseFailed msg)
	{
		Debug.Log("Load Failed.");
		if (m_mode == Mode.DownloadList)
		{
			m_retryRequest.Add(msg.m_request);
			if (NetMonitor.Instance != null)
			{
				NetMonitor.Instance.EndMonitorForward(new MsgAssetBundleResponseFailedMonitor(), null, null);
				NetMonitor.Instance.EndMonitorBackward();
			}
			return;
		}
		string fileName = msg.m_request.FileName;
		LoadingDataInfo value = null;
		if (m_loadingDataList.TryGetValue(fileName, out value))
		{
			m_retryRequest.Add(msg.m_request);
			NetMonitor.Instance.EndMonitorForward(new MsgAssetBundleResponseFailedMonitor(), value.returnObject, null);
			NetMonitor.Instance.EndMonitorBackward();
		}
	}

	private IEnumerator WaitCachingReady()
	{
		while (!Caching.ready)
		{
			yield return null;
		}
	}

	private void AssetBundleResponseFailedMonitor(MsgAssetBundleResponseFailedMonitor msg)
	{
	}

	private void ParseAssetBundleList(string str)
	{
		List<CsvParser.CsvFields> list = CsvParser.ParseCsvFromText(str);
		if (list == null || list.Count <= 0)
		{
			return;
		}
		m_bundleInfoList = new Dictionary<string, CachedFileInfo>();
		foreach (CsvParser.CsvFields item in list)
		{
			List<string> fieldList = item.FieldList;
			if (fieldList.Count >= 2)
			{
				CachedFileInfo cachedFileInfo = new CachedFileInfo();
				cachedFileInfo.name = Path.GetFileNameWithoutExtension(fieldList[0]);
				int.TryParse(fieldList[1], out cachedFileInfo.version);
				if (fieldList.Count >= 3)
				{
					uint.TryParse(fieldList[2], out cachedFileInfo.crc);
				}
				string downloadURL = GetDownloadURL(cachedFileInfo.name);
				if (Caching.IsVersionCached(downloadURL, cachedFileInfo.version))
				{
					cachedFileInfo.downloaded = true;
				}
				else
				{
					cachedFileInfo.downloaded = false;
				}
				m_bundleInfoList.Add(cachedFileInfo.name, cachedFileInfo);
			}
		}
	}

	private CachedFileInfo GetFileInfo(string path)
	{
		if (m_bundleInfoList == null)
		{
			return null;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
		CachedFileInfo value = null;
		m_bundleInfoList.TryGetValue(fileNameWithoutExtension, out value);
		return value;
	}

	private string GetDownloadURL(string filePath)
	{
		string text = NetUtil.GetAssetBundleUrl() + filePath;
		string extension = Path.GetExtension(filePath);
		if (string.IsNullOrEmpty(extension))
		{
			text += ".unity3d";
		}
		return text;
	}

	protected void Awake()
	{
		CheckInstance();
	}

	public static void Create()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("AssetBundleLoader");
			gameObject.AddComponent<AssetBundleLoader>();
			if (AssetBundleManager.Instance == null)
			{
				AssetBundleManager.Create();
			}
		}
	}

	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = this;
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(base.gameObject);
		return false;
	}

	private void OnDestroy()
	{
		if (this == instance)
		{
			instance = null;
		}
	}
}
