using AnimationOrTween;
using App;
using DataTable;
using Message;
using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class GameModeTitle : MonoBehaviour
{
	public enum ProgressBarLeaveState
	{
		IDLE = -1,
		StateEndDataLoad,
		StateGameServerLogin,
		StateGetServerContinueParameter,
		StateCheckAtom,
		StateCheckNoLoginIncentive,
		StateSnsAdditionalData,
		StateWaitToGetMenuData_PlayerState,
		StateWaitToGetMenuData_CharacterState,
		StateWaitToGetMenuData_ChaoState,
		StateWaitToGetMenuData_WheelOptions,
		StateWaitToGetMenuData_DailyMissionData,
		StateWaitToGetMenuData_MessageList,
		StateWaitToGetMenuData_RedStarExchangeList,
		StateWaitToGetMenuData_RingExchangeList,
		StateWaitToGetMenuData_ChallengeExchangeList,
		StateWaitToGetMenuData_RaidBossEnergyExchangeList,
		StateWaitToGetMenuData,
		StateAchievementLogin,
		StateGetLeagueData,
		StateGetCostList,
		StateGetEventList,
		StateGetMileageMap,
		StateIapInitialize,
		StateLoadEventResource,
		StateLoadingUIData,
		NUM
	}

	private enum EventSignal
	{
		SCENE_CHANGE_REQUESTED = 100,
		SERVER_GETVERSION_END,
		SERVER_GET_CONTINUE_PARAMETER_END,
		SERVER_GETMENUDATA_END,
		SERVER_GET_RANKING_END,
		SERVER_GET_LEAGUE_DATA_END,
		SERVER_GET_COSTLIST_END,
		SERVER_GET_MILEAGEMAP_END,
		SERVER_GET_EVENT_LIST_END,
		FADE_END,
		SCREEN_TOUCHED,
		TAKEOVER_REQUESTED,
		TAKEOVER_ERROR,
		TAKEOVER_PASSERROR
	}

	private class AtomDataInfo
	{
		public string campain;

		public string serial;
	}

	private enum RedStarExchangeType
	{
		RSRING,
		RING,
		CHALLENGE,
		RAIDBOSS_ENERGY,
		Count
	}

	private enum SubStateSaveError
	{
		ShowMessage,
		Error
	}

	private enum SubStateTakeover
	{
		CautionWindow,
		InputIdAndPass,
		End
	}

	private enum SubStateCheckAtom
	{
		StartText,
		WaitServer,
		EndText,
		End
	}

	private enum SubStateCheckNoLogin
	{
		WaitServer,
		EndText,
		End
	}

	private const string GameSceneName = "s_playingstage";

	private const string MainMenuSceneName = "MainMenu";

	private HudProgressBar m_progressBar;

	private static bool s_first = true;

	private readonly string ANCHOR_PATH = "Camera/menu_Anim/MainMenuUI4/Anchor_5_MC";

	private readonly string VersionStr = "Ver:" + CurrentBundleVersion.version.ToString();

	private string m_nextSceneName;

	private SettingPartsPushNotice m_pushNotice;

	private SettingTakeoverInput m_takeoverInput;

	private bool m_isTakeoverLogin;

	private float m_timer;

	private bool m_isGetCountry;

	private static readonly float TAKEOVER_WAIT_TIME = 2f;

	private ResourceSceneLoader.ResourceInfo m_loadInfoForEvent = new ResourceSceneLoader.ResourceInfo(ResourceCategory.EVENT_RESOURCE, "EventResourceCommon", true, false, true, "EventResourceCommon");

	private TinyFsmBehavior m_fsm;

	private StageInfo m_stageInfo;

	private TitleDataLoader m_loader;

	private static bool m_isLogined;

	private bool m_isSessionValid;

	private bool m_isFirstTutorial;

	private static bool m_isReturnFirstTutorial;

	private GameObject m_loginLabel;

	private GameObject m_touchScreenObject;

	private GameObject m_startButton;

	private GameObject m_movetButton;

	private GameObject m_cacheButton;

	private GameObject m_sceneLoader;

	private UILabel m_userIdLabel;

	private SettingPartsSnsLogin m_snsLogin;

	private HudLoadingWindow m_loadingWindow;

	private HudNetworkConnect m_loadingConnect;

	private AtomDataInfo m_atomInfo;

	private readonly int ACHIEVEMENT_HIDE_COUNT = 3;

	private bool m_isSkip;

	private int m_subState;

	private bool m_initUser;

	private string m_agreementText;

	private RedStarExchangeType m_exchangeType;

	public static bool Logined
	{
		get
		{
			return m_isLogined;
		}
		set
		{
			m_isLogined = value;
		}
	}

	public static bool FirstTutorialReturned
	{
		get
		{
			return m_isReturnFirstTutorial;
		}
		set
		{
			m_isReturnFirstTutorial = value;
		}
	}

	private void Awake()
	{
		Application.targetFrameRate = 60;
		base.gameObject.AddComponent<HudNetworkConnect>();
	}

	private void Start()
	{
		HudUtility.SetInvalidNGUIMitiTouch();
		SystemSettings.ChangeQualityLevelBySaveData();
		SystemData systemSaveData = SystemSaveManager.GetSystemSaveData();
		bool flag = false;
		if (systemSaveData != null)
		{
			flag = systemSaveData.highTexture;
		}
		if (flag)
		{
			Caching.maximumAvailableDiskSpace = 524288000L;
		}
		else
		{
			Caching.maximumAvailableDiskSpace = 314572800L;
		}
		Caching.expirationDelay = 2592000;
		GameObject gameObject = GameObject.Find("StageInfo");
		if (gameObject == null)
		{
			gameObject = new GameObject();
			if (gameObject != null)
			{
				gameObject.name = "StageInfo";
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				gameObject.AddComponent("StageInfo");
			}
		}
		if (gameObject != null)
		{
			m_stageInfo = gameObject.GetComponent<StageInfo>();
			if (m_stageInfo == null)
			{
				return;
			}
		}
		SoundManager.AddTitleCueSheet();
		GameObject gameObject2 = GameObject.Find("UI Root (2D)");
		if (gameObject2 == null)
		{
			return;
		}
		m_touchScreenObject = GameObjectUtil.FindChildGameObject(gameObject2, "Lbl_mainmenu");
		if (m_touchScreenObject != null)
		{
			m_touchScreenObject.SetActive(false);
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject2, "sega_logo");
		if (gameObject3 != null)
		{
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "sega_logo_img");
			if (uISprite != null)
			{
				if (Env.language == Env.Language.JAPANESE || Env.language == Env.Language.KOREAN || Env.language == Env.Language.CHINESE_ZH || Env.language == Env.Language.CHINESE_ZHJ)
				{
					uISprite.spriteName = "ui_title_img_segalogo_jp";
				}
				else
				{
					uISprite.spriteName = "ui_title_img_segalogo_en";
				}
			}
			gameObject3.SetActive(s_first);
		}
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (m_fsm == null)
		{
			return;
		}
		TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
		RouletteManager.Remove();
		if (m_isReturnFirstTutorial)
		{
			if (SystemSaveManager.Instance != null)
			{
				SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
				if (systemdata != null)
				{
					systemdata.SetFlagStatus(SystemData.FlagStatus.FIRST_LAUNCH_TUTORIAL_END, true);
					SystemSaveManager.Instance.SaveSystemData();
				}
			}
			GameObject gameObject4 = GameObject.Find("UI Root (2D)");
			if (gameObject4 != null)
			{
				GameObject gameObject5 = GameObjectUtil.FindChildGameObject(gameObject4, "img_blinder");
				if (gameObject5 != null)
				{
					gameObject5.SetActive(true);
				}
			}
			m_nextSceneName = "MainMenu";
			description.initState = new TinyFsmState(StateSnsInitialize);
		}
		else if (m_isLogined)
		{
			description.initState = new TinyFsmState(StateFadeOut);
		}
		else
		{
			description.initState = new TinyFsmState(StateLoadFont);
		}
		description.onFixedUpdate = true;
		m_fsm.SetUp(description);
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_ver");
		if (uILabel == null)
		{
			return;
		}
		string str = string.Empty;
		if (systemSaveData != null && systemSaveData.highTexture)
		{
			str = "t";
		}
		string text = VersionStr + str + NetBaseUtil.ServerTypeString;
		if (!string.IsNullOrEmpty(NetBaseUtil.ServerTypeString))
		{
			text = "[ff0000]" + text;
		}
		uILabel.text = text;
		m_loginLabel = GameObjectUtil.FindChildGameObject(gameObject2, "Lbl_login");
		if (m_loginLabel != null)
		{
			m_loginLabel.SetActive(false);
		}
		m_loadingWindow = GameObjectUtil.FindChildGameObjectComponent<HudLoadingWindow>(gameObject2, "DownloadWindow");
		m_userIdLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_userid");
		if (m_userIdLabel != null)
		{
			if (TitleUtil.IsExistSaveDataGameId())
			{
				m_initUser = true;
				m_userIdLabel.gameObject.SetActive(true);
				m_userIdLabel.text = GetViewUserID();
				GameObject gameObject6 = GameObjectUtil.FindChildGameObject(gameObject2, "Btn_policy");
				if (gameObject6 != null)
				{
					gameObject6.SetActive(false);
				}
				GameObject gameObject7 = GameObjectUtil.FindChildGameObject(gameObject2, "Btn_mainmenu");
				if (gameObject7 != null)
				{
					gameObject7.SetActive(true);
				}
				m_startButton = gameObject7;
			}
			else
			{
				m_initUser = false;
				m_userIdLabel.gameObject.SetActive(false);
				GameObject gameObject8 = GameObjectUtil.FindChildGameObject(gameObject2, "Btn_policy");
				if (gameObject8 != null)
				{
					gameObject8.SetActive(true);
				}
				GameObject gameObject9 = GameObjectUtil.FindChildGameObject(gameObject2, "Btn_mainmenu");
				if (gameObject9 != null)
				{
					gameObject9.SetActive(false);
				}
				m_startButton = gameObject8;
			}
			BackKeyManager.AddEventCallBack(base.gameObject);
			BackKeyManager.StartScene();
			BackKeyManager.InvalidFlag = true;
		}
		m_snsLogin = base.gameObject.AddComponent<SettingPartsSnsLogin>();
		m_snsLogin.SetCancelWindowUseFlag(false);
		m_snsLogin.Setup("Camera/TitleScreen/Anchor_5_MC");
		Debug.Log("GetLang:" + Language.GetLocalLanguage());
		GameObject gameObject10 = GameObjectUtil.FindChildGameObject(gameObject2, "Btn_fb");
		if (gameObject10 != null)
		{
			gameObject10.SetActive(false);
		}
		if (m_userIdLabel != null)
		{
			m_userIdLabel.gameObject.SetActive(true);
			m_userIdLabel.text = GetViewUserID();
		}
		CameraFade.StartAlphaFade(Color.black, true, 2f, 0f, FinishFadeCallback);
		m_movetButton = null;
		m_cacheButton = null;
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(gameObject2, "Btn_move");
		if (uIButtonMessage != null)
		{
			m_movetButton = uIButtonMessage.gameObject;
			m_movetButton.SetActive(false);
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickMigrationButton";
		}
		UIButtonMessage uIButtonMessage2 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(gameObject2, "Btn_cache");
		if (uIButtonMessage2 != null)
		{
			m_cacheButton = uIButtonMessage2.gameObject;
			m_cacheButton.SetActive(false);
			uIButtonMessage2.target = base.gameObject;
			uIButtonMessage2.functionName = "OnClickCacheClearButton";
		}
		GameObject gameObject11 = GameObjectUtil.FindChildGameObject(gameObject2, "logo_jp");
		if (gameObject11 != null)
		{
			if (Env.language == Env.Language.JAPANESE)
			{
				gameObject11.SetActive(true);
			}
			else
			{
				gameObject11.SetActive(false);
			}
		}
		m_loadingConnect = base.gameObject.GetComponent<HudNetworkConnect>();
		if (!m_isReturnFirstTutorial)
		{
			GameObject gameObject12 = GameObjectUtil.FindChildGameObject(gameObject2, "img_titlelogo");
			if (gameObject12 != null)
			{
				gameObject12.SetActive(false);
			}
		}
		m_progressBar = GameObjectUtil.FindChildGameObjectComponent<HudProgressBar>(gameObject2, "Pgb_loading");
		if (m_progressBar != null)
		{
			m_progressBar.SetUp(25);
		}
	}

	private TinyFsmState StateLoadFont(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (FontManager.Instance != null && FontManager.Instance.IsNecessaryLoadFont())
			{
				FontManager.Instance.LoadResourceData();
				FontManager.Instance.ReplaceFont();
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (s_first)
			{
				m_fsm.ChangeState(new TinyFsmState(StateFadeOut));
			}
			else
			{
				SegaLogoAnimationSkip();
				if (SystemSaveManager.Instance != null && SystemSaveManager.Instance.ErrorOnStart())
				{
					m_fsm.ChangeState(new TinyFsmState(StateSaveDataError));
					return TinyFsmState.End();
				}
				m_fsm.ChangeState(new TinyFsmState(StateSnsInitialize));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateFadeOut(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (m_touchScreenObject != null)
			{
				m_touchScreenObject.SetActive(true);
				TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "ui_Lbl_mainmenu");
				UILabel component = m_touchScreenObject.GetComponent<UILabel>();
				if (component != null)
				{
					component.text = text.text;
				}
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_touchScreenObject, "Lbl_mainmenu_sh");
				if (uILabel != null)
				{
					uILabel.text = text.text;
				}
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 109:
		{
			GameObject parent = GameObject.Find("UI Root (2D)");
			GameObject gameObject = GameObjectUtil.FindChildGameObject(parent, "img_titlelogo");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(parent, "TitleScreen");
			if (animation != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(animation, "ui_title_intro_logo_Anim", Direction.Forward);
				EventDelegate.Add(activeAnimation.onFinished, SegaLogoAnimationFinishCallback, false);
			}
			s_first = false;
			if (SystemSaveManager.Instance != null && SystemSaveManager.Instance.ErrorOnStart())
			{
				m_fsm.ChangeState(new TinyFsmState(StateSaveDataError));
				return TinyFsmState.End();
			}
			m_fsm.ChangeState(new TinyFsmState(StateSnsInitialize));
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSaveDataError(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			CreateSaveErrorWindow(false);
			m_subState = 0;
			BackKeyManager.InvalidFlag = false;
			return TinyFsmState.End();
		case -4:
			BackKeyManager.InvalidFlag = true;
			return TinyFsmState.End();
		case 0:
			switch (m_subState)
			{
			case 0:
				if (!GeneralWindow.IsButtonPressed)
				{
					break;
				}
				GeneralWindow.Close();
				if (SystemSaveManager.Instance.SaveForStartingError())
				{
					if (m_isLogined)
					{
						m_fsm.ChangeState(new TinyFsmState(StateSnsInitialize));
					}
					else
					{
						m_fsm.ChangeState(new TinyFsmState(StateSnsInitialize));
					}
				}
				else
				{
					CreateSaveErrorWindow(true);
					m_subState = 1;
				}
				break;
			case 1:
				if (GeneralWindow.IsButtonPressed)
				{
					GeneralWindow.Close();
					CreateSaveErrorWindow(false);
					m_subState = 0;
				}
				break;
			}
			return TinyFsmState.End();
		case 109:
			m_fsm.ChangeState(new TinyFsmState(StateSnsInitialize));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void CreateSaveErrorWindow(bool error)
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", (!error) ? "savedata_recreate" : "savedata_error").text;
		info.anchor_path = "Camera/Anchor_5_MC";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		GeneralWindow.Create(info);
	}

	private TinyFsmState StateSnsInitialize(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
			if (socialInterface != null)
			{
				socialInterface.Initialize(base.gameObject);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			m_fsm.ChangeState(new TinyFsmState(StateNoahConnect));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateNoahConnect(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isReturnFirstTutorial)
			{
				m_fsm.ChangeState(new TinyFsmState(StateGameServerPreLogin));
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StateWaitTouchScreen));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitTouchScreen(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			BackKeyManager.InvalidFlag = false;
			if (m_startButton != null)
			{
				m_startButton.SetActive(true);
			}
			if (m_movetButton != null)
			{
				m_movetButton.SetActive(true);
			}
			if (m_cacheButton != null)
			{
				m_cacheButton.SetActive(true);
			}
			return TinyFsmState.End();
		case -4:
			BackKeyManager.InvalidFlag = true;
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("quit_app"))
			{
				if (GeneralWindow.IsButtonPressed)
				{
					if (GeneralWindow.IsYesButtonPressed)
					{
						Application.Quit();
					}
					else if (GeneralWindow.IsNoButtonPressed)
					{
					}
					GeneralWindow.Close();
					SetUIEffect(true);
				}
			}
			else if (GeneralWindow.IsCreated("cache_clear"))
			{
				if (GeneralWindow.IsButtonPressed)
				{
					if (GeneralWindow.IsYesButtonPressed)
					{
						GeneralUtil.CleanAllCache();
						GeneralWindow.Close();
						GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
						info.name = "cache_clear_end";
						info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Option", "cash_cashclear_confirmation_bar");
						info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Option", "cash_cashclear_confirmation_title");
						info.anchor_path = "Camera/Anchor_5_MC";
						info.buttonType = GeneralWindow.ButtonType.Ok;
						GeneralWindow.Create(info);
					}
					else
					{
						GeneralWindow.Close();
					}
				}
			}
			else if (GeneralWindow.IsCreated("cache_clear_end") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
			}
			return TinyFsmState.End();
		case 111:
			m_fsm.ChangeState(new TinyFsmState(StateTakeoverFunction));
			return TinyFsmState.End();
		case 100:
		{
			if (m_startButton != null)
			{
				m_startButton.SetActive(false);
			}
			if (m_movetButton != null)
			{
				m_movetButton.SetActive(false);
			}
			if (m_cacheButton != null)
			{
				m_cacheButton.SetActive(false);
			}
			if (m_isLogined)
			{
				m_fsm.ChangeState(new TinyFsmState(StateFadeIn));
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StateGameServerPreLogin));
			}
			GameObject parent = GameObject.Find("UI Root (2D)");
			UIObjectContainer uIObjectContainer = GameObjectUtil.FindChildGameObjectComponent<UIObjectContainer>(parent, "TitleScreen");
			if (uIObjectContainer != null)
			{
				GameObject[] objects = uIObjectContainer.Objects;
				if (objects != null)
				{
					GameObject[] array = objects;
					foreach (GameObject gameObject in array)
					{
						if (gameObject != null)
						{
							gameObject.SetActive(false);
						}
					}
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateTakeoverFunction(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			BackKeyManager.InvalidFlag = false;
			m_subState = 0;
			CreateTakeoverCautionWindow();
			return TinyFsmState.End();
		case -4:
			BackKeyManager.InvalidFlag = true;
			return TinyFsmState.End();
		case 0:
			switch (m_subState)
			{
			case 0:
			{
				if (!GeneralWindow.IsCreated("TakeoverCaution") || !GeneralWindow.IsButtonPressed)
				{
					break;
				}
				bool flag = false;
				if (GeneralWindow.IsYesButtonPressed)
				{
					m_subState = 1;
					if (m_takeoverInput == null)
					{
						m_takeoverInput = base.gameObject.AddComponent<SettingTakeoverInput>();
						m_takeoverInput.Setup(ANCHOR_PATH);
					}
					if (m_takeoverInput != null)
					{
						m_takeoverInput.PlayStart();
					}
					flag = true;
				}
				else if (GeneralWindow.IsNoButtonPressed)
				{
					m_fsm.ChangeState(new TinyFsmState(StateWaitTouchScreen));
				}
				GeneralWindow.Close();
				if (flag)
				{
					SetUIEffect(false);
				}
				break;
			}
			case 1:
				if (!(m_takeoverInput != null) || !m_takeoverInput.IsEndPlay())
				{
					break;
				}
				if (m_takeoverInput.IsDicide)
				{
					string inputIdText = m_takeoverInput.InputIdText;
					string inputPassText = m_takeoverInput.InputPassText;
					Debug.Log("Input Finished! Input ID is " + inputIdText);
					Debug.Log("Input Finished! Input PASS is " + inputPassText);
					if (inputIdText.Length == 0 || inputPassText.Length == 0)
					{
						m_fsm.ChangeState(new TinyFsmState(StateTakeoverError));
						m_subState = 2;
					}
					else
					{
						m_fsm.ChangeState(new TinyFsmState(StateTakeoverExecute));
						m_subState = 2;
					}
				}
				if (m_takeoverInput.IsCanceled)
				{
					m_subState = 2;
					m_fsm.ChangeState(new TinyFsmState(StateWaitTouchScreen));
				}
				SetUIEffect(true);
				break;
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateTakeoverError(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			BackKeyManager.InvalidFlag = false;
			CreateTakeoverErrorWindow();
			return TinyFsmState.End();
		case -4:
			BackKeyManager.InvalidFlag = true;
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("TakeoverError") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateWaitTouchScreen));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateTakeoverExecute(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			BackKeyManager.InvalidFlag = false;
			m_isTakeoverLogin = false;
			string inputIdText = m_takeoverInput.InputIdText;
			string inputPassText = m_takeoverInput.InputPassText;
			ServerInterface serverInterface = GameObjectUtil.FindGameObjectComponent<ServerInterface>("ServerInterface");
			if (serverInterface != null)
			{
				string migrationPassword = NetUtil.CalcMD5String(inputPassText);
				serverInterface.RequestServerMigration(inputIdText, migrationPassword, base.gameObject);
			}
			CreateTakeoverExecWindow();
			m_timer = 0f;
			return TinyFsmState.End();
		}
		case -4:
			BackKeyManager.InvalidFlag = true;
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("TakeoverExec"))
			{
				m_timer += Time.deltaTime;
				if (m_timer >= TAKEOVER_WAIT_TIME)
				{
					m_timer = TAKEOVER_WAIT_TIME;
				}
				if (m_isTakeoverLogin && m_timer >= TAKEOVER_WAIT_TIME)
				{
					Debug.Log("Takeover Finished! Result Success! ");
					GeneralWindow.Close();
					m_fsm.ChangeState(new TinyFsmState(StateTakeoverFinished));
				}
			}
			return TinyFsmState.End();
		case 113:
			if (GeneralWindow.IsCreated("TakeoverExec"))
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateTakeoverError));
			}
			break;
		}
		return TinyFsmState.End();
	}

	private TinyFsmState StateTakeoverFinished(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			BackKeyManager.InvalidFlag = false;
			CreateTakeoverFinishedWindow();
			Debug.Log("Takeover Finished!");
			return TinyFsmState.End();
		case -4:
			BackKeyManager.InvalidFlag = true;
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("TakeoverFinished") && GeneralWindow.IsButtonPressed)
			{
				OnMsgGotoHead();
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void CreateTakeoverCautionWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "TakeoverCaution";
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "takeover_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "takeover_text").text;
		GeneralWindow.Create(info);
	}

	private void CreateTakeoverErrorWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "TakeoverError";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "takeover_error_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "takeover_error_text").text;
		GeneralWindow.Create(info);
	}

	private void CreateTakeoverExecWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "TakeoverExec";
		info.buttonType = GeneralWindow.ButtonType.None;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "takeover_exec_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "takeover_exec_text").text;
		GeneralWindow.Create(info);
	}

	private void CreateTakeoverFinishedWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "TakeoverFinished";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "takeover_finished_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "takeover_finished_text").text;
		GeneralWindow.Create(info);
	}

	private TinyFsmState StateGamePushNotification(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			BackKeyManager.InvalidFlag = false;
			m_pushNotice = base.gameObject.AddComponent<SettingPartsPushNotice>();
			m_pushNotice.Setup(ANCHOR_PATH);
			m_pushNotice.PlayStart();
			m_pushNotice.SetCloseButtonEnabled(false);
			return TinyFsmState.End();
		case -4:
			BackKeyManager.InvalidFlag = true;
			return TinyFsmState.End();
		case 100:
			m_fsm.ChangeState(new TinyFsmState(StateGameServerPreLogin));
			return TinyFsmState.End();
		case 0:
			if (m_pushNotice != null && m_pushNotice.IsEndPlay())
			{
				if (m_pushNotice.IsOverwrite && SystemSaveManager.Instance != null)
				{
					SystemSaveManager.Instance.SaveSystemData();
				}
				m_fsm.ChangeState(new TinyFsmState(StateGameServerPreLogin));
				Debug.Log("m_fsm.ChangeState(new TinyFsmState(this.StateGameServerPreLogin));");
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGameServerPreLogin(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			ServerSessionWatcher serverSessionWatcher = GameObjectUtil.FindGameObjectComponent<ServerSessionWatcher>("NetMonitor");
			if (serverSessionWatcher != null)
			{
				m_isSessionValid = false;
				serverSessionWatcher.ValidateSession(ServerSessionWatcher.ValidateType.PRELOGIN, ValidateSessionCallback);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isSessionValid)
			{
				Debug.Log("GameModeTitle.StateGameServerPreLogin:Finished");
				if (m_userIdLabel != null)
				{
					m_userIdLabel.gameObject.SetActive(true);
					m_userIdLabel.text = GetViewUserID();
				}
				bool flag = true;
				if (SystemSaveManager.Instance != null)
				{
					string countryCode = SystemSaveManager.GetCountryCode();
					if (string.IsNullOrEmpty(countryCode))
					{
						flag = false;
					}
				}
				if (flag)
				{
					m_fsm.ChangeState(new TinyFsmState(StateAssetBundleInitialize));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateGetCountryCodeRetry));
					Debug.Log("GameModeTitle.StateGameServerPreLogin:LostCountryCode!!");
				}
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGetCountryCodeRetry(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_isGetCountry = false;
			ServerInterface serverInterface = GameObjectUtil.FindGameObjectComponent<ServerInterface>("ServerInterface");
			if (serverInterface != null)
			{
				serverInterface.RequestServerGetCountry(base.gameObject);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isGetCountry)
			{
				m_fsm.ChangeState(new TinyFsmState(StateAssetBundleInitialize));
				Debug.Log("GameModeTitle.StateGetCountryCodeRetry:Finished");
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateAssetBundleInitialize(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			NetBaseUtil.SetAssetServerURL();
			Screen.sleepTimeout = -1;
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
			return TinyFsmState.End();
		case -4:
			Screen.sleepTimeout = -2;
			return TinyFsmState.End();
		case 0:
			if (!Env.useAssetBundle || AssetBundleLoader.Instance.IsEnableDownlad())
			{
				m_fsm.ChangeState(new TinyFsmState(StateStreamingLoaderInitialize));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateStreamingLoaderInitialize(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			Screen.sleepTimeout = -1;
			if (StreamingDataLoader.Instance == null)
			{
				StreamingDataKeyRetryProcess process = new StreamingDataKeyRetryProcess(base.gameObject, this);
				NetMonitor.Instance.StartMonitor(process, 0f, HudNetworkConnect.DisplayType.ALL);
				StreamingDataLoader.Create();
				StreamingDataLoader.Instance.Initialize(base.gameObject);
			}
			return TinyFsmState.End();
		case -4:
			Screen.sleepTimeout = -2;
			return TinyFsmState.End();
		case 0:
			if (StreamingDataLoader.Instance.IsEnableDownlad())
			{
				m_fsm.ChangeState(new TinyFsmState(StateCheckExistDownloadData));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateCheckExistDownloadData(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (m_loader == null)
			{
				GameObject gameObject = new GameObject();
				m_loader = gameObject.AddComponent<TitleDataLoader>();
				if (StreamingDataLoader.Instance != null)
				{
					if (SystemSaveManager.Instance != null)
					{
						SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
						if (systemdata != null && !systemdata.IsFlagStatus(SystemData.FlagStatus.FIRST_LAUNCH_TUTORIAL_END) && !m_isReturnFirstTutorial)
						{
							if (systemdata.IsFlagStatus(SystemData.FlagStatus.TUTORIAL_BOSS_MAP_1))
							{
								systemdata.SetFlagStatus(SystemData.FlagStatus.FIRST_LAUNCH_TUTORIAL_END, true);
								SystemSaveManager.Instance.SaveSystemData();
							}
							else
							{
								m_isFirstTutorial = true;
							}
						}
					}
					List<string> getData = new List<string>();
					if (m_isFirstTutorial)
					{
						m_loader.AddStreamingSoundData("BGM_z01.acb");
						m_loader.AddStreamingSoundData("BGM_z01_streamfiles.awb");
						m_loader.AddStreamingSoundData("BGM_jingle.acb");
						m_loader.AddStreamingSoundData("BGM_jingle_streamfiles.awb");
						m_loader.AddStreamingSoundData("se_runners.acb");
					}
					else
					{
						StreamingDataLoader.Instance.GetLoadList(ref getData);
						foreach (string item in getData)
						{
							bool flag = item.Contains("BGM_z");
							bool flag2 = item.Contains("BGM_boss");
							if (!flag && !flag2)
							{
								m_loader.AddStreamingSoundData(item);
							}
						}
					}
				}
				m_loader.Setup(m_isFirstTutorial);
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_loader.EndCheckExistingDownloadData)
			{
				if (m_loader.RequestedDownloadCount > 0)
				{
					m_fsm.ChangeState(new TinyFsmState(StateAskDataDownload));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateWaitDataLoad));
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateAskDataDownload(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			if (m_isFirstTutorial)
			{
				info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "ui_Lbl_load_caption").text;
				info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "ui_Lbl_load_ask_FirstTutorial").text;
			}
			else if (m_isReturnFirstTutorial)
			{
				info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "ui_Lbl_load_caption").text;
				info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "ui_Lbl_load_ask_FirstTutorialReturn").text;
			}
			else
			{
				info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "ui_Lbl_load_caption").text;
				if (TitleUtil.initUser)
				{
					info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "ui_Lbl_load_ask").text;
				}
				else
				{
					info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "ui_Lbl_load_ask_2").text;
				}
			}
			info.anchor_path = "Camera/Anchor_5_MC";
			info.buttonType = GeneralWindow.ButtonType.YesNo;
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsYesButtonPressed)
			{
				if (m_isReturnFirstTutorial)
				{
					m_isReturnFirstTutorial = false;
				}
				GeneralWindow.Close();
				SoundManager.BgmPlay("bgm_sys_load");
				m_fsm.ChangeState(new TinyFsmState(StateWaitDataDownload));
			}
			else if (GeneralWindow.IsNoButtonPressed)
			{
				GeneralWindow.Close();
				m_isLogined = false;
				if (m_isReturnFirstTutorial)
				{
					m_isReturnFirstTutorial = false;
					m_fsm.ChangeState(new TinyFsmState(StateLoadTitleResetScene));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateWaitTouchScreen));
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitDataDownload(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			Screen.sleepTimeout = -1;
			if (m_loadingWindow != null)
			{
				m_loadingWindow.PlayStart();
			}
			if (m_loader != null)
			{
				m_loader.StartLoad();
			}
			SetUIEffect(false);
			return TinyFsmState.End();
		case -4:
			if (m_loader != null)
			{
				UnityEngine.Object.Destroy(m_loader.gameObject);
				m_loader = null;
			}
			Screen.sleepTimeout = -2;
			return TinyFsmState.End();
		case 0:
			if (m_loader == null)
			{
				m_fsm.ChangeState(new TinyFsmState(StateEndDataLoad));
			}
			else if (m_loader.LoadEnd)
			{
				TextManager.NotLoadSetupCommonText();
				TextManager.NotLoadSetupChaoText();
				TextManager.NotLoadSetupEventText();
				MissionTable.LoadSetup();
				CharacterDataNameInfo.LoadSetup();
				StageAbilityManager.SetupAbilityDataTable();
				OverlapBonusTable overlapBonusTable = GameObjectUtil.FindGameObjectComponent<OverlapBonusTable>("OverlapBonusTable");
				if (overlapBonusTable != null)
				{
					overlapBonusTable.Setup();
				}
				SetUIEffect(true);
				if (m_loadingWindow != null)
				{
					m_loadingWindow.PlayEnd();
				}
				m_fsm.ChangeState(new TinyFsmState(StateEndDataLoad));
			}
			else if (m_loader != null && m_loadingWindow != null)
			{
				float num = m_loader.RequestedLoadCount;
				float num2 = m_loader.LoadEndCount;
				if (num == 0f)
				{
					num = 1f;
				}
				float loadingPercentage = num2 * 100f / num;
				m_loadingWindow.SetLoadingPercentage(loadingPercentage);
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitDataLoad(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			Screen.sleepTimeout = -1;
			if (m_loadingConnect != null)
			{
				m_loadingConnect.Setup();
				m_loadingConnect.PlayStart(HudNetworkConnect.DisplayType.NO_BG);
			}
			if (m_loader != null)
			{
				m_loader.StartLoad();
			}
			return TinyFsmState.End();
		case -4:
			if (m_loader != null)
			{
				UnityEngine.Object.Destroy(m_loader.gameObject);
				m_loader = null;
			}
			Screen.sleepTimeout = -2;
			return TinyFsmState.End();
		case 0:
			if (m_loader == null)
			{
				m_fsm.ChangeState(new TinyFsmState(StateEndDataLoad));
			}
			else if (m_loader.LoadEnd)
			{
				TextManager.NotLoadSetupCommonText();
				TextManager.NotLoadSetupChaoText();
				TextManager.NotLoadSetupEventText();
				MissionTable.LoadSetup();
				CharacterDataNameInfo.LoadSetup();
				StageAbilityManager.SetupAbilityDataTable();
				OverlapBonusTable overlapBonusTable = GameObjectUtil.FindGameObjectComponent<OverlapBonusTable>("OverlapBonusTable");
				if (overlapBonusTable != null)
				{
					overlapBonusTable.Setup();
				}
				if (m_loadingConnect != null)
				{
					m_loadingConnect.PlayEnd();
				}
				m_fsm.ChangeState(new TinyFsmState(StateEndDataLoad));
			}
			else if (m_loader != null && m_loadingWindow != null)
			{
				float num = m_loader.RequestedLoadCount;
				float num2 = m_loader.LoadEndCount;
				if (num == 0f)
				{
					num = 1f;
				}
				float loadingPercentage = num2 * 100f / num;
				m_loadingWindow.SetLoadingPercentage(loadingPercentage);
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEndDataLoad(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(0);
			}
			return TinyFsmState.End();
		case 0:
			m_fsm.ChangeState(new TinyFsmState(StateGameServerLogin));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGameServerLogin(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			ServerSessionWatcher serverSessionWatcher = GameObjectUtil.FindGameObjectComponent<ServerSessionWatcher>("NetMonitor");
			if (serverSessionWatcher != null)
			{
				m_isSessionValid = false;
				serverSessionWatcher.ValidateSession(ServerSessionWatcher.ValidateType.LOGIN_OR_RELOGIN, ValidateSessionCallback);
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(1);
			}
			return TinyFsmState.End();
		case 0:
			if (m_isSessionValid)
			{
				m_isLogined = true;
				Debug.Log("GameModeTitle.StateGameServerLogin:Finished");
				if (m_isFirstTutorial)
				{
					m_nextSceneName = "s_playingstage";
					SetFirstTutorialInfo();
					m_fsm.ChangeState(new TinyFsmState(StateFadeIn));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateGetServerContinueParameter));
				}
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGetServerContinueParameter(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetVariousParameter(base.gameObject);
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(2);
			}
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 102:
			if (RegionManager.Instance.IsUseHardlightAds())
			{
				GameObject x = GameObject.Find("HardlightAds");
				if (x == null)
				{
					x = new GameObject();
					if (x != null)
					{
						x.name = "HardlightAds";
						UnityEngine.Object.DontDestroyOnLoad(x);
						x.AddComponent("HardlightAds");
					}
				}
			}
			m_fsm.ChangeState(new TinyFsmState(StateCheckAtom));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateCheckAtom(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (Binding.Instance != null)
			{
				string urlSchemeStr = Binding.Instance.GetUrlSchemeStr();
				Binding.Instance.ClearUrlSchemeStr();
				if (!string.IsNullOrEmpty(urlSchemeStr))
				{
					string campaign = string.Empty;
					string serial = string.Empty;
					if (ServerAtomSerial.GetSerialFromScheme(urlSchemeStr, ref campaign, ref serial))
					{
						m_atomInfo = new AtomDataInfo();
						m_atomInfo.campain = campaign;
						m_atomInfo.serial = serial;
						GeneralWindow.CInfo info3 = default(GeneralWindow.CInfo);
						info3.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "atom_check");
						info3.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "atom_check_caption");
						info3.anchor_path = "Camera/Anchor_5_MC";
						info3.buttonType = GeneralWindow.ButtonType.Ok;
						GeneralWindow.Create(info3);
						m_subState = 0;
					}
					else
					{
						m_subState = 3;
					}
				}
				else
				{
					m_subState = 3;
				}
			}
			else
			{
				m_subState = 3;
			}
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(3);
			}
			m_atomInfo = null;
			return TinyFsmState.End();
		case 0:
			switch (m_subState)
			{
			case 0:
			{
				if (!GeneralWindow.IsButtonPressed)
				{
					break;
				}
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
				loggedInServerInterface.RequestServerAtomSerial(m_atomInfo.campain, m_atomInfo.serial, new_user, base.gameObject);
				GeneralWindow.Close();
				m_subState = 1;
				break;
			}
			case 1:
				Debug.Log("Wait Server");
				return TinyFsmState.End();
			case 2:
				if (GeneralWindow.IsButtonPressed)
				{
					Debug.Log("EndText end:");
					GeneralWindow.Close();
					m_subState = 3;
				}
				return TinyFsmState.End();
			case 3:
				m_fsm.ChangeState(new TinyFsmState(StateCheckNoLoginIncentive));
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		case 1:
		{
			int iD = e.GetMessage.ID;
			TextManager.TextType type = TextManager.TextType.TEXTTYPE_FIXATION_TEXT;
			switch (iD)
			{
			case 61495:
			{
				GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
				info2.message = TextUtility.GetText(type, "Title", "atom_present_get");
				info2.caption = TextUtility.GetText(type, "Title", "atom_success_caption");
				info2.anchor_path = "Camera/Anchor_5_MC";
				info2.buttonType = GeneralWindow.ButtonType.Ok;
				GeneralWindow.Create(info2);
				m_subState = 2;
				break;
			}
			case 61517:
			{
				MsgServerConnctFailed msgServerConnctFailed = e.GetMessage as MsgServerConnctFailed;
				string cellID = "atom_invalid_serial";
				if (msgServerConnctFailed != null && msgServerConnctFailed.m_status == ServerInterface.StatusCode.UsedSerialCode)
				{
					cellID = "atom_used_serial";
				}
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				info.message = TextUtility.GetText(type, "Title", cellID);
				info.caption = TextUtility.GetText(type, "Title", "atom_failure_caption");
				info.anchor_path = "Camera/Anchor_5_MC";
				info.buttonType = GeneralWindow.ButtonType.Ok;
				GeneralWindow.Create(info);
				m_subState = 2;
				break;
			}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateCheckNoLoginIncentive(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (PnoteNotification.CheckEnableGetNoLoginIncentive())
			{
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				if (loggedInServerInterface != null)
				{
					loggedInServerInterface.RequestServerGetFacebookIncentive(4, 1, base.gameObject);
					m_subState = 0;
				}
			}
			else
			{
				m_subState = 2;
			}
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(4);
			}
			m_atomInfo = null;
			return TinyFsmState.End();
		case 0:
			switch (m_subState)
			{
			case 0:
				Debug.Log("Wait Server");
				return TinyFsmState.End();
			case 1:
				if (GeneralWindow.IsButtonPressed)
				{
					Debug.Log("EndText end:");
					GeneralWindow.Close();
					m_subState = 3;
				}
				return TinyFsmState.End();
			case 2:
				m_fsm.ChangeState(new TinyFsmState(StateSnsAdditionalData));
				return TinyFsmState.End();
			default:
				return TinyFsmState.End();
			}
		case 1:
			switch (e.GetMessage.ID)
			{
			case 61490:
				m_subState = 2;
				break;
			case 61517:
				m_subState = 2;
				break;
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSnsAdditionalData(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			SocialInterface socialInterface2 = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
			if (socialInterface2 != null && socialInterface2.IsLoggedIn)
			{
				socialInterface2.RequestFriendRankingInfoSet(null, null, SettingPartsSnsAdditional.Mode.BACK_GROUND_LOAD);
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
		{
			SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
			if (socialInterface != null && socialInterface.IsLoggedIn)
			{
				if (socialInterface.IsEnableFriendInfo)
				{
					m_fsm.ChangeState(new TinyFsmState(StatePushNoticeCheck));
				}
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StatePushNoticeCheck));
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StatePushNoticeCheck(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (SystemSaveManager.Instance != null)
			{
				SystemData systemSaveData = SystemSaveManager.GetSystemSaveData();
				if (systemSaveData != null)
				{
					if (systemSaveData.pushNotice)
					{
						PnoteNotification.RequestRegister();
					}
					else
					{
						PnoteNotification.RequestUnregister();
					}
				}
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			m_fsm.ChangeState(new TinyFsmState(StateWaitToGetMenuData));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitToGetMenuData(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerRetrievePlayerState(base.gameObject);
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(16);
			}
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 103:
			m_fsm.ChangeState(new TinyFsmState(StateAchievementLogin));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateAchievementLogin(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			AchievementManager.Setup();
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
				if (systemdata != null && systemdata.achievementCancelCount >= ACHIEVEMENT_HIDE_COUNT)
				{
					AchievementManager.RequestSkipAuthenticate();
					return TinyFsmState.End();
				}
			}
			AchievementManager.RequestUpdate();
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(17);
			}
			return TinyFsmState.End();
		case 0:
			m_fsm.ChangeState(new TinyFsmState(StateGetCostList));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGetCostList(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetCostList(base.gameObject);
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(19);
			}
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 106:
			m_fsm.ChangeState(new TinyFsmState(StateGetEventList));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGetEventList(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_isSkip = true;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetEventList(base.gameObject);
				m_isSkip = false;
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(20);
			}
			return TinyFsmState.End();
		case 0:
			if (m_isSkip)
			{
				m_fsm.ChangeState(new TinyFsmState(StateGetMileageMap));
			}
			return TinyFsmState.End();
		case 108:
			m_fsm.ChangeState(new TinyFsmState(StateGetMileageMap));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGetMileageMap(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_isSkip = true;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				List<string> list = new List<string>();
				if (list != null)
				{
					SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
					if (socialInterface != null)
					{
						list = SocialInterface.GetGameIdList(socialInterface.FriendList);
					}
				}
				if (list != null && list.Count > 0)
				{
					loggedInServerInterface.RequestServerGetMileageData(list.ToArray(), base.gameObject);
					m_isSkip = false;
				}
				else
				{
					loggedInServerInterface.RequestServerGetMileageData(null, base.gameObject);
					m_isSkip = false;
				}
			}
			return TinyFsmState.End();
		}
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(21);
			}
			return TinyFsmState.End();
		case 0:
			if (m_isSkip)
			{
				m_fsm.ChangeState(new TinyFsmState(StateIapInitialize));
			}
			return TinyFsmState.End();
		case 107:
			m_fsm.ChangeState(new TinyFsmState(StateIapInitialize));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateIapInitialize(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			IapInitializeEndCallback(NativeObserver.IAPResult.ProductsRequestCompleted);
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(22);
			}
			return TinyFsmState.End();
		case 0:
			m_fsm.ChangeState(new TinyFsmState(StateLoadEventResource));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoadEventResource(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (EventManager.Instance != null && EventManager.Instance.IsInEvent())
			{
				m_sceneLoader = new GameObject("SceneLoader");
				if (m_sceneLoader != null)
				{
					ResourceSceneLoader resourceSceneLoader = m_sceneLoader.AddComponent<ResourceSceneLoader>();
					m_loadInfoForEvent.m_scenename = "EventResourceCommon" + EventManager.GetResourceName();
					resourceSceneLoader.AddLoadAndResourceManager(m_loadInfoForEvent);
				}
				AtlasManager.Instance.StartLoadAtlasForTitle();
			}
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(23);
			}
			return TinyFsmState.End();
		case 0:
		{
			bool flag = true;
			if (m_sceneLoader != null)
			{
				if (m_sceneLoader.GetComponent<ResourceSceneLoader>().Loaded && AtlasManager.Instance.IsLoadAtlas())
				{
					flag = true;
					UnityEngine.Object.Destroy(m_sceneLoader);
					m_sceneLoader = null;
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				m_fsm.ChangeState(new TinyFsmState(StateLoadingUIData));
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoadingUIData(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			ChaoTextureManager.Instance.RequestTitleLoadChaoTexture();
			return TinyFsmState.End();
		case -4:
			if (m_progressBar != null)
			{
				m_progressBar.SetState(24);
			}
			return TinyFsmState.End();
		case 0:
			if (ChaoTextureManager.Instance.IsLoaded())
			{
				m_fsm.ChangeState(new TinyFsmState(StateJailBreakCheck));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateJailBreakCheck(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			CPlusPlusLink instance = CPlusPlusLink.Instance;
			if (instance != null)
			{
				Debug.Log("GameModeTitle.StateJailBreakCheck");
				instance.BootGameCheatCheck();
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			m_fsm.ChangeState(new TinyFsmState(StateFadeIn));
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
			CameraFade.StartAlphaFade(Color.black, false, 1f, 0f, FinishFadeCallback);
			SoundManager.BgmFadeOut(0.5f);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 109:
			CameraFade.StartAlphaFade(Color.black, false, -1f);
			m_fsm.ChangeState(new TinyFsmState(StateLoadLevel));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoadLevel(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			BackKeyManager.EndScene();
			m_stageInfo.FromTitle = true;
			Resources.UnloadUnusedAssets();
			GC.Collect();
			TimeProfiler.StartCountTime("Title-NextScene");
			NativeObserver.Instance.CheckCurrentTransaction();
			Application.LoadLevel(m_nextSceneName);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLoadTitleResetScene(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			CameraFade.StartAlphaFade(Color.black, false, 2f, 0f, FinishFadeCallback);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 109:
			Application.LoadLevel("s_title_reset");
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void TrySnsLoginButtonActive()
	{
	}

	private void FinishFadeCallback()
	{
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(109);
			m_fsm.Dispatch(signal);
		}
	}

	private void InitEndCallback(MsgSocialNormalResponse msg)
	{
		Debug.Log("InitEndCallback");
		TrySnsLoginButtonActive();
	}

	private void ValidateSessionCallback(bool isSuccess)
	{
		Debug.Log("GameModeTitle.ValidateSessionCallback");
		m_isSessionValid = true;
	}

	private void ServerGetVersion_Succeeded(MsgGetVersionSucceed msg)
	{
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(101);
			m_fsm.Dispatch(signal);
		}
	}

	private void ServerGetVariousParameter_Succeeded(MsgGetVariousParameterSucceed msg)
	{
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(102);
			m_fsm.Dispatch(signal);
		}
	}

	private void ServerRetrievePlayerState_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		m_progressBar.SetState(6);
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetCharacterState(base.gameObject);
		}
	}

	private void ServerGetCharacterState_Succeeded(MsgGetCharacterStateSucceed msg)
	{
		m_progressBar.SetState(7);
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetChaoState(base.gameObject);
		}
	}

	private void ServerGetChaoState_Succeeded(MsgGetChaoStateSucceed msg)
	{
		m_progressBar.SetState(8);
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetWheelOptions(base.gameObject);
		}
	}

	private void ServerGetWheelOptions_Succeeded(MsgGetWheelOptionsSucceed msg)
	{
		m_progressBar.SetState(9);
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetDailyMissionData(base.gameObject);
		}
	}

	private void ServerGetDailyMissionData_Succeeded(MsgGetDailyMissionDataSucceed msg)
	{
		m_progressBar.SetState(10);
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetMessageList(base.gameObject);
		}
	}

	private void ServerGetMessageList_Succeeded(MsgGetMessageListSucceed msg)
	{
		m_progressBar.SetState(11);
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			m_exchangeType = RedStarExchangeType.RSRING;
			loggedInServerInterface.RequestServerGetRedStarExchangeList((int)m_exchangeType, base.gameObject);
		}
	}

	private void ServerGetRedStarExchangeList_Succeeded(MsgGetRedStarExchangeListSucceed msg)
	{
		switch (m_exchangeType)
		{
		case RedStarExchangeType.RSRING:
			m_progressBar.SetState(12);
			break;
		case RedStarExchangeType.RING:
			m_progressBar.SetState(13);
			break;
		case RedStarExchangeType.CHALLENGE:
			m_progressBar.SetState(14);
			break;
		case RedStarExchangeType.RAIDBOSS_ENERGY:
			m_progressBar.SetState(15);
			break;
		}
		bool flag = false;
		m_exchangeType++;
		if (m_exchangeType >= RedStarExchangeType.Count)
		{
			flag = true;
		}
		if (flag)
		{
			ServerTickerInfo tickerInfo = ServerInterface.TickerInfo;
			tickerInfo.Init(0);
			if (m_fsm != null)
			{
				TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(103);
				m_fsm.Dispatch(signal);
			}
			ServerLoginState loginState = ServerInterface.LoginState;
			if (loginState != null)
			{
				loginState.IsChangeDataVersion = false;
				loginState.IsChangeAssetsVersion = false;
			}
			return;
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			if (m_exchangeType == RedStarExchangeType.RAIDBOSS_ENERGY)
			{
				loggedInServerInterface.RequestServerGetRedStarExchangeList(4, base.gameObject);
			}
			else
			{
				loggedInServerInterface.RequestServerGetRedStarExchangeList((int)m_exchangeType, base.gameObject);
			}
		}
	}

	private void ServerGetLeaderboardEntries_Succeeded(MsgGetLeaderboardEntriesSucceed msg)
	{
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(104);
			m_fsm.Dispatch(signal);
		}
	}

	private void ServerGetLeagueData_Succeeded(MsgGetLeagueDataSucceed msg)
	{
		RankingLeagueTable.SetupRankingLeagueTable();
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(105);
			m_fsm.Dispatch(signal);
		}
	}

	private void GetCostList_Succeeded(MsgGetCostListSucceed msg)
	{
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(106);
			m_fsm.Dispatch(signal);
		}
	}

	private void ServerAtomSerial_Succeeded(MsgSendAtomSerialSucceed msg)
	{
		DispatchMessage(msg);
	}

	private void ServerAtomSerial_Failed(MsgServerConnctFailed msg)
	{
		DispatchMessage(msg);
	}

	private void ServerGetFacebookIncentive_Succeeded(MsgGetNormalIncentiveSucceed msg)
	{
		DispatchMessage(msg);
	}

	private void ServerGetFacebookIncentive_Failed(MsgServerConnctFailed msg)
	{
		DispatchMessage(msg);
	}

	private void ServerMigration_Succeeded(MsgLoginSucceed msg)
	{
		m_isTakeoverLogin = true;
		if (SystemSaveManager.Instance != null)
		{
			SystemSaveManager.Instance.DeleteSystemFile();
		}
		if (InformationSaveManager.Instance != null)
		{
			InformationSaveManager.Instance.DeleteInformationFile();
		}
		if (!(SystemSaveManager.Instance != null))
		{
			return;
		}
		SystemSaveManager.SetGameID(msg.m_userId);
		SystemSaveManager.SetGamePassword(msg.m_password);
		SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
		if (systemdata != null)
		{
			systemdata.SetFlagStatus(SystemData.FlagStatus.FIRST_LAUNCH_TUTORIAL_END, true);
			if (!string.IsNullOrEmpty(msg.m_countryCode))
			{
				SystemSaveManager.SetCountryCode(msg.m_countryCode);
				SystemSaveManager.CheckIAPMessage();
			}
			SystemSaveManager.Instance.CheckLightMode();
			SystemSaveManager.Instance.SaveSystemData();
		}
	}

	private void ServerMigration_Failed(MsgServerConnctFailed msg)
	{
		if (m_fsm != null)
		{
			TinyFsmEvent signal = (msg.m_status != ServerInterface.StatusCode.PassWordError) ? TinyFsmEvent.CreateUserEvent(112) : TinyFsmEvent.CreateUserEvent(113);
			m_fsm.Dispatch(signal);
		}
	}

	private void ServerGetCountry_Succeeded(MsgGetCountrySucceed msg)
	{
		m_isGetCountry = true;
		if (SystemSaveManager.Instance != null)
		{
			SystemSaveManager.SetCountryCode(msg.m_countryCode);
			SystemSaveManager.CheckIAPMessage();
		}
	}

	private void DispatchMessage(MessageBase message)
	{
		if (m_fsm != null && message != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateMessage(message);
			m_fsm.Dispatch(signal);
		}
	}

	private void ServerGetMileageData_Succeeded()
	{
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(107);
			m_fsm.Dispatch(signal);
		}
	}

	private void ServerGetEventList_Succeeded(MsgGetEventListSucceed msg)
	{
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(108);
			m_fsm.Dispatch(signal);
		}
	}

	private void IapInitializeEndCallback(NativeObserver.IAPResult result)
	{
	}

	public void OnTouchedScreen()
	{
		if (m_initUser)
		{
			m_nextSceneName = "MainMenu";
			SoundManager.SePlay("sys_menu_decide");
			if (m_fsm != null)
			{
				TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
				m_fsm.Dispatch(signal);
			}
		}
	}

	public void OnTouchedAcknowledgment()
	{
		m_nextSceneName = "MainMenu";
		SoundManager.SePlay("sys_menu_decide");
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
			m_fsm.Dispatch(signal);
		}
	}

	public void OnTouchedAgreement()
	{
		SoundManager.SePlay("sys_menu_decide");
		Application.OpenURL(NetBaseUtil.RedirectTrmsOfServicePageUrlForTitle);
	}

	private IEnumerator OpenAgreementWindow()
	{
		m_agreementText = "test";
		if (m_agreementText == null || m_agreementText == string.Empty)
		{
			string url = NetUtil.GetWebPageURL(InformationDataTable.Type.TERMS_OF_SERVICE);
			GameObject htmlParserGameObject = HtmlParserFactory.Create(url, HtmlParser.SyncType.TYPE_ASYNC, HtmlParser.SyncType.TYPE_ASYNC);
			if (htmlParserGameObject == null)
			{
				yield return null;
			}
			HtmlParser htmlParser = htmlParserGameObject.GetComponent<HtmlParser>();
			if (htmlParser == null)
			{
				yield return null;
			}
			if (htmlParser != null)
			{
				while (!htmlParser.IsEndParse)
				{
					yield return null;
				}
				m_agreementText = htmlParser.ParsedString;
				UnityEngine.Object.Destroy(htmlParserGameObject);
			}
		}
		if (m_agreementText != null && m_agreementText != string.Empty)
		{
			TextObject title = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "gw_legal_caption");
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "AgreementLegal";
			info.anchor_path = "Camera/Anchor_5_MC";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = title.text;
			info.message = m_agreementText;
			GeneralWindow.Create(info);
		}
	}

	public void OnClickMigrationButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		Debug.Log("GameModeTitle:Migration button pressed");
		if (m_fsm != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(111);
			m_fsm.Dispatch(signal);
		}
	}

	public void OnClickCacheClearButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		Debug.Log("GameModeTitle:cache clear button pressed");
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "cache_clear";
		info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Option", "cash_cashclear_bar");
		info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Option", "cash_cashclear_explanation_title");
		info.anchor_path = "Camera/Anchor_5_MC";
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		GeneralWindow.Create(info);
	}

	public void SegaLogoAnimationSkip()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject == null)
		{
			return;
		}
		SoundManager.BgmPlay("bgm_sys_title");
		if (m_touchScreenObject != null)
		{
			m_touchScreenObject.SetActive(true);
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "ui_Lbl_mainmenu");
			UILabel component = m_touchScreenObject.GetComponent<UILabel>();
			if (component != null)
			{
				component.text = text.text;
			}
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_touchScreenObject, "Lbl_mainmenu_sh");
			if (uILabel != null)
			{
				uILabel.text = text.text;
			}
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "img_titlelogo");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(true);
		}
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(gameObject, "title_logo");
		if (animation != null)
		{
			foreach (AnimationState item in animation)
			{
				if (!(item == null))
				{
					item.time = item.length * 0.99f;
				}
			}
			animation.enabled = true;
			animation.Play("ui_title_loop_Anim");
		}
		Animation animation2 = GameObjectUtil.FindChildGameObjectComponent<Animation>(gameObject, "TitleScreen");
		if (!(animation2 != null))
		{
			return;
		}
		foreach (AnimationState item2 in animation2)
		{
			if (!(item2 == null))
			{
				item2.time = item2.length * 0.99f;
			}
		}
		ActiveAnimation.Play(animation2, "ui_title_intro_all_Anim", Direction.Forward);
	}

	public void SegaLogoAnimationFinishCallback()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (!(gameObject == null))
		{
			SoundManager.BgmPlay("bgm_sys_title");
			Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(gameObject, "title_logo");
			if (animation != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(animation, "ui_title_intro_Anim", Direction.Forward);
				EventDelegate.Add(activeAnimation.onFinished, TitleLogoAnimationFinishCallback, false);
			}
			Animation animation2 = GameObjectUtil.FindChildGameObjectComponent<Animation>(gameObject, "TitleScreen");
			if (animation2 != null)
			{
				ActiveAnimation.Play(animation2, "ui_title_intro_all_Anim", Direction.Forward);
			}
		}
	}

	public void TitleLogoAnimationFinishCallback()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (!(gameObject == null))
		{
			Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(gameObject, "title_logo");
			if (animation != null)
			{
				animation.enabled = true;
				animation.Play("ui_title_loop_Anim");
			}
		}
	}

	public void OnMsgGotoHead()
	{
		if (!(m_fsm != null))
		{
			return;
		}
		if (m_loader != null)
		{
			UnityEngine.Object.Destroy(m_loader);
			m_loader = null;
		}
		if (m_loadingWindow != null)
		{
			UnityEngine.Object.Destroy(m_loadingWindow);
			m_loadingWindow = null;
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				m_loadingWindow = GameObjectUtil.FindChildGameObjectComponent<HudLoadingWindow>(gameObject, "DownloadWindow");
			}
		}
		m_fsm.ChangeState(new TinyFsmState(StateLoadTitleResetScene));
	}

	public void StreamingKeyDataRetry()
	{
		StreamingDataKeyRetryProcess process = new StreamingDataKeyRetryProcess(base.gameObject, this);
		NetMonitor.Instance.StartMonitor(process, 0f, HudNetworkConnect.DisplayType.ALL);
		StreamingDataLoader.Instance.LoadServerKey(base.gameObject);
	}

	private void OnClickPlatformBackButtonEvent()
	{
		Debug.Log("GameModeTitle::Platform Back button pressed");
		SetUIEffect(false);
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "quit_app";
		info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "gw_quit_app_caption");
		info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "gw_quit_app_text");
		info.anchor_path = "Camera/Anchor_5_MC";
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		GeneralWindow.Create(info);
	}

	private void SetUIEffect(bool flag)
	{
		if (UIEffectManager.Instance != null)
		{
			UIEffectManager.Instance.SetActiveEffect(HudMenuUtility.EffectPriority.Menu, flag);
		}
	}

	private string GetViewUserID()
	{
		string text = TitleUtil.GetSystemSaveDataGameId();
		if (text.Length > 7)
		{
			text = text.Insert(6, " ");
			text = text.Insert(3, " ");
		}
		return text;
	}

	private void SetFirstTutorialInfo()
	{
		GameObject gameObject = GameObject.Find("StageInfo");
		if (gameObject != null)
		{
			StageInfo component = gameObject.GetComponent<StageInfo>();
			if (component != null)
			{
				StageInfo.MileageMapInfo mileageMapInfo = new StageInfo.MileageMapInfo();
				mileageMapInfo.m_mapState.m_episode = 1;
				mileageMapInfo.m_mapState.m_chapter = 1;
				mileageMapInfo.m_mapState.m_point = 0;
				mileageMapInfo.m_mapState.m_score = 0L;
				component.SelectedStageName = StageInfo.GetStageNameByIndex(1);
				component.TenseType = TenseType.AFTERNOON;
				component.ExistBoss = true;
				component.BossStage = false;
				component.TutorialStage = false;
				component.FromTitle = false;
				component.FirstTutorial = true;
				component.MileageInfo = mileageMapInfo;
			}
		}
		LoadingInfo loadingInfo = LoadingInfo.CreateLoadingInfo();
		if (loadingInfo != null)
		{
			LoadingInfo.LoadingData info = loadingInfo.GetInfo();
			if (info != null)
			{
				string cellID = CharaName.Name[0];
				string commonText = TextUtility.GetCommonText("CharaName", cellID);
				info.m_titleText = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FirstLoading", "ui_Lbl_title_text").text;
				info.m_mainText = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FirstLoading", "ui_Lbl_main_text").text;
				info.m_optionTutorial = true;
				info.m_texture = null;
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
