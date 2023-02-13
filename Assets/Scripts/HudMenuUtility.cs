using DataTable;
using Message;
using SaveData;
using UnityEngine;

public class HudMenuUtility
{
	public enum ITEM_SELECT_MODE
	{
		NORMAL,
		EVENT_STAGE,
		EVENT_BOSS,
		EVENT_ETC
	}

	public enum EffectPriority
	{
		None = -1,
		Menu,
		UniqueWindow,
		GeneralWindow,
		NetworkErrorWindow,
		Num
	}

	private static ITEM_SELECT_MODE s_itemSelectMode;

	public static readonly int NumPlayingRouletteTutorial = 7;

	public static ITEM_SELECT_MODE itemSelectMode
	{
		get
		{
			return s_itemSelectMode;
		}
	}

	public static GameObject GetMenuAnimUIObject()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			Transform transform = gameObject.transform.FindChild("Camera/menu_Anim");
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public static GameObject GetCameraUIObject()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			Transform transform = gameObject.transform.FindChild("Camera");
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public static GameObject GetMainMenuUIObject()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			Transform transform = gameObject.transform.FindChild("Camera/menu_Anim/MainMenuUI4");
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public static GameObject GetMainMenuCmnUIObject()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			Transform transform = gameObject.transform.FindChild("Camera/menu_Anim/MainMenuCmnUI");
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public static GameObject GetMainMenuGeneralAnchor()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			Transform transform = gameObject.transform.FindChild("Camera/Anchor_5_MC");
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public static void SetTagHudSaveItem(GameObject obj)
	{
		if (obj != null)
		{
			obj.tag = "HudSaveItem";
		}
	}

	public static void SetTagHudMileageMap(GameObject obj)
	{
		if (obj != null)
		{
			obj.tag = "HudMileageMap";
		}
	}

	public static void SendMsgMenuSequenceToMainMenu(MsgMenuSequence.SequeneceType sequenece_type)
	{
		MsgMenuSequence msgMenuSequence = new MsgMenuSequence(sequenece_type);
		if (msgMenuSequence != null)
		{
			GameObjectUtil.SendMessageFindGameObject("MainMenu", "OnMsgReceive", msgMenuSequence, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void SendMsgUpdateSaveDataDisplay()
	{
		GameObjectUtil.SendMessageToTagObjects("HudSaveItem", "OnUpdateSaveDataDisplay", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgInformationDisplay()
	{
		GameObjectUtil.SendMessageToTagObjects("HudSaveItem", "OnUpdateInformationDisplay", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgTickerUpdate()
	{
		GameObjectUtil.SendMessageToTagObjects("HudSaveItem", "OnUpdateTickerDisplay", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgTickerReset()
	{
		GameObjectUtil.SendMessageToTagObjects("HudSaveItem", "OnTickerReset", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgInitMainMenuUI()
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuUI4", "OnInitDisplay", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgSetEnableSkipButton(bool enableFlag)
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuUI4", "OnSetEnableSkipButton", enableFlag, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgStartRankingProduction()
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuUI4", "OnStartRankingProduction", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgStartLoginRanking(bool fromInformaion = false)
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuUI4", "OnStartLoginRankingDisplay", fromInformaion, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgStartTutorialDisplay(bool autoBtn = false)
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuUI4", "OnStartTutorialDisplay", autoBtn, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgEndTutorialDisplayToHud()
	{
		GameObjectUtil.SendMessageToTagObjects("HudSaveItem", "OnEndTutorialDisplay", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgStartNormal()
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuUI4", "OnStartNormalDisplay", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgUpdateMileageMapDisplayToMileage()
	{
		if (ui_mm_mileage_page.Instance != null)
		{
			ui_mm_mileage_page.Instance.OnUpdateMileageMapDisplay();
		}
	}

	public static void SendMsgPrepareMileageMapProduction(ResultData result)
	{
		if (ui_mm_mileage_page.Instance != null)
		{
			ui_mm_mileage_page.Instance.OnPrepareMileageMapProduction(result);
		}
	}

	public static GameObject GetGameObjectRanking()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			return GameObjectUtil.FindChildGameObject(gameObject, "ui_mm_ranking_page(Clone)");
		}
		return null;
	}

	public static void SendMsgUpdateRanking()
	{
		GameObject gameObjectRanking = GetGameObjectRanking();
		if (gameObjectRanking != null)
		{
			gameObjectRanking.SendMessage("OnUpdateRanking", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void SendMsgNextButtonRanking()
	{
		GameObject gameObjectRanking = GetGameObjectRanking();
		if (gameObjectRanking != null)
		{
			gameObjectRanking.SendMessage("OnClickNextButton", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void SetUpdateRankingFlag()
	{
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			SingletonGameObject<RankingManager>.Instance.Init(null);
		}
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "OnUpdateRankingFlag", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendMsgUpdateChallengeDisply()
	{
		GameObjectUtil.SendMessageToTagObjects("HudSaveItem", "OnUpdateChallengeCountDisply", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendStartMainMenuDlsplay()
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "OnStartMainMenu", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendStartPlayerChaoPage()
	{
		GameObject mainMenuUIObject = GetMainMenuUIObject();
		if (mainMenuUIObject != null)
		{
			MainMenuUI component = mainMenuUIObject.GetComponent<MainMenuUI>();
			if (!(component != null))
			{
			}
		}
		MsgMenuButtonEvent msgMenuButtonEvent = new MsgMenuButtonEvent(ButtonInfoTable.ButtonType.ITEM_BACK);
		msgMenuButtonEvent.m_clearHistories = true;
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "PageChangeMessage", msgMenuButtonEvent, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendStartInformaionDlsplay()
	{
		GameObject gameObject = GameObject.Find("InformationUI");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "news_set");
			if (gameObject2 != null)
			{
				gameObject2.SendMessage("OnStartInformation", null, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static void SendMenuButtonClicked(ButtonInfoTable.ButtonType type, bool clearHistories = false)
	{
		MsgMenuButtonEvent msgMenuButtonEvent = new MsgMenuButtonEvent(type);
		msgMenuButtonEvent.m_clearHistories = clearHistories;
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "PageChangeMessage", msgMenuButtonEvent, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendItemRouletteButtonClicked()
	{
		MsgMenuButtonEvent value = new MsgMenuButtonEvent(ButtonInfoTable.ButtonType.ITEM_ROULETTE);
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "PageChangeMessage", value, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendChaoRouletteButtonClicked()
	{
		MsgMenuButtonEvent value = new MsgMenuButtonEvent(ButtonInfoTable.ButtonType.CHAO_ROULETTE);
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "PageChangeMessage", value, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendVirtualNewItemSelectClicked(ITEM_SELECT_MODE mode = ITEM_SELECT_MODE.NORMAL)
	{
		s_itemSelectMode = mode;
		MsgMenuButtonEvent value = new MsgMenuButtonEvent(ButtonInfoTable.ButtonType.VIRTUAL_NEW_ITEM);
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "PageChangeMessage", value, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendUIPageStart()
	{
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "OnPageStart", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendUIPageEnd()
	{
		MsgMenuButtonEvent value = new MsgMenuButtonEvent(ButtonInfoTable.ButtonType.ITEM_ROULETTE);
		GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "OnPageEnd", value, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendChangeHeaderText(string cellName)
	{
		MsgMenuHeaderText msgMenuHeaderText = new MsgMenuHeaderText(cellName);
		GameObject gameObject = GameObject.Find("HudSaveItem");
		if (!(gameObject == null))
		{
			HudHeaderPageName hudHeaderPageName = GameObjectUtil.FindChildGameObjectComponent<HudHeaderPageName>(gameObject, "HudHeaderPageName");
			if (!(hudHeaderPageName == null))
			{
				string headerText = HudHeaderPageName.CalcHeaderTextByCellName(cellName);
				hudHeaderPageName.ChangeHeaderText(headerText);
			}
		}
	}

	public static void SendChangeMainPageHeaderText()
	{
		GameObjectUtil.SendMessageToTagObjects("HudSaveItem", "OnSendChangeMainPageHeaderText", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void SendEnableShopButton(bool enableFlag)
	{
		GameObjectUtil.SendMessageToTagObjects("HudSaveItem", "OnEnableShopButton", enableFlag, SendMessageOptions.DontRequireReceiver);
	}

	public static void OnForceDisableShopButton(bool disableFlag)
	{
		GameObjectUtil.SendMessageToTagObjects("HudSaveItem", "OnForceDisableShopButton", disableFlag, SendMessageOptions.DontRequireReceiver);
	}

	public static bool IsSale(Constants.Campaign.emType type)
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerCampaignState campaignState = ServerInterface.CampaignState;
			if (campaignState != null)
			{
				return campaignState.InAnyIdSession(type);
			}
		}
		return false;
	}

	public static void GoToTitleScene()
	{
		GameModeTitle.Logined = false;
		CleanUpAllResources();
		ServerSessionWatcher serverSessionWatcher = GameObjectUtil.FindGameObjectComponent<ServerSessionWatcher>("NetMonitor");
		if (serverSessionWatcher != null)
		{
			serverSessionWatcher.InvalidateSession();
		}
		if (Application.loadedLevelName == TitleDefine.TitleSceneName)
		{
			GameObject gameObject = GameObject.Find("GameModeTitle");
			if (gameObject != null)
			{
				gameObject.SendMessage("OnMsgGotoHead", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			Application.LoadLevel(TitleDefine.TitleSceneName);
		}
	}

	public static void CleanUpAllResources()
	{
		ResourceManager instance = ResourceManager.Instance;
		if (instance != null)
		{
			instance.RemoveAllResources();
		}
		if (AssetBundleLoader.Instance != null)
		{
			AssetBundleLoader.Instance.ClearDownloadList();
			Object.Destroy(AssetBundleLoader.Instance);
		}
		if (AssetBundleManager.Instance != null)
		{
			Object.Destroy(AssetBundleManager.Instance);
		}
		ChaoTextureManager instance2 = ChaoTextureManager.Instance;
		if (instance2 != null)
		{
			Object.Destroy(instance2.gameObject);
		}
		AchievementManager instance3 = AchievementManager.Instance;
		if (instance3 != null)
		{
			Object.Destroy(instance3.gameObject);
		}
		StageAbilityManager instance4 = StageAbilityManager.Instance;
		if (instance4 != null)
		{
			Object.Destroy(instance4.gameObject);
		}
		CharacterDataNameInfo instance5 = CharacterDataNameInfo.Instance;
		if (instance5 != null)
		{
			Object.Destroy(instance5.gameObject);
		}
		MissionTable instance6 = MissionTable.Instance;
		if (instance6 != null)
		{
			Object.Destroy(instance6.gameObject);
		}
		InformationDataTable instance7 = InformationDataTable.Instance;
		if (instance7 != null)
		{
			Object.Destroy(instance7.gameObject);
		}
		MileageMapDataManager instance8 = MileageMapDataManager.Instance;
		if (instance8 != null)
		{
			Object.Destroy(instance8.gameObject);
		}
		AtlasManager instance9 = AtlasManager.Instance;
		if (instance9 != null)
		{
			Object.Destroy(instance9.gameObject);
		}
		EventManager instance10 = EventManager.Instance;
		if (instance10 != null)
		{
			Object.Destroy(instance10.gameObject);
		}
		GameObject gameObject = GameObject.Find("TextManager");
		if (gameObject != null)
		{
			Object.Destroy(gameObject);
		}
	}

	public static GameObject GetLoadMenuChildObject(string name, bool active)
	{
		GameObject menuAnimUIObject = GetMenuAnimUIObject();
		if (menuAnimUIObject != null)
		{
			Transform transform = menuAnimUIObject.transform.FindChild(name);
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				if (gameObject != null)
				{
					if (active)
					{
						gameObject.SetActive(true);
					}
					return gameObject;
				}
			}
			Transform transform2 = menuAnimUIObject.transform.FindChild("OptionWindows");
			if (transform2 != null)
			{
				Transform transform3 = transform2.FindChild(name);
				if (transform3 != null)
				{
					GameObject gameObject2 = transform3.gameObject;
					if (active)
					{
						gameObject2.SetActive(true);
					}
					return gameObject2;
				}
			}
		}
		GameObject mainMenuGeneralAnchor = GetMainMenuGeneralAnchor();
		if (mainMenuGeneralAnchor != null)
		{
			Transform transform4 = mainMenuGeneralAnchor.transform.FindChild(name);
			if (transform4 != null)
			{
				GameObject gameObject3 = transform4.gameObject;
				if (gameObject3 != null)
				{
					if (active)
					{
						gameObject3.SetActive(true);
					}
					return gameObject3;
				}
			}
		}
		return null;
	}

	public static UIDraggablePanel GetMainMenuDraggablePanel()
	{
		GameObject mainMenuUIObject = GetMainMenuUIObject();
		if (mainMenuUIObject != null)
		{
			Transform transform = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_contents");
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				if (gameObject != null)
				{
					return gameObject.GetComponent<UIDraggablePanel>();
				}
			}
		}
		return null;
	}

	public static bool IsTutorial_2_1_0()
	{
		ServerMileageMapState mileageMapState = ServerInterface.MileageMapState;
		if (mileageMapState != null && mileageMapState.m_episode == 2 && mileageMapState.m_chapter == 1 && mileageMapState.m_stageTotalScore == 0L)
		{
			return true;
		}
		return false;
	}

	public static bool IsItemTutorial()
	{
		if (ServerInterface.PlayerState.m_numPlaying == 0 && !IsSystemDataFlagStatus(SystemData.FlagStatus.TUTORIAL_EQIP_ITEM_END))
		{
			return true;
		}
		return false;
	}

	public static bool IsNumPlayingRouletteTutorial()
	{
		if (ServerInterface.PlayerState.m_numPlaying == NumPlayingRouletteTutorial)
		{
			return true;
		}
		return false;
	}

	public static bool IsTutorialCharaLevelUp()
	{
		if (ServerInterface.PlayerState.m_numPlaying == 9 && IsTutorial_CharaLevelUp())
		{
			return true;
		}
		return false;
	}

	public static bool IsRouletteTutorial()
	{
		if (IsNumPlayingRouletteTutorial())
		{
			return RouletteUtility.isTutorial;
		}
		return false;
	}

	public static bool IsRecommendReviewTutorial()
	{
		if (ServerInterface.PlayerState.m_numPlaying == 10 && !IsSystemDataFlagStatus(SystemData.FlagStatus.RECOMMEND_REVIEW_END))
		{
			return true;
		}
		return false;
	}

	public static bool IsTutorial_11()
	{
		if (!IsSystemDataFlagStatus(SystemData.FlagStatus.ANOTHER_CHARA_EXPLAINED))
		{
			ServerMileageMapState mileageMapState = ServerInterface.MileageMapState;
			if (mileageMapState != null && mileageMapState.m_episode == 11)
			{
				ServerPlayerState playerState = ServerInterface.PlayerState;
				if (playerState != null)
				{
					ServerCharacterState serverCharacterState = playerState.CharacterState(CharaType.TAILS);
					if (serverCharacterState != null)
					{
						return serverCharacterState.Status != ServerCharacterState.CharacterStatus.Locked;
					}
				}
			}
		}
		return false;
	}

	public static bool IsTutorial_SubCharaItem()
	{
		if (!IsSystemDataFlagStatus(SystemData.FlagStatus.SUB_CHARA_ITEM_EXPLAINED))
		{
			CharaType subChara = SaveDataManager.Instance.PlayerData.SubChara;
			if (subChara != CharaType.UNKNOWN && subChara < CharaType.NUM)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsTutorial_CharaLevelUp()
	{
		if (!IsSystemDataFlagStatus(SystemData.FlagStatus.CHARA_LEVEL_UP_EXPLAINED) && ServerInterface.PlayerState.m_numPlaying <= 9)
		{
			uint num = 0u;
			if (SaveDataManager.Instance != null)
			{
				CharaType mainChara = SaveDataManager.Instance.PlayerData.MainChara;
				num = SaveDataUtil.GetCharaLevel(mainChara);
			}
			if (num < 70)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsSystemDataFlagStatus(SystemData.FlagStatus flag)
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				return systemdata.IsFlagStatus(flag);
			}
		}
		return false;
	}

	public static void SaveSystemDataFlagStatus(SystemData.FlagStatus flag)
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				systemdata.SetFlagStatus(flag, true);
			}
			instance.SaveSystemData();
		}
	}

	public static void SetConnectAlertSimpleUI(bool on)
	{
		GameObject cameraUIObject = GetCameraUIObject();
		if (!(cameraUIObject != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "ConnectAlertSimpleUI");
		if (!(gameObject != null))
		{
			return;
		}
		ConnectAlertSimpleUI component = gameObject.GetComponent<ConnectAlertSimpleUI>();
		if (component != null)
		{
			if (on)
			{
				component.StartCollider();
			}
			else
			{
				component.EndCollider();
			}
		}
	}

	public static void SetConnectAlertMenuButtonUI(bool on)
	{
		GameObject cameraUIObject = GetCameraUIObject();
		if (!(cameraUIObject != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "ConnectAlertMenuButtonUI");
		if (!(gameObject != null))
		{
			return;
		}
		ConnectAlertSimpleUI component = gameObject.GetComponent<ConnectAlertSimpleUI>();
		if (component != null)
		{
			if (on)
			{
				component.StartCollider();
			}
			else
			{
				component.EndCollider();
			}
		}
	}

	public static void CheckCurrentTextures()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (!(gameObject != null))
		{
			return;
		}
		UITexture[] componentsInChildren = gameObject.GetComponentsInChildren<UITexture>();
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			UITexture[] array = componentsInChildren;
			foreach (UITexture uITexture in array)
			{
				Debug.Log("UITexture object:" + uITexture.gameObject);
			}
		}
	}

	public static void RemoveCurrentTextures()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (!(gameObject != null))
		{
			return;
		}
		UITexture[] componentsInChildren = gameObject.GetComponentsInChildren<UITexture>();
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			UITexture[] array = componentsInChildren;
			foreach (UITexture uITexture in array)
			{
				Object.DestroyImmediate(uITexture.mainTexture, true);
			}
		}
	}

	public static void StartMainMenuBGM()
	{
		SoundManager.BgmPlay("bgm_sys_menu_v2", "BGM_menu_v2");
	}

	public static void ChangeMainMenuBGM()
	{
		SoundManager.BgmChange("bgm_sys_menu_v2", "BGM_menu_v2");
	}

	public static void ChangeEventBGM()
	{
		string data = EventCommonDataTable.Instance.GetData(EventCommonDataItem.EventTop_BgmName);
		if (!string.IsNullOrEmpty(data))
		{
			string cueSheetName = "BGM_" + EventManager.GetEventTypeName(EventManager.Instance.Type);
			SoundManager.BgmChange(data, cueSheetName);
		}
		else
		{
			ChangeMainMenuBGM();
		}
	}
}
