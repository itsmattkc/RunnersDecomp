using System.Collections.Generic;
using Text;
using UnityEngine;

public class AtlasManager : MonoBehaviour
{
	private const string m_loadingLangAtlasName = "ui_load_word_Atlas";

	private const string m_eventDummyAtlasName = "ui_event_reference_Atlas";

	private const string m_playerAtlasName = "ui_cmn_player_bundle_Atlas";

	private const string m_chaoAtlasName = "ChaoTextures";

	private const string m_resultAtlasName = "ui_result_Atlas";

	private const string m_itemAtlasName = "ui_cmn_item_Atlas";

	private readonly string[] m_menuLangAtlasName = new string[4]
	{
		"ui_item_set_2_word_Atlas",
		"ui_mm_contents_word_Atlas",
		"ui_ranking_word_Atlas",
		"ui_mm_info_page_word_Atlas"
	};

	private readonly string[] m_dividedMenuLangAtlasName = new string[3]
	{
		"ui_player_set_2_word_Atlas",
		"ui_shop_word_Atlas",
		"ui_roulette_word_Atlas"
	};

	private readonly string[] m_stageLangAtlasName = new string[4]
	{
		"ui_gp_bit_word_Atlas",
		"ui_result_word_Atlas",
		"ui_tutrial_word_Atlas",
		"ui_shop_word_Atlas"
	};

	private ResourceSceneLoader.ResourceInfo m_loadInfoForEvent = new ResourceSceneLoader.ResourceInfo(ResourceCategory.EVENT_RESOURCE, "EventResourceCommon", true, false, true, "EventResourceCommon");

	private UIAtlas m_itemAtlas;

	private ResourceSceneLoader m_sceneLoader;

	private bool m_loadedAtlas;

	private string m_eventLangDummyAtlasName = string.Empty;

	private string m_eventLangAtlasName = string.Empty;

	private static AtlasManager instance;

	private List<string> m_dontDestryAtlasList = new List<string>();

	public static AtlasManager Instance
	{
		get
		{
			return instance;
		}
	}

	public UIAtlas ItemAtlas
	{
		get
		{
			return m_itemAtlas;
		}
	}

	public string EventLangAtlasName
	{
		get
		{
			return m_eventLangAtlasName;
		}
	}

	public void StartLoadAtlasForMenu()
	{
		if (m_sceneLoader == null)
		{
			GameObject gameObject = new GameObject("SceneLoader");
			if (gameObject != null)
			{
				m_sceneLoader = gameObject.AddComponent<ResourceSceneLoader>();
				AddLoadAtlasForMenu();
				base.enabled = true;
				m_loadedAtlas = false;
			}
		}
	}

	public void StartLoadAtlasForEventMenu()
	{
		if (m_sceneLoader == null)
		{
			GameObject gameObject = new GameObject("SceneLoader");
			if (gameObject != null)
			{
				m_sceneLoader = gameObject.AddComponent<ResourceSceneLoader>();
				AddLoadAtlasForTitle();
				base.enabled = true;
				m_loadedAtlas = false;
			}
		}
	}

	public void StartLoadAtlasForDividedMenu()
	{
		if (m_sceneLoader == null)
		{
			GameObject gameObject = new GameObject("SceneLoader");
			if (gameObject != null)
			{
				m_sceneLoader = gameObject.AddComponent<ResourceSceneLoader>();
				AddLoadAtlasForDividedMenu();
				base.enabled = true;
				m_loadedAtlas = false;
			}
		}
	}

	public void StartLoadAtlasForStage()
	{
		if (m_sceneLoader == null)
		{
			GameObject gameObject = new GameObject("SceneLoader");
			if (gameObject != null)
			{
				m_sceneLoader = gameObject.AddComponent<ResourceSceneLoader>();
				AddLoadAtlasForStage();
				base.enabled = true;
				m_loadedAtlas = false;
			}
		}
	}

	public void StartLoadAtlasForTitle()
	{
		if (m_sceneLoader == null)
		{
			GameObject gameObject = new GameObject("SceneLoader");
			if (gameObject != null)
			{
				m_sceneLoader = gameObject.AddComponent<ResourceSceneLoader>();
				AddLoadAtlasForTitle();
				base.enabled = true;
				m_loadedAtlas = false;
			}
		}
	}

	public void ResetReplaceAtlas()
	{
		if (m_itemAtlas != null)
		{
			m_itemAtlas = null;
		}
		UIAtlas[] array = Resources.FindObjectsOfTypeAll(typeof(UIAtlas)) as UIAtlas[];
		for (int i = 0; i < array.Length; i++)
		{
			array[i].replacement = null;
		}
	}

	public void ResetEventRelaceAtlas()
	{
		UIAtlas[] array = Resources.FindObjectsOfTypeAll(typeof(UIAtlas)) as UIAtlas[];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].name == "ui_event_reference_Atlas")
			{
				array[i].replacement = null;
			}
		}
	}

	public void ReplaceAtlasForMenu(bool isReplaceDividedMenu)
	{
		if (!m_loadedAtlas)
		{
			return;
		}
		string str = "_" + TextUtility.GetSuffixe();
		UIAtlas[] array = Resources.FindObjectsOfTypeAll(typeof(UIAtlas)) as UIAtlas[];
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, "ui_cmn_item_Atlas");
		if (gameObject != null)
		{
			m_itemAtlas = gameObject.GetComponent<UIAtlas>();
		}
		for (int i = 0; i < m_menuLangAtlasName.Length; i++)
		{
			string name = m_menuLangAtlasName[i] + str;
			GameObject gameObject2 = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, name);
			if (!(gameObject2 != null))
			{
				continue;
			}
			UIAtlas component = gameObject2.GetComponent<UIAtlas>();
			if (!(component != null))
			{
				continue;
			}
			UIAtlas[] array2 = array;
			foreach (UIAtlas uIAtlas in array2)
			{
				if (uIAtlas.name == m_menuLangAtlasName[i])
				{
					uIAtlas.replacement = component;
				}
			}
		}
		if (isReplaceDividedMenu)
		{
			for (int k = 0; k < m_dividedMenuLangAtlasName.Length; k++)
			{
				string name2 = m_dividedMenuLangAtlasName[k] + str;
				GameObject gameObject3 = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, name2);
				if (!(gameObject3 != null))
				{
					continue;
				}
				UIAtlas component2 = gameObject3.GetComponent<UIAtlas>();
				if (!(component2 != null))
				{
					continue;
				}
				UIAtlas[] array3 = array;
				foreach (UIAtlas uIAtlas2 in array3)
				{
					if (uIAtlas2.name == m_dividedMenuLangAtlasName[k])
					{
						uIAtlas2.replacement = component2;
					}
				}
			}
		}
		GameObject gameObject4 = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, "ui_cmn_player_bundle_Atlas");
		if (gameObject4 != null)
		{
			UIAtlas component3 = gameObject4.GetComponent<UIAtlas>();
			UIAtlas[] array4 = array;
			foreach (UIAtlas uIAtlas3 in array4)
			{
				if (uIAtlas3 != null && uIAtlas3.name == "ui_cmn_player_Atlas" && uIAtlas3 != component3)
				{
					uIAtlas3.replacement = component3;
				}
			}
		}
		ReplaceEventAtlas(array);
		ReplaceItemAtlas(array);
	}

	public void ReplaceAtlasForStage()
	{
		if (!m_loadedAtlas)
		{
			return;
		}
		string str = "_" + TextUtility.GetSuffixe();
		UIAtlas[] array = Resources.FindObjectsOfTypeAll(typeof(UIAtlas)) as UIAtlas[];
		for (int i = 0; i < m_stageLangAtlasName.Length; i++)
		{
			string name = m_stageLangAtlasName[i] + str;
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, name);
			if (!(gameObject != null))
			{
				continue;
			}
			UIAtlas component = gameObject.GetComponent<UIAtlas>();
			if (!(component != null))
			{
				continue;
			}
			UIAtlas[] array2 = array;
			foreach (UIAtlas uIAtlas in array2)
			{
				if (uIAtlas.name == m_stageLangAtlasName[i])
				{
					uIAtlas.replacement = component;
				}
			}
		}
		ReplaceEventAtlas(array);
		GameObject gameObject2 = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, "ui_result_Atlas");
		if (gameObject2 != null)
		{
			UIAtlas component2 = gameObject2.GetComponent<UIAtlas>();
			UIAtlas[] array3 = array;
			foreach (UIAtlas uIAtlas2 in array3)
			{
				if (uIAtlas2 != null && uIAtlas2.name == "ui_result_reference_Atlas")
				{
					uIAtlas2.replacement = component2;
				}
			}
		}
		ReplaceItemAtlas(array);
	}

	public void ReplaceAtlasForMenuLoading(UIAtlas[] atlasArray)
	{
		if (atlasArray != null)
		{
			ReplaceEventAtlas(atlasArray);
		}
	}

	public void ReplaceAtlasForLoading(UIAtlas referenceLoadingAtlas)
	{
		if (!(referenceLoadingAtlas != null))
		{
			return;
		}
		string str = "_" + TextUtility.GetSuffixe();
		string name = "ui_load_word_Atlas" + str;
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, name);
		if (gameObject != null)
		{
			UIAtlas component = gameObject.GetComponent<UIAtlas>();
			if (component != null)
			{
				referenceLoadingAtlas.replacement = component;
			}
		}
	}

	private void ReplaceEventAtlas(UIAtlas[] atlasList)
	{
		if (!string.IsNullOrEmpty(m_eventLangDummyAtlasName))
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, m_eventLangAtlasName);
			if (gameObject != null)
			{
				UIAtlas component = gameObject.GetComponent<UIAtlas>();
				foreach (UIAtlas uIAtlas in atlasList)
				{
					if (uIAtlas.name == m_eventLangDummyAtlasName || uIAtlas.name == "ui_event_00000_Atlas")
					{
						Transform parent = uIAtlas.gameObject.transform.parent;
						if (!(parent != null) || !(parent.name == "EventResourceAtlas"))
						{
							uIAtlas.replacement = component;
						}
					}
				}
			}
		}
		ReplaceEventCommonAtlas(atlasList);
	}

	private UIAtlas GetEventCommonAtlas()
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, "EventResourceCommon");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "EventResourceAtlas");
			if (gameObject2 != null)
			{
				for (int i = 0; i < gameObject2.transform.childCount; i++)
				{
					UIAtlas component = gameObject2.transform.GetChild(i).GetComponent<UIAtlas>();
					if (component != null)
					{
						return component;
					}
				}
			}
		}
		return null;
	}

	private void ReplaceEventCommonAtlas(UIAtlas[] atlasList)
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, "EventResourceCommon");
		if (gameObject == null)
		{
			gameObject = GameObject.Find("EventResourceCommon");
		}
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "EventResourceAtlas");
		if (!(gameObject2 != null))
		{
			return;
		}
		UIAtlas uIAtlas = null;
		for (int i = 0; i < gameObject2.transform.childCount; i++)
		{
			uIAtlas = gameObject2.transform.GetChild(i).GetComponent<UIAtlas>();
			if (uIAtlas != null)
			{
				break;
			}
		}
		if (!(uIAtlas != null))
		{
			return;
		}
		foreach (UIAtlas uIAtlas2 in atlasList)
		{
			if (uIAtlas2.name == "ui_event_reference_Atlas")
			{
				uIAtlas2.replacement = uIAtlas;
			}
		}
	}

	public void ReplacePlayerAtlasForRaidResult(UIAtlas referenceAtlas)
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, "ui_result_Atlas");
		if (gameObject != null)
		{
			UIAtlas component = gameObject.GetComponent<UIAtlas>();
			if (referenceAtlas != null && referenceAtlas.name == "ui_cmn_player_Atlas" && referenceAtlas != component)
			{
				referenceAtlas.replacement = component;
			}
		}
	}

	private void ReplaceItemAtlas(UIAtlas[] atlasList)
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, "ui_cmn_item_Atlas");
		if (!(gameObject != null))
		{
			return;
		}
		UIAtlas component = gameObject.GetComponent<UIAtlas>();
		foreach (UIAtlas uIAtlas in atlasList)
		{
			if (uIAtlas != null && uIAtlas.name == "ui_cmn_item_reference_Atlas")
			{
				uIAtlas.replacement = component;
			}
		}
	}

	public void ClearAllAtlas()
	{
		List<UIAtlas> list = new List<UIAtlas>();
		foreach (string dontDestryAtlas in m_dontDestryAtlasList)
		{
			if (string.IsNullOrEmpty(dontDestryAtlas))
			{
				continue;
			}
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, dontDestryAtlas);
			if (!(gameObject == null))
			{
				UIAtlas component = gameObject.GetComponent<UIAtlas>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
		UIAtlas eventCommonAtlas = GetEventCommonAtlas();
		if (eventCommonAtlas != null)
		{
			list.Add(eventCommonAtlas);
		}
		UIAtlas[] array = Resources.FindObjectsOfTypeAll<UIAtlas>();
		UIAtlas[] array2 = array;
		foreach (UIAtlas uIAtlas in array2)
		{
			if (uIAtlas == null)
			{
				continue;
			}
			bool flag = true;
			foreach (UIAtlas item in list)
			{
				if (list != null)
				{
					if (uIAtlas.name == item.name)
					{
						flag = false;
						break;
					}
					if (uIAtlas.texture != null && uIAtlas.texture.name == item.name)
					{
						flag = false;
						break;
					}
					if (uIAtlas.spriteMaterial != null && uIAtlas.spriteMaterial.name == item.name)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				Resources.UnloadAsset(uIAtlas.texture);
				Resources.UnloadAsset(uIAtlas.spriteMaterial);
			}
		}
	}

	public bool IsLoadAtlas()
	{
		return m_loadedAtlas;
	}

	protected void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			if (m_itemAtlas != null)
			{
				m_itemAtlas = null;
			}
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
		if (m_sceneLoader != null && m_sceneLoader.Loaded)
		{
			m_loadedAtlas = true;
			Object.Destroy(m_sceneLoader.gameObject);
			m_sceneLoader = null;
			base.enabled = false;
		}
	}

	private void AddLoadEventLangAtlas()
	{
		if (!(EventManager.Instance != null))
		{
			return;
		}
		m_eventLangDummyAtlasName = string.Empty;
		if (EventManager.Instance.IsInEvent())
		{
			switch (EventManager.GetType(EventManager.Instance.Id))
			{
			case EventManager.EventType.SPECIAL_STAGE:
				m_eventLangDummyAtlasName = "ui_event_10000_Atlas";
				break;
			case EventManager.EventType.RAID_BOSS:
				m_eventLangDummyAtlasName = "ui_event_20000_Atlas";
				break;
			case EventManager.EventType.COLLECT_OBJECT:
				m_eventLangDummyAtlasName = "ui_event_30000_Atlas";
				break;
			case EventManager.EventType.GACHA:
				m_eventLangDummyAtlasName = "ui_event_40000_Atlas";
				break;
			case EventManager.EventType.ADVERT:
				m_eventLangDummyAtlasName = "ui_event_50000_Atlas";
				break;
			case EventManager.EventType.QUICK:
				m_eventLangDummyAtlasName = "ui_event_60000_Atlas";
				break;
			case EventManager.EventType.BGM:
				m_eventLangDummyAtlasName = "ui_event_70000_Atlas";
				break;
			}
			int specificId = EventManager.GetSpecificId();
			if (specificId > 0)
			{
				m_eventLangAtlasName = "ui_event_" + specificId + "_Atlas_" + TextUtility.GetSuffixe();
				ResourceSceneLoader.ResourceInfo resInfo = CreateResourceSceneLoader(m_eventLangAtlasName, true);
				AddSceneLoaderAndResourceManager(resInfo);
			}
		}
	}

	private void AddLoadAtlasForMenu()
	{
		for (int i = 0; i < m_menuLangAtlasName.Length; i++)
		{
			string sceneName = m_menuLangAtlasName[i] + "_" + TextUtility.GetSuffixe();
			ResourceSceneLoader.ResourceInfo resInfo = CreateResourceSceneLoader(sceneName);
			AddSceneLoaderAndResourceManager(resInfo);
		}
		string sceneName2 = "ui_load_word_Atlas_" + TextUtility.GetSuffixe();
		ResourceSceneLoader.ResourceInfo resInfo2 = CreateResourceSceneLoader(sceneName2, true);
		AddSceneLoaderAndResourceManager(resInfo2);
		AddLoadEventLangAtlas();
		ResourceSceneLoader.ResourceInfo resInfo3 = CreateResourceSceneLoader("ui_cmn_player_bundle_Atlas");
		AddSceneLoaderAndResourceManager(resInfo3);
		ResourceSceneLoader.ResourceInfo resInfo4 = CreateResourceSceneLoader("ui_cmn_item_Atlas", true);
		AddSceneLoaderAndResourceManager(resInfo4);
	}

	private void AddLoadAtlasForDividedMenu()
	{
		for (int i = 0; i < m_dividedMenuLangAtlasName.Length; i++)
		{
			string sceneName = m_dividedMenuLangAtlasName[i] + "_" + TextUtility.GetSuffixe();
			ResourceSceneLoader.ResourceInfo resInfo = CreateResourceSceneLoader(sceneName);
			AddSceneLoaderAndResourceManager(resInfo);
		}
	}

	private void AddLoadAtlasForStage()
	{
		for (int i = 0; i < m_stageLangAtlasName.Length; i++)
		{
			string sceneName = m_stageLangAtlasName[i] + "_" + TextUtility.GetSuffixe();
			ResourceSceneLoader.ResourceInfo resInfo = CreateResourceSceneLoader(sceneName);
			AddSceneLoaderAndResourceManager(resInfo);
		}
		AddLoadEventLangAtlas();
		ResourceSceneLoader.ResourceInfo resInfo2 = CreateResourceSceneLoader("ui_result_Atlas");
		AddSceneLoaderAndResourceManager(resInfo2);
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, "ui_cmn_item_Atlas");
		if (gameObject == null)
		{
			ResourceSceneLoader.ResourceInfo resInfo3 = CreateResourceSceneLoader("ui_cmn_item_Atlas", true);
			AddSceneLoaderAndResourceManager(resInfo3);
		}
	}

	private void AddLoadAtlasForTitle()
	{
		AddLoadEventLangAtlas();
	}

	private void AddSceneLoaderAndResourceManager(ResourceSceneLoader.ResourceInfo resInfo)
	{
		if (!(m_sceneLoader == null) && m_sceneLoader.AddLoadAndResourceManager(resInfo) && resInfo.m_dontDestroyOnChangeScene)
		{
			m_dontDestryAtlasList.Add(resInfo.m_scenename);
		}
	}

	private ResourceSceneLoader.ResourceInfo CreateResourceSceneLoader(string sceneName, bool dontDestroy = false)
	{
		return new ResourceSceneLoader.ResourceInfo(ResourceCategory.UI, sceneName, true, false, dontDestroy);
	}
}
