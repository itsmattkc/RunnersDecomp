using DataTable;
using Message;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class TitleDataLoader : MonoBehaviour
{
	private enum EventSignal
	{
		SuccessStreamingDataLoad = 100,
		NUM
	}

	private TinyFsmBehavior m_fsm_behavior;

	private ResourceSceneLoader m_sceneLoader;

	private static readonly float LoadWaitTime = 60f;

	private static readonly int CountToAskGiveUp = 3;

	private bool m_Retry;

	private bool m_loadEnd;

	private List<ResourceSceneLoader.ResourceInfo> m_defaultLoadInfoFirst = new List<ResourceSceneLoader.ResourceInfo>
	{
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.COMMON_EFFECT, "ResourcesCommonEffect", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.PLAYER_COMMON, "CharacterCommonResource", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.OBJECT_RESOURCE, "CommonObjectResource", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.OBJECT_PREFAB, "CommonObjectPrefabs", false, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ENEMY_RESOURCE, "CommonEnemyResource", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ENEMY_PREFAB, "CommonEnemyPrefabs", false, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, "ChaoDataTable", true, false, true, "ChaoTable"),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, "CharaAbilityDataTable", true, false, true, "ImportAbilityTable"),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, "CharacterDataNameInfo", true, false, true)
	};

	private List<ResourceSceneLoader.ResourceInfo> m_defaultLoadInfo = new List<ResourceSceneLoader.ResourceInfo>
	{
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.COMMON_EFFECT, "ResourcesCommonEffect", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.PLAYER_COMMON, "CharacterCommonResource", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.OBJECT_RESOURCE, "CommonObjectResource", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.OBJECT_PREFAB, "CommonObjectPrefabs", false, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ENEMY_RESOURCE, "CommonEnemyResource", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ENEMY_PREFAB, "CommonEnemyPrefabs", false, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, "AchievementTable", true, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, "MissionTable", true, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, "ChaoDataTable", true, false, true, "ChaoTable"),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, "CharaAbilityDataTable", true, false, true, "ImportAbilityTable"),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, "CharacterDataNameInfo", true, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, "OverlapBonusTable", true, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.QUICK_MODE, "StageTimeTable", true, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "ChaoTextures", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "MainMenuPages", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "RouletteTopUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "ChaoWindows", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "ShopPage", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "ChaoSetUIPage", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "OptionWindows", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "ChaoSetWindowUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "DailyInfoUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "DailyWindowUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "DailybattleRewardWindowUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "InformationUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "ItemSet_3_UI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "LoginWindowUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "NewsWindow", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "OptionUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "PlayerSetWindowUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "PlayerSet_3_UI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "PresentBoxUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "StartDashWindowUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "WorldRankingWindowUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "ui_mm_mileage2_page", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "ui_mm_ranking_page", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "ui_tex_mm_ep_001", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "ui_tex_mm_ep_002", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "window_name_setting", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "DailyBattleDetailWindow", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "DeckViewWindow", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "LeagueResultWindowUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "Mileage_rankup", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "RankingFriendOptionWindow", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "RankingResultBitWindow", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "RankingWindowUI", true, true, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UNKNOWN, "item_get_Window", true, true, false)
	};

	private List<string> m_streamingSoundData = new List<string>();

	private List<ResourceSceneLoader.ResourceInfo> m_nowLoadingList;

	private List<ResourceSceneLoader.ResourceInfo> m_loadInfo;

	private int m_requestDownloadCount;

	private int m_requestLoadCount;

	private bool m_endCheckExistingCheckDownloadData;

	public bool LoadEnd
	{
		get
		{
			return m_loadEnd;
		}
	}

	public int LoadEndCount
	{
		get
		{
			int num = (!(m_sceneLoader == null)) ? m_sceneLoader.LoadEndCount : 0;
			num += StreamingDataLoader.Instance.NumLoaded;
			if (InformationDataTable.Instance != null && InformationDataTable.Instance.Loaded)
			{
				num++;
			}
			return num;
		}
		private set
		{
		}
	}

	public int RequestedLoadCount
	{
		get
		{
			return m_requestLoadCount;
		}
	}

	public int RequestedDownloadCount
	{
		get
		{
			return m_requestDownloadCount;
		}
	}

	public bool EndCheckExistingDownloadData
	{
		get
		{
			return m_endCheckExistingCheckDownloadData;
		}
	}

	private void Start()
	{
	}

	public void AddStreamingSoundData(string data)
	{
		m_streamingSoundData.Add(data);
	}

	public void Setup(bool is_first)
	{
		Init(is_first);
		m_requestDownloadCount = 0;
		m_requestLoadCount = m_loadInfo.Count + m_streamingSoundData.Count + 1;
		m_endCheckExistingCheckDownloadData = false;
	}

	private void OnDestroy()
	{
		if ((bool)m_fsm_behavior)
		{
			m_fsm_behavior.ShutDown();
			m_fsm_behavior = null;
		}
	}

	private void Update()
	{
	}

	public void StartLoad()
	{
		if (m_fsm_behavior != null)
		{
			if (StreamingDataLoader.Instance.NumInLoadList > 0)
			{
				m_fsm_behavior.ChangeState(new TinyFsmState(StateLoadStreaming));
			}
			else
			{
				m_fsm_behavior.ChangeState(new TinyFsmState(StateLoadScene));
			}
		}
	}

	public void RetryStreamingDataLoad(int retryCount)
	{
		StreamingDataLoadRetryProcess process = new StreamingDataLoadRetryProcess(retryCount, base.gameObject, this);
		NetMonitor.Instance.StartMonitor(process, -1f, HudNetworkConnect.DisplayType.ALL);
		StreamingDataLoader.Instance.StartDownload(retryCount, base.gameObject);
	}

	public void RetryInformationDataLoad()
	{
		InformationDataLoadRetryProcess process = new InformationDataLoadRetryProcess(base.gameObject, this);
		NetMonitor.Instance.StartMonitor(process, 0f, HudNetworkConnect.DisplayType.ALL);
		InformationDataTable.Instance.Initialize(base.gameObject);
	}

	private void Init(bool is_first)
	{
		if (ResourceManager.Instance == null)
		{
			GameObject gameObject = new GameObject("ResourceManager");
			gameObject.AddComponent<ResourceManager>();
		}
		m_loadInfo = new List<ResourceSceneLoader.ResourceInfo>();
		if (is_first)
		{
			foreach (ResourceSceneLoader.ResourceInfo item in m_defaultLoadInfoFirst)
			{
				m_loadInfo.Add(item);
			}
		}
		else
		{
			foreach (ResourceSceneLoader.ResourceInfo item2 in m_defaultLoadInfo)
			{
				m_loadInfo.Add(item2);
			}
		}
		string suffixe = TextUtility.GetSuffixe();
		string name = "text_common_text_" + suffixe;
		string name2 = "text_event_common_text_" + suffixe;
		string name3 = "text_chao_text_" + suffixe;
		m_loadInfo.Add(new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, name, true, false, false));
		m_loadInfo.Add(new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, name3, true, false, false));
		if (!is_first)
		{
			m_loadInfo.Add(new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, name2, true, false, false));
		}
		if (!is_first)
		{
			AddSceneLoaderChaoTexture();
		}
		m_fsm_behavior = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (m_fsm_behavior != null)
		{
			TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
			description.initState = new TinyFsmState(StateAddDownloadFile);
			m_fsm_behavior.SetUp(description);
		}
		GameObject gameObject2 = new GameObject("ResourceSceneLoader");
		m_sceneLoader = gameObject2.AddComponent<ResourceSceneLoader>();
		if (m_sceneLoader != null)
		{
			m_sceneLoader.Pause(true);
		}
	}

	private void AddSceneLoaderChaoTexture()
	{
		GameObject gameObject = GameObject.Find("AssetBundleLoader");
		if (!(gameObject != null))
		{
			return;
		}
		AssetBundleLoader component = gameObject.GetComponent<AssetBundleLoader>();
		if (component != null)
		{
			string[] chaoTextureList = component.GetChaoTextureList();
			string[] array = chaoTextureList;
			foreach (string name in array)
			{
				m_loadInfo.Add(new ResourceSceneLoader.ResourceInfo(ResourceCategory.ETC, name, true, true, false));
			}
		}
	}

	private TinyFsmState StateAddDownloadFile(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			string[] array = new string[5]
			{
				"CharacterEffectSonic",
				"CharacterModelSonic",
				"w01_StageResource",
				"w01_TerrainData",
				"TenseEffectTable"
			};
			string[] array2 = array;
			foreach (string scenename in array2)
			{
				ResourceSceneLoader.ResourceInfo resourceInfo = new ResourceSceneLoader.ResourceInfo();
				resourceInfo.m_category = ResourceCategory.UNKNOWN;
				resourceInfo.m_scenename = scenename;
				resourceInfo.m_onlyDownload = true;
				resourceInfo.m_isAssetBundle = true;
				m_loadInfo.Add(resourceInfo);
			}
			m_fsm_behavior.ChangeState(new TinyFsmState(StateCheckDownload));
			return TinyFsmState.End();
		}
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateCheckDownload(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			m_requestLoadCount = 0;
			int num = 0;
			if (AssetBundleLoader.Instance != null)
			{
				foreach (ResourceSceneLoader.ResourceInfo item in m_loadInfo)
				{
					if (item.m_isAssetBundle && !AssetBundleLoader.Instance.IsDownloaded(item.m_scenename))
					{
						m_requestDownloadCount++;
					}
				}
			}
			m_nowLoadingList = new List<ResourceSceneLoader.ResourceInfo>();
			foreach (ResourceSceneLoader.ResourceInfo item2 in m_loadInfo)
			{
				if (m_sceneLoader.AddLoadAndResourceManager(item2))
				{
					m_nowLoadingList.Add(item2);
				}
			}
			if (StreamingDataLoader.Instance != null)
			{
				foreach (string streamingSoundDatum in m_streamingSoundData)
				{
					string url = SoundManager.GetDownloadURL() + streamingSoundDatum;
					string path = SoundManager.GetDownloadedDataPath() + streamingSoundDatum;
					StreamingDataLoader.Instance.AddFileIfNotDownloaded(url, path);
				}
				m_requestDownloadCount += StreamingDataLoader.Instance.NumInLoadList;
				num = StreamingDataLoader.Instance.NumInLoadList;
			}
			m_requestLoadCount = m_nowLoadingList.Count + num + 1;
			m_endCheckExistingCheckDownloadData = true;
			m_fsm_behavior.ChangeState(new TinyFsmState(StateIdle));
			return TinyFsmState.End();
		}
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateIdle(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoadStreaming(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			StreamingDataLoadRetryProcess process = new StreamingDataLoadRetryProcess(0, base.gameObject, this);
			NetMonitor.Instance.StartMonitor(process, -1f, HudNetworkConnect.DisplayType.ALL);
			StreamingDataLoader.Instance.StartDownload(0, base.gameObject);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		case 100:
			m_fsm_behavior.ChangeState(new TinyFsmState(StateLoadScene));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoadScene(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_sceneLoader.Pause(false);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_sceneLoader.Loaded)
			{
				m_nowLoadingList = null;
				m_fsm_behavior.ChangeState(new TinyFsmState(StateLoadInfoData));
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoadInfoData(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (InformationDataTable.Instance == null)
			{
				InformationDataLoadRetryProcess process = new InformationDataLoadRetryProcess(base.gameObject, this);
				NetMonitor.Instance.StartMonitor(process, 0f, HudNetworkConnect.DisplayType.ALL);
				InformationDataTable.Create();
				InformationDataTable.Instance.Initialize(base.gameObject);
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			m_loadEnd = true;
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void StreamingDataLoad_Succeed()
	{
		if (m_fsm_behavior != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
			m_fsm_behavior.Dispatch(signal);
		}
	}

	private void StreamingDataLoad_Failed()
	{
		NetMonitor.Instance.EndMonitorForward(new MsgAssetBundleResponseFailedMonitor(), null, null);
		NetMonitor.Instance.EndMonitorBackward();
	}

	private void InformationDataLoad_Succeed()
	{
		NetMonitor.Instance.EndMonitorForward(new MsgAssetBundleResponseSucceed(null, null), null, null);
		NetMonitor.Instance.EndMonitorBackward();
	}

	private void InformationDataLoad_Failed()
	{
		NetMonitor.Instance.EndMonitorForward(new MsgAssetBundleResponseFailedMonitor(), null, null);
		NetMonitor.Instance.EndMonitorBackward();
	}
}
