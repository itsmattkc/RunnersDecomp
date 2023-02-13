using App;
using App.Utility;
using DataTable;
using Message;
using SaveData;
using System;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	public enum SequenceState
	{
		Init,
		RequestDailyBattle,
		RequestChaoWheelOption,
		RequestDayCrossWatcher,
		RequestMsgList,
		RequestNoticeInfo,
		Load,
		LoadAtlas,
		StartMessage,
		RankingWait,
		FadeIn,
		Main,
		MainConnect,
		TickerCommunicate,
		MsgBoxCommunicate,
		SchemeCommunicate,
		ResultSchemeCommunicate,
		DayCrossCommunicate,
		LoadMileageXml,
		LoadNextMileageXml,
		LoadMileageTexture,
		MileageReward,
		WaitFadeIfNotEndFade,
		Episode,
		DailyMissionWindow,
		LoginBonusWindow,
		LoginBonusWindowDisplay,
		FirstLoginBonusWindow,
		FirstLoginBonusWindowDisplay,
		PlayButton,
		ChallengeDisplyWindow,
		RecommendReview,
		InformationWindow,
		InformationWindowCreate,
		EventRankingResultWindow,
		RankingResultLeagueWindow,
		DisplayInformaon,
		DailyBattle,
		DailyBattleRewardWindow,
		DailyBattleRewardWindowDisplay,
		CharaSelect,
		ChaoSelect,
		Option,
		Ranking,
		Infomation,
		Roulette,
		Shop,
		PresentBox,
		PlayItem,
		TutorialMenuRoulette,
		TutorialCharaLevelUpMenuStart,
		TutorialCharaLevelUpMenuMoveChara,
		BestRecordCheckEnableFeed,
		BestRecordAskFeed,
		BestRecordFeed,
		QuickModeRankUp,
		QuickModeRankUpDisplay,
		LoadEventResource,
		LoadEventTextureResource,
		EventDisplayProduction,
		UserNameSetting,
		UserNameSettingDisplay,
		AgeVerification,
		AgeVerificationDisplay,
		CheckStage,
		CautionStage,
		Stage,
		VersionChangeWindow,
		EventResourceChangeWindow,
		Title,
		CheckBackTitle,
		FadeOut,
		End,
		NUM
	}

	private enum EventSignal
	{
		SERVER_GET_EVENT_REWARD_END = 100,
		SERVER_GET_EVENT_STATE_END,
		TITLE_BACK
	}

	private enum Flags
	{
		FadeIn,
		FadeOut,
		ForceBackMainMenu,
		GoStage,
		GoSpecialStage,
		GoRaidBoss,
		MileageNextMapLoad,
		MileageProduction,
		LoginRanking,
		EndMileageMapProduction,
		ReceiveMileageState,
		RecieveDailyChallengeInfo,
		MileageReward,
		EventLoadAgain,
		OptionResourceLoaded,
		OptionTutorialStage,
		DailyChallenge,
		LoginBonus,
		FirstLoginBonus,
		MsgBox,
		EndMainConnect,
		REQUEST_CHECK_SCHEME,
		EventWait,
		EventConnetctBeforeLoadMenu,
		TuorialWindow,
		QuickRankingResult,
		QuickRankingUpProduction,
		BestRecordFeed,
		FromMileage,
		NUM
	}

	public enum ProgressBarLeaveState
	{
		IDLE = -1,
		StateInit,
		StateRequestDayCrossWatcher,
		StateRequestDailyBattle,
		StateRequestChaoWheelOption,
		StateRequestMsgList,
		StateRequestNoticeInfo,
		StateLoad,
		StateLoadAtlas,
		StateStartMessage,
		StateRankingWait,
		StateEventRankingWait,
		NUM
	}

	private enum CautionType
	{
		NON,
		CHALLENGE_COUNT,
		NEW_EVENT,
		END_EVENT,
		EVENT_LAST_TIME
	}

	private enum PressedButtonType
	{
		NONE = -1,
		NEXT_STATE,
		GOTO_SHOP,
		BACK,
		CANCEL,
		NUM
	}

	private enum CollisionType
	{
		ALERT_BUTTON_ON,
		ALERT_BUTTON_OFF,
		NON
	}

	private enum ResType
	{
		EVENT_COMMON,
		EVENT_MENU,
		MENU_TOP_TEXTURE,
		NUM
	}

	private enum Communicate
	{
		TICKER,
		TICKER_END,
		MSGBOX,
		MSGBOX_END,
		SCHEME,
		SCHEME_FAILD,
		VERSION,
		DAY_CROSS,
		DAY_CROSS_END,
		LOAD_EVENT_RESOURCE
	}

	private enum CallBack
	{
		DAY_CROSS,
		DAY_CROSS_SERVER_CONNECT,
		DAILY_MISSION_CHALLENGE_END,
		DAILY_MISSION_CHALLENGE_END_SERVER_CONNECT,
		DAILY_MISSION_CHALLENGE_INFO_END,
		DAILY_MISSION_CHALLENGE_INFO_END_SERVER_CONNECT,
		LOGINBONUS_UPDATE_SERVER_CONNECT,
		LOGINBONUS_UPDATE_END
	}

	private const string STAGE_MODE_INFO = "StageInfo";

	public bool m_debugInfo;

	private TinyFsmBehavior m_fsm_behavior;

	private GameObject m_stage_info_obj;

	private MainMenuWindow m_main_menu_window;

	private GameObject m_scene_loader_obj;

	private List<int> m_request_face_list = new List<int>();

	private List<int> m_request_bg_list = new List<int>();

	private ButtonEventResourceLoader m_buttonEventResourceLoader;

	private ServerMileageMapState m_mileage_map_state = new ServerMileageMapState();

	private ServerMileageMapState m_prev_mileage_map_state = new ServerMileageMapState();

	private ResultData m_stageResultData;

	private int m_eventResourceId = -1;

	private SendApollo m_sendApollo;

	private ButtonEvent m_buttonEvent;

	private bool m_bossChallenge;

	private Bitset32 m_flags;

	private HudProgressBar m_progressBar;

	private CautionType m_cautionType;

	private PressedButtonType m_pressedButtonType = PressedButtonType.NONE;

	private readonly CollisionType[] COLLISION_TYPE_TABLE = new CollisionType[73]
	{
		CollisionType.NON,
		CollisionType.NON,
		CollisionType.NON,
		CollisionType.NON,
		CollisionType.NON,
		CollisionType.NON,
		CollisionType.NON,
		CollisionType.NON,
		CollisionType.NON,
		CollisionType.NON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_OFF,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON,
		CollisionType.ALERT_BUTTON_ON
	};

	private bool m_alertBtnFlag;

	private bool m_eventConnectSkip;

	private EasySnsFeed m_easySnsFeed;

	private DailyBattleRewardWindow m_dailyBattleRewardWindow;

	private DailyWindowUI m_dailyWindowUI;

	private bool m_episodeLoaded;

	private ButtonInfoTable.ButtonType m_ButtonOfNextMenu;

	private RankingResultLeague m_rankingResultLeagueWindow;

	private RankingResultWorldRanking m_eventRankingResult;

	private NetNoticeItem m_currentResultInfo;

	private NetNoticeItem m_eventRankingResultInfo;

	private List<NetNoticeItem> m_rankingResultList = new List<NetNoticeItem>();

	private ServerInformationWindow m_serverInformationWindow;

	private bool m_is_end_notice_connect;

	private bool m_connected;

	private FirstLaunchInviteFriend m_fristLaunchInviteFriend;

	private bool m_startLauncherInviteFriendFlag;

	private bool m_eventSpecficText;

	private List<ResourceSceneLoader.ResourceInfo> m_loadInfo = new List<ResourceSceneLoader.ResourceInfo>
	{
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.EVENT_RESOURCE, "EventResourceCommon", true, false, true, "EventResourceCommon"),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.EVENT_RESOURCE, "EventResourceMenu", true, false, false, "EventResourceMenu"),
		new ResourceSceneLoader.ResourceInfo(ResourceCategory.UI, string.Empty, true, false, false)
	};

	private LoginBonusWindowUI m_LoginWindowUI;

	private Bitset32 m_communicateFlag;

	private Bitset32 m_callBackFlag;

	private string m_atomCampain = string.Empty;

	private string m_atomSerial = string.Empty;

	private string m_atomInvalidTextId = string.Empty;

	private static int m_debug;

	private FirstLaunchUserName m_userNameSetting;

	private AgeVerification m_ageVerification;

	private FirstLaunchRecommendReview m_fristLaunchRecommendReview;

	private bool m_startLauncherRecommendReviewFlag;

	private bool m_rankingCallBack;

	private bool m_eventRankingCallBack;

	public bool BossChallenge
	{
		get
		{
			return m_bossChallenge;
		}
		set
		{
			m_bossChallenge = value;
		}
	}

	private void Awake()
	{
		Application.targetFrameRate = SystemSettings.TargetFrameRate;
		float fadeDuration = 0.3f;
		float fadeDelay = 0f;
		bool isFadeIn = true;
		CameraFade.StartAlphaFade(Color.black, isFadeIn, fadeDuration, fadeDelay, OnFinishedFadeOutCallback);
	}

	private void Start()
	{
		TimeProfiler.EndCountTime("Title-NextScene");
		HudUtility.SetInvalidNGUIMitiTouch();
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "ConnectAlertMaskUI");
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
			}
		}
		ConnectAlertMaskUI.StartScreen();
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				SoundManager.BgmVolume = (float)systemdata.bgmVolume / 100f;
				SoundManager.SeVolume = (float)systemdata.seVolume / 100f;
			}
		}
		MenuPlayerSetUtil.ResetMarkCharaPage();
		SoundManager.AddMainMenuCommonCueSheet();
		HudMenuUtility.StartMainMenuBGM();
		GC.Collect();
		m_flags.Reset();
		m_flags.Set(25, true);
		m_flags.Set(26, true);
		m_flags.Set(27, true);
		GameObject gameObject3 = GameObject.Find("AllocationStatus");
		if (gameObject3 != null)
		{
			gameObject3.SetActive(false);
			UnityEngine.Object.Destroy(gameObject3);
		}
		m_stage_info_obj = GameObject.Find("StageInfo");
		if (m_stage_info_obj == null)
		{
			m_stage_info_obj = new GameObject();
			if (m_stage_info_obj != null)
			{
				m_stage_info_obj.name = "StageInfo";
				UnityEngine.Object.DontDestroyOnLoad(m_stage_info_obj);
				m_stage_info_obj.AddComponent("StageInfo");
			}
		}
		GameObject mainMenuUIObject = HudMenuUtility.GetMainMenuUIObject();
		GameObject mainMenuCmnUIObject = HudMenuUtility.GetMainMenuCmnUIObject();
		if (mainMenuUIObject != null)
		{
			mainMenuUIObject.SetActive(false);
		}
		if (mainMenuCmnUIObject != null)
		{
			mainMenuCmnUIObject.SetActive(false);
		}
		GameObject gameObject4 = GameObject.Find("MainMenuWindow");
		if (gameObject4 != null)
		{
			m_main_menu_window = gameObject4.GetComponent<MainMenuWindow>();
		}
		GameObject gameObject5 = GameObject.Find("MainMenuButtonEvent");
		if (gameObject5 != null)
		{
			m_buttonEvent = gameObject5.GetComponent<ButtonEvent>();
		}
		if (EventManager.Instance != null && EventManager.Instance.IsStandby())
		{
			m_flags.Set(22, true);
		}
		BackKeyManager.AddTutorialEventCallBack(base.gameObject);
		m_fsm_behavior = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (m_fsm_behavior != null)
		{
			TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
			description.initState = new TinyFsmState(StateInit);
			m_fsm_behavior.SetUp(description);
		}
		GameObject gameObject6 = GameObject.Find("ConnectAlertMaskUI");
		if (gameObject6 != null)
		{
			m_progressBar = GameObjectUtil.FindChildGameObjectComponent<HudProgressBar>(gameObject6, "Pgb_loading");
			if (m_progressBar != null)
			{
				m_progressBar.SetUp(11);
			}
		}
	}

	private void OnDestroy()
	{
		if ((bool)m_fsm_behavior)
		{
			m_fsm_behavior.ShutDown();
			m_fsm_behavior = null;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		m_flags.Set(21, true);
	}

	private void ChangeState(TinyFsmState nextState, SequenceState sequenceState)
	{
		if (m_fsm_behavior.ChangeState(nextState))
		{
			SetCollisionState(COLLISION_TYPE_TABLE[(int)sequenceState]);
		}
		DebugInfoDraw("MainMenu SequenceState = " + sequenceState);
	}

	private void OnClickPlatformBackButtonTutorialEvent()
	{
		if (m_fsm_behavior != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(102);
			m_fsm_behavior.Dispatch(signal);
		}
	}

	private void OnMsgReceive(MsgMenuSequence message)
	{
		DebugInfoDraw("MainMenu OnMsgReceive " + message.Sequenece);
		if (m_fsm_behavior != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateMessage(message);
			m_fsm_behavior.Dispatch(signal);
		}
	}

	private void SetEventStage(bool flag)
	{
		if (EventManager.Instance != null)
		{
			EventManager.Instance.EventStage = flag;
		}
	}

	private void DebugInfoDraw(string msg)
	{
	}

	private void ServerEquipChao_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		NetUtil.SyncSaveDataAndDataBase(msg.m_playerState);
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private void ServerEquipChao_Failed(MsgServerConnctFailed msg)
	{
	}

	private void SetCollisionState(CollisionType enterCollisionType)
	{
		switch (enterCollisionType)
		{
		case CollisionType.ALERT_BUTTON_ON:
			SetConnectAlertButtonCollision(true);
			break;
		case CollisionType.ALERT_BUTTON_OFF:
			SetConnectAlertButtonCollision(false);
			break;
		}
	}

	private void SetConnectAlertButtonCollision(bool on)
	{
		if (on)
		{
			if (!m_alertBtnFlag)
			{
				HudMenuUtility.SetConnectAlertMenuButtonUI(true);
				BackKeyManager.MenuSequenceTransitionFlag = true;
				m_alertBtnFlag = true;
			}
		}
		else if (m_alertBtnFlag)
		{
			HudMenuUtility.SetConnectAlertMenuButtonUI(false);
			BackKeyManager.MenuSequenceTransitionFlag = false;
			m_alertBtnFlag = false;
		}
	}

	private CautionType GetCautionType()
	{
		if (StageModeManager.Instance != null && EventManager.Instance != null)
		{
			if (StageModeManager.Instance.IsQuickMode())
			{
				if (EventManager.Instance.IsStandby())
				{
					if (EventManager.Instance.IsInEvent())
					{
						return CautionType.NEW_EVENT;
					}
				}
				else if (EventManager.Instance.Type != EventManager.EventType.UNKNOWN && !EventManager.Instance.IsInEvent())
				{
					return CautionType.END_EVENT;
				}
			}
			else if (EventManager.Instance.IsStandby())
			{
				if ((EventManager.Instance.StandbyType == EventManager.EventType.COLLECT_OBJECT || EventManager.Instance.StandbyType == EventManager.EventType.BGM) && EventManager.Instance.IsInEvent())
				{
					return CautionType.NEW_EVENT;
				}
			}
			else if ((EventManager.Instance.Type == EventManager.EventType.COLLECT_OBJECT || EventManager.Instance.Type == EventManager.EventType.BGM) && !EventManager.Instance.IsInEvent())
			{
				return CautionType.END_EVENT;
			}
		}
		if (SaveDataManager.Instance != null && SaveDataManager.Instance.PlayerData.ChallengeCount == 0)
		{
			return CautionType.CHALLENGE_COUNT;
		}
		return CautionType.NON;
	}

	private void CreateStageCautionWindow()
	{
		m_pressedButtonType = PressedButtonType.NONE;
		switch (m_cautionType)
		{
		case CautionType.CHALLENGE_COUNT:
			m_main_menu_window.CreateWindow(MainMenuWindow.WindowType.ChallengeGoShop, delegate(bool yesButtonClicked)
			{
				m_pressedButtonType = (yesButtonClicked ? PressedButtonType.GOTO_SHOP : PressedButtonType.CANCEL);
			});
			break;
		case CautionType.NEW_EVENT:
			m_main_menu_window.CreateWindow(MainMenuWindow.WindowType.EventStart, delegate
			{
				m_pressedButtonType = PressedButtonType.BACK;
			});
			break;
		case CautionType.END_EVENT:
			m_main_menu_window.CreateWindow(MainMenuWindow.WindowType.EventOutOfTime, delegate
			{
				m_pressedButtonType = PressedButtonType.BACK;
			});
			break;
		case CautionType.EVENT_LAST_TIME:
			m_main_menu_window.CreateWindow(MainMenuWindow.WindowType.EventLastPlay, delegate
			{
				m_pressedButtonType = PressedButtonType.NEXT_STATE;
			});
			break;
		}
	}

	private void CreateTitleBackWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "BackTitle";
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		info.caption = TextUtility.GetCommonText("MainMenu", "back_title_caption");
		info.message = TextUtility.GetCommonText("MainMenu", "back_title_text");
		GeneralWindow.Create(info);
	}

	private void CheckTutoralWindow()
	{
		if (GeneralWindow.IsCreated("BackTitle") && GeneralWindow.IsButtonPressed)
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				ChangeState(new TinyFsmState(StateTitle), SequenceState.FadeOut);
			}
			GeneralWindow.Close();
		}
	}

	private TinyFsmState MenuStateLoadEventResource(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CeateSceneLoader();
			if (EventManager.Instance != null)
			{
				if (EventManager.Instance.Type == EventManager.EventType.QUICK || EventManager.Instance.Type == EventManager.EventType.BGM)
				{
					SetLoadEventResource();
					if (m_flags.Test(22) && AtlasManager.Instance != null)
					{
						AtlasManager.Instance.StartLoadAtlasForEventMenu();
					}
				}
				else if (EventManager.Instance.Type == EventManager.EventType.UNKNOWN)
				{
					SetLoadTopMenuTexture();
				}
			}
			return TinyFsmState.End();
		case -4:
			UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
			m_buttonEventResourceLoader = null;
			return TinyFsmState.End();
		case 0:
		{
			bool flag = true;
			if (m_buttonEventResourceLoader != null)
			{
				flag = m_buttonEventResourceLoader.IsLoaded;
			}
			if (flag && CheckSceneLoad())
			{
				DestroySceneLoader();
				SetEventResources();
				if (EventManager.Instance != null)
				{
					EventManager.EventType type = EventManager.Instance.Type;
					if (type == EventManager.EventType.QUICK)
					{
						if (m_flags.Test(22))
						{
							m_flags.Set(22, false);
							StageModeManager.Instance.DrawQuickStageIndex();
							ChangeState(new TinyFsmState(MenuStateLoadEventTextureResource), SequenceState.LoadEventTextureResource);
						}
						else
						{
							ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
						}
					}
					else
					{
						ChangeState(new TinyFsmState(MenuStateLoadEventTextureResource), SequenceState.LoadEventTextureResource);
					}
				}
			}
			return TinyFsmState.End();
		}
		case 1:
			return TinyFsmState.End();
		case 100:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState MenuStateLoadEventTextureResource(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CeateSceneLoader();
			SetLoadTopMenuTexture();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (CheckSceneLoad())
			{
				DestroySceneLoader();
				ChangeState(new TinyFsmState(MenuStateEventDisplayProduction), SequenceState.EventDisplayProduction);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		case 100:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState MenuStateEventDisplayProduction(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			bool isFadeIn2 = false;
			float fadeDuration2 = 1f;
			float fadeDelay2 = 0f;
			m_flags.Set(1, false);
			CameraFade.StartAlphaFade(Color.black, isFadeIn2, fadeDuration2, fadeDelay2, OnFinishedFadeOutCallback);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_flags.Test(1))
			{
				m_flags.Set(1, false);
				if (EventManager.Instance != null)
				{
					EventManager.EventType type = EventManager.Instance.Type;
					if (type == EventManager.EventType.QUICK)
					{
						GameObjectUtil.SendMessageFindGameObject("MainMenuUI4", "OnUpdateQuickModeData", null, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						GameObjectUtil.SendMessageFindGameObject("MainMenuUI4", "OnUpdateQuickModeData", null, SendMessageOptions.DontRequireReceiver);
					}
				}
				float fadeDuration = 2f;
				float fadeDelay = 0f;
				bool isFadeIn = true;
				CameraFade.StartAlphaFade(Color.black, isFadeIn, fadeDuration, fadeDelay, null);
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		case 100:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void ServerGetEventReward_Succeeded(MsgGetEventRewardSucceed msg)
	{
		if (m_fsm_behavior != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
			m_fsm_behavior.Dispatch(signal);
		}
	}

	private void ServerGetEventState_Succeeded(MsgGetEventStateSucceed msg)
	{
		if (m_fsm_behavior != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(101);
			m_fsm_behavior.Dispatch(signal);
		}
	}

	private TinyFsmState StateBestRecordCheckEnableFeed(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			bool flag = false;
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				SystemData systemdata = instance.GetSystemdata();
				if (systemdata != null)
				{
					flag = systemdata.IsFacebookWindow();
				}
			}
			if (flag)
			{
				ChangeState(new TinyFsmState(StateBestRecordAskFeed), SequenceState.BestRecordAskFeed);
			}
			else
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		}
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateBestRecordAskFeed(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.buttonType = GeneralWindow.ButtonType.TweetCancel;
			info.caption = MileageMapUtility.GetText("gw_highscore_caption");
			info.message = MileageMapUtility.GetText("gw_highscore_text");
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsButtonPressed)
			{
				if (GeneralWindow.IsYesButtonPressed)
				{
					ChangeState(new TinyFsmState(StateBestRecordFeed), SequenceState.BestRecordFeed);
				}
				else
				{
					SystemSaveManager instance = SystemSaveManager.Instance;
					if (instance != null)
					{
						SystemData systemdata = instance.GetSystemdata();
						if (systemdata != null)
						{
							systemdata.SetFacebookWindow(false);
							instance.SaveSystemData();
						}
					}
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
				GeneralWindow.Close();
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateBestRecordFeed(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			long highScore = m_stageResultData.m_highScore;
			m_easySnsFeed = new EasySnsFeed(base.gameObject, "Camera/menu_Anim/MainMenuUI4/Anchor_5_MC", MileageMapUtility.GetText("feed_highscore_caption"), MileageMapUtility.GetText("feed_highscore_text", new Dictionary<string, string>
			{
				{
					"{HIGHSCORE}",
					highScore.ToString()
				}
			}));
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			EasySnsFeed.Result result = m_easySnsFeed.Update();
			if (result == EasySnsFeed.Result.COMPLETED || result == EasySnsFeed.Result.FAILED)
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		}
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateChaoSelect(TinyFsmEvent fsm_event)
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
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null)
			{
				if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.MAIN)
				{
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.CHAO_ROULETTE)
				{
					ChangeState(new TinyFsmState(StateRoulette), SequenceState.Roulette);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.ROULETTE)
				{
					ChangeState(new TinyFsmState(StateRoulette), SequenceState.Roulette);
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateMainCharaSelect(TinyFsmEvent fsm_event)
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
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null)
			{
				if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.MAIN)
				{
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.CHAO_ROULETTE)
				{
					ChangeState(new TinyFsmState(StateRoulette), SequenceState.Roulette);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.ROULETTE)
				{
					ChangeState(new TinyFsmState(StateRoulette), SequenceState.Roulette);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.CHAO)
				{
					ChangeState(new TinyFsmState(StateChaoSelect), SequenceState.ChaoSelect);
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateDailyBattle(TinyFsmEvent fsm_event)
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
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null && msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.MAIN)
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateDailyBattleRewardWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
			m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync("DailybattleRewardWindowUI", ResourceLoadEndCallback);
			return TinyFsmState.End();
		case -4:
			UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
			m_buttonEventResourceLoader = null;
			return TinyFsmState.End();
		case 0:
			if (m_buttonEventResourceLoader.IsLoaded)
			{
				ChangeState(new TinyFsmState(StateDailyBattleRewardWindowDisplay), SequenceState.DailyBattleRewardWindowDisplay);
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateDailyBattleRewardWindowDisplay(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (SingletonGameObject<DailyBattleManager>.Instance != null)
			{
				m_dailyBattleRewardWindow = DailyBattleRewardWindow.Open(SingletonGameObject<DailyBattleManager>.Instance.GetRewardDataPair());
			}
			return TinyFsmState.End();
		case -4:
			m_dailyBattleRewardWindow = null;
			return TinyFsmState.End();
		case 0:
			if (m_dailyBattleRewardWindow != null && m_dailyBattleRewardWindow.IsEnd)
			{
				DailyBattleManager instance = SingletonGameObject<DailyBattleManager>.Instance;
				if (instance != null)
				{
					instance.RestReward();
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateInfomation(TinyFsmEvent fsm_event)
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
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null)
			{
				if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.MAIN)
				{
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.ROULETTE)
				{
					ChangeState(new TinyFsmState(StateRoulette), SequenceState.Roulette);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.CHAO_ROULETTE)
				{
					ChangeState(new TinyFsmState(StateRoulette), SequenceState.Roulette);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.SHOP)
				{
					ChangeState(new TinyFsmState(StateShop), SequenceState.Shop);
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateDailyMissionWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_flags.Set(16, false);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_dailyWindowUI == null)
			{
				GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
				if (menuAnimUIObject != null)
				{
					m_dailyWindowUI = GameObjectUtil.FindChildGameObjectComponent<DailyWindowUI>(menuAnimUIObject, "DailyWindowUI");
					if (m_dailyWindowUI != null)
					{
						m_dailyWindowUI.gameObject.SetActive(true);
						m_dailyWindowUI.PlayStart();
					}
				}
			}
			else if (m_dailyWindowUI.IsEnd)
			{
				m_dailyWindowUI = null;
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.DAILY_CHALLENGE_BACK);
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEnd(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			ResetRingCountOffset();
			SetEventManagerParam();
			ChaoTextureManager.Instance.RemoveChaoTextureForMainMenuEnd();
			AtlasManager.Instance.ClearAllAtlas();
			CeateSceneLoader();
			if (StageModeManager.Instance.StageMode == StageModeManager.Mode.ENDLESS)
			{
				LoadMileageText();
				m_request_face_list.Clear();
				m_request_bg_list.Clear();
				EntryMileageTexturesList();
				LoadMileageTextures();
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			CPlusPlusLink instance = CPlusPlusLink.Instance;
			if (instance != null)
			{
				instance.BeforeGameCheatCheck();
			}
			if (CheckSceneLoad())
			{
				SetupMileageText();
				TransTextureObj();
				if (m_flags.Test(4))
				{
					DestroySceneLoader();
					EventUtility.SetDontDestroyLoadingFaceTexture();
					EndSpecialStageProcessing();
					ChangeState(new TinyFsmState(StateIdle), SequenceState.End);
				}
				else if (m_flags.Test(5))
				{
					DestroySceneLoader();
					EventUtility.SetDontDestroyLoadingFaceTexture();
					EndRaidBossProcessing();
					ChangeState(new TinyFsmState(StateIdle), SequenceState.End);
				}
				else if (m_flags.Test(3))
				{
					EndStageProcessing();
					ChangeState(new TinyFsmState(StateIdle), SequenceState.End);
				}
				else
				{
					EndTitleProcessing();
					ChangeState(new TinyFsmState(StateIdle), SequenceState.End);
				}
			}
			return TinyFsmState.End();
		}
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
		default:
			return TinyFsmState.End();
		}
	}

	private void EndStageProcessing()
	{
		if (m_flags.Test(15))
		{
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				instance.PlayerData.MainChaoID = -1;
				instance.PlayerData.SubChaoID = -1;
				instance.PlayerData.MainChara = CharaType.SONIC;
			}
			SetTutorialStageInfo();
			SetTutorialLoadingInfo();
		}
		else
		{
			SetStageInfo();
			SetLoadingInfo();
			DestroyMileageInfo();
		}
		PrepareForSceneMove();
		TimeProfiler.StartCountTime("MainMenu-GameModeStage");
		Application.LoadLevel("s_playingstage");
	}

	private void EndSpecialStageProcessing()
	{
		SetSpecialStageInfo();
		SetEventLoadingInfo();
		PrepareForSceneMove();
		TimeProfiler.StartCountTime("MainMenu-GameModeStage");
		Application.LoadLevel("s_playingstage");
	}

	private void EndRaidBossProcessing()
	{
		SetRaidBossInfo();
		SetEventLoadingInfo();
		PrepareForSceneMove();
		TimeProfiler.StartCountTime("MainMenu-GameModeStage");
		Application.LoadLevel("s_playingstage");
	}

	private void EndTitleProcessing()
	{
		if (m_stage_info_obj != null)
		{
			UnityEngine.Object.Destroy(m_stage_info_obj);
		}
		if (MileageMapDataManager.Instance != null)
		{
			UnityEngine.Object.Destroy(MileageMapDataManager.Instance.gameObject);
		}
		PrepareForSceneMove();
		HudMenuUtility.GoToTitleScene();
	}

	private StageInfo.MileageMapInfo CreateMileageInfo()
	{
		StageInfo.MileageMapInfo mileageMapInfo = new StageInfo.MileageMapInfo();
		if (m_mileage_map_state != null)
		{
			mileageMapInfo.m_mapState.m_episode = m_mileage_map_state.m_episode;
			mileageMapInfo.m_mapState.m_chapter = m_mileage_map_state.m_chapter;
			mileageMapInfo.m_mapState.m_point = m_mileage_map_state.m_point;
			mileageMapInfo.m_mapState.m_score = m_mileage_map_state.m_stageTotalScore;
		}
		else
		{
			mileageMapInfo.m_mapState.m_episode = 1;
			mileageMapInfo.m_mapState.m_chapter = 1;
			mileageMapInfo.m_mapState.m_point = 0;
			mileageMapInfo.m_mapState.m_score = 0L;
		}
		if (mileageMapInfo.m_pointScore != null)
		{
			int num = mileageMapInfo.m_pointScore.Length;
			long num2 = MileageMapUtility.GetPointInterval();
			for (int i = 0; i < num; i++)
			{
				mileageMapInfo.m_pointScore[i] = num2 * i;
			}
		}
		return mileageMapInfo;
	}

	private void SetBoostItemValidFlag(StageInfo stageInfo)
	{
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		bool[] boostItemValidFlag = instance.BoostItemValidFlag;
		if (boostItemValidFlag != null)
		{
			for (int i = 0; i < 3; i++)
			{
				boostItemValidFlag[i] = stageInfo.BoostItemValid[i];
			}
		}
	}

	private void ResetRingCountOffset()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			instance.ItemData.RingCountOffset = 0;
		}
	}

	private void PrepareForSceneMove()
	{
		if (InformationImageManager.Instance != null)
		{
			InformationImageManager.Instance.ResetImage();
		}
		AtlasManager.Instance.ResetReplaceAtlas();
		MileageMapText.DestroyPreEPisodeText();
		ResourceManager.Instance.RemoveResourcesOnThisScene();
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	private void SetStageInfo()
	{
		if (!(m_stage_info_obj != null))
		{
			return;
		}
		StageInfo component = m_stage_info_obj.GetComponent<StageInfo>();
		if (!(component != null))
		{
			return;
		}
		component.MileageInfo = CreateMileageInfo();
		int point_type = 0;
		int numBossAttack = 0;
		if (m_mileage_map_state != null)
		{
			point_type = m_mileage_map_state.m_point;
			numBossAttack = m_mileage_map_state.m_numBossAttack;
		}
		if (StageModeManager.Instance.IsQuickMode())
		{
			if (EventManager.Instance != null && EventManager.Instance.Type == EventManager.EventType.QUICK)
			{
				int index = 1;
				TenseType tenseType = TenseType.AFTERNOON;
				EventStageData stageData = EventManager.Instance.GetStageData();
				if (stageData != null)
				{
					index = MileageMapUtility.GetStageIndex(stageData.stage_key);
					tenseType = MileageMapUtility.GetTenseType(stageData.stage_key);
				}
				component.SelectedStageName = StageInfo.GetStageNameByIndex(index);
				component.TenseType = tenseType;
				component.NotChangeTense = true;
			}
			else
			{
				switch (StageModeManager.Instance.QuickStageCharaAttribute)
				{
				case CharacterAttribute.SPEED:
					component.SelectedStageName = StageInfo.GetStageNameByIndex(1);
					break;
				case CharacterAttribute.FLY:
					component.SelectedStageName = StageInfo.GetStageNameByIndex(2);
					break;
				case CharacterAttribute.POWER:
					component.SelectedStageName = StageInfo.GetStageNameByIndex(3);
					break;
				default:
					component.SelectedStageName = StageInfo.GetStageNameByIndex(1);
					break;
				}
				component.TenseType = TenseType.AFTERNOON;
				component.NotChangeTense = true;
			}
			component.ExistBoss = false;
			component.BossStage = false;
			component.QuickMode = true;
			component.FromTitle = false;
			component.BossType = BossType.NONE;
			component.NumBossAttack = 0;
		}
		else
		{
			component.SelectedStageName = MileageMapUtility.GetMileageStageName();
			component.TenseType = MileageMapUtility.GetTenseType((PointType)point_type);
			component.NotChangeTense = !MileageMapUtility.GetChangeTense((PointType)point_type);
			component.ExistBoss = MileageMapUtility.IsExistBoss();
			component.BossStage = BossChallenge;
			component.QuickMode = false;
			component.FromTitle = false;
			component.BossType = MileageMapUtility.GetBossType();
			component.NumBossAttack = numBossAttack;
		}
		if (m_flags.Test(15))
		{
			component.TutorialStage = true;
		}
		if (!component.TutorialStage && EventManager.Instance != null)
		{
			if (EventManager.Instance.Type == EventManager.EventType.COLLECT_OBJECT && EventManager.Instance.IsInEvent())
			{
				SetEventStage(true);
			}
			component.EventStage = EventManager.Instance.EventStage;
		}
		SetBoostItemValidFlag(component);
	}

	private void SetSpecialStageInfo()
	{
		if (!(m_stage_info_obj != null))
		{
			return;
		}
		StageInfo component = m_stage_info_obj.GetComponent<StageInfo>();
		if (component != null)
		{
			EventStageData stageData = EventManager.Instance.GetStageData();
			if (stageData != null)
			{
				string stage_key = stageData.stage_key;
				component.SelectedStageName = MileageMapUtility.GetEventStageName(stage_key);
				component.TenseType = MileageMapUtility.GetTenseType(stage_key);
				component.NotChangeTense = !MileageMapUtility.GetChangeTense(stage_key);
				component.ExistBoss = false;
				component.BossStage = false;
				component.TutorialStage = false;
			}
			if (EventManager.Instance != null)
			{
				component.EventStage = EventManager.Instance.EventStage;
			}
			component.MileageInfo = CreateMileageInfo();
			SetBoostItemValidFlag(component);
			component.FromTitle = false;
		}
	}

	private void SetRaidBossInfo()
	{
		if (!(m_stage_info_obj != null))
		{
			return;
		}
		StageInfo component = m_stage_info_obj.GetComponent<StageInfo>();
		if (!(component != null))
		{
			return;
		}
		EventStageData stageData = EventManager.Instance.GetStageData();
		if (stageData != null)
		{
			string stage_key = stageData.stage_key;
			component.SelectedStageName = MileageMapUtility.GetEventStageName(stage_key);
			component.TenseType = MileageMapUtility.GetTenseType(stage_key);
			component.NotChangeTense = !MileageMapUtility.GetChangeTense(stage_key);
			component.ExistBoss = true;
			component.BossStage = true;
			component.TutorialStage = false;
			if (RaidBossInfo.currentRaidData != null)
			{
				component.NumBossAttack = (int)(RaidBossInfo.currentRaidData.hpMax - RaidBossInfo.currentRaidData.hp);
				switch (RaidBossInfo.currentRaidData.rarity)
				{
				case 0:
					component.BossType = BossType.EVENT1;
					break;
				case 1:
					component.BossType = BossType.EVENT2;
					break;
				case 2:
					component.BossType = BossType.EVENT3;
					break;
				default:
					component.BossType = BossType.EVENT1;
					break;
				}
			}
			else
			{
				component.NumBossAttack = 0;
				component.BossType = BossType.EVENT1;
			}
		}
		if (EventManager.Instance != null)
		{
			component.EventStage = EventManager.Instance.EventStage;
		}
		component.MileageInfo = CreateMileageInfo();
		SetBoostItemValidFlag(component);
		component.FromTitle = false;
	}

	private void DestroyMileageInfo()
	{
		if (MileageMapDataManager.Instance != null)
		{
			MileageMapDataManager.Instance.DestroyData();
		}
	}

	private void SetLoadingInfo()
	{
		LoadingInfo loadingInfo = LoadingInfo.CreateLoadingInfo();
		if (!(loadingInfo != null))
		{
			return;
		}
		if (StageModeManager.Instance.IsQuickMode())
		{
			LoadingInfo.LoadingData info = loadingInfo.GetInfo();
			if (info != null)
			{
				UnityEngine.Random.seed = NetUtil.GetCurrentUnixTime();
				int num = UnityEngine.Random.Range(1, 13);
				info.m_titleText = TextUtility.GetCommonText("quick", "loading_title");
				if (num == 1)
				{
					info.m_mainText = TextUtility.GetCommonText("quick", "loading_text");
				}
				else
				{
					info.m_mainText = TextUtility.GetCommonText("quick", "loading_text" + num);
				}
			}
		}
		else
		{
			if (!(MileageMapDataManager.Instance != null))
			{
				return;
			}
			int episode = 1;
			int chapter = 1;
			if (m_mileage_map_state != null)
			{
				episode = m_mileage_map_state.m_episode;
				chapter = m_mileage_map_state.m_chapter;
			}
			MileageMapData mileageMapData = MileageMapDataManager.Instance.GetMileageMapData(episode, chapter);
			if (mileageMapData == null)
			{
				return;
			}
			LoadingInfo.LoadingData info2 = loadingInfo.GetInfo();
			if (info2 != null)
			{
				int num2 = (!IsBossLoading()) ? mileageMapData.loading.window_id : mileageMapData.loading.boss_window_id;
				if (num2 < mileageMapData.window_data.Length)
				{
					info2.m_titleText = MileageMapText.GetText(mileageMapData.scenario.episode, mileageMapData.scenario.title_cell_id);
					info2.m_mainText = MileageMapText.GetText(mileageMapData.scenario.episode, mileageMapData.window_data[num2].body[0].text_cell_id);
					int face_id = mileageMapData.window_data[num2].body[0].product[0].face_id;
					info2.m_texture = MileageMapUtility.GetFaceTexture(face_id);
				}
			}
		}
	}

	private void SetEventLoadingInfo()
	{
		LoadingInfo loadingInfo = LoadingInfo.CreateLoadingInfo();
		if (!(loadingInfo != null))
		{
			return;
		}
		LoadingInfo.LoadingData info = loadingInfo.GetInfo();
		if (info != null && EventManager.Instance != null)
		{
			WindowEventData loadingEventData = EventUtility.GetLoadingEventData();
			if (loadingEventData != null)
			{
				TextManager.TextType type = TextManager.TextType.TEXTTYPE_EVENT_SPECIFIC;
				info.m_titleText = TextUtility.GetText(type, "Production", loadingEventData.title_cell_id);
				info.m_mainText = TextUtility.GetText(type, "Production", loadingEventData.body[0].text_cell_id);
				info.m_texture = EventUtility.GetLoadingFaceTexture();
			}
		}
	}

	private bool IsBossLoading()
	{
		if (m_mileage_map_state != null && MileageMapUtility.IsExistBoss())
		{
			return BossChallenge;
		}
		return false;
	}

	private void SetTutorialStageInfo()
	{
		if (!(m_stage_info_obj != null))
		{
			return;
		}
		StageInfo component = m_stage_info_obj.GetComponent<StageInfo>();
		if (!(component != null))
		{
			return;
		}
		StageInfo.MileageMapInfo mileageMapInfo = new StageInfo.MileageMapInfo();
		mileageMapInfo.m_mapState.m_episode = 1;
		mileageMapInfo.m_mapState.m_chapter = 1;
		mileageMapInfo.m_mapState.m_point = 0;
		mileageMapInfo.m_mapState.m_score = 0L;
		component.SelectedStageName = StageInfo.GetStageNameByIndex(1);
		component.TenseType = TenseType.AFTERNOON;
		component.ExistBoss = false;
		component.BossStage = false;
		component.TutorialStage = true;
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (instance != null)
		{
			bool[] boostItemValidFlag = instance.BoostItemValidFlag;
			if (boostItemValidFlag != null)
			{
				for (int i = 0; i < 3; i++)
				{
					boostItemValidFlag[i] = false;
				}
			}
		}
		component.FromTitle = false;
		component.MileageInfo = mileageMapInfo;
	}

	private void SetTutorialLoadingInfo()
	{
		LoadingInfo loadingInfo = LoadingInfo.CreateLoadingInfo();
		if (loadingInfo != null)
		{
			LoadingInfo.LoadingData info = loadingInfo.GetInfo();
			if (info != null)
			{
				string cellID = CharaName.Name[0];
				string commonText = TextUtility.GetCommonText("CharaName", cellID);
				info.m_titleText = TextUtility.GetCommonText("Option", "chara_operation_method", "{CHARA_NAME}", commonText);
				info.m_mainText = TextUtility.GetCommonText("Option", "sonic_operation_comment");
				info.m_optionTutorial = true;
				int face_id = 1;
				info.m_texture = MileageMapUtility.GetFaceTexture(face_id);
			}
		}
	}

	private void SetEventManagerParam()
	{
		if (EventManager.Instance != null)
		{
			if (m_flags.Test(4) || m_flags.Test(5))
			{
				EventManager.Instance.EventStage = true;
				EventManager.Instance.ReCalcEndPlayTime();
			}
			else
			{
				EventManager.Instance.EventStage = false;
			}
		}
	}

	private TinyFsmState StateLoadMileageXml(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (!m_episodeLoaded)
			{
				CeateSceneLoader();
				if (!IsExistMileageMapData(m_mileage_map_state))
				{
					AddSceneLoader(GetMileageMapDataScenaName(m_mileage_map_state));
				}
				if (GameObject.Find("MileageDataTable") == null)
				{
					AddSceneLoader("MileageDataTable");
				}
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_episodeLoaded)
			{
				ChangeState(new TinyFsmState(StateMileageReward), SequenceState.MileageReward);
			}
			else if (CheckSceneLoad())
			{
				m_episodeLoaded = true;
				DestroySceneLoader();
				SetupMileageDataTable();
				if (m_flags.Test(6))
				{
					ChangeState(new TinyFsmState(StateLoadNextMileageXml), SequenceState.LoadNextMileageXml);
				}
				else
				{
					ChangeState(new TinyFsmState(StateLoadMileageTexture), SequenceState.LoadMileageTexture);
				}
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoadNextMileageXml(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CeateSceneLoader();
			if (!IsExistMileageMapData(m_prev_mileage_map_state))
			{
				AddSceneLoader(GetMileageMapDataScenaName(m_prev_mileage_map_state));
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (CheckSceneLoad())
			{
				DestroySceneLoader();
				MileageMapDataManager instance = MileageMapDataManager.Instance;
				if (instance != null)
				{
					instance.SetCurrentData(m_prev_mileage_map_state.m_episode, m_prev_mileage_map_state.m_chapter);
				}
				ChangeState(new TinyFsmState(StateLoadMileageTexture), SequenceState.LoadMileageTexture);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoadMileageTexture(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			MileageMapDataManager instance = MileageMapDataManager.Instance;
			if (instance != null)
			{
				m_request_face_list.Clear();
				m_request_bg_list.Clear();
				EntryMileageTexturesList();
				CeateSceneLoader();
				LoadMileageText();
				LoadMileageTextures();
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (CheckSceneLoad())
			{
				SetIncentive();
				SetupMileageText();
				TransTextureObj();
				DestroySceneLoader();
				Resources.UnloadUnusedAssets();
				ChangeState(new TinyFsmState(StateMileageReward), SequenceState.MileageReward);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateMileageReward(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (ServerInterface.LoggedInServerInterface != null)
			{
				ServerMileageMapState mileageMapState = ServerInterface.MileageMapState;
				List<ServerMileageReward> mileageRewardList = ServerInterface.MileageRewardList;
				foreach (ServerMileageReward item in mileageRewardList)
				{
					if (item.m_episode == mileageMapState.m_episode && item.m_episode == mileageMapState.m_chapter)
					{
						ServerGetMileageReward_Succeeded(null);
						break;
					}
				}
				if (!m_flags.Test(12))
				{
					ServerInterface.LoggedInServerInterface.RequestServerGetMileageReward(mileageMapState.m_episode, mileageMapState.m_chapter, base.gameObject);
				}
			}
			else
			{
				ServerGetMileageReward_Succeeded(null);
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_flags.Test(12))
			{
				ChangeState(new TinyFsmState(StateWaitFadeIfNotEndFade), SequenceState.WaitFadeIfNotEndFade);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitFadeIfNotEndFade(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (!m_flags.Test(0))
			{
				ConnectAlertMaskUI.EndScreen(OnFinishedFadeInCallback);
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_flags.Test(0))
			{
				ChangeState(new TinyFsmState(StateEpisode), SequenceState.Episode);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEpisode(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_flags.Set(28, true);
			if (m_flags.Test(7))
			{
				m_flags.Set(7, false);
				HudMenuUtility.SendMsgPrepareMileageMapProduction(m_stageResultData);
			}
			else
			{
				HudMenuUtility.SendMsgUpdateMileageMapDisplayToMileage();
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null)
			{
				if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.MAIN)
				{
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.PLAY_AT_EPISODE_PAGE)
				{
					ChangeState(new TinyFsmState(MenuStatePlayButton), SequenceState.PlayButton);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.SHOP)
				{
					ChangeState(new TinyFsmState(StateShop), SequenceState.Shop);
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private void TransTextureObj()
	{
		if (!(MileageMapDataManager.Instance != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(MileageMapDataManager.Instance.gameObject, "MileageMapBG");
		if (gameObject != null && MileageMapBGDataTable.Instance != null)
		{
			foreach (int item in m_request_bg_list)
			{
				GameObject gameObject2 = GameObject.Find(MileageMapBGDataTable.Instance.GetTextureName(item));
				if (gameObject2 != null)
				{
					gameObject2.transform.parent = gameObject.transform;
				}
			}
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(MileageMapDataManager.Instance.gameObject, "MileageMapFace");
		if (!(gameObject3 != null))
		{
			return;
		}
		foreach (int item2 in m_request_face_list)
		{
			GameObject gameObject4 = GameObject.Find(MileageMapUtility.GetFaceTextureName(item2));
			if (gameObject4 != null)
			{
				gameObject4.transform.parent = gameObject3.transform;
			}
		}
	}

	private void EntryMileageTexturesList()
	{
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		MileageMapData mileageMapData = null;
		MileageMapData mileageMapData2 = null;
		if (instance != null)
		{
			mileageMapData2 = instance.GetMileageMapData(m_mileage_map_state.m_episode, m_mileage_map_state.m_chapter);
			if (m_flags.Test(6))
			{
				mileageMapData = instance.GetMileageMapData(m_prev_mileage_map_state.m_episode, m_prev_mileage_map_state.m_chapter);
			}
		}
		if (mileageMapData2 == null)
		{
			return;
		}
		bool keep = true;
		bool keep2 = false;
		int bg_id = mileageMapData2.map_data.bg_id;
		AddIDList(ref m_request_bg_list, bg_id, "bg", keep);
		if (mileageMapData != null)
		{
			bg_id = mileageMapData.map_data.bg_id;
			AddIDList(ref m_request_bg_list, bg_id, "bg", keep2);
		}
		List<int> loadingList = new List<int>();
		if (m_mileage_map_state.m_point == 5 && mileageMapData2.event_data.IsBossEvent())
		{
			int boss_window_id = mileageMapData2.loading.boss_window_id;
			SetLoadingFaceTexture(ref m_request_face_list, ref loadingList, mileageMapData2, boss_window_id);
		}
		int window_id = mileageMapData2.loading.window_id;
		SetLoadingFaceTexture(ref m_request_face_list, ref loadingList, mileageMapData2, window_id);
		int num = 1;
		if (!m_request_face_list.Contains(num))
		{
			AddIDList(ref m_request_face_list, num, "face", keep);
			loadingList.Add(num);
		}
		instance.SetLoadingFaceId(loadingList);
		if (m_flags.Test(6))
		{
			int num2 = mileageMapData.event_data.point.Length;
			for (int i = 0; i < num2; i++)
			{
				if (m_prev_mileage_map_state.m_point <= i)
				{
					if (i != 5 || !mileageMapData.event_data.IsBossEvent())
					{
						int balloon_face_id = mileageMapData.event_data.point[i].balloon_face_id;
						int balloon_on_arrival_face_id = mileageMapData.event_data.point[i].balloon_on_arrival_face_id;
						AddIDList(ref m_request_face_list, balloon_face_id, "face", keep2);
						AddIDList(ref m_request_face_list, balloon_on_arrival_face_id, "face", keep2);
					}
					else
					{
						BossEvent bossEvent = mileageMapData.event_data.GetBossEvent();
						AddIDList(ref m_request_face_list, bossEvent.balloon_on_arrival_face_id, "face", keep2);
						AddIDList(ref m_request_face_list, bossEvent.balloon_clear_face_id, "face", keep2);
					}
				}
			}
			num2 = mileageMapData2.event_data.point.Length;
			for (int j = 0; j < num2; j++)
			{
				if (m_mileage_map_state.m_point <= j)
				{
					if (j != 5 || !mileageMapData2.event_data.IsBossEvent())
					{
						int balloon_face_id2 = mileageMapData2.event_data.point[j].balloon_face_id;
						AddIDList(ref m_request_face_list, balloon_face_id2, "face", keep);
					}
					else
					{
						BossEvent bossEvent2 = mileageMapData2.event_data.GetBossEvent();
						AddIDList(ref m_request_face_list, bossEvent2.balloon_init_face_id, "face", keep);
					}
				}
			}
			SetLoadWindowFaceTexture(ref m_request_face_list, mileageMapData, m_prev_mileage_map_state);
			SetLoadWindowFaceTexture(ref m_request_face_list, mileageMapData2, m_mileage_map_state);
			return;
		}
		int num3 = mileageMapData2.event_data.point.Length;
		for (int k = 0; k < num3; k++)
		{
			if (m_prev_mileage_map_state.m_point <= k)
			{
				if (k != 5 || !mileageMapData2.event_data.IsBossEvent())
				{
					int balloon_face_id3 = mileageMapData2.event_data.point[k].balloon_face_id;
					int balloon_on_arrival_face_id2 = mileageMapData2.event_data.point[k].balloon_on_arrival_face_id;
					AddIDList(ref m_request_face_list, balloon_face_id3, "face", keep);
					AddIDList(ref m_request_face_list, balloon_on_arrival_face_id2, "face", keep2);
				}
				else
				{
					BossEvent bossEvent3 = mileageMapData2.event_data.GetBossEvent();
					AddIDList(ref m_request_face_list, bossEvent3.balloon_init_face_id, "face", keep);
					AddIDList(ref m_request_face_list, bossEvent3.balloon_on_arrival_face_id, "face", keep);
					AddIDList(ref m_request_face_list, bossEvent3.balloon_clear_face_id, "face", keep2);
				}
			}
		}
		if (m_mileage_map_state.m_episode == 1 || m_flags.Test(7))
		{
			SetLoadWindowFaceTexture(ref m_request_face_list, mileageMapData2);
		}
	}

	private void LoadEventProductTexture()
	{
	}

	private void LoadMileageTextures()
	{
		if (!(MileageMapDataManager.Instance != null))
		{
			return;
		}
		foreach (int item in m_request_bg_list)
		{
			string textureName = MileageMapBGDataTable.Instance.GetTextureName(item);
			if (GameObjectUtil.FindChildGameObject(MileageMapDataManager.Instance.gameObject, textureName) == null)
			{
				AddSceneLoader(textureName);
			}
		}
		foreach (int item2 in m_request_face_list)
		{
			string faceTextureName = MileageMapUtility.GetFaceTextureName(item2);
			if (GameObjectUtil.FindChildGameObject(MileageMapDataManager.Instance.gameObject, faceTextureName) == null)
			{
				AddSceneLoader(faceTextureName);
			}
		}
	}

	private void LoadMileageText()
	{
		int episode = 1;
		int pre_episode = -1;
		GetMileageTextParam(ref episode, ref pre_episode);
		if (m_scene_loader_obj != null)
		{
			MileageMapText.Load(m_scene_loader_obj.GetComponent<ResourceSceneLoader>(), episode, pre_episode);
		}
	}

	private void GetMileageTextParam(ref int episode, ref int pre_episode)
	{
		if (m_mileage_map_state != null)
		{
			episode = m_mileage_map_state.m_episode;
		}
		if (m_stageResultData != null && m_stageResultData.m_oldMapState != null)
		{
			pre_episode = m_stageResultData.m_oldMapState.m_episode;
		}
	}

	public void SetupMileageText()
	{
		MileageMapText.Setup();
	}

	public void SetIncentive()
	{
		if (m_stageResultData == null || m_stageResultData.m_mileageIncentiveList == null)
		{
			return;
		}
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		int episode = m_mileage_map_state.m_episode;
		int chapter = m_mileage_map_state.m_chapter;
		int num = 0;
		int num2 = 0;
		foreach (ServerMileageIncentive mileageIncentive in m_stageResultData.m_mileageIncentiveList)
		{
			RewardData src_reward = new RewardData(mileageIncentive.m_itemId, mileageIncentive.m_num);
			if (mileageIncentive.m_type == ServerMileageIncentive.Type.POINT)
			{
				if (m_stageResultData != null && m_stageResultData.m_oldMapState != null && mileageIncentive.m_pointId > m_stageResultData.m_oldMapState.m_point)
				{
					episode = m_stageResultData.m_oldMapState.m_episode;
					chapter = m_stageResultData.m_oldMapState.m_chapter;
				}
				instance.SetPointIncentiveData(episode, chapter, mileageIncentive.m_pointId, src_reward);
			}
			else if (mileageIncentive.m_type == ServerMileageIncentive.Type.CHAPTER)
			{
				if (m_stageResultData != null && m_stageResultData.m_oldMapState != null)
				{
					episode = m_stageResultData.m_oldMapState.m_episode;
					chapter = m_stageResultData.m_oldMapState.m_chapter;
				}
				instance.SetChapterIncentiveData(episode, chapter, num, src_reward);
				num++;
			}
			else if (mileageIncentive.m_type == ServerMileageIncentive.Type.EPISODE)
			{
				if (m_stageResultData != null && m_stageResultData.m_oldMapState != null)
				{
					episode = m_stageResultData.m_oldMapState.m_episode;
					chapter = m_stageResultData.m_oldMapState.m_chapter;
				}
				instance.SetEpisodeIncentiveData(episode, chapter, num2, src_reward);
				num2++;
			}
		}
	}

	private void SetupMileageDataTable()
	{
		if (GameObject.Find("MileageDataTable") == null)
		{
			GameObject gameObject = new GameObject("MileageDataTable");
			GameObject gameObject2 = GameObject.Find("BGDataTable");
			if (gameObject2 != null)
			{
				gameObject2.transform.parent = gameObject.gameObject.transform;
			}
			GameObject gameObject3 = GameObject.Find("PointDataTable");
			if (gameObject3 != null)
			{
				gameObject3.transform.parent = gameObject.gameObject.transform;
			}
			GameObject gameObject4 = GameObject.Find("RouteDataTable");
			if (gameObject4 != null)
			{
				gameObject4.transform.parent = gameObject.gameObject.transform;
			}
			GameObject gameObject5 = GameObject.Find("StageSuggestedDataTable");
			if (gameObject5 != null)
			{
				gameObject5.transform.parent = gameObject.gameObject.transform;
			}
			GameObject gameObject6 = GameObject.Find("MileageMapDataManager");
			if (gameObject6 != null)
			{
				gameObject.transform.parent = gameObject6.transform;
			}
		}
	}

	private bool IsExistMileageMapData(ServerMileageMapState state)
	{
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			return instance.IsExist(state.m_episode, state.m_chapter);
		}
		return false;
	}

	private TinyFsmState StateFadeIn(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			TimeProfiler.StartCountTime("StateFadeIn");
			if (m_flags.Test(7))
			{
				MileageMapUtility.SetDisplayOffset_FromResultData(m_stageResultData);
			}
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
			HudMenuUtility.SendStartMainMenuDlsplay();
			HudMenuUtility.SendMsgTickerUpdate();
			m_ButtonOfNextMenu = ButtonInfoTable.ButtonType.UNKNOWN;
			if (((m_stageResultData != null) ? true : false) && !m_stageResultData.m_quickMode)
			{
				if (m_stageResultData.m_fromOptionTutorial)
				{
					m_ButtonOfNextMenu = ButtonInfoTable.ButtonType.OPTION;
				}
				else
				{
					m_ButtonOfNextMenu = ButtonInfoTable.ButtonType.EPISODE;
				}
			}
			if (m_ButtonOfNextMenu == ButtonInfoTable.ButtonType.UNKNOWN)
			{
				ConnectAlertMaskUI.EndScreen(OnFinishedFadeInCallback);
			}
			else
			{
				OnFinishedFadeInCallback();
			}
			return TinyFsmState.End();
		case -4:
			BackKeyManager.StartScene();
			return TinyFsmState.End();
		case 0:
			if (m_flags.Test(0))
			{
				TimeProfiler.EndCountTime("StateFadeIn");
				if (m_ButtonOfNextMenu == ButtonInfoTable.ButtonType.UNKNOWN)
				{
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
				else
				{
					HudMenuUtility.SendMenuButtonClicked(m_ButtonOfNextMenu);
					m_flags.Set(0, false);
					ChangeState(new TinyFsmState(StateMainMenu), SequenceState.Main);
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void OnFinishedFadeInCallback()
	{
		m_flags.Set(0, true);
	}

	private TinyFsmState StateFadeOut(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			BackKeyManager.EndScene();
			bool isFadeIn = false;
			float fadeDuration = 1f;
			float fadeDelay = 0f;
			m_flags.Set(1, false);
			CameraFade.StartAlphaFade(Color.black, isFadeIn, fadeDuration, fadeDelay, OnFinishedFadeOutCallback);
			SoundManager.BgmFadeOut(0.5f);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_flags.Test(1))
			{
				ChangeState(new TinyFsmState(StateEnd), SequenceState.End);
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void OnFinishedFadeOutCallback()
	{
		m_flags.Set(1, true);
	}

	private TinyFsmState StateInformationWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
			m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync(ButtonInfoTable.PageType.INFOMATION, ResourceLoadEndCallback);
			return TinyFsmState.End();
		case -4:
			UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
			m_buttonEventResourceLoader = null;
			return TinyFsmState.End();
		case 0:
			if (m_buttonEventResourceLoader.IsLoaded)
			{
				ChangeState(new TinyFsmState(StateInformationWindowCreate), SequenceState.InformationWindowCreate);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateInformationWindowCreate(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			SetRankingResult();
			m_eventRankingResultInfo = GetEventRankingResultInformation();
			CreateInformationWindow();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_eventRankingResultInfo != null)
			{
				ChangeState(new TinyFsmState(StateEventRankingResultWindow), SequenceState.EventRankingResultWindow);
			}
			else if (m_rankingResultList.Count > 0)
			{
				ChangeState(new TinyFsmState(StateRankingResultLeagueWindow), SequenceState.RankingResultLeagueWindow);
			}
			else if (m_serverInformationWindow != null)
			{
				ChangeState(new TinyFsmState(StateDisplayInformaon), SequenceState.DisplayInformaon);
			}
			else
			{
				ChangeNextStateForInformation();
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEventRankingResultWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_eventRankingResult = CreateWorldRankingWindow(m_eventRankingResultInfo);
			return TinyFsmState.End();
		case -4:
			if (m_eventRankingResult != null)
			{
				m_eventRankingResult = null;
			}
			return TinyFsmState.End();
		case 0:
			if (m_eventRankingResult != null && m_eventRankingResult.IsEnd)
			{
				ServerNoticeInfoUpdateChecked(m_eventRankingResultInfo);
				ServerNoticeInfoSave();
				if (m_rankingResultList.Count > 0)
				{
					ChangeState(new TinyFsmState(StateRankingResultLeagueWindow), SequenceState.RankingResultLeagueWindow);
				}
				else if (m_serverInformationWindow != null)
				{
					ChangeState(new TinyFsmState(StateDisplayInformaon), SequenceState.DisplayInformaon);
				}
				else
				{
					ChangeNextStateForInformation();
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRankingResultLeagueWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			m_rankingResultLeagueWindow = null;
			using (List<NetNoticeItem>.Enumerator enumerator2 = m_rankingResultList.GetEnumerator())
			{
				if (enumerator2.MoveNext())
				{
					NetNoticeItem current2 = enumerator2.Current;
					m_rankingResultLeagueWindow = RankingResultLeague.Create(current2);
					m_currentResultInfo = current2;
				}
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_rankingResultLeagueWindow != null)
			{
				m_rankingResultLeagueWindow = null;
			}
			return TinyFsmState.End();
		case 0:
		{
			bool flag = m_rankingResultLeagueWindow == null;
			if (m_rankingResultLeagueWindow != null && m_rankingResultLeagueWindow.IsEnd())
			{
				ServerNoticeInfoUpdateChecked(m_currentResultInfo);
				m_rankingResultList.Remove(m_currentResultInfo);
				m_rankingResultLeagueWindow = null;
				m_currentResultInfo = null;
				using (List<NetNoticeItem>.Enumerator enumerator = m_rankingResultList.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						NetNoticeItem current = enumerator.Current;
						m_rankingResultLeagueWindow = RankingResultLeague.Create(current);
						m_currentResultInfo = current;
					}
				}
			}
			if (flag)
			{
				if (m_serverInformationWindow != null)
				{
					ChangeState(new TinyFsmState(StateDisplayInformaon), SequenceState.DisplayInformaon);
				}
				else
				{
					ChangeNextStateForInformation();
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateDisplayInformaon(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (m_serverInformationWindow != null)
			{
				m_serverInformationWindow.SetSaveFlag();
				m_serverInformationWindow.PlayStart();
			}
			return TinyFsmState.End();
		case -4:
			if (m_serverInformationWindow != null)
			{
				m_serverInformationWindow = null;
			}
			return TinyFsmState.End();
		case 0:
			if (m_serverInformationWindow != null && m_serverInformationWindow.IsEnd())
			{
				ChangeNextStateForInformation();
				if (InformationImageManager.Instance != null)
				{
					InformationImageManager.Instance.ClearWinowImage();
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void CreateInformationWindow()
	{
		if (m_serverInformationWindow == null)
		{
			GameObject gameObject = new GameObject();
			if (gameObject != null)
			{
				gameObject.name = "ServerInformationWindow";
				gameObject.AddComponent<ServerInformationWindow>();
				m_serverInformationWindow = gameObject.GetComponent<ServerInformationWindow>();
			}
		}
	}

	private RankingResultWorldRanking CreateWorldRankingWindow(NetNoticeItem item)
	{
		if (item != null)
		{
			RankingResultWorldRanking resultWorldRanking = RankingResultWorldRanking.GetResultWorldRanking();
			if (resultWorldRanking != null)
			{
				resultWorldRanking.Setup(item);
				resultWorldRanking.PlayStart();
				return resultWorldRanking;
			}
		}
		return null;
	}

	private void SetRankingResult()
	{
		m_eventRankingResultInfo = GetInformation(NetNoticeItem.OPERATORINFO_EVENTRANKINGRESULT_ID);
		NetNoticeItem information = GetInformation(NetNoticeItem.OPERATORINFO_RANKINGRESULT_ID);
		NetNoticeItem information2 = GetInformation(NetNoticeItem.OPERATORINFO_QUICKRANKINGRESULT_ID);
		if (information != null && information2 != null)
		{
			if (information.Priority < information2.Priority)
			{
				m_rankingResultList.Add(information);
				m_rankingResultList.Add(information2);
			}
			else
			{
				m_rankingResultList.Add(information2);
				m_rankingResultList.Add(information);
			}
			return;
		}
		if (information != null)
		{
			m_rankingResultList.Add(information);
		}
		if (information2 != null)
		{
			m_rankingResultList.Add(information2);
		}
	}

	private NetNoticeItem GetRankingResultInformation()
	{
		return GetInformation(NetNoticeItem.OPERATORINFO_RANKINGRESULT_ID);
	}

	private NetNoticeItem GetEventRankingResultInformation()
	{
		return GetInformation(NetNoticeItem.OPERATORINFO_EVENTRANKINGRESULT_ID);
	}

	private NetNoticeItem GetInformation(int id)
	{
		if (ServerInterface.NoticeInfo != null)
		{
			List<NetNoticeItem> noticeItems = ServerInterface.NoticeInfo.m_noticeItems;
			if (noticeItems != null)
			{
				foreach (NetNoticeItem item in noticeItems)
				{
					if (item != null && item.Id == id && !ServerInterface.NoticeInfo.IsChecked(item))
					{
						return item;
					}
				}
			}
		}
		return null;
	}

	private void ServerNoticeInfoUpdateChecked(NetNoticeItem item)
	{
		if (ServerInterface.NoticeInfo != null)
		{
			ServerInterface.NoticeInfo.UpdateChecked(item);
		}
	}

	private void ServerNoticeInfoSave()
	{
		if (ServerInterface.NoticeInfo != null)
		{
			ServerInterface.NoticeInfo.m_isShowedNoticeInfo = true;
			ServerInterface.NoticeInfo.SaveInformation();
		}
	}

	private void ChangeNextStateForInformation()
	{
		HudMenuUtility.SendMsgInformationDisplay();
		ServerNoticeInfoSave();
		HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.INFOMATION);
		ChangeState(new TinyFsmState(StateMainMenu), SequenceState.Main);
	}

	private void ResourceLoadEndCallback()
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "OnPageResourceLoadedCallback", null, SendMessageOptions.DontRequireReceiver);
	}

	private TinyFsmState StateInit(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			TimeProfiler.StartCountTime("StateInit");
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
			StageAbilityManager instance = StageAbilityManager.Instance;
			if (instance != null)
			{
				for (int i = 0; i < 3; i++)
				{
					instance.BoostItemValidFlag[i] = false;
				}
			}
			NativeObserver.Instance.CheckCurrentTransaction();
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (!Env.useAssetBundle || AssetBundleLoader.Instance.IsEnableDownlad())
			{
				TimeProfiler.EndCountTime("StateInit");
				ChangeState(new TinyFsmState(StateRequestDayCrossWatcher), SequenceState.RequestDayCrossWatcher);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRequestDayCrossWatcher(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			TimeProfiler.StartCountTime("StateRequestDayCrossWatcher");
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				m_callBackFlag.Reset();
				if (ServerDayCrossWatcher.Instance != null)
				{
					ServerDayCrossWatcher.Instance.UpdateClientInfosByDayCross(DataCrossCallBack);
				}
			}
			else
			{
				m_callBackFlag.Set(0, true);
			}
			return TinyFsmState.End();
		}
		case -4:
			m_callBackFlag.Reset();
			if (m_progressBar != null)
			{
				m_progressBar.SetState(1);
			}
			return TinyFsmState.End();
		case 0:
			if (m_callBackFlag.Test(0))
			{
				TimeProfiler.EndCountTime("StateRequestDayCrossWatcher");
				ChangeState(new TinyFsmState(StateRequestDailyBattle), SequenceState.RequestDailyBattle);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRequestDailyBattle(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			TimeProfiler.StartCountTime("StateRequestDailyBattle");
			if (IsRequestDailyBattle())
			{
				if (SingletonGameObject<DailyBattleManager>.Instance != null)
				{
					SingletonGameObject<DailyBattleManager>.Instance.FirstSetup(DailyBattleManagerCallBack);
				}
				else
				{
					m_connected = true;
				}
			}
			else
			{
				m_connected = true;
			}
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(2);
			}
			return TinyFsmState.End();
		case 0:
			if (m_connected)
			{
				TimeProfiler.EndCountTime("StateRequestDailyBattle");
				ChangeState(new TinyFsmState(StateRequestChaoWheelOption), SequenceState.RequestChaoWheelOption);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRequestChaoWheelOption(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			TimeProfiler.StartCountTime("StateRequestChaoWheelOption");
			if (IsRequestChoaWheelOption())
			{
				if (RouletteManager.Instance != null)
				{
					RouletteManager.CallbackRouletteInit callback = CallbackRouletteInit;
					RouletteManager.Instance.InitRouletteRequest(callback);
				}
				else
				{
					m_connected = true;
				}
			}
			else
			{
				m_connected = true;
			}
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(3);
			}
			return TinyFsmState.End();
		case 0:
			if (m_connected)
			{
				SetStageResultData();
				CheckEventEnd();
				TimeProfiler.EndCountTime("StateRequestChaoWheelOption");
				ChangeState(new TinyFsmState(StateRequestMsgList), SequenceState.RequestMsgList);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRequestMsgList(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			TimeProfiler.StartCountTime("StateRequestMsgList");
			m_flags.Set(19, false);
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetMessageList(base.gameObject);
				m_flags.Set(19, true);
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
			if (!m_flags.Test(19))
			{
				TimeProfiler.EndCountTime("StateRequestMsgList");
				ChangeState(new TinyFsmState(StateRequestNoticeInfo), SequenceState.RequestNoticeInfo);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRequestNoticeInfo(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			TimeProfiler.StartCountTime("StateRequestNoticeInfo");
			m_is_end_notice_connect = false;
			bool flag = true;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				ServerNoticeInfo noticeInfo = ServerInterface.NoticeInfo;
				if (noticeInfo != null && noticeInfo.IsNeedUpdateInfo())
				{
					loggedInServerInterface.RequestServerGetInformation(base.gameObject);
					flag = false;
				}
			}
			if (flag)
			{
				ServerGetInformation_Succeeded(null);
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(5);
			}
			return TinyFsmState.End();
		case 0:
			if (m_is_end_notice_connect)
			{
				RouletteInformationManager instance = RouletteInformationManager.Instance;
				if (instance != null)
				{
					instance.SetUp();
				}
				TimeProfiler.EndCountTime("StateRequestNoticeInfo");
				ChangeState(new TinyFsmState(StateLoad), SequenceState.Load);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void SetStageResultData()
	{
		m_flags.Set(6, false);
		m_flags.Set(7, false);
		m_flags.Set(8, false);
		ServerMileageMapState mileageMapState = ServerInterface.MileageMapState;
		if (mileageMapState != null)
		{
			mileageMapState.CopyTo(m_mileage_map_state);
			mileageMapState.CopyTo(m_prev_mileage_map_state);
		}
		GameObject gameObject = GameObject.Find("ResultInfo");
		bool flag = gameObject != null;
		bool flag2 = !flag;
		if (flag)
		{
			ResultInfo component = gameObject.GetComponent<ResultInfo>();
			if (!(component != null))
			{
				return;
			}
			m_stageResultData = component.GetInfo();
			UnityEngine.Object.Destroy(gameObject);
			if (m_stageResultData.m_quickMode && m_stageResultData.m_missionComplete)
			{
				m_flags.Set(16, true);
			}
			if (m_stageResultData.m_validResult)
			{
				ReflectMileageProductionResultData();
				m_flags.Set(7, true);
			}
			else if (m_stageResultData.m_fromOptionTutorial && SaveDataManager.Instance != null)
			{
				PlayerData playerData = SaveDataManager.Instance.PlayerData;
				ServerPlayerState playerState = ServerInterface.PlayerState;
				if (playerState != null && playerData != null)
				{
					playerData.MainChara = new ServerItem((ServerItem.Id)playerState.m_mainCharaId).charaType;
					playerData.MainChaoID = new ServerItem((ServerItem.Id)playerState.m_mainChaoId).chaoId;
					playerData.SubChaoID = new ServerItem((ServerItem.Id)playerState.m_subChaoId).chaoId;
				}
			}
		}
		else if (flag2)
		{
			m_flags.Set(8, false);
			if (!HudMenuUtility.IsTutorial_11() && !HudMenuUtility.IsRouletteTutorial() && !HudMenuUtility.IsTutorialCharaLevelUp() && SaveDataManager.Instance != null && !SaveDataManager.Instance.PlayerData.DailyMission.missions_complete)
			{
				m_flags.Set(16, true);
			}
		}
	}

	private bool HaveNoticeInfo()
	{
		if (ServerInterface.NoticeInfo != null)
		{
			return !ServerInterface.NoticeInfo.IsAllChecked();
		}
		return false;
	}

	private void ReflectMileageProductionResultData()
	{
		if (m_stageResultData.m_oldMapState == null)
		{
			m_stageResultData.m_oldMapState = new MileageMapState();
		}
		if (m_stageResultData.m_newMapState == null)
		{
			m_stageResultData.m_newMapState = m_stageResultData.m_oldMapState;
		}
		if (ServerInterface.MileageMapState != null)
		{
			ServerInterface.MileageMapState.CopyTo(m_mileage_map_state);
		}
		SetMapState(ref m_mileage_map_state, m_stageResultData.m_newMapState);
		SetMapState(ref m_prev_mileage_map_state, m_stageResultData.m_oldMapState);
		if (CheckNextMap())
		{
			m_flags.Set(6, true);
		}
	}

	private void SetMapState(ref ServerMileageMapState map_state, MileageMapState result_map_state)
	{
		if (map_state != null && result_map_state != null)
		{
			map_state.m_episode = result_map_state.m_episode;
			map_state.m_chapter = result_map_state.m_chapter;
			map_state.m_point = result_map_state.m_point;
			map_state.m_stageTotalScore = result_map_state.m_score;
			if (map_state.m_episode == 0)
			{
				map_state.m_episode = 1;
			}
			if (map_state.m_chapter == 0)
			{
				map_state.m_chapter = 1;
			}
		}
	}

	private bool IsRequestChoaWheelOption()
	{
		ServerMileageMapState mileageMapState = ServerInterface.MileageMapState;
		if (mileageMapState != null && !ServerInterface.ChaoWheelOptions.IsConnected)
		{
			return true;
		}
		return false;
	}

	private bool IsRequestDailyBattle()
	{
		if (ServerInterface.LoggedInServerInterface != null)
		{
			GameObject x = GameObject.Find("ResultInfo");
			if (x == null)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckNextMap()
	{
		if (m_stageResultData != null && m_stageResultData.m_newMapState != null && m_stageResultData.m_oldMapState != null)
		{
			if (m_stageResultData.m_oldMapState.m_episode != m_stageResultData.m_newMapState.m_episode)
			{
				return true;
			}
			if (m_stageResultData.m_oldMapState.m_chapter != m_stageResultData.m_newMapState.m_chapter)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckEventStateRequest()
	{
		if (EventManager.Instance != null && EventManager.Instance.Type == EventManager.EventType.COLLECT_OBJECT && !EventManager.Instance.IsSetEventStateInfo)
		{
			return EventManager.Instance.IsInEvent();
		}
		return false;
	}

	private void CheckEventEnd()
	{
		if (EventManager.Instance != null && EventManager.Instance.Type != EventManager.EventType.UNKNOWN && !EventManager.Instance.IsInEvent())
		{
			EventManager.Instance.CheckEvent();
			if (ResourceManager.Instance != null)
			{
				ResourceManager.Instance.RemoveResources(ResourceCategory.EVENT_RESOURCE);
			}
		}
	}

	private void ServerGetChaoWheelOptions_Succeeded(MsgGetChaoWheelOptionsSucceed msg)
	{
		m_connected = true;
	}

	private void CallbackRouletteInit(int specialEggNum)
	{
		m_connected = true;
	}

	private void ServerGetInformation_Succeeded(MsgGetInformationSucceed msg)
	{
		m_is_end_notice_connect = true;
	}

	private void DailyBattleManagerCallBack()
	{
		m_connected = true;
	}

	private TinyFsmState StateInviteFriend(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CreateFirstLaunchInviteFriend();
			return TinyFsmState.End();
		case -4:
			if (m_fristLaunchInviteFriend != null)
			{
				UnityEngine.Object.Destroy(m_fristLaunchInviteFriend.gameObject);
			}
			return TinyFsmState.End();
		case 0:
			if (m_fristLaunchInviteFriend != null)
			{
				if (m_startLauncherInviteFriendFlag)
				{
					if (m_fristLaunchInviteFriend.IsEndPlay)
					{
						ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
					}
				}
				else
				{
					m_fristLaunchInviteFriend.Setup("Camera/menu_Anim/MainMenuUI4/Anchor_5_MC");
					m_fristLaunchInviteFriend.PlayStart();
					m_startLauncherInviteFriendFlag = true;
				}
			}
			else
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void CreateFirstLaunchInviteFriend()
	{
		if (m_fristLaunchInviteFriend == null)
		{
			GameObject gameObject = new GameObject();
			if (gameObject != null)
			{
				gameObject.name = "FirstLaunchInviteFriend";
				gameObject.AddComponent<FirstLaunchInviteFriend>();
				m_fristLaunchInviteFriend = gameObject.GetComponent<FirstLaunchInviteFriend>();
			}
		}
	}

	private TinyFsmState StatePlayItem(TinyFsmEvent fsm_event)
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
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null)
			{
				if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.MAIN)
				{
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.SHOP)
				{
					ChangeState(new TinyFsmState(StateShop), SequenceState.Shop);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.STAGE_CHECK)
				{
					ChangeState(new TinyFsmState(StateCheckStage), SequenceState.CheckStage);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.EPISODE)
				{
					ChangeState(new TinyFsmState(StateLoadMileageXml), SequenceState.LoadMileageXml);
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoad(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			TimeProfiler.StartCountTime("StateLoad");
			CeateSceneLoader();
			if (m_scene_loader_obj != null)
			{
				ResourceSceneLoader component = m_scene_loader_obj.GetComponent<ResourceSceneLoader>();
				TextManager.LoadCommonText(component);
				TextManager.LoadChaoText(component);
				TextManager.LoadEventText(component);
			}
			CeateSceneLoader();
			SetLoadEventResource();
			if (GameObject.Find("MissionTable") == null)
			{
				AddSceneLoader("MissionTable");
			}
			if (GameObject.Find("CharacterDataNameInfo") == null)
			{
				AddSceneLoader("CharacterDataNameInfo");
			}
			if (!IsExistMileageMapData(m_mileage_map_state))
			{
				AddSceneLoader(GetMileageMapDataScenaName(m_mileage_map_state));
			}
			if (GameObject.Find("MileageDataTable") == null)
			{
				AddSceneLoader("MileageDataTable");
			}
			string suffixe = TextUtility.GetSuffixe();
			if (GameObject.Find("ui_tex_ranking_" + suffixe) == null)
			{
				AddSceneLoader("ui_tex_ranking_" + suffixe);
			}
			if (InformationDataTable.Instance == null)
			{
				InformationDataTable.Create();
				InformationDataTable.Instance.Initialize(base.gameObject);
			}
			if (m_scene_loader_obj != null)
			{
				StageAbilityManager.LoadAbilityDataTable(m_scene_loader_obj.GetComponent<ResourceSceneLoader>());
			}
			AddSceneLoader("MainMenuPages");
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(6);
			}
			UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
			m_buttonEventResourceLoader = null;
			return TinyFsmState.End();
		case 0:
		{
			bool flag = true;
			if (m_buttonEventResourceLoader != null)
			{
				flag = m_buttonEventResourceLoader.IsLoaded;
			}
			if (flag && CheckSceneLoad())
			{
				TextManager.SetupCommonText();
				TextManager.SetupChaoText();
				TextManager.SetupEventText();
				if (m_eventSpecficText)
				{
					TextManager.SetupEventProductionText();
				}
				DestroySceneLoader();
				SetMainMenuPages();
				SetEventResources();
				StageAbilityManager.SetupAbilityDataTable();
				TimeProfiler.EndCountTime("StateLoad");
				ChangeState(new TinyFsmState(StateLoadAtlas), SequenceState.LoadAtlas);
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoadAtlas(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			TimeProfiler.StartCountTime("StateLoadAtlas");
			CeateSceneLoader();
			StartLoadAtlas();
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(7);
			}
			return TinyFsmState.End();
		case 0:
			TimeProfiler.EndCountTime("StateLoadAtlas");
			ChangeState(new TinyFsmState(StateStartMessage), SequenceState.StartMessage);
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private bool CheckSceneLoad()
	{
		if (m_scene_loader_obj != null)
		{
			ResourceSceneLoader component = m_scene_loader_obj.GetComponent<ResourceSceneLoader>();
			if (component != null)
			{
				return component.Loaded;
			}
		}
		return true;
	}

	private string GetMileageMapDataScenaName(ServerMileageMapState state)
	{
		if (state != null)
		{
			return "MileageMapData" + state.m_episode.ToString("000") + "_" + state.m_chapter.ToString("00");
		}
		return null;
	}

	private void CeateSceneLoader()
	{
		if (m_scene_loader_obj == null)
		{
			m_scene_loader_obj = new GameObject("SceneLoader");
			if (m_scene_loader_obj != null)
			{
				m_scene_loader_obj.AddComponent<ResourceSceneLoader>();
			}
		}
	}

	private void DestroySceneLoader()
	{
		UnityEngine.Object.Destroy(m_scene_loader_obj);
		m_scene_loader_obj = null;
	}

	private void AddSceneLoader(string scene_name)
	{
		if (scene_name != null && m_scene_loader_obj != null)
		{
			ResourceSceneLoader component = m_scene_loader_obj.GetComponent<ResourceSceneLoader>();
			if (component != null)
			{
				bool onAssetBundle = true;
				component.AddLoad(scene_name, onAssetBundle, false);
			}
		}
	}

	private void AddSceneLoaderAndResourceManager(ResourceSceneLoader.ResourceInfo resInfo)
	{
		if (m_scene_loader_obj != null)
		{
			ResourceSceneLoader component = m_scene_loader_obj.GetComponent<ResourceSceneLoader>();
			if (component != null)
			{
				component.AddLoadAndResourceManager(resInfo);
			}
		}
	}

	private void AddIDList(ref List<int> request_list, int id, string type, bool keep)
	{
		if (request_list == null || id == -1 || id == 0)
		{
			return;
		}
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			string text = string.Empty;
			if (type == "bg")
			{
				text = MileageMapBGDataTable.Instance.GetTextureName(id);
			}
			else if (type == "face")
			{
				text = MileageMapUtility.GetFaceTextureName(id);
			}
			if (text != string.Empty && keep)
			{
				instance.AddKeepList(text);
			}
		}
		foreach (int item in request_list)
		{
			if (item == id)
			{
				return;
			}
		}
		request_list.Add(id);
	}

	private void SetLoadingFaceTexture(ref List<int> requestFaceList, ref List<int> loadingList, MileageMapData mileageMapData, int windowId)
	{
		if (mileageMapData != null && windowId < mileageMapData.window_data.Length)
		{
			int face_id = mileageMapData.window_data[windowId].body[0].product[0].face_id;
			AddIDList(ref requestFaceList, face_id, "face", true);
			loadingList.Add(face_id);
		}
	}

	private void SetLoadWindowFaceTexture(ref List<int> requestFaceList, MileageMapData mileageMapData, ServerMileageMapState state = null)
	{
		if (mileageMapData == null)
		{
			return;
		}
		List<int> list = new List<int>();
		int num = mileageMapData.event_data.point.Length;
		if (state != null)
		{
			for (int i = 0; i < num; i++)
			{
				if (state.m_point <= i)
				{
					if (i != 5 || !mileageMapData.event_data.IsBossEvent())
					{
						list.Add(mileageMapData.event_data.point[i].window_id);
						continue;
					}
					BossEvent bossEvent = mileageMapData.event_data.GetBossEvent();
					list.Add(bossEvent.after_window_id);
				}
			}
		}
		else
		{
			for (int j = 0; j < num; j++)
			{
				if (m_prev_mileage_map_state.m_point <= j && m_mileage_map_state.m_point >= j)
				{
					if (j != 5 || !mileageMapData.event_data.IsBossEvent())
					{
						list.Add(mileageMapData.event_data.point[j].window_id);
					}
					else
					{
						list.Add(mileageMapData.event_data.point[j].boss.before_window_id);
					}
				}
			}
		}
		int num2 = mileageMapData.window_data.Length;
		for (int k = 0; k < num2; k++)
		{
			int num3 = mileageMapData.window_data[k].body.Length;
			for (int l = 0; l < num3; l++)
			{
				if (mileageMapData.window_data[k].body[l].product == null)
				{
					mileageMapData.window_data[k].body[l].product = new WindowProductData[0];
				}
				if (list.Contains(mileageMapData.window_data[k].id))
				{
					int num4 = mileageMapData.window_data[k].body[l].product.Length;
					for (int m = 0; m < num4; m++)
					{
						AddIDList(ref requestFaceList, mileageMapData.window_data[k].body[l].product[m].face_id, "face", false);
					}
				}
			}
		}
	}

	public void SetMainMenuPages()
	{
		GameObject gameObject = GameObject.Find("MainMenuPages");
		if (!(gameObject != null))
		{
			return;
		}
		GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
		GameObject mainMenuGeneralAnchor = HudMenuUtility.GetMainMenuGeneralAnchor();
		if (menuAnimUIObject != null && mainMenuGeneralAnchor != null)
		{
			Transform transform = gameObject.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(0);
				Vector3 localPosition = child.localPosition;
				Vector3 localScale = child.localScale;
				if (child.name == "item_get_Window" || child.name == "RankingWindowUI")
				{
					child.parent = mainMenuGeneralAnchor.transform;
				}
				else
				{
					child.parent = menuAnimUIObject.transform;
				}
				child.localPosition = localPosition;
				child.localScale = localScale;
				child.gameObject.SetActive(false);
			}
		}
		UnityEngine.Object.Destroy(gameObject);
	}

	private void SetLoadEventResource()
	{
		if (EventManager.Instance != null)
		{
			if (EventManager.Instance.IsQuickEvent())
			{
				string resourceName = EventManager.GetResourceName();
				ResourceSceneLoader.ResourceInfo resourceInfo = m_loadInfo[0];
				resourceInfo.m_scenename += resourceName;
				AddSceneLoaderAndResourceManager(resourceInfo);
				ResourceSceneLoader.ResourceInfo resourceInfo2 = m_loadInfo[1];
				resourceInfo2.m_scenename += resourceName;
				AddSceneLoaderAndResourceManager(resourceInfo2);
				m_eventResourceId = EventManager.Instance.Id;
				LoadNewsWindow();
			}
			else if (EventManager.Instance.IsBGMEvent())
			{
				string resourceName2 = EventManager.GetResourceName();
				ResourceSceneLoader.ResourceInfo resourceInfo3 = m_loadInfo[0];
				resourceInfo3.m_scenename += resourceName2;
				AddSceneLoaderAndResourceManager(resourceInfo3);
				m_eventResourceId = EventManager.Instance.Id;
				LoadNewsWindow();
			}
		}
	}

	private void LoadNewsWindow()
	{
		m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
		m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync("NewsWindow", ResourceLoadEndCallback);
	}

	private void SetLoadTopMenuTexture()
	{
		ResourceSceneLoader.ResourceInfo resourceInfo = m_loadInfo[2];
		int num = 1;
		if (StageModeManager.Instance != null)
		{
			num = StageModeManager.Instance.QuickStageIndex;
		}
		resourceInfo.m_scenename = "ui_tex_mile_w" + num.ToString("D2") + "A";
		AddSceneLoaderAndResourceManager(resourceInfo);
	}

	private void SetEventResources()
	{
		if (EventManager.Instance != null && EventManager.Instance.IsQuickEvent())
		{
			EventCommonDataTable.LoadSetup();
		}
	}

	private void StartLoadAtlas()
	{
		if (FontManager.Instance != null)
		{
			FontManager.Instance.LoadResourceData();
		}
		if (AtlasManager.Instance != null)
		{
			AtlasManager.Instance.StartLoadAtlasForMenu();
		}
		if (TextureAsyncLoadManager.Instance != null)
		{
			PlayerData playerData = SaveDataManager.Instance.PlayerData;
			TextureRequestChara request = new TextureRequestChara(playerData.MainChara, null);
			TextureRequestChara request2 = new TextureRequestChara(playerData.SubChara, null);
			TextureAsyncLoadManager.Instance.Request(request);
			TextureAsyncLoadManager.Instance.Request(request2);
			UnityEngine.Random.seed = NetUtil.GetCurrentUnixTime();
			int textureIndex = UnityEngine.Random.Range(0, TextureRequestEpisodeBanner.BannerCount);
			TextureRequestEpisodeBanner request3 = new TextureRequestEpisodeBanner(textureIndex, null);
			TextureAsyncLoadManager.Instance.Request(request3);
			StageModeManager instance = StageModeManager.Instance;
			if (instance != null)
			{
				instance.DrawQuickStageIndex();
				TextureRequestStagePicture request4 = new TextureRequestStagePicture(instance.QuickStageIndex, null);
				TextureAsyncLoadManager.Instance.Request(request4);
			}
		}
		if (ChaoTextureManager.Instance != null)
		{
			ChaoTextureManager.Instance.RequestLoadingPageChaoTexture();
		}
	}

	private bool IsLoadedAtlas()
	{
		if (AtlasManager.Instance != null && !AtlasManager.Instance.IsLoadAtlas())
		{
			return false;
		}
		return true;
	}

	private void SetReplaceAtlas()
	{
		if (FontManager.Instance != null)
		{
			FontManager.Instance.ReplaceFont();
		}
		if (AtlasManager.Instance != null)
		{
			AtlasManager.Instance.ReplaceAtlasForMenu(true);
		}
	}

	private TinyFsmState StateLoginBonusWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
			m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync("LoginWindowUI", ResourceLoadEndCallback);
			m_flags.Set(18, false);
			return TinyFsmState.End();
		case -4:
			UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
			m_buttonEventResourceLoader = null;
			return TinyFsmState.End();
		case 0:
			if (m_buttonEventResourceLoader.IsLoaded)
			{
				ChangeState(new TinyFsmState(StateLoginBonusWindowDisplay), SequenceState.LoginBonusWindowDisplay);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoginBonusWindowDisplay(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
			if (menuAnimUIObject != null)
			{
				m_LoginWindowUI = GameObjectUtil.FindChildGameObjectComponent<LoginBonusWindowUI>(menuAnimUIObject, "LoginWindowUI");
				if (m_LoginWindowUI != null)
				{
					m_LoginWindowUI.gameObject.SetActive(true);
					m_LoginWindowUI.PlayStart(LoginBonusWindowUI.LoginBonusType.NORMAL);
				}
			}
			m_flags.Set(17, false);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_LoginWindowUI != null && m_LoginWindowUI.IsEnd)
			{
				m_LoginWindowUI = null;
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateFirstLoginBonusWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
			m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync("StartDashWindowUI", ResourceLoadEndCallback);
			m_flags.Set(18, false);
			return TinyFsmState.End();
		case -4:
			UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
			m_buttonEventResourceLoader = null;
			return TinyFsmState.End();
		case 0:
			if (m_buttonEventResourceLoader.IsLoaded)
			{
				ChangeState(new TinyFsmState(StateFirstLoginBonusWindowDisplay), SequenceState.FirstLoginBonusWindowDisplay);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateFirstLoginBonusWindowDisplay(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
			if (menuAnimUIObject != null)
			{
				m_LoginWindowUI = GameObjectUtil.FindChildGameObjectComponent<LoginBonusWindowUI>(menuAnimUIObject, "StartDashWindowUI");
				if (m_LoginWindowUI != null)
				{
					m_LoginWindowUI.gameObject.SetActive(true);
					m_LoginWindowUI.PlayStart(LoginBonusWindowUI.LoginBonusType.FIRST);
				}
			}
			m_flags.Set(18, false);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_LoginWindowUI != null && m_LoginWindowUI.IsEnd)
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				m_LoginWindowUI = null;
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoginRanking(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_flags.Set(8, false);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			return TinyFsmState.End();
		case 1:
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null && msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.RANKING_END)
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateMainMenu(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			BossChallenge = false;
			m_flags.Set(4, false);
			m_flags.Set(5, false);
			m_flags.Set(28, false);
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
			HudMenuUtility.OnForceDisableShopButton(false);
			GameObjectUtil.SendMessageFindGameObject("MainMenuUI4", "UpdateView", null, SendMessageOptions.DontRequireReceiver);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			CheckTutoralWindow();
			return TinyFsmState.End();
		case 1:
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null && JudgeMainMenuReceiveEvent(msgMenuSequence))
			{
				m_communicateFlag.Reset();
			}
			return TinyFsmState.End();
		}
		case 102:
			CreateTitleBackWindow();
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateMainMenuConnect(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CheckEventParameter();
			m_communicateFlag.Set(0, IsTickerCommunicate());
			m_communicateFlag.Set(2, IsMsgBoxCommunicate());
			m_communicateFlag.Set(4, IsSchemeCommunicate());
			m_communicateFlag.Set(6, IsChangeDataVersion());
			if (!m_communicateFlag.Test(8))
			{
				m_communicateFlag.Set(7, true);
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			if (m_buttonEvent != null && !m_buttonEvent.IsTransform && m_flags.Test(21))
			{
				m_communicateFlag.Set(4, IsSchemeCommunicate());
				m_flags.Set(21, false);
			}
			bool flag = (m_stageResultData != null) ? true : false;
			bool flag2 = m_stageResultData != null && m_stageResultData.m_quickMode;
			bool flag3 = m_stageResultData != null && m_stageResultData.m_rivalHighScore;
			bool flag4 = false;
			if (SingletonGameObject<RankingManager>.Instance != null)
			{
				RankingUtil.RankChange rankingRankChange = SingletonGameObject<RankingManager>.Instance.GetRankingRankChange(RankingUtil.RankingMode.QUICK, RankingUtil.RankingScoreType.HIGH_SCORE, RankingUtil.RankingRankerType.RIVAL);
				flag4 = (rankingRankChange == RankingUtil.RankChange.UP);
			}
			if (m_flags.Test(25) && flag && flag2 && flag3)
			{
				ChangeState(new TinyFsmState(StateBestRecordCheckEnableFeed), SequenceState.BestRecordCheckEnableFeed);
				m_flags.Set(25, false);
			}
			else if (m_flags.Test(26) && flag && flag2 && flag3 && flag4)
			{
				m_debug++;
				m_flags.Set(26, false);
				ChangeState(new TinyFsmState(StateQuickModeRankUp), SequenceState.QuickModeRankUp);
			}
			else if (m_communicateFlag.Test(6))
			{
				ChangeState(new TinyFsmState(StateVersionChangeWindow), SequenceState.VersionChangeWindow);
			}
			else if (m_flags.Test(13))
			{
				ChangeState(new TinyFsmState(StateEventResourceChangeWindow), SequenceState.EventResourceChangeWindow);
			}
			else if (FirstLaunchUserName.IsFirstLaunch)
			{
				ChangeState(new TinyFsmState(StateUserNameSetting), SequenceState.UserNameSetting);
			}
			else if (!AgeVerification.IsAgeVerificated)
			{
				ChangeState(new TinyFsmState(StateAgeVerification), SequenceState.AgeVerification);
			}
			else if (m_communicateFlag.Test(0) && !m_communicateFlag.Test(1))
			{
				HudMenuUtility.SendMsgTickerReset();
				m_communicateFlag.Set(1, true);
				ChangeState(new TinyFsmState(StateTickerCommunicate), SequenceState.TickerCommunicate);
			}
			else if (m_communicateFlag.Test(4))
			{
				ChangeState(new TinyFsmState(StateSchemeCommunicate), SequenceState.SchemeCommunicate);
			}
			else if (m_communicateFlag.Test(2) && !m_communicateFlag.Test(3))
			{
				m_communicateFlag.Set(3, true);
				ChangeState(new TinyFsmState(StateMsgBoxCommunicate), SequenceState.MsgBoxCommunicate);
			}
			else if (m_communicateFlag.Test(7))
			{
				m_communicateFlag.Set(8, true);
				m_communicateFlag.Set(7, false);
				ChangeState(new TinyFsmState(StateDayCrossCommunicate), SequenceState.DayCrossCommunicate);
			}
			else if (m_communicateFlag.Test(9))
			{
				m_communicateFlag.Set(9, false);
				ChangeState(new TinyFsmState(MenuStateLoadEventResource), SequenceState.LoadEventResource);
			}
			else
			{
				m_communicateFlag.Set(7, false);
				m_communicateFlag.Set(8, false);
				m_flags.Set(20, true);
				if (m_flags.Test(18))
				{
					ChangeState(new TinyFsmState(StateFirstLoginBonusWindow), SequenceState.FirstLoginBonusWindow);
				}
				else if (m_flags.Test(17))
				{
					ChangeState(new TinyFsmState(StateLoginBonusWindow), SequenceState.LoginBonusWindow);
				}
				else if (m_flags.Test(16))
				{
					HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.DAILY_CHALLENGE);
					ChangeState(new TinyFsmState(StateDailyMissionWindow), SequenceState.DailyMissionWindow);
				}
				else if (SingletonGameObject<DailyBattleManager>.Instance != null && SingletonGameObject<DailyBattleManager>.Instance.currentRewardFlag)
				{
					ChangeState(new TinyFsmState(StateDailyBattleRewardWindow), SequenceState.DailyBattleRewardWindow);
				}
				else if (HaveNoticeInfo())
				{
					ChangeState(new TinyFsmState(StateInformationWindow), SequenceState.InformationWindow);
				}
				else if (HudMenuUtility.IsTutorialCharaLevelUp())
				{
					ChangeState(new TinyFsmState(StateTutorialCharaLevelUpMenuStart), SequenceState.TutorialCharaLevelUpMenuStart);
				}
				else if (HudMenuUtility.IsRouletteTutorial())
				{
					ChangeState(new TinyFsmState(StateTutorialMenuRoulette), SequenceState.TutorialMenuRoulette);
				}
				else if (HudMenuUtility.IsRecommendReviewTutorial())
				{
					ChangeState(new TinyFsmState(StateRecommendReview), SequenceState.RecommendReview);
				}
				else
				{
					ChangeState(new TinyFsmState(StateMainMenu), SequenceState.Main);
				}
			}
			return TinyFsmState.End();
		}
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateTickerCommunicate(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetTicker(base.gameObject);
			}
			else
			{
				m_communicateFlag.Set(0, false);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (!m_communicateFlag.Test(0))
			{
				HudMenuUtility.SendMsgTickerUpdate();
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateMsgBoxCommunicate(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetMessageList(base.gameObject);
			}
			else
			{
				m_communicateFlag.Set(2, false);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (!m_communicateFlag.Test(2))
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSchemeCommunicate(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (ServerAtomSerial.GetSerialFromScheme(GetUrlSchemeStr(), ref m_atomCampain, ref m_atomSerial))
			{
				CreateSchemeCheckWinow();
			}
			else
			{
				m_communicateFlag.Set(4, false);
			}
			return TinyFsmState.End();
		case -4:
			ClearUrlScheme();
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("SchemeCheck") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				bool new_user = true;
				SystemSaveManager instance = SystemSaveManager.Instance;
				if (instance != null)
				{
					SystemData systemdata = instance.GetSystemdata();
					if (systemdata != null)
					{
						new_user = systemdata.IsNewUser();
					}
				}
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				loggedInServerInterface.RequestServerAtomSerial(m_atomCampain, m_atomSerial, new_user, base.gameObject);
			}
			if (!m_communicateFlag.Test(4))
			{
				ChangeState(new TinyFsmState(StateResultSchemeCommunicate), SequenceState.ResultSchemeCommunicate);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateResultSchemeCommunicate(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CreateSchemeResultWinow();
			return TinyFsmState.End();
		case -4:
			ClearUrlScheme();
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("SchemeResult") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateDayCrossCommunicate(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			m_callBackFlag.Reset();
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null && ServerDayCrossWatcher.Instance != null)
			{
				ServerDayCrossWatcher.Instance.UpdateClientInfosByDayCross(DataCrossCallBack);
				ServerDayCrossWatcher.Instance.UpdateDailyMissionForOneDay(DailyMissionForOneDataCallBack);
				ServerDayCrossWatcher.Instance.UpdateDailyMissionInfoByChallengeEnd(DailyMissionChallengeEndCallBack);
				ServerDayCrossWatcher.Instance.UpdateLoginBonusEnd(LoginBonusUpdateCallBack);
			}
			else
			{
				m_callBackFlag.Set(0, true);
				m_callBackFlag.Set(2, true);
				m_callBackFlag.Set(4, true);
				m_callBackFlag.Set(7, true);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_callBackFlag.Test(0) && m_callBackFlag.Test(2) && m_callBackFlag.Test(4) && m_callBackFlag.Test(7))
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void CreateSchemeCheckWinow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "SchemeCheck";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "atom_check");
		info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "atom_check_caption");
		GeneralWindow.Create(info);
	}

	private void CreateSchemeResultWinow()
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (m_communicateFlag.Test(5))
		{
			empty = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", m_atomInvalidTextId);
			empty2 = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "atom_failure_caption");
		}
		else
		{
			empty = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "atom_present_get");
			empty2 = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "atom_success_caption");
		}
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "SchemeResult";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.message = empty;
		info.caption = empty2;
		GeneralWindow.Create(info);
	}

	private bool JudgeMainMenuReceiveEvent(MsgMenuSequence msg_sequence)
	{
		switch (msg_sequence.Sequenece)
		{
		case MsgMenuSequence.SequeneceType.TITLE:
			ChangeState(new TinyFsmState(StateTitle), SequenceState.Title);
			return true;
		case MsgMenuSequence.SequeneceType.STAGE:
			ChangeState(new TinyFsmState(StateStage), SequenceState.Stage);
			return true;
		case MsgMenuSequence.SequeneceType.CHARA_MAIN:
			ChangeState(new TinyFsmState(StateMainCharaSelect), SequenceState.CharaSelect);
			return true;
		case MsgMenuSequence.SequeneceType.CHAO:
			ChangeState(new TinyFsmState(StateChaoSelect), SequenceState.ChaoSelect);
			return true;
		case MsgMenuSequence.SequeneceType.OPTION:
			ChangeState(new TinyFsmState(StateOption), SequenceState.Option);
			return true;
		case MsgMenuSequence.SequeneceType.INFOMATION:
			ChangeState(new TinyFsmState(StateInfomation), SequenceState.Infomation);
			return true;
		case MsgMenuSequence.SequeneceType.ROULETTE:
			ChangeState(new TinyFsmState(StateRoulette), SequenceState.Roulette);
			return true;
		case MsgMenuSequence.SequeneceType.SHOP:
			ChangeState(new TinyFsmState(StateShop), SequenceState.Shop);
			return true;
		case MsgMenuSequence.SequeneceType.PRESENT_BOX:
			ChangeState(new TinyFsmState(StatePresentBox), SequenceState.PresentBox);
			return true;
		case MsgMenuSequence.SequeneceType.DAILY_CHALLENGE:
			ChangeState(new TinyFsmState(StateDailyMissionWindow), SequenceState.DailyMissionWindow);
			return true;
		case MsgMenuSequence.SequeneceType.EPISODE_PLAY:
		case MsgMenuSequence.SequeneceType.MAIN_PLAY_BUTTON:
		{
			StageModeManager instance3 = StageModeManager.Instance;
			if (instance3 != null)
			{
				instance3.StageMode = StageModeManager.Mode.ENDLESS;
			}
			ChangeState(new TinyFsmState(MenuStatePlayButton), SequenceState.PlayButton);
			return true;
		}
		case MsgMenuSequence.SequeneceType.QUICK:
		{
			StageModeManager instance2 = StageModeManager.Instance;
			if (instance2 != null)
			{
				instance2.StageMode = StageModeManager.Mode.QUICK;
			}
			ChangeState(new TinyFsmState(MenuStatePlayButton), SequenceState.PlayButton);
			return true;
		}
		case MsgMenuSequence.SequeneceType.EPISODE:
		{
			StageModeManager instance = StageModeManager.Instance;
			if (instance != null)
			{
				instance.StageMode = StageModeManager.Mode.ENDLESS;
			}
			ChangeState(new TinyFsmState(StateLoadMileageXml), SequenceState.LoadMileageXml);
			return true;
		}
		case MsgMenuSequence.SequeneceType.DAILY_BATTLE:
			ChangeState(new TinyFsmState(StateDailyBattle), SequenceState.DailyBattle);
			return true;
		case MsgMenuSequence.SequeneceType.EPISODE_RANKING:
		case MsgMenuSequence.SequeneceType.QUICK_RANKING:
			ChangeState(new TinyFsmState(StateRanking), SequenceState.Ranking);
			return true;
		case MsgMenuSequence.SequeneceType.BACK:
			ChangeState(new TinyFsmState(StateCheckBackTitle), SequenceState.CheckBackTitle);
			return true;
		default:
			return false;
		}
	}

	private void ServerGetTicker_Succeeded(MsgGetTickerDataSucceed msg)
	{
		SetTickerCommunicate();
		m_communicateFlag.Set(0, false);
	}

	private void ServerGetTicker_Failed(MsgServerConnctFailed msg)
	{
		m_communicateFlag.Set(0, false);
	}

	private void ServerGetMessageList_Succeeded(MsgGetMessageListSucceed msg)
	{
		if (m_communicateFlag.Test(2))
		{
			SetMsgBoxCommunicate(false);
			m_communicateFlag.Set(2, false);
		}
		m_flags.Set(19, false);
	}

	private void ServerGetMessageList_Failed(MsgServerConnctFailed msg)
	{
		m_communicateFlag.Set(2, false);
	}

	private void ServerAtomSerial_Succeeded(MsgSendAtomSerialSucceed msg)
	{
		m_communicateFlag.Set(4, false);
		m_communicateFlag.Set(5, false);
		SetMsgBoxCommunicate(true);
	}

	private void ServerAtomSerial_Failed(MsgServerConnctFailed msg)
	{
		m_communicateFlag.Set(4, false);
		m_communicateFlag.Set(5, true);
		m_atomInvalidTextId = "atom_invalid_serial";
		if (msg != null && msg.m_status == ServerInterface.StatusCode.UsedSerialCode)
		{
			m_atomInvalidTextId = "atom_used_serial";
		}
	}

	private void DataCrossCallBack(ServerDayCrossWatcher.MsgDayCross msg)
	{
		if (msg != null && msg.ServerConnect)
		{
			m_callBackFlag.Set(1, true);
		}
		m_callBackFlag.Set(0, true);
	}

	private void DailyMissionForOneDataCallBack(ServerDayCrossWatcher.MsgDayCross msg)
	{
		if (msg != null && msg.ServerConnect)
		{
			m_callBackFlag.Set(3, true);
		}
		m_callBackFlag.Set(2, true);
	}

	private void DailyMissionChallengeEndCallBack(ServerDayCrossWatcher.MsgDayCross msg)
	{
		if (msg != null && msg.ServerConnect)
		{
			m_callBackFlag.Set(5, true);
		}
		m_callBackFlag.Set(4, true);
	}

	private void LoginBonusUpdateCallBack(ServerDayCrossWatcher.MsgDayCross msg)
	{
		if (msg == null)
		{
			return;
		}
		if (msg.ServerConnect)
		{
			m_callBackFlag.Set(6, true);
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (!(loggedInServerInterface != null))
			{
				return;
			}
			ServerLoginBonusData loginBonusData = ServerInterface.LoginBonusData;
			if (loginBonusData != null)
			{
				if (!loginBonusData.isGetLoginBonusToday())
				{
					loggedInServerInterface.RequestServerLoginBonusSelect(loginBonusData.m_rewardId, loginBonusData.m_rewardDays, 0, loginBonusData.m_firstRewardDays, 0, base.gameObject);
				}
				else
				{
					m_callBackFlag.Set(7, true);
				}
			}
		}
		else
		{
			m_callBackFlag.Set(7, true);
		}
	}

	private void ServerLoginBonusSelect_Succeeded(MsgLoginBonusSelectSucceed msg)
	{
		bool flag = false;
		if (msg.m_loginBonusReward != null)
		{
			m_flags.Set(17, true);
			flag = true;
		}
		if (msg.m_firstLoginBonusReward != null)
		{
			m_flags.Set(18, true);
			flag = true;
		}
		m_callBackFlag.Set(7, true);
		if (flag)
		{
			SetMsgBoxCommunicate(true);
		}
	}

	private bool IsTickerCommunicate()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerTickerInfo tickerInfo = ServerInterface.TickerInfo;
			if (tickerInfo != null && tickerInfo.ExistNewData)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsMsgBoxCommunicate()
	{
		if (SaveDataManager.Instance != null && SaveDataManager.Instance.ConnectData != null)
		{
			return SaveDataManager.Instance.ConnectData.ReplaceMessageBox;
		}
		return false;
	}

	private bool IsSchemeCommunicate()
	{
		if (Binding.Instance != null)
		{
			string urlSchemeStr = Binding.Instance.GetUrlSchemeStr();
			return !string.IsNullOrEmpty(urlSchemeStr);
		}
		return false;
	}

	private string GetUrlSchemeStr()
	{
		if (Binding.Instance != null)
		{
			return Binding.Instance.GetUrlSchemeStr();
		}
		return string.Empty;
	}

	private void ClearUrlScheme()
	{
		if (Binding.Instance != null)
		{
			Binding.Instance.ClearUrlSchemeStr();
		}
	}

	private bool IsChangeDataVersion()
	{
		if (ServerInterface.LoginState != null)
		{
			if (ServerInterface.LoginState.IsChangeDataVersion)
			{
				return true;
			}
			if (ServerInterface.LoginState.IsChangeAssetsVersion)
			{
				return true;
			}
		}
		return false;
	}

	private void SetTickerCommunicate()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerTickerInfo tickerInfo = ServerInterface.TickerInfo;
			if (tickerInfo != null)
			{
				tickerInfo.ExistNewData = false;
			}
		}
	}

	private void SetMsgBoxCommunicate(bool flag)
	{
		if (SaveDataManager.Instance != null && SaveDataManager.Instance.ConnectData != null)
		{
			SaveDataManager.Instance.ConnectData.ReplaceMessageBox = flag;
		}
	}

	private void CheckLimitEvent()
	{
		if (EventManager.Instance == null)
		{
			return;
		}
		if (EventManager.Instance.IsStandby())
		{
			m_flags.Set(22, true);
			if (EventManager.Instance.IsInEvent())
			{
				EventManager.Instance.CheckEvent();
				if (m_eventResourceId > 0 && m_eventResourceId != EventManager.Instance.Id)
				{
					m_flags.Set(13, true);
				}
				else
				{
					m_communicateFlag.Set(9, true);
				}
			}
		}
		else
		{
			if (EventManager.Instance.Type == EventManager.EventType.UNKNOWN || EventManager.Instance.IsInEvent())
			{
				return;
			}
			EventManager.EventType type = EventManager.Instance.Type;
			EventManager.Instance.CheckEvent();
			if (EventManager.Instance.Type != EventManager.EventType.UNKNOWN && EventManager.Instance.IsInEvent())
			{
				m_flags.Set(13, true);
				return;
			}
			if (type == EventManager.EventType.QUICK)
			{
				StageModeManager.Instance.DrawQuickStageIndex();
			}
			m_communicateFlag.Set(9, true);
			if (AtlasManager.Instance != null)
			{
				AtlasManager.Instance.ResetEventRelaceAtlas();
			}
			if (ResourceManager.Instance != null)
			{
				ResourceManager.Instance.RemoveResources(ResourceCategory.EVENT_RESOURCE);
			}
		}
	}

	private void CheckEventParameter()
	{
		SetEventStage(false);
		CheckLimitEvent();
	}

	private void SetMilageStageIndex()
	{
		int episode = 1;
		int chapter = 1;
		int type = 0;
		if (m_mileage_map_state != null)
		{
			episode = m_mileage_map_state.m_episode;
			chapter = m_mileage_map_state.m_chapter;
			type = m_mileage_map_state.m_point;
		}
		MileageMapUtility.SetMileageStageIndex(episode, chapter, (PointType)type);
	}

	private void ServerGetMileageReward_Succeeded(MsgGetMileageRewardSucceed msg)
	{
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			if (msg != null)
			{
				instance.SetRewardData(msg.m_mileageRewardList);
			}
			else
			{
				instance.SetRewardData(ServerInterface.MileageRewardList);
			}
		}
		m_flags.Set(12, true);
	}

	private void ServerGetMileageReward_Failed()
	{
	}

	private TinyFsmState StateOption(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (!m_flags.Test(0))
			{
				ConnectAlertMaskUI.EndScreen(OnFinishedFadeInCallback);
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null)
			{
				if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.MAIN)
				{
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.STAGE)
				{
					m_flags.Set(15, true);
					ChangeState(new TinyFsmState(StateStage), SequenceState.Stage);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.TITLE)
				{
					ChangeState(new TinyFsmState(StateTitle), SequenceState.Title);
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState MenuStatePlayButton(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_cautionType = GetCautionType();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_cautionType != CautionType.CHALLENGE_COUNT)
			{
				m_bossChallenge = MileageMapUtility.IsBossStage();
				HudMenuUtility.SendVirtualNewItemSelectClicked();
				ChangeState(new TinyFsmState(StatePlayItem), SequenceState.PlayItem);
			}
			else
			{
				ChangeState(new TinyFsmState(StateChallengeDisplyWindow), SequenceState.ChallengeDisplyWindow);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateChallengeDisplyWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CreateStageCautionWindow();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			switch (m_pressedButtonType)
			{
			case PressedButtonType.GOTO_SHOP:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHALLENGE_TO_SHOP);
				ChangeState(new TinyFsmState(StateShop), SequenceState.Shop);
				break;
			case PressedButtonType.BACK:
			case PressedButtonType.CANCEL:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.ITEM_BACK);
				if (m_flags.Test(28))
				{
					ChangeState(new TinyFsmState(StateLoadMileageXml), SequenceState.LoadMileageXml);
				}
				else
				{
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
				break;
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StatePresentBox(TinyFsmEvent fsm_event)
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
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null)
			{
				if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.MAIN)
				{
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.CHARA_MAIN)
				{
					ChangeState(new TinyFsmState(StateMainCharaSelect), SequenceState.CharaSelect);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.CHAO)
				{
					ChangeState(new TinyFsmState(StateChaoSelect), SequenceState.ChaoSelect);
				}
				else if (msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.SHOP)
				{
					ChangeState(new TinyFsmState(StateShop), SequenceState.Shop);
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateUserNameSetting(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
			m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync("window_name_setting", ResourceLoadEndCallback);
			BackKeyManager.InvalidFlag = true;
			return TinyFsmState.End();
		case -4:
			UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
			m_buttonEventResourceLoader = null;
			return TinyFsmState.End();
		case 0:
			if (m_buttonEventResourceLoader.IsLoaded)
			{
				ChangeState(new TinyFsmState(StateUserNameSettingDisplay), SequenceState.UserNameSettingDisplay);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateUserNameSettingDisplay(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (m_userNameSetting == null)
			{
				GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
				if (menuAnimUIObject != null)
				{
					m_userNameSetting = GameObjectUtil.FindChildGameObjectComponent<FirstLaunchUserName>(menuAnimUIObject, "window_name_setting");
				}
			}
			if (m_userNameSetting != null)
			{
				m_userNameSetting.Setup(string.Empty);
				m_userNameSetting.PlayStart();
			}
			return TinyFsmState.End();
		case -4:
			BackKeyManager.InvalidFlag = false;
			return TinyFsmState.End();
		case 0:
			if (m_userNameSetting != null && m_userNameSetting.IsEndPlay)
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateAgeVerification(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
			m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync("AgeVerificationWindow", ResourceLoadEndCallback);
			m_flags.Set(18, false);
			BackKeyManager.InvalidFlag = true;
			return TinyFsmState.End();
		case -4:
			UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
			m_buttonEventResourceLoader = null;
			return TinyFsmState.End();
		case 0:
			if (m_buttonEventResourceLoader.IsLoaded)
			{
				ChangeState(new TinyFsmState(StateAgeVerificationDisplay), SequenceState.AgeVerificationDisplay);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateAgeVerificationDisplay(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (m_ageVerification == null)
			{
				GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
				if (menuAnimUIObject != null)
				{
					m_ageVerification = GameObjectUtil.FindChildGameObjectComponent<AgeVerification>(menuAnimUIObject, "AgeVerificationWindow");
				}
			}
			if (m_ageVerification != null)
			{
				m_ageVerification.Setup(string.Empty);
				m_ageVerification.PlayStart();
			}
			return TinyFsmState.End();
		case -4:
			BackKeyManager.InvalidFlag = false;
			return TinyFsmState.End();
		case 0:
			if (m_ageVerification != null && m_ageVerification.IsEnd)
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateQuickModeRankUp(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
			m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync("RankingResultBitWindow", ResourceLoadEndCallback);
			return TinyFsmState.End();
		case -4:
			UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
			m_buttonEventResourceLoader = null;
			return TinyFsmState.End();
		case 0:
			if (m_buttonEventResourceLoader.IsLoaded)
			{
				ChangeState(new TinyFsmState(StateQuickModeRankUpDisplay), SequenceState.QuickModeRankUpDisplay);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateQuickModeRankUpDisplay(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			RankingUtil.ShowRankingChangeWindow(RankingUtil.RankingMode.QUICK);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (RankingUtil.IsEndRankingChangeWindow())
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRanking(TinyFsmEvent fsm_event)
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
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null && msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.MAIN)
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRecommendReview(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CreateFirstLaunchRecommendReview();
			return TinyFsmState.End();
		case -4:
			if (m_fristLaunchRecommendReview != null)
			{
				UnityEngine.Object.Destroy(m_fristLaunchRecommendReview.gameObject);
			}
			return TinyFsmState.End();
		case 0:
			if (m_fristLaunchRecommendReview != null)
			{
				if (m_startLauncherRecommendReviewFlag)
				{
					if (m_fristLaunchRecommendReview.IsEndPlay)
					{
						ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
					}
				}
				else
				{
					m_fristLaunchRecommendReview.Setup("Camera/menu_Anim/MainMenuUI4/Anchor_5_MC");
					m_fristLaunchRecommendReview.PlayStart();
					m_startLauncherRecommendReviewFlag = true;
				}
			}
			else
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void CreateFirstLaunchRecommendReview()
	{
		if (m_fristLaunchRecommendReview == null)
		{
			GameObject gameObject = new GameObject();
			if (gameObject != null)
			{
				gameObject.name = "FirstLaunchRecommendReview";
				gameObject.AddComponent<FirstLaunchRecommendReview>();
				m_fristLaunchRecommendReview = gameObject.GetComponent<FirstLaunchRecommendReview>();
			}
		}
	}

	private TinyFsmState StateRoulette(TinyFsmEvent fsm_event)
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
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null && msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.MAIN)
			{
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateShop(TinyFsmEvent fsm_event)
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
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null)
			{
				switch (msgMenuSequence.Sequenece)
				{
				case MsgMenuSequence.SequeneceType.MAIN:
					ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
					break;
				case MsgMenuSequence.SequeneceType.CHARA_MAIN:
					ChangeState(new TinyFsmState(StateMainCharaSelect), SequenceState.CharaSelect);
					break;
				case MsgMenuSequence.SequeneceType.CHAO:
					ChangeState(new TinyFsmState(StateChaoSelect), SequenceState.ChaoSelect);
					break;
				case MsgMenuSequence.SequeneceType.OPTION:
					ChangeState(new TinyFsmState(StateOption), SequenceState.Option);
					break;
				case MsgMenuSequence.SequeneceType.INFOMATION:
					ChangeState(new TinyFsmState(StateInfomation), SequenceState.Infomation);
					break;
				case MsgMenuSequence.SequeneceType.ROULETTE:
					ChangeState(new TinyFsmState(StateRoulette), SequenceState.Roulette);
					break;
				case MsgMenuSequence.SequeneceType.PRESENT_BOX:
					ChangeState(new TinyFsmState(StatePresentBox), SequenceState.PresentBox);
					break;
				case MsgMenuSequence.SequeneceType.PLAY_ITEM:
				case MsgMenuSequence.SequeneceType.EPISODE_PLAY:
				case MsgMenuSequence.SequeneceType.QUICK:
				case MsgMenuSequence.SequeneceType.PLAY_AT_EPISODE_PAGE:
					ChangeState(new TinyFsmState(MenuStatePlayButton), SequenceState.PlayButton);
					break;
				case MsgMenuSequence.SequeneceType.EPISODE:
					ChangeState(new TinyFsmState(StateLoadMileageXml), SequenceState.LoadMileageXml);
					break;
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateCheckStage(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_cautionType = GetCautionType();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_cautionType == CautionType.NON)
			{
				ChangeState(new TinyFsmState(StateStage), SequenceState.Stage);
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.GO_STAGE);
			}
			else
			{
				ChangeState(new TinyFsmState(StateStageCautionWindow), SequenceState.CautionStage);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateStageCautionWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CreateStageCautionWindow();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			switch (m_pressedButtonType)
			{
			case PressedButtonType.NEXT_STATE:
				ChangeState(new TinyFsmState(StateStage), SequenceState.Stage);
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.GO_STAGE);
				break;
			case PressedButtonType.GOTO_SHOP:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHALLENGE_TO_SHOP);
				ChangeState(new TinyFsmState(StateShop), SequenceState.Shop);
				break;
			case PressedButtonType.BACK:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.FORCE_MAIN_BACK, true);
				ChangeState(new TinyFsmState(StateMainMenuConnect), SequenceState.MainConnect);
				break;
			case PressedButtonType.CANCEL:
				ChangeState(new TinyFsmState(StatePlayItem), SequenceState.PlayItem);
				break;
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateStage(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_flags.Set(3, true);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			LoadEventFaceTexture();
			ChangeState(new TinyFsmState(StateFadeOut), SequenceState.FadeOut);
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void LoadEventFaceTexture()
	{
		if (m_flags.Test(4) || m_flags.Test(5))
		{
			string eventResourceLoadingTextureName = EventUtility.GetEventResourceLoadingTextureName();
			if (eventResourceLoadingTextureName != null)
			{
				CeateSceneLoader();
				AddSceneLoader(eventResourceLoadingTextureName);
			}
		}
	}

	private TinyFsmState StateStartMessage(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			TimeProfiler.StartCountTime("StateStartMessage");
			RouletteUtility.rouletteDefault = RouletteCategory.PREMIUM;
			SetMilageStageIndex();
			HudMenuUtility.OnForceDisableShopButton(true);
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(8);
			}
			return TinyFsmState.End();
		case 0:
			TimeProfiler.EndCountTime("StateStartMessage");
			ChangeState(new TinyFsmState(StateRankingWait), SequenceState.RankingWait);
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRankingWait(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
		{
			TimeProfiler.StartCountTime("StateRankingWait");
			m_rankingCallBack = false;
			m_eventRankingCallBack = false;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null && SingletonGameObject<RankingManager>.Instance != null)
			{
				SingletonGameObject<RankingManager>.Instance.Init(NormalRankingDataCallback, EventDataCallback);
			}
			else
			{
				m_rankingCallBack = true;
				m_eventRankingCallBack = true;
			}
			GeneralUtil.SetDailyBattleBtnIcon();
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(9);
			}
			return TinyFsmState.End();
		case 0:
			if (m_rankingCallBack && m_eventRankingCallBack && IsLoadedAtlas() && CheckSceneLoad())
			{
				DestroySceneLoader();
				SetReplaceAtlas();
				TimeProfiler.EndCountTime("StateRankingWait");
				ChangeState(new TinyFsmState(StateFadeIn), SequenceState.FadeIn);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void NormalRankingDataCallback(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData)
	{
		m_rankingCallBack = true;
		RankingUI.Setup();
		bool flag = false;
		if (EventManager.Instance != null && EventManager.Instance.IsInEvent() && EventManager.Instance.Type == EventManager.EventType.SPECIAL_STAGE)
		{
			flag = true;
		}
		if (!flag)
		{
			m_eventRankingCallBack = true;
		}
	}

	private void EventDataCallback(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData)
	{
		m_eventRankingCallBack = true;
	}

	private TinyFsmState StateTitle(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_flags.Set(3, false);
			m_flags.Set(4, false);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			ChangeState(new TinyFsmState(StateFadeOut), SequenceState.FadeOut);
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateCheckBackTitle(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CreateTitleBackWindow();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("BackTitle") && GeneralWindow.IsButtonPressed)
			{
				if (GeneralWindow.IsYesButtonPressed)
				{
					ChangeState(new TinyFsmState(StateTitle), SequenceState.Title);
				}
				else
				{
					ChangeState(new TinyFsmState(StateMainMenu), SequenceState.Main);
				}
				GeneralWindow.Close();
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateTutorialCharaLevelUpMenuStart(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CreateCharaLevelUpWinow();
			BackKeyManager.TutorialFlag = true;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("chara_level_up") && GeneralWindow.IsOkButtonPressed)
			{
				GeneralWindow.Close();
				ChangeState(new TinyFsmState(StateTutorialCharaLevelUpMenuMoveChara), SequenceState.TutorialCharaLevelUpMenuMoveChara);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateTutorialCharaLevelUpMenuMoveChara(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			TutorialCursor.StartTutorialCursor(TutorialCursor.Type.CHAOSELECT_MAIN);
			return TinyFsmState.End();
		case -4:
			BackKeyManager.TutorialFlag = false;
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.CHAOSELECT_MAIN);
			return TinyFsmState.End();
		case 0:
			CheckTutoralWindow();
			return TinyFsmState.End();
		case 1:
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null && msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.CHARA_MAIN)
			{
				ChangeState(new TinyFsmState(StateMainCharaSelect), SequenceState.CharaSelect);
			}
			return TinyFsmState.End();
		}
		case 102:
			CreateTitleBackWindow();
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void CreateCharaLevelUpWinow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "chara_level_up";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextUtility.GetCommonText("MainMenu", "chara_level_up_move_explan_caption");
		info.message = TextUtility.GetCommonText("MainMenu", "chara_level_up_move_explan");
		GeneralWindow.Create(info);
	}

	private TinyFsmState StateTutorialMenuRoulette(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			TutorialCursor.StartTutorialCursor(TutorialCursor.Type.MAINMENU_ROULETTE);
			BackKeyManager.InvalidFlag = true;
			return TinyFsmState.End();
		case -4:
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.MAINMENU_ROULETTE);
			BackKeyManager.InvalidFlag = false;
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
		{
			MsgMenuSequence msgMenuSequence = fsm_event.GetMessage as MsgMenuSequence;
			if (msgMenuSequence != null && msgMenuSequence.Sequenece == MsgMenuSequence.SequeneceType.ROULETTE)
			{
				ChangeState(new TinyFsmState(StateRoulette), SequenceState.Roulette);
			}
			return TinyFsmState.End();
		}
		case 102:
			CreateTitleBackWindow();
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private bool CheckTutorialRoulette()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			return RouletteUtility.isTutorial;
		}
		return false;
	}

	private void SetTutorialFlagToMainMenuUI()
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuUI4", "OnSetTutorial", null, SendMessageOptions.DontRequireReceiver);
	}

	private TinyFsmState StateVersionChangeWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CreateVersionChangeWindow(false);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("VersionChange") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				ChangeState(new TinyFsmState(StateTitle), SequenceState.Title);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEventResourceChangeWindow(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			CreateVersionChangeWindow(true);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("VersionChange") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				ChangeState(new TinyFsmState(StateTitle), SequenceState.Title);
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void CreateVersionChangeWindow(bool eventFlag)
	{
		string name = "VersionChange";
		if (!GeneralWindow.IsCreated(name))
		{
			if (eventFlag)
			{
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				info.name = name;
				info.buttonType = GeneralWindow.ButtonType.Ok;
				info.caption = TextUtility.GetCommonText("Option", "take_over_attention");
				info.message = TextUtility.GetCommonText("MainMenu", "title_back_message");
				GeneralWindow.Create(info);
			}
			else
			{
				GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
				info2.name = name;
				info2.buttonType = GeneralWindow.ButtonType.Ok;
				info2.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_reboot_app_caption");
				info2.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_reboot_app");
				GeneralWindow.Create(info2);
			}
		}
	}
}
