using App;
using DataTable;
using Message;
using Mission;
using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using Text;
using Tutorial;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/GameMode/Stage")]
public class GameModeStage : MonoBehaviour
{
	private struct PostGameResultInfo
	{
		public bool m_existBoss;

		public StageInfo.MileageMapInfo m_prevMapInfo;
	}

	private enum LoadType
	{
		COMMON_OBJECT_RESOURCE,
		COMMON_OBJECT_PREFABS,
		COMMON_ENEMY_RESOURCE,
		COMMON_ENEMY_PREFABS,
		RESOURCES_COMMON_EFFECT,
		CHARACTER_COMMON_RESOURCE,
		EVENT_RESOURCE_STAGE,
		EVENT_RESOURCE_COMMON,
		NUM
	}

	public enum ProgressBarLeaveState
	{
		IDLE = -1,
		StateInit,
		StateLoad,
		StateLoad2,
		StateRequestStartAct,
		StateSoundConnectIfNotFound,
		StateAccessNetworkForStartAct,
		StateSetupPrepareBlock,
		StateSetupBlock,
		StateSendApolloStageStart,
		NUM
	}

	private enum ChangeLevelSubState
	{
		FADEOUT,
		FADEOUT_STOPCHARACTER,
		SETUP_SPEEDLEVEL,
		WAITPREPARE_STAGE,
		SETUP_STAGE,
		WAIT,
		FADEIN
	}

	private enum TutorialMissionEndSubState
	{
		SHOWRESULT,
		FADEOUT,
		WAIT,
		FADEIN,
		END
	}

	private const float LightModeFixedTimeStep = 0.033333f;

	private const float LightModeMaxFixedTimeStep = 0.333333f;

	private const string pre_characterModelResourceName = "CharacterModel";

	private const string pre_characterEffectName = "CharacterEffect";

	private const string PathManagerName = "StagePathManager";

	private bool m_isLoadResources = true;

	public bool m_isCreatespawnableManager;

	public bool m_isCreateTerrainPlacementManager;

	public bool m_notPlaceTerrain;

	public bool m_showBlockInfo;

	public bool m_randomBlock;

	public bool m_useTemporarySet;

	public int m_blockNumOfNotPlaceTerrain = -1;

	public bool m_bossStage;

	public int m_debugBossLevel;

	public BossType m_bossType = BossType.MAP1;

	public bool m_useCharaInInspector;

	public bool m_noStartHud;

	private bool m_exitFromResult;

	private bool m_bossClear;

	private bool m_bossTimeUp;

	private bool m_quickModeTimeUp;

	[SerializeField]
	private bool m_tutorialStage;

	[SerializeField]
	private bool m_eventStage;

	private bool m_showMapBossTutorial;

	private bool m_showFeverBossTutorial;

	private bool m_showEventBossTutorial;

	private int m_showItemTutorial = -1;

	private int m_showCharaTutorial = -1;

	private int m_showActionTutorial = -1;

	private int m_showQuickTurorial = -1;

	private bool m_fromTitle;

	private bool m_serverActEnd;

	private bool m_bossNoMissChance;

	private bool m_saveFlag;

	private bool m_firstTutorial;

	private bool m_equipItemTutorial;

	private bool m_missonCompleted;

	private int m_oldNumBossAttack;

	private bool m_retired;

	private TinyFsmBehavior m_fsm;

	[SerializeField]
	private string m_stageName = "w01";

	[SerializeField]
	private TenseType m_stageTenseType = TenseType.NONE;

	[SerializeField]
	private CharaType m_mainChara;

	[SerializeField]
	private CharaType m_subChara = CharaType.UNKNOWN;

	private int m_substate;

	private List<ItemType> m_useEquippedItem = new List<ItemType>();

	private List<BoostItemType> m_useBoostItem = new List<BoostItemType>();

	private GameObject m_sceneLoader;

	private GameObject m_stageBlockManager;

	private PathManager m_stagePathManager;

	private List<GameObject> m_pausedObject;

	private PlayerInformation m_playerInformation;

	private LevelInformation m_levelInformation;

	private HudCaution m_hudCaution;

	private CharacterContainer m_characterContainer;

	private StageMissionManager m_missionManager;

	private StageTutorialManager m_tutorialManager;

	private FriendSignManager m_friendSignManager;

	private EventManager m_eventManager;

	private string m_terrainDataName;

	private string m_stageResourceName;

	private string m_stageResourceObjectName;

	private bool m_onSpeedUp;

	private bool m_onDestroyRingMode;

	[SerializeField]
	private int m_numEnableContinue = 2;

	private int m_invalidExtremeCount;

	private bool m_invalidExtremeFlag;

	private float m_timer;

	private int m_counter;

	private bool m_reqPause;

	private bool m_reqPauseBackMain;

	private bool m_reqTutorialPause;

	private bool m_IsNowLastChanceHudCautionBoss;

	private bool m_receiveInvincibleMsg;

	[SerializeField]
	private float m_defaultTimeScale = 1f;

	private float m_chaoEasyTimeScale = 1f;

	private float m_gameResultTimer;

	private GameResult m_gameResult;

	private MsgTutorialPlayEnd m_tutorialEndMsg;

	private Tutorial.EventID m_tutorialMissionID;

	private HudTutorial.Kind m_tutorialKind;

	private string m_mainBgmName = "bgm_z_w01";

	private RareEnemyTable m_rareEnemyTable;

	private EnemyExtendItemTable m_enemyExtendItemTable;

	private BossTable m_bossTable;

	private BossMap3Table m_bossMap3Table;

	private ObjectPartTable m_objectPartTable;

	private float m_savedFixedTimeStep;

	private float m_savedMaxFixedTimeStep;

	private PostGameResultInfo m_postGameResults = default(PostGameResultInfo);

	private ServerMileageMapState m_resultMapState;

	private List<ServerMileageIncentive> m_mileageIncentive;

	private List<ServerItemState> m_dailyIncentive;

	private SendApollo m_sendApollo;

	private StageEffect m_stageEffect;

	private List<ResourceSceneLoader.ResourceInfo> m_loadInfo = new List<ResourceSceneLoader.ResourceInfo>
	{
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.OBJECT_RESOURCE, "CommonObjectResource", true, false, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.OBJECT_PREFAB, "CommonObjectPrefabs", false, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ENEMY_RESOURCE, "CommonEnemyResource", true, false, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.ENEMY_PREFAB, "CommonEnemyPrefabs", false, false, true),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.COMMON_EFFECT, "ResourcesCommonEffect", true, false, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.PLAYER_COMMON, "CharacterCommonResource", true, false, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.EVENT_RESOURCE, "EventResourceStage", true, false, false),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.EVENT_RESOURCE, "EventResourceCommon", true, false, false)
	};

	private List<ResourceSceneLoader.ResourceInfo> m_quickModeLoadInfo = new List<ResourceSceneLoader.ResourceInfo>
	{
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.QUICK_MODE, "StageTimeTable", true, false, true)
	};

	private GameObject m_uiRootObj;

	private GameObject m_continueWindowObj;

	private ServerEventRaidBossBonus m_raidBossBonus;

	private HudProgressBar m_progressBar;

	private GameObject m_connectAlertUI2;

	private bool m_quickMode;

	private readonly Vector3 PlayerResetPosition = Vector3.zero;

	private readonly Quaternion PlayerResetRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));

	public int numEnableContinue
	{
		get
		{
			return m_numEnableContinue;
		}
	}

	public string GetStageName()
	{
		return m_stageName;
	}

	public static int ContinueRestCount()
	{
		GameObject gameObject = GameObject.Find("GameModeStage");
		if (gameObject != null)
		{
			GameModeStage component = gameObject.GetComponent<GameModeStage>();
			if (component != null)
			{
				return component.numEnableContinue;
			}
		}
		return 0;
	}

	private void Awake()
	{
		Application.targetFrameRate = SystemSettings.TargetFrameRate;
	}

	private void Start()
	{
		HudUtility.SetInvalidNGUIMitiTouch();
		base.gameObject.tag = "GameModeStage";
		m_pausedObject = new List<GameObject>();
		m_rareEnemyTable = new RareEnemyTable();
		m_enemyExtendItemTable = new EnemyExtendItemTable();
		m_bossTable = new BossTable();
		m_bossMap3Table = new BossMap3Table();
		m_objectPartTable = new ObjectPartTable();
		m_savedFixedTimeStep = Time.fixedDeltaTime;
		m_savedMaxFixedTimeStep = Time.maximumDeltaTime;
		m_useEquippedItem = new List<ItemType>();
		if (SystemSaveManager.GetSystemSaveData() != null && SystemSaveManager.GetSystemSaveData().lightMode)
		{
			Time.fixedDeltaTime = 0.033333f;
			Time.maximumDeltaTime = 0.333333f;
		}
		if (FontManager.Instance != null && FontManager.Instance.IsNecessaryLoadFont())
		{
			FontManager.Instance.LoadResourceData();
		}
		BackKeyManager.AddEventCallBack(base.gameObject);
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (m_fsm != null)
		{
			TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
			description.initState = new TinyFsmState(StateInit);
			description.onFixedUpdate = false;
			m_fsm.SetUp(description);
		}
		SoundManager.AddStageCommonCueSheet();
		if (ServerInterface.SettingState != null)
		{
			m_numEnableContinue = ServerInterface.SettingState.m_onePlayContinueCount;
		}
		m_uiRootObj = GameObject.Find("UI Root (2D)");
		if (!(m_uiRootObj != null))
		{
			return;
		}
		m_connectAlertUI2 = GameObjectUtil.FindChildGameObject(m_uiRootObj, "ConnectAlert_2_UI");
		if (m_connectAlertUI2 != null)
		{
			m_connectAlertUI2.SetActive(true);
			m_progressBar = GameObjectUtil.FindChildGameObjectComponent<HudProgressBar>(m_connectAlertUI2, "Pgb_loading");
			if (m_progressBar != null)
			{
				m_progressBar.SetUp(9);
			}
		}
	}

	private void LateUpdate()
	{
		if (m_reqPause)
		{
			ChangeState(new TinyFsmState(StatePause));
		}
		else if (m_quickMode && !m_quickModeTimeUp && StageTimeManager.Instance != null && StageTimeManager.Instance.IsTimeUp())
		{
			OnQuickModeTimeUp(new MsgQuickModeTimeUp());
		}
		if (m_levelInformation != null)
		{
			m_levelInformation.RequestCharaChange = false;
			m_levelInformation.RequestEqitpItem = false;
		}
	}

	private IEnumerator NotSendPostGameResult()
	{
		yield return null;
		DispatchMessage(new MsgPostGameResultsSucceed());
	}

	private IEnumerator NotSendEventUpdateGameResult()
	{
		yield return null;
		DispatchMessage(new MsgEventUpdateGameResultsSucceed());
	}

	private IEnumerator NotSendEventPostGameResult()
	{
		yield return null;
		DispatchMessage(new MsgEventPostGameResultsSucceed());
	}

	private void OnDestroy()
	{
		if ((bool)m_fsm)
		{
			m_fsm.ShutDown();
			m_fsm = null;
		}
		RemoveAllResource();
		Time.fixedDeltaTime = m_savedFixedTimeStep;
		Time.maximumDeltaTime = m_savedMaxFixedTimeStep;
		StopStageEffect();
	}

	private void OnMsgNotifyDead(MsgNotifyDead message)
	{
		DispatchMessage(message);
	}

	private void OnMsgNotifyStartPause(MsgNotifyStartPause message)
	{
		DispatchMessage(message);
	}

	private void OnMsgNotifyEndPause(MsgNotifyEndPause message)
	{
		DispatchMessage(message);
	}

	private void OnMsgNotifyEndPauseExitStage(MsgNotifyEndPauseExitStage message)
	{
		DispatchMessage(message);
	}

	private void OnSendToGameModeStage(MessageBase message)
	{
		DispatchMessage(message);
	}

	private void OnBossEnd(MsgBossEnd message)
	{
		DispatchMessage(message);
	}

	private void OnBossClear(MsgBossClear message)
	{
		DispatchMessage(message);
	}

	private void OnBossTimeUp()
	{
		m_bossTimeUp = true;
	}

	private void OnQuickModeTimeUp(MsgQuickModeTimeUp message)
	{
		DispatchMessage(message);
	}

	private void OnMsgChangeChara(MsgChangeChara message)
	{
		DispatchMessage(message);
	}

	private void OnTransformPhantom(MsgTransformPhantom message)
	{
		DispatchMessage(message);
		if (message.m_type == PhantomType.LASER)
		{
			ObjUtil.CreatePrism();
		}
	}

	private void OnReturnFromPhantom(MsgReturnFromPhantom message)
	{
		DispatchMessage(message);
	}

	private void OnMsgInvincible(MsgInvincible message)
	{
		DispatchMessage(message);
	}

	private void OnMsgExternalGamePause(MsgExternalGamePause message)
	{
		m_reqPauseBackMain = message.m_backMainMenu;
		DispatchMessage(message);
	}

	private void OnMsgTutorialBackKey(MsgTutorialBackKey message)
	{
		DispatchMessage(message);
	}

	private void OnMsgContinueBackKey(MsgContinueBackKey message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialPlayStart(MsgTutorialPlayStart message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialPlayAction(MsgTutorialPlayAction message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialItemButtonEnd(MsgTutorialEnd message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialPlayEnd(MsgTutorialPlayEnd message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialMapBoss(MsgTutorialMapBoss message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialFeverBoss(MsgTutorialFeverBoss message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialItem(MsgTutorialItem message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialItemButton(MsgTutorialItemButton message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialChara(MsgTutorialChara message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialAction(MsgTutorialAction message)
	{
		DispatchMessage(message);
	}

	private void OnMsgTutorialQuickMode(MsgTutorialQuickMode message)
	{
		DispatchMessage(message);
	}

	private void OnChangeCharaSucceed(MsgChangeCharaSucceed message)
	{
		if (HudTutorial.IsCharaTutorial((CharaType)m_playerInformation.SubCharacterID))
		{
			m_showCharaTutorial = (int)CharaTypeUtil.GetCharacterTutorialID((CharaType)m_playerInformation.SubCharacterID);
			if (m_showCharaTutorial != -1)
			{
				GameObjectUtil.SendDelayedMessageToGameObject(base.gameObject, "OnMsgTutorialChara", new MsgTutorialChara((HudTutorial.Id)m_showCharaTutorial));
			}
		}
	}

	private void OnClickPlatformBackButtonEvent()
	{
		bool backMainMenu = !m_firstTutorial;
		OnMsgExternalGamePause(new MsgExternalGamePause(backMainMenu, true));
		OnMsgTutorialBackKey(new MsgTutorialBackKey());
		OnMsgContinueBackKey(new MsgContinueBackKey());
	}

	private void ServerPostGameResults_Succeeded(MsgPostGameResultsSucceed message)
	{
		NetUtil.SyncSaveDataAndDataBase(message.m_playerState);
		m_resultMapState = message.m_mileageMapState;
		if (message.m_mileageIncentive != null)
		{
			m_mileageIncentive = new List<ServerMileageIncentive>(message.m_mileageIncentive.Count);
			foreach (ServerMileageIncentive item in message.m_mileageIncentive)
			{
				m_mileageIncentive.Add(item);
			}
		}
		if (message.m_dailyIncentive != null)
		{
			m_dailyIncentive = new List<ServerItemState>(message.m_dailyIncentive.Count);
			foreach (ServerItemState item2 in message.m_dailyIncentive)
			{
				m_dailyIncentive.Add(item2);
			}
		}
		DispatchMessage(message);
	}

	private void ServerPostGameResults_Failed(MsgServerConnctFailed message)
	{
		DispatchMessage(message);
	}

	private void ServerStartAct_Succeeded(MsgActStartSucceed message)
	{
		m_serverActEnd = true;
		NetUtil.SyncSaveDataAndDataBase(message.m_playerState);
		DispatchMessage(message);
	}

	private void ServerStartAct_Failed(MsgServerConnctFailed message)
	{
		DispatchMessage(message);
	}

	private void ServerQuickModeStartAct_Succeeded(MsgQuickModeActStartSucceed message)
	{
		m_serverActEnd = true;
		NetUtil.SyncSaveDataAndDataBase(message.m_playerState);
		DispatchMessage(message);
	}

	private void ServerQuickModePostGameResults_Succeeded(MsgQuickModePostGameResultsSucceed message)
	{
		NetUtil.SyncSaveDataAndDataBase(message.m_playerState);
		if (message.m_dailyIncentive != null)
		{
			m_dailyIncentive = new List<ServerItemState>(message.m_dailyIncentive.Count);
			foreach (ServerItemState item in message.m_dailyIncentive)
			{
				m_dailyIncentive.Add(item);
			}
		}
		DispatchMessage(message);
	}

	private void ServerEventStartAct_Succeeded(MsgEventActStartSucceed message)
	{
		m_serverActEnd = true;
		NetUtil.SyncSaveDataAndDataBase(message.m_playerState);
		DispatchMessage(message);
	}

	private void ServerEventStartAct_Failed(MsgServerConnctFailed message)
	{
		DispatchMessage(message);
	}

	private void ServerUpdateGameResults_Succeeded(MsgEventUpdateGameResultsSucceed message)
	{
		if (message.m_bonus != null)
		{
			m_raidBossBonus = new ServerEventRaidBossBonus();
			message.m_bonus.CopyTo(m_raidBossBonus);
		}
		DispatchMessage(message);
	}

	private void ServerEventPostGameResults_Succeeded(MsgEventPostGameResultsSucceed message)
	{
		DispatchMessage(message);
	}

	private void ServerDrawRaidBoss_Succeeded(MsgDrawRaidBossSucceed message)
	{
		DispatchMessage(message);
	}

	private void ServerGetEventUserRaidBossState_Succeeded(MsgGetEventUserRaidBossStateSucceed message)
	{
		DispatchMessage(message);
	}

	private void DailyBattleResultCallBack()
	{
		MessageBase message = new MessageBase(61515);
		DispatchMessage(message);
	}

	private void DispatchMessage(MessageBase message)
	{
		if (m_fsm != null && message != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateMessage(message);
			m_fsm.Dispatch(signal);
		}
	}

	private void ChangeState(TinyFsmState nextState)
	{
		m_fsm.ChangeState(nextState);
		m_substate = 0;
	}

	private TinyFsmState StateInit(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			TimeProfiler.EndCountTime("MainMenu-GameModeStage");
			TimeProfiler.StartCountTime("GameModeStage:StateInit");
			GC.Collect();
			Resources.UnloadUnusedAssets();
			GC.Collect();
			CameraFade.StartAlphaFade(Color.black, true, 1f, 0f);
			HudLoading.StartScreen();
			if (m_uiRootObj == null)
			{
				m_uiRootObj = GameObject.Find("UI Root (2D)");
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				if (gameObject.activeInHierarchy)
				{
					m_pausedObject.Add(gameObject);
				}
				gameObject.SetActive(false);
			}
			GameObject[] array3 = GameObject.FindGameObjectsWithTag("Chao");
			GameObject[] array4 = array3;
			foreach (GameObject gameObject2 in array4)
			{
				m_pausedObject.Add(gameObject2);
				gameObject2.SetActive(false);
			}
			if (AtlasManager.Instance == null)
			{
				GameObject gameObject3 = new GameObject("AtlasManager");
				gameObject3.AddComponent<AtlasManager>();
			}
			if (Env.useAssetBundle)
			{
				if (AssetBundleLoader.Instance == null)
				{
					AssetBundleLoader.Create();
				}
				if (!AssetBundleLoader.Instance.IsEnableDownlad())
				{
					AssetBundleLoader.Instance.Initialize();
				}
			}
			if (GameObjectUtil.FindGameObjectComponent<StageAbilityManager>("StageAbilityManager") == null)
			{
				GameObject gameObject4 = new GameObject("StageAbilityManager");
				gameObject4.AddComponent<StageAbilityManager>();
				gameObject4.tag = "Manager";
			}
			if (InformationDataTable.Instance == null)
			{
				InformationDataTable.Create();
				InformationDataTable.Instance.Initialize(base.gameObject);
			}
			m_levelInformation = GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
			if (m_levelInformation == null)
			{
				GameObject gameObject5 = new GameObject("LevelInformation");
				gameObject5.tag = "StageManager";
				m_levelInformation = gameObject5.AddComponent<LevelInformation>();
			}
			m_eventManager = EventManager.Instance;
			bool flag = false;
			StageInfo stageInfo = GameObjectUtil.FindGameObjectComponent<StageInfo>("StageInfo");
			if (stageInfo != null)
			{
				if (m_levelInformation != null)
				{
					m_levelInformation.Missed = false;
					m_stageName = stageInfo.SelectedStageName;
					m_firstTutorial = stageInfo.FirstTutorial;
					m_tutorialStage = stageInfo.TutorialStage;
					m_fromTitle = stageInfo.FromTitle;
					m_quickMode = stageInfo.QuickMode;
					m_bossStage = (!m_quickMode && stageInfo.BossStage);
					if (m_firstTutorial)
					{
						m_quickMode = false;
						m_tutorialStage = false;
						m_bossStage = false;
					}
					else if (m_tutorialStage)
					{
						m_quickMode = false;
						m_bossStage = false;
					}
					if (!m_firstTutorial && !m_tutorialStage && HudMenuUtility.IsItemTutorial())
					{
						m_equipItemTutorial = true;
					}
					if (StageModeManager.Instance != null)
					{
						StageModeManager.Instance.FirstTutorial = m_firstTutorial;
						if (m_quickMode)
						{
							StageModeManager.Instance.StageMode = StageModeManager.Mode.QUICK;
						}
						else if (m_firstTutorial || m_tutorialStage)
						{
							StageModeManager.Instance.StageMode = StageModeManager.Mode.UNKNOWN;
						}
						else
						{
							StageModeManager.Instance.StageMode = StageModeManager.Mode.ENDLESS;
						}
					}
					if (m_bossStage)
					{
						m_bossType = stageInfo.BossType;
					}
					else
					{
						m_bossType = BossType.FEVER;
					}
					m_eventStage = stageInfo.EventStage;
					flag = stageInfo.BoostItemValid[2];
					if (TenseEffectManager.Instance != null)
					{
						TenseEffectManager.Type type = (stageInfo.TenseType != 0) ? TenseEffectManager.Type.TENSE_B : TenseEffectManager.Type.TENSE_A;
						TenseEffectManager.Instance.SetType(type);
						m_stageTenseType = stageInfo.TenseType;
						TenseEffectManager.Instance.NotChangeTense = stageInfo.NotChangeTense;
					}
					m_postGameResults.m_existBoss = stageInfo.ExistBoss;
					m_postGameResults.m_prevMapInfo = new StageInfo.MileageMapInfo();
					m_postGameResults.m_prevMapInfo.m_mapState.Set(stageInfo.MileageInfo.m_mapState);
					stageInfo.MileageInfo.m_pointScore.CopyTo(m_postGameResults.m_prevMapInfo.m_pointScore, 0);
					m_levelInformation.NumBossAttack = stageInfo.NumBossAttack;
					m_oldNumBossAttack = m_levelInformation.NumBossAttack;
					if (m_levelInformation.NumBossAttack > 0)
					{
						m_bossNoMissChance = false;
					}
					else
					{
						m_bossNoMissChance = true;
					}
					bool flag2 = true;
					if (SystemSaveManager.GetSystemSaveData() != null)
					{
						m_levelInformation.LightMode = SystemSaveManager.GetSystemSaveData().lightMode;
						int numRank = ServerInterface.PlayerState.m_numRank;
						if (numRank < 2)
						{
							flag2 = SystemSaveManager.GetSystemSaveData().IsFlagStatus(SystemData.FlagStatus.TUTORIAL_FEVER_BOSS);
						}
					}
					if (m_bossStage)
					{
						if (m_eventManager != null && m_eventManager.IsRaidBossStage() && m_eventStage)
						{
							int num = 0;
							if (RaidBossInfo.currentRaidData != null)
							{
								num = RaidBossInfo.currentRaidData.lv;
							}
							if (m_levelInformation.NumBossAttack == 0 && (num == 1 || num == 5))
							{
								m_showEventBossTutorial = true;
							}
						}
						else if (stageInfo.MileageInfo.m_mapState != null && m_levelInformation.NumBossAttack == 0)
						{
							MileageMapState mapState = stageInfo.MileageInfo.m_mapState;
							if (mapState.m_episode == 1)
							{
								m_showMapBossTutorial = true;
							}
							if (mapState.m_episode == 2)
							{
								m_showMapBossTutorial = true;
							}
							if (mapState.m_episode == 3)
							{
								m_showMapBossTutorial = true;
							}
						}
					}
					else
					{
						m_showFeverBossTutorial = !flag2;
					}
				}
				if (StageItemManager.Instance != null)
				{
					StageItemManager.Instance.SetEquipItemTutorial(m_equipItemTutorial);
					StageItemManager.Instance.SetEquippedItem(stageInfo.EquippedItems);
					m_useEquippedItem.Clear();
					if (stageInfo.EquippedItems != null)
					{
						ItemType[] equippedItems = stageInfo.EquippedItems;
						foreach (ItemType item in equippedItems)
						{
							m_useEquippedItem.Add(item);
						}
					}
				}
				for (int l = 0; l < 3; l++)
				{
					bool flag3 = stageInfo.BoostItemValid[l];
					if (flag3)
					{
						BoostItemType item2 = (BoostItemType)l;
						m_useBoostItem.Add(item2);
					}
					if (l == 1 && StageItemManager.Instance != null)
					{
						StageItemManager.Instance.SetActiveAltitudeTrampoline(flag3);
					}
				}
				UnityEngine.Object.Destroy(stageInfo.gameObject);
			}
			TerrainXmlData.SetAssetName(m_stageName);
			StageScoreManager instance = StageScoreManager.Instance;
			if (instance != null)
			{
				instance.Setup(m_bossStage);
			}
			m_stagePathManager = GameObjectUtil.FindGameObjectComponent<PathManager>("StagePathManager");
			if (m_stagePathManager == null)
			{
				GameObject gameObject6 = new GameObject("StagePathManager");
				m_stagePathManager = gameObject6.AddComponent<PathManager>();
			}
			m_playerInformation = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
			if (m_playerInformation != null)
			{
				if (m_playerInformation != null && SaveDataManager.Instance != null)
				{
					PlayerData playerData = SaveDataManager.Instance.PlayerData;
					m_mainChara = playerData.MainChara;
					m_subChara = ((!flag) ? CharaType.UNKNOWN : playerData.SubChara);
				}
				else
				{
					m_mainChara = CharaType.SONIC;
					m_subChara = CharaType.UNKNOWN;
				}
				m_playerInformation.SetPlayerCharacter((int)m_mainChara, (int)m_subChara);
			}
			m_characterContainer = GameObjectUtil.FindGameObjectComponent<CharacterContainer>("CharacterContainer");
			if (m_characterContainer != null)
			{
				m_characterContainer.Init();
			}
			m_hudCaution = HudCaution.Instance;
			m_missionManager = GameObjectUtil.FindGameObjectComponent<StageMissionManager>("StageMissionManager");
			m_tutorialManager = GameObjectUtil.FindGameObjectComponent<StageTutorialManager>("StageTutorialManager");
			m_friendSignManager = GameObjectUtil.FindGameObjectComponent<FriendSignManager>("FriendSignManager");
			if (m_tutorialStage)
			{
				if (m_tutorialManager == null)
				{
					GameObject gameObject7 = new GameObject("StageTutorialManager");
					m_tutorialManager = gameObject7.AddComponent<StageTutorialManager>();
				}
			}
			else if (m_tutorialManager != null)
			{
				UnityEngine.Object.Destroy(m_tutorialManager.gameObject);
				m_tutorialManager = null;
			}
			if (m_uiRootObj != null)
			{
				GameObject gameObject8 = GameObjectUtil.FindChildGameObject(m_uiRootObj, "Result");
				if (gameObject8 != null)
				{
					m_gameResult = gameObject8.GetComponent<GameResult>();
					gameObject8.SetActive(false);
				}
			}
			if (ServerInterface.PlayerState != null && m_levelInformation != null)
			{
				m_levelInformation.PlayerRank = ServerInterface.PlayerState.m_numRank;
			}
			CreateStageBlockManager();
			if (m_eventManager != null && m_eventManager.IsRaidBossStage())
			{
				if (m_bossType == BossType.EVENT2)
				{
					SendPlayerSpeedLevel(PlayerSpeed.LEVEL_2);
				}
				else if (m_bossType == BossType.EVENT3)
				{
					SendPlayerSpeedLevel(PlayerSpeed.LEVEL_3);
				}
			}
			else if (m_quickMode)
			{
				int a = 0;
				if (m_stageBlockManager != null)
				{
					a = m_stageBlockManager.GetComponent<StageBlockManager>().GetBlockLevel();
				}
				int speedLevel = Mathf.Min(a, 2);
				SendPlayerSpeedLevel((PlayerSpeed)speedLevel);
			}
			if (DelayedMessageManager.Instance == null)
			{
				GameObject gameObject9 = new GameObject("DelayedMessageManager");
				gameObject9.AddComponent<DelayedMessageManager>();
			}
			m_tutorialKind = HudTutorial.Kind.NONE;
			m_showItemTutorial = -1;
			if (m_playerInformation != null)
			{
				if (HudTutorial.IsCharaTutorial((CharaType)m_playerInformation.MainCharacterID))
				{
					m_showCharaTutorial = (int)CharaTypeUtil.GetCharacterTutorialID((CharaType)m_playerInformation.MainCharacterID);
				}
				else
				{
					m_showCharaTutorial = -1;
				}
			}
			m_showActionTutorial = -1;
			if (m_quickMode)
			{
				if (HudTutorial.IsQuickModeTutorial(HudTutorial.Id.QUICK_1))
				{
					m_showQuickTurorial = 54;
				}
			}
			else
			{
				m_showQuickTurorial = -1;
			}
			m_saveFlag = false;
			GameObject gameObject10 = GameObject.Find("AllocationStatus");
			if (gameObject10 != null)
			{
				UnityEngine.Object.Destroy(gameObject10);
			}
			TimeProfiler.EndCountTime("GameModeStage:StateInit");
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(0);
			}
			return TinyFsmState.End();
		case 0:
			if (!Env.useAssetBundle || AssetBundleLoader.Instance.IsEnableDownlad())
			{
				ChangeState(new TinyFsmState(StateLoad));
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoad(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_sceneLoader = new GameObject("SceneLoader");
			ResourceSceneLoader resourceSceneLoader = m_sceneLoader.AddComponent<ResourceSceneLoader>();
			TextManager.LoadCommonText(resourceSceneLoader);
			TextManager.LoadEventText(resourceSceneLoader);
			TextManager.LoadChaoText(resourceSceneLoader);
			resourceSceneLoader.AddLoad("TenseEffectTable", true, false);
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(1);
			}
			return TinyFsmState.End();
		case 0:
			if (m_sceneLoader != null && m_sceneLoader.GetComponent<ResourceSceneLoader>().Loaded)
			{
				TextManager.SetupCommonText();
				TextManager.SetupEventText();
				TextManager.SetupChaoText();
				UnityEngine.Object.Destroy(m_sceneLoader);
				m_sceneLoader = null;
				ChangeState(new TinyFsmState(StateLoad2));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoad2(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			TimeProfiler.StartCountTime("GameModeStage:Load");
			m_counter = 0;
			if (AtlasManager.Instance != null)
			{
				AtlasManager.Instance.StartLoadAtlasForStage();
			}
			if (m_isLoadResources)
			{
				m_sceneLoader = new GameObject("SceneLoader");
				ResourceSceneLoader resourceSceneLoader = m_sceneLoader.AddComponent<ResourceSceneLoader>();
				for (int i = 0; i < m_loadInfo.Count; i++)
				{
					ResourceSceneLoader.ResourceInfo resourceInfo = m_loadInfo[i];
					switch (i)
					{
					case 6:
						if (m_quickMode && m_eventManager != null && m_eventManager.Type == EventManager.EventType.QUICK)
						{
							ResourceSceneLoader.ResourceInfo resourceInfo3 = resourceInfo;
							resourceInfo3.m_scenename += EventManager.GetResourceName();
							resourceSceneLoader.AddLoadAndResourceManager(resourceInfo3);
						}
						break;
					case 7:
						if (m_eventManager != null && m_eventManager.Type != EventManager.EventType.UNKNOWN)
						{
							ResourceSceneLoader.ResourceInfo resourceInfo2 = resourceInfo;
							resourceInfo2.m_scenename += EventManager.GetResourceName();
							resourceSceneLoader.AddLoadAndResourceManager(resourceInfo2);
						}
						break;
					default:
						resourceSceneLoader.AddLoadAndResourceManager(resourceInfo);
						break;
					}
				}
				if (m_quickMode)
				{
					foreach (ResourceSceneLoader.ResourceInfo item in m_quickModeLoadInfo)
					{
						if (item.m_category == ResourceCategory.EVENT_RESOURCE)
						{
							if (m_eventManager != null && m_eventStage)
							{
								ResourceSceneLoader.ResourceInfo resourceInfo4 = item;
								resourceInfo4.m_scenename += EventManager.GetResourceName();
								resourceSceneLoader.AddLoadAndResourceManager(resourceInfo4);
							}
						}
						else
						{
							resourceSceneLoader.AddLoadAndResourceManager(item);
						}
					}
				}
				m_terrainDataName = m_stageName + "_TerrainData";
				resourceSceneLoader.AddLoad(m_terrainDataName, true, false);
				m_stageResourceName = m_stageName + "_StageResource";
				m_stageResourceObjectName = m_stageName + "_StageModelResource";
				resourceSceneLoader.AddLoad(m_stageResourceName, true, false);
				if (m_playerInformation != null)
				{
					string mainCharacterName = m_playerInformation.MainCharacterName;
					if (mainCharacterName != null)
					{
						resourceSceneLoader.AddLoad("CharacterModel" + mainCharacterName, true, false);
						resourceSceneLoader.AddLoad("CharacterEffect" + mainCharacterName, true, false);
					}
					string subCharacterName = m_playerInformation.SubCharacterName;
					if (subCharacterName != null)
					{
						resourceSceneLoader.AddLoad("CharacterModel" + subCharacterName, true, false);
						resourceSceneLoader.AddLoad("CharacterEffect" + subCharacterName, true, false);
					}
					BossType tutorialBossType = BossType.NONE;
					if (m_showMapBossTutorial)
					{
						tutorialBossType = m_bossType;
					}
					else if (m_showFeverBossTutorial)
					{
						tutorialBossType = m_bossType;
					}
					else if (m_tutorialStage)
					{
						tutorialBossType = BossType.FEVER;
						m_showFeverBossTutorial = true;
					}
					HudTutorial.Load(resourceSceneLoader, m_tutorialStage, m_bossStage, tutorialBossType, (CharaType)m_playerInformation.MainCharacterID, (CharaType)m_playerInformation.SubCharacterID);
				}
				SaveDataManager instance3 = SaveDataManager.Instance;
				if (instance3 != null)
				{
					bool onAssetBundle = true;
					int mainChaoID = instance3.PlayerData.MainChaoID;
					int subChaoID = instance3.PlayerData.SubChaoID;
					if (mainChaoID >= 0)
					{
						resourceSceneLoader.AddLoad("chao_" + mainChaoID.ToString("0000"), onAssetBundle, false);
					}
					if (subChaoID >= 0 && subChaoID != mainChaoID)
					{
						resourceSceneLoader.AddLoad("chao_" + subChaoID.ToString("0000"), onAssetBundle, false);
					}
				}
				StageAbilityManager.LoadAbilityDataTable(resourceSceneLoader);
			}
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(2);
			}
			return TinyFsmState.End();
		case 0:
			if (m_sceneLoader != null && AtlasManager.Instance != null)
			{
				if (m_sceneLoader.GetComponent<ResourceSceneLoader>().Loaded && AtlasManager.Instance.IsLoadAtlas())
				{
					TimeProfiler.EndCountTime("GameModeStage:Load");
					RegisterAllResource();
					UnityEngine.Object.Destroy(m_sceneLoader);
					m_sceneLoader = null;
					AtlasManager.Instance.ReplaceAtlasForStage();
					m_stagePathManager.CreatePathObjectData();
					if (m_quickMode)
					{
						StageTimeManager stageTimeManager = GameObjectUtil.FindGameObjectComponent<StageTimeManager>("StageTimeManager");
						if (stageTimeManager != null)
						{
							stageTimeManager.SetTable();
						}
					}
					EventObjectTable.LoadSetup();
					EventSPStageObjectTable.LoadSetup();
					EventBossObjectTable.LoadSetup();
					EventBossParamTable.LoadSetup();
					EventCommonDataTable.LoadSetup();
					TimeProfiler.StartCountTime("GameModeStage:SetupStageBlocks");
					StageBlockManager component = m_stageBlockManager.GetComponent<StageBlockManager>();
					if (component != null)
					{
						component.Setup(m_bossStage);
						component.PauseTerrainPlacement(m_notPlaceTerrain);
					}
					TimeProfiler.EndCountTime("GameModeStage:SetupStageBlocks");
					ResourceManager instance = ResourceManager.Instance;
					GameObject gameObject = instance.GetGameObject(ResourceCategory.TERRAIN_MODEL, TerrainXmlData.DataAssetName);
					if (gameObject != null)
					{
						TerrainXmlData component2 = gameObject.GetComponent<TerrainXmlData>();
						if (component2 != null)
						{
							if (m_levelInformation != null)
							{
								m_levelInformation.MoveTrapBooRand = component2.MoveTrapBooRand;
							}
							if (StageItemManager.Instance != null)
							{
								ItemTable itemTable = StageItemManager.Instance.GetItemTable();
								if (itemTable != null)
								{
									itemTable.Setup(component2);
								}
							}
							RareEnemyTable rareEnemyTable = GetRareEnemyTable();
							if (rareEnemyTable != null)
							{
								rareEnemyTable.Setup(component2);
							}
							EnemyExtendItemTable enemyExtendItemTable = GetEnemyExtendItemTable();
							if (enemyExtendItemTable != null)
							{
								enemyExtendItemTable.Setup(component2);
							}
							BossTable bossTable = GetBossTable();
							if (bossTable != null)
							{
								bossTable.Setup(component2);
							}
							BossMap3Table bossMap3Table = GetBossMap3Table();
							if (bossMap3Table != null)
							{
								bossMap3Table.Setup(component2);
							}
							ObjectPartTable objectPartTable = GetObjectPartTable();
							if (objectPartTable != null)
							{
								objectPartTable.Setup(component2);
							}
						}
					}
					if (m_characterContainer != null)
					{
						m_characterContainer.SetupCharacter();
					}
					StageComboManager instance2 = StageComboManager.Instance;
					if (instance2 != null)
					{
						instance2.Setup();
						instance2.SetComboTime(m_quickMode);
					}
					if (m_uiRootObj != null)
					{
						m_continueWindowObj = GameObjectUtil.FindChildGameObject(m_uiRootObj, "ContinueWindow");
						if (m_continueWindowObj != null)
						{
							HudContinue component3 = m_continueWindowObj.GetComponent<HudContinue>();
							if (component3 != null)
							{
								component3.Setup(m_bossStage);
							}
							m_continueWindowObj.SetActive(false);
						}
					}
					if (m_hudCaution != null)
					{
						m_hudCaution.SetBossWord(m_bossStage);
					}
					BossType bossType = BossType.NONE;
					if (m_bossStage)
					{
						bossType = m_bossType;
					}
					bool spCrystal = EventManager.Instance != null && EventManager.Instance.IsSpecialStage();
					bool animal = EventManager.Instance != null && EventManager.Instance.IsGetAnimalStage();
					GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnSetup", new MsgHudCockpitSetup(bossType, spCrystal, animal, !m_tutorialStage, m_firstTutorial), SendMessageOptions.DontRequireReceiver);
					if (FontManager.Instance != null)
					{
						FontManager.Instance.ReplaceFont();
					}
					if (StageEffectManager.Instance != null)
					{
						StageEffectManager.Instance.StockStageEffect(m_bossStage | m_tutorialStage);
					}
					if (AnimalResourceManager.Instance != null)
					{
						AnimalResourceManager.Instance.StockAnimalObject(m_bossStage | m_tutorialStage);
					}
					PlayStageEffect();
					ChangeState(new TinyFsmState(StateRequestActStart));
				}
			}
			else
			{
				ChangeState(new TinyFsmState(StateRequestActStart));
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRequestActStart(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			RequestServerStartAct();
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(3);
			}
			return TinyFsmState.End();
		case 0:
			if (m_serverActEnd)
			{
				ChangeState(new TinyFsmState(StateSoundConnectIfNotFound));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSoundConnectIfNotFound(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			string text = null;
			string text2 = null;
			string cueSheetName = GetCueSheetName();
			if (!string.IsNullOrEmpty(cueSheetName))
			{
				text = cueSheetName + ".acb";
				text2 = cueSheetName + "_streamfiles.awb";
			}
			string downloadURL = SoundManager.GetDownloadURL();
			string downloadedDataPath = SoundManager.GetDownloadedDataPath();
			StreamingDataLoader instance2 = StreamingDataLoader.Instance;
			if (instance2 != null)
			{
				if (text != null)
				{
					instance2.AddFileIfNotDownloaded(downloadURL + text, downloadedDataPath + text);
				}
				if (text2 != null)
				{
					instance2.AddFileIfNotDownloaded(downloadURL + text2, downloadedDataPath + text2);
				}
				StageStreamingDataLoadRetryProcess process = new StageStreamingDataLoadRetryProcess(base.gameObject, this);
				NetMonitor.Instance.StartMonitor(process, 0f, HudNetworkConnect.DisplayType.ALL);
				instance2.StartDownload(0, base.gameObject);
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(4);
			}
			return TinyFsmState.End();
		case 0:
		{
			StreamingDataLoader instance = StreamingDataLoader.Instance;
			if (instance != null)
			{
				if (instance.Loaded)
				{
					ChangeState(new TinyFsmState(StateAccessNetworkForStartAct));
				}
			}
			else
			{
				ChangeState(new TinyFsmState(StateAccessNetworkForStartAct));
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateAccessNetworkForStartAct(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			TimeProfiler.StartCountTime("GameModeStage:StateAccessNetworkForStartAct");
			return TinyFsmState.End();
		case -4:
			TimeProfiler.EndCountTime("GameModeStage:StateAccessNetworkForStartAct");
			if (m_progressBar != null)
			{
				m_progressBar.SetState(5);
			}
			return TinyFsmState.End();
		case 0:
			if (m_serverActEnd && m_stagePathManager.SetupEnd && m_stageBlockManager.GetComponent<StageBlockManager>().IsSetupEnded())
			{
				SetMainBGMName();
				SoundManager.AddStageCueSheet(GetCueSheetName());
				ObjUtil.CreateSharedMateriaDummyObject(ResourceCategory.OBJECT_RESOURCE, "obj_cmn_ring");
				ObjUtil.CreateSharedMateriaDummyObject(ResourceCategory.OBJECT_RESOURCE, "obj_cmn_movetrap");
				if (EventManager.Instance != null && EventManager.Instance.IsQuickEvent() && m_quickMode)
				{
					ObjUtil.CreateSharedMateriaDummyObject(ResourceCategory.EVENT_RESOURCE, "obj_sp_goldcoin");
					ObjUtil.CreateSharedMateriaDummyObject(ResourceCategory.EVENT_RESOURCE, "obj_sp_goldcoin10");
					ObjUtil.CreateSharedMateriaDummyObject(ResourceCategory.EVENT_RESOURCE, "obj_sp_pearl10");
				}
				ChangeState(new TinyFsmState(StateSetupPrepareBlock));
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSetupPrepareBlock(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if ((bool)m_stageBlockManager && (bool)m_playerInformation)
			{
				MsgPrepareStageReplace value = new MsgPrepareStageReplace(m_playerInformation.SpeedLevel, m_stageName);
				m_stageBlockManager.SendMessage("OnMsgPrepareStageReplace", value);
			}
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(6);
			}
			return TinyFsmState.End();
		case 0:
			if (m_stageBlockManager.GetComponent<StageBlockManager>().IsSetupEnded())
			{
				ChangeState(new TinyFsmState(StateSetupBlock));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSetupBlock(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			foreach (GameObject item in m_pausedObject)
			{
				if (item != null)
				{
					item.SetActive(true);
				}
			}
			m_pausedObject.Clear();
			if (m_progressBar != null)
			{
				m_progressBar.SetState(7);
			}
			return TinyFsmState.End();
		case 0:
			if ((bool)m_stageBlockManager && (bool)m_playerInformation)
			{
				TimeProfiler.StartCountTime("GameModeStage:SetupBlock");
				MsgStageReplace value = new MsgStageReplace(m_playerInformation.SpeedLevel, PlayerResetPosition, PlayerResetRotation, m_stageName);
				if (m_bossStage)
				{
					BossType bossType = m_bossType;
					MsgSetStageOnMapBoss value2 = new MsgSetStageOnMapBoss(PlayerResetPosition, PlayerResetRotation, m_stageName, bossType);
					m_stageBlockManager.SendMessage("OnMsgSetStageOnMapBoss", value2);
					if (m_levelInformation != null)
					{
						m_levelInformation.NowBoss = true;
					}
				}
				else if (m_tutorialStage)
				{
					m_stageBlockManager.GetComponent<StageBlockManager>().SetStageOnTutorial(PlayerResetPosition);
				}
				else
				{
					m_stageBlockManager.SendMessage("OnMsgStageReplace", value);
				}
				StageFarTerrainManager stageFarTerrainManager = GameObjectUtil.FindGameObjectComponent<StageFarTerrainManager>("StageFarManager");
				if (stageFarTerrainManager != null)
				{
					stageFarTerrainManager.SendMessage("OnMsgStageReplace", value);
				}
				TimeProfiler.EndCountTime("GameModeStage:SetupBlock");
			}
			if (!m_tutorialStage && !m_firstTutorial && m_missionManager != null)
			{
				m_missionManager.SetupMissions();
				m_missonCompleted = m_missionManager.Completed;
			}
			if (m_tutorialManager != null)
			{
				m_tutorialManager.SetupTutorial();
			}
			TimeProfiler.StartCountTime("GameModeStage:SetupFriendManager");
			if (m_friendSignManager != null && !m_bossStage)
			{
				m_friendSignManager.SetupFriendSignManager();
			}
			TimeProfiler.EndCountTime("GameModeStage:SetupFriendManager");
			ChangeState(new TinyFsmState(StateSendApolloStageStart));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSendApolloStageStart(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			ApolloTutorialIndex apolloStartTutorialIndex = GetApolloStartTutorialIndex();
			if (apolloStartTutorialIndex != ApolloTutorialIndex.NONE)
			{
				string[] value = new string[1];
				SendApollo.GetTutorialValue(apolloStartTutorialIndex, ref value);
				m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_START, value);
			}
			else
			{
				m_sendApollo = null;
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_sendApollo != null)
			{
				UnityEngine.Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
			}
			if (m_progressBar != null)
			{
				m_progressBar.SetState(8);
			}
			return TinyFsmState.End();
		case 0:
		{
			bool flag = true;
			if (m_sendApollo != null && m_sendApollo.GetState() == SendApollo.State.Succeeded)
			{
				flag = false;
			}
			if (flag)
			{
				ChangeState(new TinyFsmState(StateFadeIn));
			}
			return TinyFsmState.End();
		}
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateFadeIn(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GC.Collect();
			Resources.UnloadUnusedAssets();
			GC.Collect();
			SoundManager.BgmPlay(m_mainBgmName);
			m_timer = 0.5f;
			SetChaoAblityTimeScale();
			SetDefaultTimeScale();
			HudLoading.EndScreen();
			MsgDisableInput value = new MsgDisableInput(true);
			GameObjectUtil.SendMessageToTagObjects("Player", "OnInputDisable", value, SendMessageOptions.DontRequireReceiver);
			return TinyFsmState.End();
		}
		case -4:
			if (m_connectAlertUI2 != null)
			{
				m_connectAlertUI2.SetActive(false);
			}
			return TinyFsmState.End();
		case 0:
			m_timer -= e.GetDeltaTime;
			if (m_timer < 0f)
			{
				ChangeState(new TinyFsmState(StateGameStart));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGameStart(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (m_bossStage)
			{
				if (m_eventManager != null && m_eventManager.IsRaidBossStage() && m_eventStage)
				{
					SendMessageToHudCaution(HudCaution.Type.EVENTBOSS);
				}
				else
				{
					SendMessageToHudCaution(HudCaution.Type.BOSS);
				}
				SoundManager.SePlay("sys_boss_warning");
			}
			else
			{
				SendMessageToHudCaution(HudCaution.Type.GO);
				SoundManager.SePlay("sys_go");
			}
			m_timer = 1f;
			return TinyFsmState.End();
		case -4:
		{
			MsgDisableInput value = new MsgDisableInput(false);
			GameObjectUtil.SendMessageToTagObjects("Player", "OnInputDisable", value, SendMessageOptions.DontRequireReceiver);
			if (m_bossStage || ObjUtil.IsUseTemporarySet())
			{
				ObjUtil.SendStartItemAndChao();
			}
			return TinyFsmState.End();
		}
		case 0:
			m_timer -= e.GetDeltaTime;
			if (m_timer < 0f)
			{
				BackKeyManager.StartScene();
				HudPlayerChangeCharaButton(true, false);
				if (m_chaoEasyTimeScale < 1f)
				{
					ObjUtil.RequestStartAbilityToChao(ChaoAbility.EASY_SPEED, false);
				}
				ChangeState(new TinyFsmState(StateNormal));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateNormal(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			EnablePause(true);
			if (m_quickMode && m_showQuickTurorial != -1)
			{
				GameObjectUtil.SendDelayedMessageToGameObject(base.gameObject, "OnMsgTutorialQuickMode", new MsgTutorialQuickMode((HudTutorial.Id)m_showQuickTurorial));
			}
			else if (m_showCharaTutorial != -1)
			{
				GameObjectUtil.SendDelayedMessageToGameObject(base.gameObject, "OnMsgTutorialChara", new MsgTutorialChara((HudTutorial.Id)m_showCharaTutorial));
			}
			m_reqPause = false;
			if (m_levelInformation != null)
			{
				m_levelInformation.RequestPause = m_reqPause;
			}
			m_reqTutorialPause = false;
			if (m_IsNowLastChanceHudCautionBoss)
			{
				SendMessageToHudCaution(HudCaution.Type.BOSS);
				SoundManager.SePlay("sys_boss_warning");
				m_IsNowLastChanceHudCautionBoss = false;
			}
			return TinyFsmState.End();
		case -4:
		{
			bool pause = false;
			if (m_reqPause || m_reqTutorialPause)
			{
				pause = true;
			}
			HudPlayerChangeCharaButton(false, pause);
			m_reqPause = false;
			if (m_levelInformation != null)
			{
				m_levelInformation.RequestPause = m_reqPause;
			}
			EnablePause(false);
			return TinyFsmState.End();
		}
		case 0:
			if (IsEventTimeup())
			{
				ChangeState(new TinyFsmState(StateGameOver));
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		case 1:
			switch (e.GetMessage.ID)
			{
			case 12361:
				if (m_quickMode)
				{
					m_quickModeTimeUp = true;
					if (StageTimeManager.Instance != null)
					{
						StageTimeManager.Instance.Pause();
					}
					if (IsEnableContinue())
					{
						ChangeState(new TinyFsmState(StateCheckContinue));
					}
					else
					{
						ChangeState(new TinyFsmState(StateQuickModeTimeUp));
					}
				}
				return TinyFsmState.End();
			case 20480:
				if (!m_bossClear)
				{
					if (m_quickMode && StageTimeManager.Instance != null)
					{
						StageTimeManager.Instance.Pause();
					}
					if (IsEnableContinue())
					{
						ChangeState(new TinyFsmState(StateCheckContinue));
					}
					else if (m_firstTutorial)
					{
						ChangeState(new TinyFsmState(StateSendApolloStageEnd));
					}
					else if (m_tutorialStage)
					{
						ChangeState(new TinyFsmState(StateEndFadeOut));
					}
					else
					{
						ChangeState(new TinyFsmState(StateGameOver));
					}
				}
				return TinyFsmState.End();
			case 4096:
				m_reqPauseBackMain = false;
				m_reqPause = true;
				if (m_levelInformation != null)
				{
					m_levelInformation.RequestPause = m_reqPause;
				}
				return TinyFsmState.End();
			case 12358:
			{
				MsgExternalGamePause msgExternalGamePause = e.GetMessage as MsgExternalGamePause;
				m_reqPauseBackMain = msgExternalGamePause.m_backMainMenu;
				m_reqPause = true;
				if (m_levelInformation != null)
				{
					m_levelInformation.RequestPause = m_reqPause;
				}
				return TinyFsmState.End();
			}
			case 12306:
				if (m_levelInformation != null)
				{
					m_levelInformation.NowFeverBoss = true;
				}
				if (StageItemManager.Instance != null)
				{
					MsgPauseItemOnBoss msg2 = new MsgPauseItemOnBoss();
					StageItemManager.Instance.OnPauseItemOnBoss(msg2);
				}
				SendBossStartMessageToChao();
				if (!m_playerInformation.IsNowLastChance())
				{
					SendMessageToHudCaution(HudCaution.Type.BOSS);
					SoundManager.SePlay("sys_boss_warning");
				}
				else
				{
					m_IsNowLastChanceHudCautionBoss = true;
				}
				if (m_quickMode && StageTimeManager.Instance != null)
				{
					StageTimeManager.Instance.Pause();
				}
				break;
			case 12308:
				if (m_bossStage)
				{
					m_bossClear = true;
				}
				break;
			case 12307:
			{
				MsgBossEnd msgBossEnd = e.GetMessage as MsgBossEnd;
				if (msgBossEnd == null)
				{
					break;
				}
				if (m_bossStage)
				{
					if (m_levelInformation != null)
					{
						m_levelInformation.BossDestroy = msgBossEnd.m_dead;
					}
					ChangeState(new TinyFsmState(StatePrepareUpdateDatabase));
					return TinyFsmState.End();
				}
				if (m_levelInformation != null)
				{
					m_levelInformation.InvalidExtreme = false;
					DrawingInvalidExtreme();
				}
				if (StageItemManager.Instance != null)
				{
					MsgPauseItemOnChageLevel msg = new MsgPauseItemOnChageLevel();
					StageItemManager.Instance.OnPauseItemOnChangeLevel(msg);
				}
				GameObjectUtil.SendMessageToTagObjects("Chao", "OnPauseChangeLevel", null, SendMessageOptions.DontRequireReceiver);
				if (m_tutorialStage)
				{
					ChangeState(new TinyFsmState(StateEndFadeOut));
				}
				else
				{
					ChangeState(new TinyFsmState(StateChangeLevel));
				}
				return TinyFsmState.End();
			}
			case 12313:
				if (m_levelInformation != null)
				{
					m_levelInformation.RequestCharaChange = true;
				}
				if (m_characterContainer != null)
				{
					MsgChangeChara msgChangeChara = e.GetMessage as MsgChangeChara;
					if (msgChangeChara != null)
					{
						m_characterContainer.SendMessage("OnMsgChangeChara", e.GetMessage);
					}
				}
				break;
			case 12304:
			{
				MsgTransformPhantom msgTransformPhantom = e.GetMessage as MsgTransformPhantom;
				PhantomType type = msgTransformPhantom.m_type;
				string text = null;
				switch (type)
				{
				case PhantomType.DRILL:
					text = "bgm_p_drill";
					break;
				case PhantomType.LASER:
					text = "bgm_p_laser";
					break;
				case PhantomType.ASTEROID:
					text = "bgm_p_asteroid";
					break;
				}
				if (text != null && !m_bossStage)
				{
					SoundManager.BgmChange(m_mainBgmName);
					SoundManager.BgmCrossFadePlay(text, "BGM_jingle");
				}
				return TinyFsmState.End();
			}
			case 12305:
				if (!m_bossStage)
				{
					SoundManager.BgmCrossFadeStop(1f, 1f);
				}
				return TinyFsmState.End();
			case 12329:
			{
				MsgInvincible msgInvincible = e.GetMessage as MsgInvincible;
				if (msgInvincible != null)
				{
					if (msgInvincible.m_mode == MsgInvincible.Mode.Start)
					{
						SoundManager.ItemBgmCrossFadePlay("jingle_invincible", "BGM_jingle");
					}
					else
					{
						SoundManager.BgmCrossFadeStop(1f, 0.5f);
					}
				}
				return TinyFsmState.End();
			}
			case 12333:
				if (m_tutorialManager != null)
				{
					MsgTutorialPlayStart msgTutorialPlayStart = e.GetMessage as MsgTutorialPlayStart;
					if (msgTutorialPlayStart != null)
					{
						m_tutorialMissionID = msgTutorialPlayStart.m_eventID;
						m_tutorialKind = HudTutorial.Kind.MISSION;
						m_reqTutorialPause = true;
						ChangeState(new TinyFsmState(StateTutorialPause));
					}
				}
				return TinyFsmState.End();
			case 12335:
			{
				MsgTutorialPlayEnd msgTutorialPlayEnd = m_tutorialEndMsg = (e.GetMessage as MsgTutorialPlayEnd);
				ChangeState(new TinyFsmState(StateTutorialMissionEnd));
				return TinyFsmState.End();
			}
			case 12339:
				if (m_showMapBossTutorial)
				{
					m_tutorialKind = HudTutorial.Kind.MAPBOSS;
					m_reqTutorialPause = true;
					ChangeState(new TinyFsmState(StateTutorialPause));
				}
				else if (m_showEventBossTutorial)
				{
					m_tutorialKind = HudTutorial.Kind.EVENTBOSS;
					m_reqTutorialPause = true;
					ChangeState(new TinyFsmState(StateTutorialPause));
				}
				return TinyFsmState.End();
			case 12340:
				if (m_showFeverBossTutorial)
				{
					m_tutorialKind = HudTutorial.Kind.FEVERBOSS;
					m_reqTutorialPause = true;
					ChangeState(new TinyFsmState(StateTutorialPause));
				}
				return TinyFsmState.End();
			case 12341:
				if (!m_bossStage)
				{
					MsgTutorialItem msgTutorialItem = e.GetMessage as MsgTutorialItem;
					m_showItemTutorial = (int)msgTutorialItem.m_id;
					m_tutorialKind = HudTutorial.Kind.ITEM;
					m_reqTutorialPause = true;
					ChangeState(new TinyFsmState(StateTutorialPause));
				}
				return TinyFsmState.End();
			case 12342:
				m_reqTutorialPause = true;
				ChangeState(new TinyFsmState(StateItemButtonTutorialPause));
				return TinyFsmState.End();
			case 12343:
				m_tutorialKind = HudTutorial.Kind.CHARA;
				m_reqTutorialPause = true;
				ChangeState(new TinyFsmState(StateTutorialPause));
				return TinyFsmState.End();
			case 12344:
				if (!m_bossStage && !m_tutorialStage)
				{
					MsgTutorialAction msgTutorialAction = e.GetMessage as MsgTutorialAction;
					m_showActionTutorial = (int)msgTutorialAction.m_id;
					m_tutorialKind = HudTutorial.Kind.ACTION;
					m_reqTutorialPause = true;
					ChangeState(new TinyFsmState(StateTutorialPause));
				}
				return TinyFsmState.End();
			case 12345:
				if (m_quickMode)
				{
					MsgTutorialQuickMode msgTutorialQuickMode = e.GetMessage as MsgTutorialQuickMode;
					m_showQuickTurorial = (int)msgTutorialQuickMode.m_id;
					m_tutorialKind = HudTutorial.Kind.QUICK;
					m_reqTutorialPause = true;
					ChangeState(new TinyFsmState(StateTutorialPause));
				}
				return TinyFsmState.End();
			case 12350:
				GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnPhantomActStart", e.GetMessage, SendMessageOptions.DontRequireReceiver);
				return TinyFsmState.End();
			case 12351:
				GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnPhantomActEnd", e.GetMessage, SendMessageOptions.DontRequireReceiver);
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StatePause(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnSetPause", new MsgSetPause(m_reqPauseBackMain, false), SendMessageOptions.DontRequireReceiver);
			m_reqPauseBackMain = false;
			SetTimeScale(0f);
			SoundManager.BgmPause(true);
			SoundManager.SePausePlaying(true);
			SoundManager.SePlay("sys_pause");
			StageDebugInformation.CreateActivateButton();
			GC.Collect();
			Resources.UnloadUnusedAssets();
			GC.Collect();
			return TinyFsmState.End();
		case -4:
			StageDebugInformation.DestroyActivateButton();
			SetDefaultTimeScale();
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
			switch (e.GetMessage.ID)
			{
			case 4097:
				SoundManager.BgmPause(false);
				SoundManager.SePausePlaying(false);
				HudPlayerChangeCharaButton(true, true);
				ChangeState(new TinyFsmState(StateNormal));
				return TinyFsmState.End();
			case 4098:
				m_retired = true;
				HoldPlayerAndDestroyTerrainOnEnd();
				ChangeState(new TinyFsmState(StateUpdateDatabase));
				return TinyFsmState.End();
			case 12358:
			{
				MsgExternalGamePause msgExternalGamePause = e.GetMessage as MsgExternalGamePause;
				GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnSetPause", new MsgSetPause(msgExternalGamePause.m_backMainMenu, msgExternalGamePause.m_backKey), SendMessageOptions.DontRequireReceiver);
				return TinyFsmState.End();
			}
			default:
				return TinyFsmState.End();
			}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateChangeLevel(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_timer = 0.4f;
			SendMessageToHudCaution(HudCaution.Type.STAGE_OUT);
			SoundManager.SePlay("boss_scene_change");
			m_substate = 0;
			return TinyFsmState.End();
		case -4:
			m_onSpeedUp = false;
			m_onDestroyRingMode = false;
			return TinyFsmState.End();
		case 0:
			switch (m_substate)
			{
			case 0:
			{
				m_timer -= e.GetDeltaTime;
				if (!(m_timer <= 0f))
				{
					break;
				}
				GC.Collect();
				Resources.UnloadUnusedAssets();
				GC.Collect();
				StageScoreManager instance = StageScoreManager.Instance;
				if (instance != null)
				{
					StageScorePool scorePool = instance.ScorePool;
					if (scorePool != null)
					{
						scorePool.AddScore(ScoreType.distance, (int)m_playerInformation.TotalDistance);
						scorePool.CheckHalfWay();
					}
				}
				MsgPLHold value3 = new MsgPLHold();
				GameObjectUtil.SendMessageToTagObjects("Player", "OnMsgPLHold", value3, SendMessageOptions.DontRequireReceiver);
				ResetPosStageEffect(true);
				if ((bool)TenseEffectManager.Instance)
				{
					TenseEffectManager.Instance.FlipTenseType();
				}
				if (m_stageBlockManager != null)
				{
					StageBlockManager component2 = m_stageBlockManager.GetComponent<StageBlockManager>();
					if (component2 != null)
					{
						component2.ReCreateTerrain();
					}
				}
				m_timer = 0.6f;
				m_counter = 2;
				m_substate = 1;
				break;
			}
			case 1:
				m_timer -= e.GetDeltaTime;
				m_counter--;
				if (m_timer <= 0f && m_counter < 0)
				{
					PlayerSpeed speedLevel2 = (PlayerSpeed)Mathf.Min((int)(m_playerInformation.SpeedLevel + 1), 2);
					MsgPrepareStageReplace value4 = new MsgPrepareStageReplace(speedLevel2, m_stageName);
					m_stageBlockManager.SendMessage("OnMsgPrepareStageReplace", value4);
					ObjUtil.PauseCombo(MsgPauseComboTimer.State.PAUSE);
					m_substate = 2;
				}
				break;
			case 2:
				if ((bool)m_playerInformation)
				{
					bool flag2 = false;
					if (m_stageBlockManager != null)
					{
						flag2 = m_stageBlockManager.GetComponent<StageBlockManager>().IsBlockLevelUp();
					}
					switch (m_playerInformation.SpeedLevel)
					{
					case PlayerSpeed.LEVEL_1:
						if (flag2)
						{
							m_onSpeedUp = true;
							SendPlayerSpeedLevel(PlayerSpeed.LEVEL_2);
						}
						break;
					case PlayerSpeed.LEVEL_2:
						if (flag2)
						{
							m_onSpeedUp = true;
							SendPlayerSpeedLevel(PlayerSpeed.LEVEL_3);
						}
						break;
					case PlayerSpeed.LEVEL_3:
						if (flag2)
						{
							m_onDestroyRingMode = true;
							if (m_levelInformation != null)
							{
								m_levelInformation.Extreme = true;
							}
						}
						break;
					}
				}
				m_counter = 3;
				m_substate = 3;
				break;
			case 3:
				if (m_counter > 0)
				{
					m_counter--;
				}
				if (m_counter <= 0 && m_stageBlockManager.GetComponent<StageBlockManager>().IsSetupEnded())
				{
					m_substate = 4;
				}
				break;
			case 4:
			{
				if ((bool)m_playerInformation)
				{
					PlayerSpeed speedLevel = m_playerInformation.SpeedLevel;
					MsgStageReplace msgStageReplace = new MsgStageReplace(speedLevel, PlayerResetPosition, PlayerResetRotation, m_stageName);
					GameObjectUtil.SendMessageToTagObjects("Player", "OnMsgStageReplace", msgStageReplace, SendMessageOptions.RequireReceiver);
					GameObjectUtil.SendMessageToTagObjects("Chao", "OnMsgStageReplace", msgStageReplace, SendMessageOptions.DontRequireReceiver);
					if (m_stageBlockManager != null)
					{
						StageBlockManager component = m_stageBlockManager.GetComponent<StageBlockManager>();
						if (component != null)
						{
							component.OnMsgStageReplace(msgStageReplace);
						}
					}
					StageFarTerrainManager stageFarTerrainManager = GameObjectUtil.FindGameObjectComponent<StageFarTerrainManager>("StageFarManager");
					if (stageFarTerrainManager != null)
					{
						stageFarTerrainManager.SendMessage("OnMsgStageReplace", msgStageReplace, SendMessageOptions.DontRequireReceiver);
					}
				}
				if (m_levelInformation != null)
				{
					m_levelInformation.NowFeverBoss = false;
				}
				HudCockpit hudCockpit = GameObjectUtil.FindGameObjectComponent<HudCockpit>("HudCockpit");
				if (hudCockpit != null)
				{
					MsgBossEnd value2 = new MsgBossEnd(true);
					hudCockpit.SendMessage("OnBossEnd", value2);
				}
				m_counter = 3;
				m_substate = 5;
				break;
			}
			case 5:
				if (--m_counter < 0)
				{
					MsgStageRestart value = new MsgStageRestart();
					GameObjectUtil.SendMessageToTagObjects("Player", "OnMsgStageRestart", value, SendMessageOptions.RequireReceiver);
					SendMessageToHudCaution(HudCaution.Type.STAGE_IN);
					m_substate = 6;
					m_timer = 0.5f;
				}
				break;
			case 6:
				m_timer -= e.GetDeltaTime;
				if (!(m_timer <= 0f))
				{
					break;
				}
				ResetPosStageEffect(false);
				if (m_onSpeedUp || m_onDestroyRingMode || m_invalidExtremeFlag)
				{
					bool flag = false;
					if (m_onSpeedUp)
					{
						SendMessageToHudCaution(HudCaution.Type.SPEEDUP);
					}
					else
					{
						if (m_levelInformation != null && m_levelInformation.InvalidExtreme)
						{
							flag = true;
							if (m_levelInformation != null && m_levelInformation.InvalidExtreme)
							{
								ObjUtil.RequestStartAbilityToChao(ChaoAbility.INVALIDI_EXTREME_STAGE, false);
							}
						}
						if (!flag)
						{
							m_invalidExtremeFlag = false;
							SendMessageToHudCaution(HudCaution.Type.EXTREMEMODE);
						}
					}
					if (!flag)
					{
						SoundManager.SePlay("sys_speedup");
					}
				}
				if (m_tutorialStage && m_playerInformation.SpeedLevel == PlayerSpeed.LEVEL_2)
				{
					if (StageItemManager.Instance != null)
					{
						MsgUseEquipItem msg = new MsgUseEquipItem();
						StageItemManager.Instance.OnUseEquipItem(msg);
					}
					if (m_levelInformation != null)
					{
						m_levelInformation.NowTutorial = false;
					}
					ChangeState(new TinyFsmState(StateNormal));
				}
				else
				{
					HudPlayerChangeCharaButton(true, false);
					ChangeState(new TinyFsmState(StateNormal));
				}
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void SendPlayerSpeedLevel(PlayerSpeed speedLevel)
	{
		MsgUpSpeedLevel msgUpSpeedLevel = new MsgUpSpeedLevel(speedLevel);
		if (msgUpSpeedLevel != null)
		{
			GameObjectUtil.SendMessageToTagObjects("Player", "OnUpSpeedLevel", msgUpSpeedLevel, SendMessageOptions.DontRequireReceiver);
			GameObjectUtil.SendMessageFindGameObject("PlayerInformation", "OnUpSpeedLevel", msgUpSpeedLevel, SendMessageOptions.DontRequireReceiver);
		}
	}

	private HudTutorial.Id GetHudTutorialID(HudTutorial.Kind kind)
	{
		switch (kind)
		{
		case HudTutorial.Kind.MISSION:
			return (HudTutorial.Id)m_tutorialMissionID;
		case HudTutorial.Kind.MISSION_END:
			return HudTutorial.Id.MISSION_END;
		case HudTutorial.Kind.FEVERBOSS:
			return HudTutorial.Id.FEVERBOSS;
		case HudTutorial.Kind.MAPBOSS:
			return (HudTutorial.Id)(10 + BossTypeUtil.GetIndexNumber(m_bossType));
		case HudTutorial.Kind.EVENTBOSS:
			return (HudTutorial.Id)(13 + BossTypeUtil.GetIndexNumber(m_bossType));
		case HudTutorial.Kind.ITEM:
			return (HudTutorial.Id)m_showItemTutorial;
		case HudTutorial.Kind.CHARA:
			return (HudTutorial.Id)m_showCharaTutorial;
		case HudTutorial.Kind.ACTION:
			return (HudTutorial.Id)m_showActionTutorial;
		case HudTutorial.Kind.QUICK:
			return (HudTutorial.Id)m_showQuickTurorial;
		default:
			return HudTutorial.Id.NONE;
		}
	}

	private void EndTutorial(HudTutorial.Kind kind)
	{
		switch (kind)
		{
		case HudTutorial.Kind.MISSION:
			break;
		case HudTutorial.Kind.EVENTBOSS:
			break;
		case HudTutorial.Kind.ITEM_BUTTON:
			break;
		case HudTutorial.Kind.MISSION_END:
			if (StageItemManager.Instance != null)
			{
				MsgUseEquipItem msg = new MsgUseEquipItem();
				StageItemManager.Instance.OnUseEquipItem(msg);
			}
			if (m_levelInformation != null)
			{
				m_levelInformation.NowTutorial = false;
			}
			break;
		case HudTutorial.Kind.FEVERBOSS:
			SetEndBossTutorialFlag(SystemData.FlagStatus.TUTORIAL_FEVER_BOSS);
			m_showFeverBossTutorial = false;
			break;
		case HudTutorial.Kind.MAPBOSS:
			switch (m_bossType)
			{
			case BossType.MAP1:
				SetEndBossTutorialFlag(SystemData.FlagStatus.TUTORIAL_BOSS_MAP_1);
				break;
			case BossType.MAP2:
				SetEndBossTutorialFlag(SystemData.FlagStatus.TUTORIAL_BOSS_MAP_2);
				break;
			case BossType.MAP3:
				SetEndBossTutorialFlag(SystemData.FlagStatus.TUTORIAL_BOSS_MAP_3);
				break;
			}
			break;
		case HudTutorial.Kind.ITEM:
		{
			if (m_showItemTutorial == -1)
			{
				break;
			}
			ItemType itemType = ItemType.UNKNOWN;
			for (int i = 0; i < 8; i++)
			{
				HudTutorial.Id itemTutorialID = ItemTypeName.GetItemTutorialID((ItemType)i);
				if (itemTutorialID == (HudTutorial.Id)m_showItemTutorial)
				{
					itemType = (ItemType)i;
					break;
				}
			}
			if (itemType != ItemType.UNKNOWN)
			{
				SetEndItemTutorialFlag(ItemTypeName.GetItemTutorialStatus(itemType));
			}
			m_showItemTutorial = -1;
			break;
		}
		case HudTutorial.Kind.CHARA:
			if (m_showCharaTutorial != -1)
			{
				CharaType commonTextCharaName = HudTutorial.GetCommonTextCharaName((HudTutorial.Id)m_showCharaTutorial);
				if (commonTextCharaName != CharaType.UNKNOWN)
				{
					SetEndCharaTutorialFlag(CharaTypeUtil.GetCharacterSaveDataFlagStatus(commonTextCharaName));
				}
				m_showCharaTutorial = -1;
			}
			break;
		case HudTutorial.Kind.ACTION:
			if (m_showActionTutorial != -1)
			{
				SetEndActionTutorialFlag(HudTutorial.GetActionTutorialSaveFlag((HudTutorial.Id)m_showActionTutorial));
				m_showActionTutorial = -1;
			}
			break;
		case HudTutorial.Kind.QUICK:
			if (m_showQuickTurorial != -1)
			{
				SetEndQuickModeTutorialFlag(HudTutorial.GetQuickModeTutorialSaveFlag((HudTutorial.Id)m_showQuickTurorial));
				m_showQuickTurorial = -1;
			}
			break;
		}
	}

	private void SetEndBossTutorialFlag(SystemData.FlagStatus flagStatus)
	{
		if (flagStatus == SystemData.FlagStatus.NONE)
		{
			return;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null && !systemdata.IsFlagStatus(flagStatus))
			{
				systemdata.SetFlagStatus(flagStatus, true);
				m_saveFlag = true;
			}
		}
	}

	private void SetEndItemTutorialFlag(SystemData.ItemTutorialFlagStatus flagStatus)
	{
		if (flagStatus == SystemData.ItemTutorialFlagStatus.NONE)
		{
			return;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null && !systemdata.IsFlagStatus(flagStatus))
			{
				systemdata.SetFlagStatus(flagStatus, true);
				m_saveFlag = true;
			}
		}
	}

	private void SetEndCharaTutorialFlag(SystemData.CharaTutorialFlagStatus flagStatus)
	{
		if (flagStatus == SystemData.CharaTutorialFlagStatus.NONE)
		{
			return;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null && !systemdata.IsFlagStatus(flagStatus))
			{
				systemdata.SetFlagStatus(flagStatus, true);
				m_saveFlag = true;
			}
		}
	}

	private void SetEndActionTutorialFlag(SystemData.ActionTutorialFlagStatus flagStatus)
	{
		if (flagStatus == SystemData.ActionTutorialFlagStatus.NONE)
		{
			return;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null && !systemdata.IsFlagStatus(flagStatus))
			{
				systemdata.SetFlagStatus(flagStatus, true);
				m_saveFlag = true;
			}
		}
	}

	private void SetEndQuickModeTutorialFlag(SystemData.QuickModeTutorialFlagStatus flagStatus)
	{
		if (flagStatus == SystemData.QuickModeTutorialFlagStatus.NONE)
		{
			return;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null && !systemdata.IsFlagStatus(flagStatus))
			{
				systemdata.SetFlagStatus(flagStatus, true);
				m_saveFlag = true;
			}
		}
	}

	private TinyFsmState StateTutorialPause(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			SetTimeScale(0f);
			HudTutorial.Id hudTutorialID = GetHudTutorialID(m_tutorialKind);
			HudTutorial.StartTutorial(hudTutorialID, m_bossType);
			SoundManager.SePausePlaying(true);
			SoundManager.SePlay("sys_pause");
			return TinyFsmState.End();
		}
		case -4:
			SetDefaultTimeScale();
			SoundManager.SePausePlaying(false);
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
			switch (e.GetMessage.ID)
			{
			case 12334:
				EndTutorial(m_tutorialKind);
				HudPlayerChangeCharaButton(true, true);
				ChangeState(new TinyFsmState(StateNormal));
				return TinyFsmState.End();
			case 12349:
				HudTutorial.PushBackKey();
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateItemButtonTutorialPause(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			SetTimeScale(0f);
			SoundManager.SePausePlaying(true);
			SoundManager.SePlay("sys_pause");
			return TinyFsmState.End();
		case -4:
			SetDefaultTimeScale();
			SoundManager.SePausePlaying(false);
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
		{
			int iD = e.GetMessage.ID;
			if (iD == 12331)
			{
				HudPlayerChangeCharaButton(true, true);
				ChangeState(new TinyFsmState(StateNormal));
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateTutorialMissionEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (m_tutorialEndMsg != null)
			{
				if (m_tutorialEndMsg.m_retry)
				{
					HudTutorial.RetryTutorial();
				}
				else
				{
					HudTutorial.SuccessTutorial();
				}
				if (!m_tutorialEndMsg.m_complete)
				{
					m_timer = 1f;
					m_substate = 0;
				}
				else
				{
					m_substate = 4;
				}
			}
			return TinyFsmState.End();
		case -4:
			m_tutorialEndMsg = null;
			if ((bool)StageTutorialManager.Instance)
			{
				GameObjectUtil.SendDelayedMessageToGameObject(StageTutorialManager.Instance.gameObject, "OnMsgTutorialNext", new MsgTutorialNext());
			}
			return TinyFsmState.End();
		case 0:
			switch (m_substate)
			{
			case 0:
				m_timer -= e.GetDeltaTime;
				if (m_timer <= 0f)
				{
					if (!m_tutorialEndMsg.m_complete)
					{
						CameraFade.StartAlphaFade(Color.white, false, 1f);
						m_timer = 1f;
					}
					else
					{
						m_timer = 0f;
					}
					m_substate = 1;
				}
				break;
			case 1:
			{
				m_timer -= e.GetDeltaTime;
				if (!(m_timer <= 0f))
				{
					break;
				}
				if (m_tutorialEndMsg != null)
				{
					if (!m_tutorialEndMsg.m_complete)
					{
						MsgWarpPlayer value = new MsgWarpPlayer(m_tutorialEndMsg.m_pos, PlayerResetRotation, true);
						GameObjectUtil.SendMessageToTagObjects("Player", "OnMsgWarpPlayer", value, SendMessageOptions.DontRequireReceiver);
					}
					if (m_tutorialEndMsg.m_retry)
					{
						bool blink = true;
						if (m_tutorialEndMsg.m_nextEventID <= Tutorial.EventID.JUMP)
						{
							blink = false;
						}
						MsgTutorialResetForRetry value2 = new MsgTutorialResetForRetry(m_tutorialEndMsg.m_pos, PlayerResetRotation, blink);
						GameObjectUtil.SendMessageFindGameObject("StageBlockManager", "OnMsgTutorialResetForRetry", value2, SendMessageOptions.DontRequireReceiver);
						GameObjectUtil.SendMessageFindGameObject("StageTutorialManager", "OnMsgTutorialResetForRetry", value2, SendMessageOptions.DontRequireReceiver);
						GameObjectUtil.SendMessageToTagObjects("MainCamera", "OnMsgTutorialResetForRetry", value2, SendMessageOptions.DontRequireReceiver);
					}
				}
				GameObject[] array = GameObject.FindGameObjectsWithTag("Animal");
				GameObject[] array2 = array;
				foreach (GameObject gameObject in array2)
				{
					gameObject.SendMessage("OnDestroyAnimal", SendMessageOptions.DontRequireReceiver);
				}
				ObjAnimalBase.DestroyAnimalEffect();
				ObjUtil.StopCombo();
				HudTutorial.EndTutorial();
				m_counter = 4;
				m_substate = 2;
				break;
			}
			case 2:
				if (--m_counter < 0)
				{
					MsgPLReleaseHold value3 = new MsgPLReleaseHold();
					GameObjectUtil.SendMessageToTagObjects("Player", "OnMsgPLReleaseHold", value3, SendMessageOptions.DontRequireReceiver);
					MsgChaoStateUtil.SendMsgChaoState(MsgChaoState.State.COME_IN);
					if (!m_tutorialEndMsg.m_complete)
					{
						m_timer = 1f;
						CameraFade.StartAlphaFade(Color.white, true, 1f);
					}
					else
					{
						m_timer = 0f;
					}
					m_substate = 3;
				}
				ObjAnimalBase.DestroyAnimalEffect();
				break;
			case 3:
				m_timer -= e.GetDeltaTime;
				if (m_timer <= 0f)
				{
					m_substate = 4;
					return TinyFsmState.End();
				}
				break;
			case 4:
				HudPlayerChangeCharaButton(true, false);
				ChangeState(new TinyFsmState(StateNormal));
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		case 1:
		{
			int iD = e.GetMessage.ID;
			int num = iD;
			if (num == 12334)
			{
				if (m_substate == 0)
				{
					CameraFade.StartAlphaFade(Color.white, false, 1f);
					m_timer = 1f;
					m_substate = 1;
				}
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGameOver(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_gameResultTimer = 1f;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			m_gameResultTimer -= Time.deltaTime;
			if (m_gameResultTimer < 0f)
			{
				ChangeState(new TinyFsmState(StatePrepareUpdateDatabase));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateCheckContinue(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_gameResultTimer = 1f;
			SetTimeScale(0f);
			ObjUtil.SetHudStockRingEffectOff(true);
			SoundManager.BgmChange(m_mainBgmName);
			SoundManager.BgmCrossFadePlay("bgm_continue", "BGM_jingle");
			if (m_continueWindowObj != null)
			{
				HudContinue component2 = m_continueWindowObj.GetComponent<HudContinue>();
				if (component2 != null)
				{
					if (m_quickMode)
					{
						component2.SetTimeUp(m_quickModeTimeUp);
					}
					m_continueWindowObj.SetActive(true);
					component2.PlayStart();
				}
			}
			return TinyFsmState.End();
		case -4:
			SetDefaultTimeScale();
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
			switch (e.GetMessage.ID)
			{
			case 12352:
			{
				MsgContinueResult msgContinueResult = e.GetMessage as MsgContinueResult;
				if (msgContinueResult != null)
				{
					if (!msgContinueResult.m_result)
					{
						SoundManager.BgmStop();
						ChangeState(new TinyFsmState(StatePrepareUpdateDatabase));
						return TinyFsmState.End();
					}
					ChangeState(new TinyFsmState(StatePrepareContinue));
				}
				break;
			}
			case 12354:
			{
				MsgContinueBackKey msgContinueBackKey = e.GetMessage as MsgContinueBackKey;
				if (msgContinueBackKey != null && m_continueWindowObj != null)
				{
					HudContinue component = m_continueWindowObj.GetComponent<HudContinue>();
					if (component != null)
					{
						component.PushBackKey();
					}
				}
				break;
			}
			case 12329:
			{
				MsgInvincible msgInvincible = e.GetMessage as MsgInvincible;
				if (msgInvincible != null && msgInvincible.m_mode == MsgInvincible.Mode.Start)
				{
					SoundManager.ItemBgmCrossFadePlay("jingle_invincible", "BGM_jingle");
					m_receiveInvincibleMsg = true;
				}
				break;
			}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StatePrepareContinue(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_numEnableContinue--;
			MsgPrepareContinue value = new MsgPrepareContinue(m_bossStage, m_quickModeTimeUp);
			GameObjectUtil.SendMessageFindGameObject("CharacterContainer", "OnMsgPrepareContinue", value, SendMessageOptions.DontRequireReceiver);
			GameObjectUtil.SendMessageToTagObjects("Boss", "OnMsgPrepareContinue", value, SendMessageOptions.DontRequireReceiver);
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnMsgPrepareContinue", value, SendMessageOptions.DontRequireReceiver);
			if (m_quickMode && StageTimeManager.Instance != null)
			{
				StageTimeManager.Instance.ExtendTime(StageTimeManager.ExtendPattern.CONTINUE);
			}
			if (m_continueWindowObj != null)
			{
				m_continueWindowObj.SetActive(false);
			}
			if (m_bossStage && m_levelInformation != null)
			{
				m_levelInformation.DistanceToBossOnStart = m_levelInformation.DistanceOnStage;
			}
			return TinyFsmState.End();
		}
		case -4:
			ObjUtil.SetHudStockRingEffectOff(false);
			return TinyFsmState.End();
		case 0:
			if (m_quickMode)
			{
				m_quickModeTimeUp = false;
				if (StageTimeManager.Instance != null)
				{
					StageTimeManager.Instance.PlayStart();
				}
			}
			if (!m_receiveInvincibleMsg)
			{
				SoundManager.BgmCrossFadeStop(0f, 1f);
			}
			m_receiveInvincibleMsg = false;
			HudPlayerChangeCharaButton(true, true);
			ChangeState(new TinyFsmState(StateNormal));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateQuickModeTimeUp(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_timer = 0.4f;
			SendMessageToHudCaution(HudCaution.Type.STAGE_OUT);
			SoundManager.BgmStop();
			if (m_levelInformation != null)
			{
				m_levelInformation.RequestPause = m_reqPause;
			}
			GameObjectUtil.SendMessageToTagObjects("Player", "OnMsgPLHold", new MsgPLHold(), SendMessageOptions.DontRequireReceiver);
			m_substate = 0;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			m_timer -= e.GetDeltaTime;
			if (m_timer <= 0f)
			{
				GC.Collect();
				Resources.UnloadUnusedAssets();
				GC.Collect();
				ChangeState(new TinyFsmState(StatePrepareUpdateDatabase));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StatePrepareUpdateDatabase(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			ObjUtil.SetHudStockRingEffectOff(true);
			ObjUtil.SendMessageScoreCheck(new StageScoreData(10, (int)m_playerInformation.TotalDistance));
			ObjUtil.SendMessageFinalScoreBeforeResult();
			GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				gameObject.SetActive(false);
			}
			bool flag = false;
			if (m_levelInformation != null)
			{
				flag = m_levelInformation.BossDestroy;
			}
			if (m_gameResult != null)
			{
				m_gameResult.gameObject.SetActive(true);
				SaveDataUtil.ReflctResultsData();
				bool isNoMiss = EnableChaoEgg();
				bool isBossTutorialClear = IsBossTutorialClear();
				m_gameResult.PlayBGStart(m_bossStage ? GameResult.ResultType.BOSS : GameResult.ResultType.NORMAL, isNoMiss, isBossTutorialClear, flag, GetEventResultState());
			}
			SoundManager.BgmStop();
			if (m_bossStage && flag)
			{
				SoundManager.BgmPlay("jingle_sys_bossclear", "BGM_jingle");
			}
			else
			{
				SoundManager.BgmPlay("jingle_sys_clear", "BGM_jingle");
			}
			m_timer = 0.5f;
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_timer > 0f)
			{
				m_timer -= e.GetDeltaTime;
				if (m_timer <= 0f)
				{
					HoldPlayerAndDestroyTerrainOnEnd();
					GameObjectUtil.SendDelayedMessageFindGameObject("HudCockpit", "OnMsgExitStage", new MsgExitStage());
					if (EventManager.Instance.IsRaidBossStage())
					{
						ChangeState(new TinyFsmState(StateUpdateRaidBossState));
					}
					else if (m_quickMode)
					{
						ChangeState(new TinyFsmState(StateUpdateQuickModeDatabase));
					}
					else
					{
						ChangeState(new TinyFsmState(StateUpdateDatabase));
					}
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateUpdateDatabase(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_counter = 0;
			if (m_fromTitle || m_firstTutorial)
			{
				m_retired = true;
			}
			m_exitFromResult = !m_retired;
			if (m_exitFromResult)
			{
				SetMissionResult();
			}
			bool flag = true;
			bool flag2 = true;
			EventResultState eventResultState = GetEventResultState();
			EventManager instance2 = EventManager.Instance;
			if (eventResultState == EventResultState.TIMEUP)
			{
				if (instance2.IsCollectEvent())
				{
					flag = true;
					flag2 = false;
				}
				else
				{
					flag = false;
					flag2 = false;
				}
			}
			else
			{
				flag = true;
				flag2 = true;
			}
			if (!flag || m_firstTutorial)
			{
				StartCoroutine(NotSendPostGameResult());
			}
			else
			{
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				if (loggedInServerInterface != null && SaveDataManager.Instance != null)
				{
					bool chaoEggPresent = EnableChaoEgg();
					int? eventId = null;
					if (flag2 && (instance2.EventStage || instance2.IsCollectEvent()))
					{
						eventId = 0;
						eventId = instance2.Id;
					}
					long? eventValue = null;
					if (eventId.HasValue)
					{
						StageScoreManager instance3 = StageScoreManager.Instance;
						if (instance3 != null)
						{
							eventValue = 0L;
							eventValue = ((!instance2.IsCollectEvent()) ? new long?(instance3.FinalCountData.sp_crystal) : new long?(instance3.CollectEventCount));
						}
					}
					ServerGameResults serverGameResults = new ServerGameResults(!m_exitFromResult, m_tutorialStage, chaoEggPresent, m_bossStage, m_oldNumBossAttack, eventId, eventValue);
					if (serverGameResults != null)
					{
						if (m_postGameResults.m_prevMapInfo != null)
						{
							serverGameResults.SetMapProgress(m_postGameResults.m_prevMapInfo.m_mapState, ref m_postGameResults.m_prevMapInfo.m_pointScore, m_postGameResults.m_existBoss);
						}
						loggedInServerInterface.RequestServerPostGameResults(serverGameResults, base.gameObject);
					}
				}
				else
				{
					m_counter++;
				}
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_counter > 0 && AchievementManager.IsRequestEnd())
			{
				if (m_saveFlag)
				{
					SystemSaveManager instance = SystemSaveManager.Instance;
					if (instance != null)
					{
						instance.SaveSystemData();
					}
				}
				if (m_retired)
				{
					ChangeState(new TinyFsmState(StateSendApolloStageEnd));
				}
				else if (IsRaidBossStateUpdate())
				{
					ChangeState(new TinyFsmState(StateEventDrawRaidBoss));
				}
				else
				{
					ChangeState(new TinyFsmState(StateResult));
				}
			}
			return TinyFsmState.End();
		case 1:
			switch (e.GetMessage.ID)
			{
			case 61449:
				if (ServerInterface.LoggedInServerInterface != null)
				{
					ServerTickerInfo tickerInfo = ServerInterface.TickerInfo;
					if (tickerInfo != null)
					{
						tickerInfo.ExistNewData = true;
					}
				}
				AchievementManager.RequestUpdate();
				if (m_exitFromResult && IsBossTutorialLose())
				{
					SetBossTutorialPresent();
				}
				m_counter++;
				return TinyFsmState.End();
			case 61517:
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateUpdateQuickModeDatabase(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_counter = 0;
			m_exitFromResult = !m_retired;
			if (!m_exitFromResult)
			{
				StartCoroutine(NotSendPostGameResult());
			}
			else
			{
				SetMissionResult();
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				if (loggedInServerInterface != null && SaveDataManager.Instance != null)
				{
					if (StageTimeManager.Instance != null)
					{
						StageTimeManager.Instance.CheckResultTimer();
					}
					ServerQuickModeGameResults serverQuickModeGameResults = new ServerQuickModeGameResults(!m_exitFromResult);
					if (serverQuickModeGameResults != null)
					{
						loggedInServerInterface.RequestServerQuickModePostGameResults(serverQuickModeGameResults, base.gameObject);
					}
				}
				else
				{
					m_counter++;
				}
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_counter > 0 && AchievementManager.IsRequestEnd())
			{
				if (m_saveFlag)
				{
					SystemSaveManager instance = SystemSaveManager.Instance;
					if (instance != null)
					{
						instance.SaveSystemData();
					}
				}
				if (m_retired)
				{
					ChangeState(new TinyFsmState(StateSendApolloStageEnd));
				}
				else
				{
					ChangeState(new TinyFsmState(StateDailyBattleResult));
				}
			}
			return TinyFsmState.End();
		case 1:
			switch (e.GetMessage.ID)
			{
			case 61514:
				if (ServerInterface.LoggedInServerInterface != null)
				{
					ServerTickerInfo tickerInfo = ServerInterface.TickerInfo;
					if (tickerInfo != null)
					{
						tickerInfo.ExistNewData = true;
					}
				}
				AchievementManager.RequestUpdate();
				m_counter++;
				return TinyFsmState.End();
			case 61517:
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateDailyBattleResult(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_counter = 0;
			if (ServerInterface.LoggedInServerInterface != null)
			{
				if (SingletonGameObject<DailyBattleManager>.Instance != null && !m_bossStage)
				{
					SingletonGameObject<DailyBattleManager>.Instance.ResultSetup(DailyBattleResultCallBack);
				}
				else
				{
					m_counter++;
				}
			}
			else
			{
				m_counter++;
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_counter > 0)
			{
				ChangeState(new TinyFsmState(StateResult));
			}
			return TinyFsmState.End();
		case 1:
		{
			int iD = e.GetMessage.ID;
			int num = iD;
			if (num == 61515)
			{
				m_counter++;
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEventDrawRaidBoss(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_counter = 0;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				long score = 0L;
				StageScoreManager instance = StageScoreManager.Instance;
				if (instance != null)
				{
					score = instance.FinalScore;
				}
				loggedInServerInterface.RequestServerDrawRaidBoss(EventManager.Instance.Id, score, base.gameObject);
			}
			else
			{
				m_counter++;
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_counter > 0)
			{
				if (EventUtility.CheckRaidbossEntry())
				{
					ChangeState(new TinyFsmState(StateEventGetEventUserRaidBoss));
				}
				else
				{
					ChangeState(new TinyFsmState(StateResult));
				}
			}
			return TinyFsmState.End();
		case 1:
		{
			int iD = e.GetMessage.ID;
			int num = iD;
			if (num == 61511)
			{
				m_counter++;
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEventGetEventUserRaidBoss(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_counter = 0;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetEventUserRaidBossState(EventManager.Instance.Id, base.gameObject);
			}
			else
			{
				m_counter++;
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_counter > 0)
			{
				ChangeState(new TinyFsmState(StateResult));
			}
			return TinyFsmState.End();
		case 1:
		{
			int iD = e.GetMessage.ID;
			int num = iD;
			if (num == 61506)
			{
				m_counter++;
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateUpdateRaidBossState(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_counter = 0;
			if (m_fromTitle)
			{
				m_retired = true;
			}
			if (GetEventResultState() == EventResultState.TIMEUP)
			{
				StartCoroutine(NotSendEventUpdateGameResult());
			}
			else
			{
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				if (loggedInServerInterface != null)
				{
					long eventValue = 0L;
					long id = RaidBossInfo.currentRaidData.id;
					ServerEventGameResults serverEventGameResults = new ServerEventGameResults(m_retired, EventManager.Instance.Id, eventValue, id);
					if (serverEventGameResults != null)
					{
						DailyMissionData dailyMission = SaveDataManager.Instance.PlayerData.DailyMission;
						if (m_missionManager != null)
						{
							serverEventGameResults.m_dailyMissionComplete = m_missionManager.Completed;
						}
						else
						{
							serverEventGameResults.m_dailyMissionComplete = false;
						}
						serverEventGameResults.m_dailyMissionValue = dailyMission.progress;
						loggedInServerInterface.RequestServerEventUpdateGameResults(serverEventGameResults, base.gameObject);
					}
				}
				else
				{
					m_counter++;
				}
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_counter > 0)
			{
				ChangeState(new TinyFsmState(StateEventPostGameResult));
			}
			return TinyFsmState.End();
		case 1:
		{
			int iD = e.GetMessage.ID;
			int num = iD;
			if (num == 61509)
			{
				m_counter++;
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEventPostGameResult(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_counter = 0;
			if (GetEventResultState() == EventResultState.TIMEUP)
			{
				StartCoroutine(NotSendEventPostGameResult());
			}
			else
			{
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				if (loggedInServerInterface != null)
				{
					int numRaidBossRings = 0;
					StageScoreManager instance = StageScoreManager.Instance;
					if (instance != null)
					{
						numRaidBossRings = (int)instance.FinalCountData.raid_boss_ring;
					}
					loggedInServerInterface.RequestServerEventPostGameResults(EventManager.Instance.Id, numRaidBossRings, base.gameObject);
				}
				else
				{
					m_counter++;
				}
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_counter > 0)
			{
				EventUtility.SetRaidBossFirstBattle();
				ChangeState(new TinyFsmState(StateResult));
			}
			return TinyFsmState.End();
		case 1:
		{
			int iD = e.GetMessage.ID;
			int num = iD;
			if (num == 61510)
			{
				m_counter++;
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateResult(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			EventUtility.UpdateCollectObjectCount();
			if (!m_fromTitle && m_gameResult != null)
			{
				m_gameResult.PlayScoreStart();
				if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
				{
					int raidbossBeatBonus = 0;
					if (m_raidBossBonus != null)
					{
						raidbossBeatBonus = m_raidBossBonus.BeatBonus;
					}
					m_gameResult.SetRaidbossBeatBonus(raidbossBeatBonus);
				}
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_gameResult != null && m_gameResult.IsEndOutAnimation)
			{
				ChangeState(new TinyFsmState(StateSendApolloStageEnd));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSendApolloStageEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			SetTimeScale(1f);
			MsgExitStage msgExitStage = new MsgExitStage();
			GameObjectUtil.SendMessageToTagObjects("StageManager", "OnMsgExitStage", msgExitStage, SendMessageOptions.DontRequireReceiver);
			GameObjectUtil.SendMessageToTagObjects("Player", "OnMsgExitStage", msgExitStage, SendMessageOptions.DontRequireReceiver);
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnMsgExitStage", msgExitStage, SendMessageOptions.DontRequireReceiver);
			if (m_hudCaution != null)
			{
				m_hudCaution.SetMsgExitStage(msgExitStage);
			}
			StopStageEffect();
			ApolloTutorialIndex apolloEndTutorialIndex = GetApolloEndTutorialIndex();
			if (apolloEndTutorialIndex != ApolloTutorialIndex.NONE)
			{
				string[] value = new string[1];
				SendApollo.GetTutorialValue(apolloEndTutorialIndex, ref value);
				m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_END, value);
			}
			else
			{
				m_sendApollo = null;
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_sendApollo != null)
			{
				UnityEngine.Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
			}
			return TinyFsmState.End();
		case 0:
		{
			bool flag = true;
			if (m_sendApollo != null && !m_sendApollo.IsEnd())
			{
				flag = false;
			}
			if (flag)
			{
				if (m_equipItemTutorial && m_exitFromResult)
				{
					HudMenuUtility.SaveSystemDataFlagStatus(SystemData.FlagStatus.TUTORIAL_EQIP_ITEM_END);
				}
				ChangeState(new TinyFsmState(StateEndFadeOut));
			}
			return TinyFsmState.End();
		}
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEndFadeOut(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			BackKeyManager.EndScene();
			SetTimeScale(1f);
			CameraFade.StartAlphaFade(Color.white, false, 1f);
			SoundManager.BgmFadeOut(0.5f);
			m_timer = 1f;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			m_timer -= e.GetDeltaTime;
			if (m_timer < 0f)
			{
				ChangeState(new TinyFsmState(StateEnd));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			SetTimeScale(1f);
			ResetReplaceAtlas();
			RemoveAllResource();
			SoundManager.BgmStop();
			AtlasManager.Instance.ClearAllAtlas();
			GC.Collect();
			Resources.UnloadUnusedAssets();
			GC.Collect();
			if (m_fromTitle || m_firstTutorial)
			{
				if (m_firstTutorial)
				{
					GameModeTitle.FirstTutorialReturned = true;
				}
				Application.LoadLevel(TitleDefine.TitleSceneName);
			}
			else
			{
				CreateResultInfo();
				Application.LoadLevel("MainMenu");
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private bool IsRaidBossStateUpdate()
	{
		if (m_firstTutorial)
		{
			return false;
		}
		if (EventManager.Instance != null && EventManager.Instance.Type == EventManager.EventType.RAID_BOSS && EventManager.Instance.IsChallengeEvent() && !EventManager.Instance.IsRaidBossStage() && !EventManager.Instance.IsEncounterRaidBoss())
		{
			return true;
		}
		return false;
	}

	private void RequestServerStartAct()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface == null)
		{
			m_serverActEnd = true;
			return;
		}
		if (m_fromTitle || m_tutorialStage || m_firstTutorial)
		{
			m_serverActEnd = true;
			return;
		}
		List<ItemType> list = new List<ItemType>();
		foreach (ItemType item in m_useEquippedItem)
		{
			if (item != ItemType.UNKNOWN)
			{
				list.Add(item);
			}
		}
		m_serverActEnd = false;
		List<BoostItemType> list2 = new List<BoostItemType>(m_useBoostItem);
		if (m_quickMode)
		{
			bool tutorial = IsTutorialOnActStart();
			if (IsTutorialItem())
			{
				list.Clear();
				list2.Clear();
			}
			loggedInServerInterface.RequestServerQuickModeStartAct(list, list2, tutorial, base.gameObject);
			return;
		}
		EventManager instance = EventManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		if (instance.IsRaidBossStage())
		{
			loggedInServerInterface.RequestServerEventStartAct(instance.Id, instance.UseRaidbossChallengeCount, RaidBossInfo.currentRaidData.id, list, list2, base.gameObject);
			return;
		}
		List<string> list3 = new List<string>();
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null && socialInterface.IsLoggedIn)
		{
			List<SocialUserData> friendList = socialInterface.FriendList;
			if (friendList != null)
			{
				foreach (SocialUserData item2 in friendList)
				{
					string gameId = item2.CustomData.GameId;
					list3.Add(gameId);
				}
			}
		}
		if (IsTutorialItem())
		{
			list.Clear();
		}
		bool tutorial2 = IsTutorialOnActStart();
		int? eventId = null;
		if (instance.EventStage || instance.IsCollectEvent())
		{
			eventId = 0;
			eventId = instance.Id;
		}
		loggedInServerInterface.RequestServerStartAct(list, list2, list3, tutorial2, eventId, base.gameObject);
	}

	private void RegisterAllResource()
	{
		if (!(ResourceManager.Instance != null))
		{
			return;
		}
		ResourceManager.Instance.AddCategorySceneObjectsAndSetActive(ResourceCategory.TERRAIN_MODEL, m_terrainDataName, GameObject.Find(TerrainXmlData.DataAssetName), false);
		ResourceManager.Instance.AddCategorySceneObjectsAndSetActive(ResourceCategory.STAGE_RESOURCE, m_stageResourceName, GameObject.Find(m_stageResourceObjectName), false);
		ResourceManager.Instance.RemoveNotActiveContainer(ResourceCategory.TERRAIN_MODEL);
		ResourceManager.Instance.RemoveNotActiveContainer(ResourceCategory.STAGE_RESOURCE);
		if ((bool)m_playerInformation)
		{
			if (m_playerInformation.MainCharacterName != null)
			{
				string text = "CharacterModel" + m_playerInformation.MainCharacterName;
				string text2 = "CharacterEffect" + m_playerInformation.MainCharacterName;
				ResourceManager.Instance.AddCategorySceneObjectsAndSetActive(ResourceCategory.CHARA_MODEL, text, GameObject.Find(text), true);
				ResourceManager.Instance.AddCategorySceneObjectsAndSetActive(ResourceCategory.CHARA_EFFECT, text2, GameObject.Find(text2), true);
			}
			if (m_playerInformation.SubCharacterName != null)
			{
				string text3 = "CharacterModel" + m_playerInformation.SubCharacterName;
				string text4 = "CharacterEffect" + m_playerInformation.SubCharacterName;
				ResourceManager.Instance.AddCategorySceneObjectsAndSetActive(ResourceCategory.CHARA_MODEL, text3, GameObject.Find(text3), true);
				ResourceManager.Instance.AddCategorySceneObjectsAndSetActive(ResourceCategory.CHARA_EFFECT, text4, GameObject.Find(text4), true);
			}
		}
		ResourceManager.Instance.RemoveNotActiveContainer(ResourceCategory.CHARA_MODEL);
		ResourceManager.Instance.RemoveNotActiveContainer(ResourceCategory.CHARA_EFFECT);
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			int mainChaoID = instance.PlayerData.MainChaoID;
			int subChaoID = instance.PlayerData.SubChaoID;
			if (mainChaoID >= 0)
			{
				ResourceManager.Instance.AddCategorySceneObjects(ResourceCategory.CHAO_MODEL, null, GameObject.Find("ChaoModel" + mainChaoID.ToString("0000")), false);
			}
			if (subChaoID >= 0 && subChaoID != mainChaoID)
			{
				ResourceManager.Instance.AddCategorySceneObjects(ResourceCategory.CHAO_MODEL, null, GameObject.Find("ChaoModel" + subChaoID.ToString("0000")), false);
			}
		}
		Resources.UnloadUnusedAssets();
		GC.Collect();
		StageAbilityManager.SetupAbilityDataTable();
	}

	private void ResetReplaceAtlas()
	{
		if (AtlasManager.Instance != null)
		{
			AtlasManager.Instance.ResetReplaceAtlas();
		}
	}

	private void RemoveAllResource()
	{
		if (ResourceManager.Instance != null)
		{
			ResourceManager.Instance.RemoveResourcesOnThisScene();
			ResourceManager.Instance.SetContainerActive(ResourceCategory.TERRAIN_MODEL, m_terrainDataName, false);
			ResourceManager.Instance.SetContainerActive(ResourceCategory.STAGE_RESOURCE, m_stageResourceName, false);
			if (m_playerInformation.MainCharacterName != null)
			{
				ResourceManager.Instance.SetContainerActive(ResourceCategory.CHARA_MODEL, "CharacterModel" + m_playerInformation.MainCharacterName, false);
				ResourceManager.Instance.SetContainerActive(ResourceCategory.CHARA_EFFECT, "CharacterEffect" + m_playerInformation.MainCharacterName, false);
			}
			if (m_playerInformation.SubCharacterName != null)
			{
				ResourceManager.Instance.SetContainerActive(ResourceCategory.CHARA_MODEL, "CharacterModel" + m_playerInformation.SubCharacterName, false);
				ResourceManager.Instance.SetContainerActive(ResourceCategory.CHARA_EFFECT, "CharacterEffect" + m_playerInformation.SubCharacterName, false);
			}
		}
		Resources.UnloadUnusedAssets();
	}

	private void CreateStageBlockManager()
	{
		if (m_stageBlockManager == null)
		{
			m_stageBlockManager = GameObject.Find("StageBlockManager");
			StageBlockManager stageBlockManager = null;
			if (m_stageBlockManager == null)
			{
				m_stageBlockManager = new GameObject("StageBlockManager");
				stageBlockManager = m_stageBlockManager.AddComponent<StageBlockManager>();
			}
			else
			{
				stageBlockManager = m_stageBlockManager.GetComponent<StageBlockManager>();
			}
			if (stageBlockManager != null)
			{
				StageBlockManager.CreateInfo cinfo = default(StageBlockManager.CreateInfo);
				cinfo.stageName = m_stageName;
				cinfo.isTerrainManager = m_isCreateTerrainPlacementManager;
				cinfo.isSpawnableManager = m_isCreatespawnableManager;
				cinfo.isPathBlockManager = (m_stagePathManager != null);
				cinfo.pathManager = m_stagePathManager;
				cinfo.showInfo = m_showBlockInfo;
				cinfo.randomBlock = m_randomBlock;
				cinfo.bossMode = m_bossStage;
				cinfo.quickMode = m_quickMode;
				stageBlockManager.Initialize(cinfo);
			}
		}
	}

	public RareEnemyTable GetRareEnemyTable()
	{
		return m_rareEnemyTable;
	}

	public EnemyExtendItemTable GetEnemyExtendItemTable()
	{
		return m_enemyExtendItemTable;
	}

	public BossTable GetBossTable()
	{
		return m_bossTable;
	}

	public BossMap3Table GetBossMap3Table()
	{
		return m_bossMap3Table;
	}

	public ObjectPartTable GetObjectPartTable()
	{
		return m_objectPartTable;
	}

	public void RetryStreamingDataLoad(int retryCount)
	{
		StageStreamingDataLoadRetryProcess process = new StageStreamingDataLoadRetryProcess(base.gameObject, this);
		NetMonitor.Instance.StartMonitor(process, 0f, HudNetworkConnect.DisplayType.ALL);
		StreamingDataLoader.Instance.StartDownload(retryCount, base.gameObject);
	}

	private void SendMessageToHudCaution(HudCaution.Type hudType)
	{
		if (m_hudCaution != null)
		{
			MsgCaution caution = new MsgCaution(hudType);
			m_hudCaution.SetCaution(caution);
		}
	}

	private void SendBossStartMessageToChao()
	{
		GameObjectUtil.SendMessageToTagObjects("Chao", "OnMsgStartBoss", null, SendMessageOptions.DontRequireReceiver);
	}

	private string GetChaoBGMName(int chaoId)
	{
		DataTable.ChaoData chaoData = ChaoTable.GetChaoData(chaoId);
		if (chaoData != null)
		{
			return chaoData.bgmName;
		}
		return string.Empty;
	}

	private string GetCueSheetName(int chaoId)
	{
		DataTable.ChaoData chaoData = ChaoTable.GetChaoData(chaoId);
		if (chaoData != null)
		{
			return chaoData.cueSheetName;
		}
		return string.Empty;
	}

	private void SetMainBGMName()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			string chaoBGMName = GetChaoBGMName(instance.PlayerData.MainChaoID);
			if (!string.IsNullOrEmpty(chaoBGMName))
			{
				m_mainBgmName = chaoBGMName;
				return;
			}
		}
		if (EventManager.Instance != null)
		{
			EventStageData stageData = EventManager.Instance.GetStageData();
			if (stageData != null)
			{
				string text = null;
				if (EventManager.Instance.Type == EventManager.EventType.QUICK)
				{
					if (m_quickMode)
					{
						text = stageData.quickStageBGM;
					}
				}
				else if (EventManager.Instance.Type == EventManager.EventType.BGM)
				{
					if (m_quickMode && stageData.IsQuickModeBGM())
					{
						text = stageData.quickStageBGM;
					}
					else if (m_bossStage && stageData.IsEndlessModeBGM())
					{
						text = stageData.bossStagBGM;
					}
					else if (stageData.IsEndlessModeBGM())
					{
						text = stageData.stageBGM;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					m_mainBgmName = text;
					return;
				}
			}
		}
		if (m_quickMode)
		{
			m_mainBgmName = StageTypeUtil.GetQuickModeBgmName(m_stageName);
			return;
		}
		if (m_bossStage)
		{
			m_mainBgmName = BossTypeUtil.GetBossBgmCueSheetName(m_bossType);
			return;
		}
		if (m_tutorialStage)
		{
			m_mainBgmName = StageTypeUtil.GetBgmName(StageType.W01);
			return;
		}
		string bgmName = StageTypeUtil.GetBgmName(m_stageName);
		if (bgmName != string.Empty)
		{
			m_mainBgmName = bgmName;
		}
	}

	private string GetCueSheetName()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			string cueSheetName = GetCueSheetName(instance.PlayerData.MainChaoID);
			if (!string.IsNullOrEmpty(cueSheetName))
			{
				return cueSheetName;
			}
		}
		if (EventManager.Instance != null)
		{
			EventStageData stageData = EventManager.Instance.GetStageData();
			if (stageData != null)
			{
				string text = null;
				if (EventManager.Instance.Type == EventManager.EventType.QUICK)
				{
					if (m_quickMode)
					{
						text = stageData.quickStageCueSheetName;
					}
				}
				else if (EventManager.Instance.Type == EventManager.EventType.BGM)
				{
					if (m_quickMode && stageData.IsQuickModeBGM())
					{
						text = stageData.quickStageCueSheetName;
					}
					else if (m_bossStage && stageData.IsEndlessModeBGM())
					{
						text = stageData.bossStagCueSheetName;
					}
					else if (stageData.IsEndlessModeBGM())
					{
						text = stageData.stageCueSheetName;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
		}
		if (m_quickMode)
		{
			return StageTypeUtil.GetQuickModeCueSheetName(m_stageName);
		}
		if (m_bossStage)
		{
			return BossTypeUtil.GetBossBgmName(m_bossType);
		}
		if (m_tutorialStage)
		{
			return StageTypeUtil.GetCueSheetName(StageType.W01);
		}
		return StageTypeUtil.GetCueSheetName(m_stageName);
	}

	private void CreateResultInfo()
	{
		if (m_tutorialStage)
		{
			ResultInfo.CreateOptionTutorialResultInfo();
			return;
		}
		ResultInfo resultInfo = ResultInfo.CreateResultInfo();
		if (!(resultInfo != null))
		{
			return;
		}
		ResultData resultData = new ResultData();
		if (resultData == null)
		{
			return;
		}
		resultData.m_stageName = m_stageName;
		resultData.m_validResult = m_exitFromResult;
		resultData.m_fromOptionTutorial = m_tutorialStage;
		resultData.m_bossStage = m_bossStage;
		resultData.m_bossDestroy = (m_levelInformation != null && m_levelInformation.BossDestroy);
		resultData.m_eventStage = m_eventStage;
		resultData.m_quickMode = m_quickMode;
		bool flag = false;
		if (m_missionManager != null)
		{
			flag = m_missionManager.Completed;
		}
		resultData.m_missionComplete = (!m_missonCompleted && flag);
		if (m_resultMapState != null)
		{
			resultData.m_newMapState = new MileageMapState(m_resultMapState);
		}
		if (m_postGameResults.m_prevMapInfo != null && m_postGameResults.m_prevMapInfo.m_mapState != null)
		{
			resultData.m_oldMapState = new MileageMapState(m_postGameResults.m_prevMapInfo.m_mapState);
		}
		if (m_mileageIncentive != null)
		{
			resultData.m_mileageIncentiveList = new List<ServerMileageIncentive>(m_mileageIncentive.Count);
			foreach (ServerMileageIncentive item in m_mileageIncentive)
			{
				resultData.m_mileageIncentiveList.Add(item);
			}
		}
		if (!m_eventStage)
		{
			StageScoreManager instance = StageScoreManager.Instance;
			if (instance != null)
			{
				if (resultData.m_validResult)
				{
					RankingUtil.RankingMode rankingMode = RankingUtil.RankingMode.ENDLESS;
					if (m_quickMode)
					{
						rankingMode = RankingUtil.RankingMode.QUICK;
					}
					long currentScore = instance.FinalScore;
					long nextRankScore = 0L;
					long prveRankScore = 0L;
					int nextRank = 0;
					bool isHighScore = false;
					RankingManager.GetCurrentHighScoreRank(rankingMode, false, ref currentScore, out isHighScore, out nextRankScore, out prveRankScore, out nextRank);
					resultData.m_rivalHighScore = isHighScore;
					if (isHighScore)
					{
						resultData.m_highScore = currentScore;
					}
					resultData.m_totalScore = currentScore;
				}
				else
				{
					resultData.m_rivalHighScore = false;
					resultData.m_highScore = 0L;
					resultData.m_totalScore = 0L;
				}
			}
		}
		if (m_dailyIncentive != null)
		{
			resultData.m_dailyMissionIncentiveList = new List<ServerItemState>(m_dailyIncentive.Count);
			foreach (ServerItemState item2 in m_dailyIncentive)
			{
				resultData.m_dailyMissionIncentiveList.Add(item2);
			}
		}
		resultInfo.SetInfo(resultData);
	}

	private void EnablePause(bool value)
	{
		MsgEnablePause value2 = new MsgEnablePause(value);
		GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnEnablePause", value2, SendMessageOptions.DontRequireReceiver);
	}

	private void HoldPlayerAndDestroyTerrainOnEnd()
	{
		MsgPLHold value = new MsgPLHold();
		GameObjectUtil.SendMessageToTagObjects("Player", "OnMsgPLHold", value, SendMessageOptions.DontRequireReceiver);
		if (m_stageBlockManager != null)
		{
			StageBlockManager component = m_stageBlockManager.GetComponent<StageBlockManager>();
			if (component != null)
			{
				component.DeactivateAll();
			}
		}
		GameObjectUtil.SendDelayedMessageFindGameObject("HudCockpit", "OnMsgExitStage", new MsgExitStage());
	}

	private bool IsBossTutorialLose()
	{
		bool flag = m_levelInformation != null && m_levelInformation.BossDestroy;
		if (m_bossStage && !flag && m_postGameResults.m_prevMapInfo != null)
		{
			MileageMapState mapState = m_postGameResults.m_prevMapInfo.m_mapState;
			if (mapState != null && mapState.m_episode == 1 && mapState.m_chapter == 1)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsBossTutorialPresent()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				return systemdata.IsFlagStatus(SystemData.FlagStatus.TUTORIAL_BOSS_PRESENT);
			}
		}
		return false;
	}

	private void SetBossTutorialPresent()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null && !systemdata.IsFlagStatus(SystemData.FlagStatus.TUTORIAL_BOSS_PRESENT))
			{
				systemdata.SetFlagStatus(SystemData.FlagStatus.TUTORIAL_BOSS_PRESENT, true);
				instance.SaveSystemData();
			}
		}
	}

	private bool IsBossTutorialClear()
	{
		bool flag = m_levelInformation != null && m_levelInformation.BossDestroy;
		if (m_bossStage && flag && m_postGameResults.m_prevMapInfo != null)
		{
			MileageMapState mapState = m_postGameResults.m_prevMapInfo.m_mapState;
			if (mapState != null && mapState.m_episode == 1 && mapState.m_chapter == 1)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsTutorialOnActStart()
	{
		if (m_tutorialStage)
		{
			return true;
		}
		if (m_firstTutorial)
		{
			return true;
		}
		if (m_equipItemTutorial)
		{
			return true;
		}
		return false;
	}

	private bool IsTutorialItem()
	{
		if (m_tutorialStage)
		{
			return true;
		}
		if (m_firstTutorial)
		{
			return true;
		}
		if (m_equipItemTutorial)
		{
			return true;
		}
		return false;
	}

	private bool EnableChaoEgg()
	{
		if (IsBossTutorialClear())
		{
			return true;
		}
		bool flag = false;
		if (m_levelInformation != null)
		{
			flag = m_levelInformation.BossDestroy;
		}
		if (m_bossStage && m_bossNoMissChance && flag)
		{
			return true;
		}
		return false;
	}

	private bool IsEnableContinue()
	{
		if (m_firstTutorial)
		{
			return false;
		}
		if (m_bossStage && m_bossTimeUp)
		{
			return false;
		}
		if (m_numEnableContinue > 0)
		{
			return true;
		}
		return false;
	}

	private ApolloTutorialIndex GetApolloStartTutorialIndex()
	{
		if (m_firstTutorial)
		{
			return ApolloTutorialIndex.START_STEP1;
		}
		if (m_equipItemTutorial)
		{
			return ApolloTutorialIndex.START_STEP5;
		}
		return ApolloTutorialIndex.NONE;
	}

	private ApolloTutorialIndex GetApolloEndTutorialIndex()
	{
		if (m_firstTutorial)
		{
			return ApolloTutorialIndex.START_STEP1;
		}
		if (m_exitFromResult && m_equipItemTutorial)
		{
			return ApolloTutorialIndex.START_STEP5;
		}
		return ApolloTutorialIndex.NONE;
	}

	private void OnApplicationPause(bool flag)
	{
		if (flag)
		{
			OnMsgExternalGamePause(new MsgExternalGamePause(false, false));
		}
	}

	private void OnGetMileageMapState(MsgGetMileageMapState msg)
	{
		if (m_postGameResults.m_prevMapInfo != null)
		{
			msg.m_mileageMapState = m_postGameResults.m_prevMapInfo.m_mapState;
		}
		else
		{
			msg.m_mileageMapState = null;
		}
		msg.m_debugLevel = (uint)m_debugBossLevel;
		msg.m_succeed = true;
	}

	private void SetMissionResult()
	{
		if (m_missionManager != null)
		{
			if (!m_bossStage)
			{
				StageScoreManager instance = StageScoreManager.Instance;
				long distance = instance.FinalCountData.distance;
				MsgMissionEvent msg = new MsgMissionEvent(Mission.EventID.TOTALDISTANCE, distance);
				ObjUtil.SendMessageMission2(msg);
				long finalScore = instance.FinalScore;
				MsgMissionEvent msg2 = new MsgMissionEvent(Mission.EventID.GET_SCORE, finalScore);
				ObjUtil.SendMessageMission2(msg2);
				long ring = instance.FinalCountData.ring;
				MsgMissionEvent msg3 = new MsgMissionEvent(Mission.EventID.GET_RING, ring);
				ObjUtil.SendMessageMission2(msg3);
				long animal = instance.FinalCountData.animal;
				MsgMissionEvent msg4 = new MsgMissionEvent(Mission.EventID.GET_ANIMALS, animal);
				ObjUtil.SendMessageMission2(msg4);
			}
			m_missionManager.SaveMissions();
		}
	}

	private bool IsEventTimeup()
	{
		if (m_eventStage && m_eventManager != null && (m_eventManager.IsSpecialStage() || m_eventManager.IsRaidBossStage()) && !m_eventManager.IsPlayEventForStage())
		{
			Debug.Log("*****Event Timeup!!!!!*****");
			return true;
		}
		return false;
	}

	private EventResultState GetEventResultState()
	{
		if (m_eventStage && m_eventManager != null)
		{
			if (!m_eventManager.IsResultEvent())
			{
				return EventResultState.TIMEUP;
			}
			if (!m_eventManager.IsPlayEventForStage())
			{
				return EventResultState.TIMEUP_RESULT;
			}
		}
		return EventResultState.NONE;
	}

	private void PlayStageEffect()
	{
		if (m_stageEffect == null)
		{
			m_stageEffect = StageEffect.CreateStageEffect(m_stageName);
		}
	}

	private void StopStageEffect()
	{
		if (m_stageEffect != null)
		{
			UnityEngine.Object.Destroy(m_stageEffect.gameObject);
			m_stageEffect = null;
		}
	}

	private void ResetPosStageEffect(bool reset)
	{
		if (m_stageEffect != null)
		{
			m_stageEffect.ResetPos(reset);
		}
	}

	private void HudPlayerChangeCharaButton(bool val, bool pause)
	{
		GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnChangeCharaButton", new MsgChangeCharaButton(val, pause), SendMessageOptions.DontRequireReceiver);
	}

	private void SetChaoAblityTimeScale()
	{
		if (StageAbilityManager.Instance != null && StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.EASY_SPEED))
		{
			int num = (int)StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.EASY_SPEED);
			int num2 = UnityEngine.Random.Range(0, 100);
			if (num >= num2)
			{
				float chaoAbilityExtraValue = StageAbilityManager.Instance.GetChaoAbilityExtraValue(ChaoAbility.EASY_SPEED);
				m_chaoEasyTimeScale = m_defaultTimeScale * (1f - chaoAbilityExtraValue * 0.01f);
			}
		}
	}

	private void SetDefaultTimeScale()
	{
		float num = m_defaultTimeScale;
		if (StageAbilityManager.Instance != null)
		{
			num = StageAbilityManager.Instance.GetTeamAbliltyTimeScale(num);
		}
		if (m_chaoEasyTimeScale < num)
		{
			num = m_chaoEasyTimeScale;
		}
		SetTimeScale(num);
	}

	private void SetTimeScale(float timeScale)
	{
		Time.timeScale = timeScale;
	}

	private void DrawingInvalidExtreme()
	{
		if (!(m_levelInformation != null) || !m_levelInformation.Extreme)
		{
			return;
		}
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (!(instance != null) || !instance.HasChaoAbility(ChaoAbility.INVALIDI_EXTREME_STAGE))
		{
			return;
		}
		int num = (int)instance.GetChaoAbilityExtraValue(ChaoAbility.INVALIDI_EXTREME_STAGE);
		if (m_invalidExtremeCount < num)
		{
			float chaoAbilityValue = instance.GetChaoAbilityValue(ChaoAbility.INVALIDI_EXTREME_STAGE);
			float num2 = UnityEngine.Random.Range(0f, 99.9f);
			if (chaoAbilityValue >= num2)
			{
				m_levelInformation.InvalidExtreme = true;
				m_invalidExtremeCount++;
				m_invalidExtremeFlag = true;
			}
		}
	}

	private void StreamingDataLoad_Succeed()
	{
		NetMonitor.Instance.EndMonitorForward(new MsgAssetBundleResponseSucceed(null, null), null, null);
		NetMonitor.Instance.EndMonitorBackward();
	}

	private void StreamingDataLoad_Failed()
	{
		NetMonitor.Instance.EndMonitorForward(new MsgAssetBundleResponseFailedMonitor(), null, null);
		NetMonitor.Instance.EndMonitorBackward();
	}
}
