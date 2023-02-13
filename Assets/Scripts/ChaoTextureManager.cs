using DataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaoTextureManager : MonoBehaviour
{
	public class TextureData
	{
		public int chao_id;

		public Texture tex;

		public TextureData()
		{
		}

		public TextureData(Texture tex, int chao_id)
		{
			this.tex = tex;
			this.chao_id = chao_id;
		}
	}

	public class CallbackInfo
	{
		public delegate void LoadFinishCallback(Texture tex);

		private Texture m_texture;

		private UITexture m_uiTex;

		private bool m_nguiRebuild;

		private LoadFinishCallback m_callback;

		public Texture Texture
		{
			get
			{
				return m_texture;
			}
			private set
			{
			}
		}

		public bool LoadEnded
		{
			get
			{
				if (m_texture != null)
				{
					return true;
				}
				return false;
			}
		}

		public CallbackInfo(UITexture uiTex, LoadFinishCallback callback = null, bool nguiRebuild = false)
		{
			m_uiTex = uiTex;
			m_callback = callback;
			m_nguiRebuild = nguiRebuild;
			if (m_uiTex != null)
			{
				m_uiTex.enabled = true;
				m_uiTex.SetTexture(Instance.m_defaultTexture);
			}
		}

		public void Disable()
		{
			if (m_uiTex != null)
			{
				m_uiTex.enabled = false;
				m_uiTex.SetTexture(null);
			}
			if (m_callback != null)
			{
				m_callback(null);
			}
		}

		public void LoadDone(Texture tex)
		{
			m_texture = tex;
			if (m_uiTex != null)
			{
				m_uiTex.enabled = true;
				if (m_nguiRebuild)
				{
					m_uiTex.mainTexture = tex;
				}
				else
				{
					m_uiTex.SetTexture(tex);
				}
			}
			if (m_callback != null)
			{
				m_callback(m_texture);
			}
		}
	}

	public class LoadRequestData
	{
		public enum LoadType
		{
			Default,
			Event
		}

		private enum RequestMode
		{
			IDLE,
			LOAD,
			LOAD_WAIT,
			SET_TEXTURE,
			END
		}

		private RequestMode m_requestMode;

		private ResourceSceneLoader m_sceneLoader;

		private GameObject m_managerObj;

		private bool m_cancel;

		public int m_chaoId;

		public LoadType m_type;

		private List<CallbackInfo> m_infoList;

		public bool Loaded
		{
			get
			{
				if (m_requestMode == RequestMode.END)
				{
					return true;
				}
				return false;
			}
		}

		public LoadRequestData()
		{
		}

		public LoadRequestData(GameObject managerObj, int chaoId, CallbackInfo info, LoadType type = LoadType.Default)
		{
			m_managerObj = managerObj;
			m_chaoId = chaoId;
			m_type = type;
			m_infoList = new List<CallbackInfo>();
			m_infoList.Add(info);
		}

		public void AddCallback(CallbackInfo info)
		{
			if (info != null && !m_infoList.Contains(info))
			{
				m_infoList.Add(info);
			}
		}

		public void StartLoad()
		{
			m_requestMode = RequestMode.LOAD;
		}

		public void Cancel()
		{
			m_cancel = true;
		}

		public void Update()
		{
			switch (m_requestMode)
			{
			case RequestMode.IDLE:
				break;
			case RequestMode.LOAD:
			{
				GameObject gameObject6 = new GameObject("SceneLoader");
				m_sceneLoader = gameObject6.AddComponent<ResourceSceneLoader>();
				string scenename = "ui_tex_chao_" + m_chaoId.ToString("0000");
				ResourceSceneLoader.ResourceInfo resourceInfo = new ResourceSceneLoader.ResourceInfo();
				resourceInfo.m_scenename = scenename;
				resourceInfo.m_isAssetBundle = true;
				resourceInfo.m_onlyDownload = false;
				resourceInfo.m_isAsycnLoad = true;
				resourceInfo.m_category = ResourceCategory.UI;
				m_sceneLoader.AddLoadAndResourceManager(resourceInfo);
				m_requestMode = RequestMode.LOAD_WAIT;
				break;
			}
			case RequestMode.LOAD_WAIT:
				if (m_sceneLoader != null)
				{
					if (m_sceneLoader.Loaded)
					{
						m_requestMode = RequestMode.SET_TEXTURE;
					}
				}
				else
				{
					m_requestMode = RequestMode.SET_TEXTURE;
				}
				break;
			case RequestMode.SET_TEXTURE:
				if (m_cancel)
				{
					string name = "ui_tex_chao_" + m_chaoId.ToString("0000");
					GameObject gameObject = GameObject.Find(name);
					if (gameObject != null)
					{
						Object.Destroy(gameObject);
					}
				}
				else
				{
					string name2 = "ui_tex_chao_" + m_chaoId.ToString("0000");
					GameObject gameObject2 = GameObject.Find(name2);
					if (gameObject2 != null)
					{
						switch (m_type)
						{
						case LoadType.Default:
						{
							GameObject gameObject4 = GameObjectUtil.FindChildGameObject(m_managerObj, "Texture");
							gameObject2.transform.parent = gameObject4.transform;
							break;
						}
						case LoadType.Event:
						{
							GameObject gameObject3 = GameObjectUtil.FindChildGameObject(m_managerObj, "Event");
							gameObject2.transform.parent = gameObject3.transform;
							break;
						}
						}
						gameObject2.SetActive(false);
					}
					Texture texture = null;
					GameObject gameObject5 = GameObjectUtil.FindChildGameObject(m_managerObj, name2);
					AssetBundleTexture component = gameObject5.GetComponent<AssetBundleTexture>();
					texture = component.m_tex;
					if (texture != null)
					{
						for (int i = 0; i < m_infoList.Count; i++)
						{
							CallbackInfo callbackInfo = m_infoList[i];
							if (callbackInfo != null)
							{
								callbackInfo.LoadDone(texture);
							}
						}
						m_infoList.Clear();
					}
				}
				Object.Destroy(m_sceneLoader.gameObject);
				m_requestMode = RequestMode.END;
				break;
			}
		}

		public Texture GetTexture(int chao_id)
		{
			string name = "ui_tex_chao_" + m_chaoId.ToString("0000");
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_managerObj, name);
			if (gameObject != null)
			{
				AssetBundleTexture component = gameObject.GetComponent<AssetBundleTexture>();
				if (component != null)
				{
					return component.m_tex;
				}
			}
			return null;
		}
	}

	private const string TEX_FOLDER = "Texture";

	private const string EVENT_FOLDER = "Event";

	[SerializeField]
	public Texture m_defaultTexture;

	private List<LoadRequestData> m_loadingList = new List<LoadRequestData>();

	private LoadRequestData m_currentRequest;

	private GameObject m_texObj;

	private int m_loadingChaoId = -1;

	private static ChaoTextureManager instance;

	public static ChaoTextureManager Instance
	{
		get
		{
			return instance;
		}
	}

	public int LoadingChaoId
	{
		get
		{
			return m_loadingChaoId;
		}
	}

	public Texture GetLoadedTexture(int chao_id)
	{
		if (chao_id < 0)
		{
			return null;
		}
		Texture result = null;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, GetChaoTexName(chao_id));
		if (gameObject != null)
		{
			AssetBundleTexture component = gameObject.GetComponent<AssetBundleTexture>();
			if (component != null)
			{
				result = component.m_tex;
			}
		}
		return result;
	}

	public void GetTexture(int chao_id, CallbackInfo info)
	{
		if (info == null)
		{
			return;
		}
		if (chao_id < 0)
		{
			info.Disable();
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, GetChaoTexName(chao_id));
		if (gameObject != null)
		{
			AssetBundleTexture component = gameObject.GetComponent<AssetBundleTexture>();
			if (component != null)
			{
				info.LoadDone(component.m_tex);
			}
			return;
		}
		bool flag = true;
		foreach (LoadRequestData loading in m_loadingList)
		{
			if (loading.m_chaoId == chao_id)
			{
				if (info != null)
				{
					loading.AddCallback(info);
				}
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		if (m_currentRequest != null && m_currentRequest.m_chaoId == chao_id)
		{
			m_currentRequest.AddCallback(info);
			flag = false;
		}
		if (flag)
		{
			LoadRequestData loadRequestData = new LoadRequestData(base.gameObject, chao_id, info);
			if (m_currentRequest == null)
			{
				m_currentRequest = loadRequestData;
				m_currentRequest.StartLoad();
			}
			else
			{
				m_loadingList.Add(loadRequestData);
			}
			base.enabled = true;
		}
	}

	public void RequestLoadingPageChaoTexture()
	{
		ChaoData loadingChao = ChaoTable.GetLoadingChao();
		if (loadingChao != null)
		{
			m_loadingChaoId = loadingChao.id;
			CallbackInfo info = new CallbackInfo(null);
			GetTexture(loadingChao.id, info);
		}
	}

	public void RequestTitleLoadChaoTexture()
	{
		RequestLoadingPageChaoTexture();
	}

	public void RemoveChaoTexture(int chao_id)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_texObj, GetChaoTexName(chao_id));
		if (gameObject != null)
		{
			string name = gameObject.name;
			int num = -1;
			int num2 = -1;
			SaveDataManager saveDataManager = SaveDataManager.Instance;
			if (saveDataManager != null)
			{
				num = saveDataManager.PlayerData.MainChaoID;
				num2 = saveDataManager.PlayerData.SubChaoID;
			}
			List<string> list = new List<string>();
			if (num >= 0)
			{
				list.Add(GetChaoTexName(num));
			}
			if (num2 >= 0)
			{
				list.Add(GetChaoTexName(num2));
			}
			if (m_loadingChaoId >= 0)
			{
				list.Add(GetChaoTexName(m_loadingChaoId));
			}
			if (!list.Contains(name))
			{
				Object.Destroy(gameObject);
			}
			StartCoroutine(WaitUnloadUnusedAssets());
		}
	}

	public void RemoveChaoTextureForMainMenuEnd()
	{
		int num = -1;
		int num2 = -1;
		SaveDataManager saveDataManager = SaveDataManager.Instance;
		if (saveDataManager != null)
		{
			num = saveDataManager.PlayerData.MainChaoID;
			num2 = saveDataManager.PlayerData.SubChaoID;
		}
		List<string> list = new List<string>();
		if (num >= 0)
		{
			list.Add(GetChaoTexName(num));
		}
		if (num2 >= 0)
		{
			list.Add(GetChaoTexName(num2));
		}
		if (m_loadingChaoId >= 0)
		{
			list.Add(GetChaoTexName(m_loadingChaoId));
		}
		if (EventManager.Instance != null)
		{
			RewardChaoData rewardChaoData = EventManager.Instance.GetRewardChaoData();
			if (rewardChaoData != null && rewardChaoData.chao_id >= 0)
			{
				list.Add(GetChaoTexName(rewardChaoData.chao_id));
			}
		}
		List<GameObject> list2 = new List<GameObject>();
		int childCount = m_texObj.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = m_texObj.transform.GetChild(i);
			string name = child.name;
			if (!list.Contains(name))
			{
				list2.Add(child.gameObject);
			}
		}
		foreach (GameObject item in list2)
		{
			Object.Destroy(item);
		}
		CancelLoad();
	}

	public void RemoveChaoTexture()
	{
		int num = -1;
		int num2 = -1;
		SaveDataManager saveDataManager = SaveDataManager.Instance;
		if (saveDataManager != null)
		{
			num = saveDataManager.PlayerData.MainChaoID;
			num2 = saveDataManager.PlayerData.SubChaoID;
		}
		List<string> list = new List<string>();
		if (num >= 0)
		{
			list.Add(GetChaoTexName(num));
		}
		if (num2 >= 0)
		{
			list.Add(GetChaoTexName(num2));
		}
		if (m_loadingChaoId >= 0)
		{
			list.Add(GetChaoTexName(m_loadingChaoId));
		}
		List<GameObject> list2 = new List<GameObject>();
		int childCount = m_texObj.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = m_texObj.transform.GetChild(i);
			string name = child.name;
			if (!list.Contains(name))
			{
				list2.Add(child.gameObject);
			}
		}
		foreach (GameObject item in list2)
		{
			Object.Destroy(item);
		}
		CancelLoad();
		StartCoroutine(WaitUnloadUnusedAssets());
	}

	private IEnumerator WaitUnloadUnusedAssets()
	{
		int waite_frame = 1;
		while (waite_frame > 0)
		{
			waite_frame--;
			yield return null;
		}
		Resources.UnloadUnusedAssets();
	}

	public bool IsLoaded()
	{
		if (m_currentRequest != null)
		{
			return false;
		}
		if (m_loadingList.Count > 0)
		{
			return false;
		}
		return true;
	}

	protected void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "Event";
		gameObject.transform.parent = base.transform;
		m_texObj = new GameObject();
		m_texObj.name = "Texture";
		m_texObj.transform.parent = base.transform;
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void SetInstance()
	{
		if (instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (m_currentRequest == null)
		{
			return;
		}
		m_currentRequest.Update();
		if (m_currentRequest.Loaded)
		{
			m_currentRequest = null;
			if (m_loadingList.Count > 0)
			{
				StartCoroutine(LoadNext());
			}
			else
			{
				base.enabled = false;
			}
		}
	}

	private IEnumerator LoadNext()
	{
		float startTime = Time.realtimeSinceStartup;
		while (true)
		{
			float spendTime2 = 0f;
			float currentTime = Time.realtimeSinceStartup;
			spendTime2 = currentTime - startTime;
			if (spendTime2 >= 0.1f)
			{
				break;
			}
			yield return null;
		}
		if (m_loadingList.Count > 0)
		{
			m_currentRequest = m_loadingList[0];
			m_currentRequest.StartLoad();
			m_loadingList.Remove(m_loadingList[0]);
		}
	}

	private void CancelLoad()
	{
		if (m_currentRequest != null)
		{
			m_currentRequest.Cancel();
		}
		m_loadingList.Clear();
	}

	private string GetChaoTexName(int chao_id)
	{
		return "ui_tex_chao_" + chao_id.ToString("0000");
	}
}
