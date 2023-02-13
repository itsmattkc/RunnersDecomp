using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEventResourceLoader : MonoBehaviour
{
	public delegate void CallbackIfNotLoaded();

	private ResourceSceneLoader m_sceneLoader;

	private bool m_isLoaded;

	private Dictionary<ButtonInfoTable.PageType, List<string>> m_resourceMap = new Dictionary<ButtonInfoTable.PageType, List<string>>
	{
		{
			ButtonInfoTable.PageType.PRESENT_BOX,
			new List<string>
			{
				"item_get_Window",
				"PlayerSetWindowUI",
				"ChaoSetWindowUI",
				"PresentBoxUI",
				"ChaoWindows"
			}
		},
		{
			ButtonInfoTable.PageType.ROULETTE,
			new List<string>
			{
				"item_get_Window",
				"PlayerSetWindowUI",
				"ChaoSetWindowUI",
				"RouletteTopUI",
				"ChaoWindows",
				"NewsWindow"
			}
		},
		{
			ButtonInfoTable.PageType.DAILY_BATTLE,
			new List<string>
			{
				"PlayerSetWindowUI",
				"ChaoSetWindowUI",
				"RankingWindowUI",
				"DailyBattleDetailWindow",
				"DailyInfoUI"
			}
		},
		{
			ButtonInfoTable.PageType.CHAO,
			new List<string>
			{
				"PlayerSetWindowUI",
				"ChaoSetWindowUI",
				"DeckViewWindow",
				"ChaoSetUIPage"
			}
		},
		{
			ButtonInfoTable.PageType.OPTION,
			new List<string>
			{
				"OptionUI",
				"window_name_setting",
				"OptionWindows"
			}
		},
		{
			ButtonInfoTable.PageType.SHOP_RSR,
			new List<string>
			{
				"item_get_Window",
				"ShopPage"
			}
		},
		{
			ButtonInfoTable.PageType.SHOP_RING,
			new List<string>
			{
				"item_get_Window",
				"ShopPage"
			}
		},
		{
			ButtonInfoTable.PageType.SHOP_ENERGY,
			new List<string>
			{
				"item_get_Window",
				"ShopPage"
			}
		},
		{
			ButtonInfoTable.PageType.INFOMATION,
			new List<string>
			{
				"NewsWindow",
				"WorldRankingWindowUI",
				"LeagueResultWindowUI",
				"InformationUI"
			}
		},
		{
			ButtonInfoTable.PageType.EPISODE_RANKING,
			new List<string>
			{
				"PlayerSetWindowUI",
				"ChaoSetWindowUI",
				"RankingFriendOptionWindow",
				"RankingResultBitWindow",
				"RankingWindowUI",
				"ui_mm_ranking_page"
			}
		},
		{
			ButtonInfoTable.PageType.QUICK_RANKING,
			new List<string>
			{
				"PlayerSetWindowUI",
				"ChaoSetWindowUI",
				"RankingFriendOptionWindow",
				"RankingResultBitWindow",
				"RankingWindowUI",
				"ui_mm_ranking_page"
			}
		},
		{
			ButtonInfoTable.PageType.QUICK,
			new List<string>
			{
				"PlayerSetWindowUI",
				"ChaoSetWindowUI",
				"DeckViewWindow",
				"ItemSet_3_UI"
			}
		},
		{
			ButtonInfoTable.PageType.EPISODE,
			new List<string>
			{
				"item_get_Window",
				"Mileage_rankup",
				"ui_mm_mileage2_page"
			}
		},
		{
			ButtonInfoTable.PageType.EPISODE_PLAY,
			new List<string>
			{
				"PlayerSetWindowUI",
				"ChaoSetWindowUI",
				"DeckViewWindow",
				"ItemSet_3_UI"
			}
		},
		{
			ButtonInfoTable.PageType.PLAY_AT_EPISODE_PAGE,
			new List<string>
			{
				"PlayerSetWindowUI",
				"ChaoSetWindowUI",
				"DeckViewWindow",
				"ItemSet_3_UI"
			}
		},
		{
			ButtonInfoTable.PageType.PLAYER_MAIN,
			new List<string>
			{
				"PlayerSetWindowUI",
				"ChaoSetWindowUI",
				"DeckViewWindow",
				"PlayerSet_3_UI"
			}
		},
		{
			ButtonInfoTable.PageType.DAILY_CHALLENGE,
			new List<string>
			{
				"DailyWindowUI"
			}
		}
	};

	public bool IsLoaded
	{
		get
		{
			return m_isLoaded;
		}
		private set
		{
		}
	}

	public Dictionary<ButtonInfoTable.PageType, List<string>> ResourceMap
	{
		get
		{
			return m_resourceMap;
		}
		private set
		{
		}
	}

	public IEnumerator LoadPageResourceIfNotLoadedSync(ButtonInfoTable.PageType pageType, CallbackIfNotLoaded callbackIfNotLoaded)
	{
		yield return StartCoroutine(LoadPageResourceIfNotLoadedCoroutine(pageType, callbackIfNotLoaded));
	}

	public void LoadResourceIfNotLoadedAsync(ButtonInfoTable.PageType pageType, CallbackIfNotLoaded callbackIfNotLoaded)
	{
		StartCoroutine(LoadPageResourceIfNotLoadedCoroutine(pageType, callbackIfNotLoaded));
	}

	public IEnumerator LoadResourceIfNotLoadedSync(string resourceName, CallbackIfNotLoaded callbackIfNotLoaded)
	{
		yield return StartCoroutine(LoadPageResourceIfNotLoadedCoroutine(resourceName, callbackIfNotLoaded));
	}

	public void LoadResourceIfNotLoadedAsync(string resourceName, CallbackIfNotLoaded callbackIfNotLoaded)
	{
		StartCoroutine(LoadPageResourceIfNotLoadedCoroutine(resourceName, callbackIfNotLoaded));
	}

	public IEnumerator LoadAtlasResourceIfNotLoaded()
	{
		AtlasManager atlasManager = AtlasManager.Instance;
		if (atlasManager != null)
		{
			atlasManager.StartLoadAtlasForDividedMenu();
			do
			{
				yield return null;
			}
			while (!atlasManager.IsLoadAtlas());
		}
	}

	private IEnumerator LoadPageResourceIfNotLoadedCoroutine(ButtonInfoTable.PageType pageType, CallbackIfNotLoaded callbackIfNotLoaded)
	{
		m_isLoaded = false;
		if (pageType == ButtonInfoTable.PageType.NON)
		{
			m_isLoaded = true;
			yield break;
		}
		GameObject mainMenuPagesObject = GameObject.Find("UI Root (2D)/Camera/menu_Anim");
		if (mainMenuPagesObject == null)
		{
			m_isLoaded = true;
			yield break;
		}
		bool isNeedCallback = false;
		if (!m_resourceMap.ContainsKey(pageType))
		{
			yield break;
		}
		List<string> resourceNameList;
		m_resourceMap.TryGetValue(pageType, out resourceNameList);
		foreach (string resourceName in resourceNameList)
		{
			if (!IsExistResource(resourceName, mainMenuPagesObject))
			{
				yield return StartCoroutine(LoadResourceRequest(resourceName, mainMenuPagesObject));
				isNeedCallback = true;
			}
		}
		m_isLoaded = true;
		if (isNeedCallback && callbackIfNotLoaded != null)
		{
			callbackIfNotLoaded();
		}
	}

	private IEnumerator LoadPageResourceIfNotLoadedCoroutine(string resourceName, CallbackIfNotLoaded callbackIfNotLoaded)
	{
		m_isLoaded = false;
		GameObject mainMenuPagesObject = GameObject.Find("UI Root (2D)/Camera/menu_Anim");
		if (mainMenuPagesObject == null)
		{
			m_isLoaded = true;
			yield break;
		}
		bool isNeedCallback2 = false;
		if (IsExistResource(resourceName, mainMenuPagesObject))
		{
			m_isLoaded = true;
			yield break;
		}
		yield return StartCoroutine(LoadResourceRequest(resourceName, mainMenuPagesObject));
		isNeedCallback2 = true;
		m_isLoaded = true;
		if (isNeedCallback2 && callbackIfNotLoaded != null)
		{
			callbackIfNotLoaded();
		}
	}

	private bool IsExistResource(string resourceName, GameObject parentObject)
	{
		int childCount = parentObject.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = parentObject.transform.GetChild(i).gameObject;
			if (!(gameObject == null) && gameObject.name == resourceName)
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerator LoadResourceRequest(string resourceName, GameObject attachObject)
	{
		HudNetworkConnect hudConnect = NetMonitor.Instance.GetComponent<HudNetworkConnect>();
		if (hudConnect != null)
		{
			hudConnect.Setup();
			hudConnect.PlayStart(HudNetworkConnect.DisplayType.LOADING);
		}
		GameObject sceneLoaderObject = GameObject.Find("ButtonEventSceneLoader");
		if (sceneLoaderObject == null)
		{
			sceneLoaderObject = new GameObject("ButtonEventSceneLoader");
		}
		m_sceneLoader = sceneLoaderObject.AddComponent<ResourceSceneLoader>();
		m_sceneLoader.AddLoadAndResourceManager(resourceName, true, ResourceCategory.UI, false, false, null);
		do
		{
			yield return null;
		}
		while (!m_sceneLoader.Loaded);
		Object.Destroy(m_sceneLoader);
		m_sceneLoader = null;
		yield return null;
		GameObject resourceObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, resourceName);
		if (resourceObject != null)
		{
			ResourceFolderMarker marker = resourceObject.GetComponent<ResourceFolderMarker>();
			if (marker == null)
			{
				resourceObject.SetActive(false);
				foreach (Transform child in resourceObject.transform)
				{
					child.gameObject.SetActive(true);
				}
			}
			Vector3 localPosition = resourceObject.transform.localPosition;
			Vector3 localScale = resourceObject.transform.localScale;
			resourceObject.transform.parent = attachObject.transform;
			resourceObject.transform.localPosition = localPosition;
			resourceObject.transform.localScale = localScale;
		}
		if (hudConnect != null)
		{
			hudConnect.PlayEnd();
		}
		yield return null;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
