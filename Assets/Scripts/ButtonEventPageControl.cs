using Message;
using System.Collections;
using UnityEngine;

public class ButtonEventPageControl : MonoBehaviour
{
	private class WaitSendMessageParam
	{
		public Component m_component;

		public int m_waitCount;

		public string m_methodName;

		public object m_value;
	}

	public delegate void ResourceLoadedCallback();

	private const int WAIT_SEND_MESSAGE_WAIT = 30;

	private ButtonInfoTable.PageType m_currentPageType;

	private MsgMenuSequence.SequeneceType m_current_sequence_type;

	private ButtonInfoTable m_info_table = new ButtonInfoTable();

	private GameObject m_menu_anim_obj;

	private ButtonEventResourceLoader m_resLoader;

	private ButtonEventBackButton m_backButton;

	private ButtonEventPageHistory m_pageHistory;

	private HudDisplay m_hud_display;

	private ButtonEventAnimation m_animation;

	private bool m_transform;

	private ResourceLoadedCallback m_resourceLoadedCallback;

	public bool IsTransform
	{
		get
		{
			return m_transform;
		}
	}

	public void Initialize(ResourceLoadedCallback callback)
	{
		m_menu_anim_obj = HudMenuUtility.GetMenuAnimUIObject();
		GameObject parent = GameObjectUtil.FindChildGameObject(m_menu_anim_obj, "MainMenuUI4");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(parent, "page_1");
		m_resLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
		HudMenuUtility.SendMsgInitMainMenuUI();
		m_pageHistory = new ButtonEventPageHistory();
		m_pageHistory.Push(ButtonInfoTable.PageType.MAIN);
		m_hud_display = new HudDisplay();
		m_animation = base.gameObject.AddComponent<ButtonEventAnimation>();
		m_animation.Initialize();
		ChangeHeaderText(ButtonInfoTable.PageType.MAIN);
		m_resourceLoadedCallback = callback;
	}

	public void PageBack()
	{
		if (!m_transform)
		{
			ButtonInfoTable.ButtonType buttonType = m_info_table.m_platformBackButtonType[(int)m_currentPageType];
			PageChange(buttonType, false, true);
		}
	}

	public void PageChange(ButtonInfoTable.ButtonType buttonType, bool clearHistory, bool buttonPressed)
	{
		if (!m_transform)
		{
			if (CheckEventTopRewardListRoutletteButtonClick(buttonType))
			{
				SendMsgEventWindow(buttonType);
			}
			else if (CheckEventTopShopButtonClick(buttonType))
			{
				SendMsgEventWindow(buttonType);
				SetClickedEvent(buttonType, clearHistory);
			}
			else if (m_currentPageType == ButtonInfoTable.PageType.EVENT)
			{
				SendMsgEventWindow(buttonType);
				SetClickedEvent(buttonType, clearHistory);
			}
			else
			{
				SetClickedEvent(buttonType, clearHistory);
			}
			if (buttonPressed)
			{
				m_info_table.PlaySE(buttonType);
			}
		}
	}

	private void SetClickedEvent(ButtonInfoTable.ButtonType button_type, bool clearHistory)
	{
		Debug.Log("SetClicedEvent " + button_type);
		MsgMenuSequence.SequeneceType sequeneceType = m_info_table.GetSequeneceType(button_type);
		if (sequeneceType == MsgMenuSequence.SequeneceType.MAIN)
		{
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		}
		bool flag = sequeneceType == MsgMenuSequence.SequeneceType.BACK;
		ButtonInfoTable.PageType pageType;
		ButtonInfoTable.PageType currentPageType;
		if (flag)
		{
			pageType = m_pageHistory.Pop();
			currentPageType = m_currentPageType;
		}
		else
		{
			pageType = m_info_table.GetPageType(button_type);
			currentPageType = m_currentPageType;
			if (clearHistory)
			{
				m_pageHistory.Clear();
			}
			else if (button_type != ButtonInfoTable.ButtonType.VIRTUAL_NEW_ITEM && pageType != ButtonInfoTable.PageType.NON)
			{
				m_pageHistory.Push(currentPageType);
			}
		}
		m_current_sequence_type = m_info_table.GetSequeneceType(pageType);
		m_currentPageType = pageType;
		if (pageType == ButtonInfoTable.PageType.NON && !flag)
		{
			HudMenuUtility.SendMsgMenuSequenceToMainMenu(sequeneceType);
			return;
		}
		if (currentPageType == ButtonInfoTable.PageType.MAIN && flag)
		{
			HudMenuUtility.SendMsgMenuSequenceToMainMenu(MsgMenuSequence.SequeneceType.BACK);
			return;
		}
		m_transform = true;
		HudMenuUtility.SetConnectAlertSimpleUI(true);
		ChangeHeaderText(pageType);
		switch (pageType)
		{
		case ButtonInfoTable.PageType.ROULETTE:
			SetRoulletePage(button_type);
			break;
		case ButtonInfoTable.PageType.EPISODE_RANKING:
			RankingUtil.SetCurrentRankingMode(RankingUtil.RankingMode.ENDLESS);
			break;
		case ButtonInfoTable.PageType.QUICK_RANKING:
			RankingUtil.SetCurrentRankingMode(RankingUtil.RankingMode.QUICK);
			break;
		}
		SendMessageEndPage(currentPageType);
		m_animation.PageOutAnimation(currentPageType, pageType, OnCurrentPageAnimEndCallback);
	}

	private bool CheckEventTopShopButtonClick(ButtonInfoTable.ButtonType btnType)
	{
		if (btnType == ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP || btnType == ButtonInfoTable.ButtonType.RING_TO_SHOP || btnType == ButtonInfoTable.ButtonType.CHALLENGE_TO_SHOP || btnType == ButtonInfoTable.ButtonType.RAIDENERGY_TO_SHOP)
		{
			return m_currentPageType == ButtonInfoTable.PageType.EVENT;
		}
		return false;
	}

	private bool CheckEventTopRewardListRoutletteButtonClick(ButtonInfoTable.ButtonType btnType)
	{
		if (btnType == ButtonInfoTable.ButtonType.REWARDLIST_TO_CHAO_ROULETTE)
		{
			return m_currentPageType == ButtonInfoTable.PageType.EVENT;
		}
		return false;
	}

	public void SetRoulletePage(ButtonInfoTable.ButtonType button_type)
	{
		if (RouletteUtility.rouletteDefault != RouletteCategory.RAID)
		{
			if (button_type == ButtonInfoTable.ButtonType.ITEM_ROULETTE)
			{
				RouletteUtility.rouletteDefault = RouletteCategory.ITEM;
			}
			else
			{
				RouletteUtility.rouletteDefault = RouletteCategory.PREMIUM;
			}
		}
	}

	private void ChangeHeaderText(ButtonInfoTable.PageType pageType)
	{
		string[] array = new string[23]
		{
			"ui_Header_main_menu",
			"ui_Header_ChaoSet",
			string.Empty,
			"ui_Header_Information",
			"ui_Header_Item",
			string.Empty,
			"ui_Header_Option",
			"ui_Header_PlayerSet",
			"ui_Header_PlayerSet",
			"ui_Header_PresentBox",
			"ui_Header_Information",
			"ui_Header_daily_battle",
			"ui_Header_Roulette_top",
			"ui_Header_Shop",
			"ui_Header_Shop",
			"ui_Header_Shop",
			"ui_Header_Shop",
			"ui_Header_MainPage2",
			"ui_Header_Item",
			"ui_Header_episodemode_score_ranking",
			"ui_Header_Item",
			"ui_Header_quickmode_score_ranking",
			"ui_Header_Item"
		};
		HudMenuUtility.SendChangeHeaderText(array[(int)pageType]);
	}

	private void OnCurrentPageAnimEndCallback()
	{
		StartCoroutine(OnCurrentPageAnimationEndCallbackCoroutine());
		switch (m_currentPageType)
		{
		case ButtonInfoTable.PageType.ROULETTE:
			break;
		case ButtonInfoTable.PageType.SHOP_RSR:
		case ButtonInfoTable.PageType.SHOP_RING:
		case ButtonInfoTable.PageType.SHOP_ENERGY:
		case ButtonInfoTable.PageType.SHOP_EVENT:
			break;
		case ButtonInfoTable.PageType.EPISODE:
		case ButtonInfoTable.PageType.EPISODE_PLAY:
		case ButtonInfoTable.PageType.EPISODE_RANKING:
		case ButtonInfoTable.PageType.PLAY_AT_EPISODE_PAGE:
			SoundManager.BgmChange("bgm_sys_menu");
			break;
		case ButtonInfoTable.PageType.DAILY_BATTLE:
		case ButtonInfoTable.PageType.QUICK:
		case ButtonInfoTable.PageType.QUICK_RANKING:
			SoundManager.BgmChange("bgm_sys_menu");
			break;
		default:
			SoundManager.BgmChange("bgm_sys_menu_v2", "BGM_menu_v2");
			break;
		}
	}

	private IEnumerator OnCurrentPageAnimationEndCallbackCoroutine()
	{
		m_hud_display.SetAllDisableDisplay();
		yield return StartCoroutine(m_resLoader.LoadAtlasResourceIfNotLoaded());
		yield return StartCoroutine(m_resLoader.LoadPageResourceIfNotLoadedSync(m_currentPageType, delegate
		{
			m_resourceLoadedCallback();
		}));
		m_hud_display = new HudDisplay();
		HudDisplay.ObjType obj_type = HudDisplay.CalcObjTypeFromSequenceType(m_current_sequence_type);
		m_hud_display.SetDisplayHudObject(obj_type);
		HudMenuUtility.SendMsgMenuSequenceToMainMenu(m_current_sequence_type);
		SendMessageNextPage(m_currentPageType);
		m_animation.PageInAnimation(m_currentPageType, OnNextPageAnimEndCallback);
	}

	private void OnNextPageAnimEndCallback()
	{
		m_transform = false;
		HudMenuUtility.SetConnectAlertSimpleUI(false);
	}

	private void SendMsgEventWindow(ButtonInfoTable.ButtonType button_type)
	{
		if (!(EventManager.Instance != null))
		{
			return;
		}
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			if (EventManager.Instance.Type == EventManager.EventType.SPECIAL_STAGE)
			{
				GameObjectUtil.SendMessageFindGameObject("SpecialStageWindowUI", "OnClickEndButton", button_type, SendMessageOptions.RequireReceiver);
			}
			else if (EventManager.Instance.Type == EventManager.EventType.RAID_BOSS)
			{
				GameObjectUtil.SendMessageFindGameObject("RaidBossWindowUI", "OnClickEndButton", button_type, SendMessageOptions.RequireReceiver);
			}
		}
	}

	private void SendMessageEndPage(ButtonInfoTable.PageType endPage)
	{
		if (endPage == ButtonInfoTable.PageType.ITEM || endPage == ButtonInfoTable.PageType.QUICK || endPage == ButtonInfoTable.PageType.EPISODE_PLAY || endPage == ButtonInfoTable.PageType.PLAY_AT_EPISODE_PAGE)
		{
			if (m_currentPageType != ButtonInfoTable.PageType.STAGE)
			{
				GameObjectUtil.SendMessageFindGameObject("ItemSet_3_UI", "OnMsgMenuBack", null, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			ButtonInfoTable.MessageInfo pageMessageInfo = m_info_table.GetPageMessageInfo(endPage, false);
			SendMessage(pageMessageInfo);
		}
	}

	private void SendMessageNextPage(ButtonInfoTable.PageType nextPage)
	{
		if (nextPage == ButtonInfoTable.PageType.ITEM || nextPage == ButtonInfoTable.PageType.QUICK || nextPage == ButtonInfoTable.PageType.EPISODE_PLAY || nextPage == ButtonInfoTable.PageType.PLAY_AT_EPISODE_PAGE)
		{
			MsgMenuItemSetStart.SetMode msgMenuItemSetStartMode = ItemSetUtility.GetMsgMenuItemSetStartMode();
			MsgMenuItemSetStart value = new MsgMenuItemSetStart(msgMenuItemSetStartMode);
			GameObjectUtil.SendMessageFindGameObject("ItemSet_3_UI", "OnMsgMenuItemSetStart", value, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			ButtonInfoTable.MessageInfo pageMessageInfo = m_info_table.GetPageMessageInfo(nextPage, true);
			SendMessage(pageMessageInfo, true);
		}
	}

	private void SendMessage(ButtonInfoTable.MessageInfo msgInfo, bool waitFlag = false)
	{
		if (msgInfo == null)
		{
			return;
		}
		if (!msgInfo.uiFlag)
		{
			GameObjectUtil.SendMessageFindGameObject(msgInfo.targetName, msgInfo.methodName, null, SendMessageOptions.DontRequireReceiver);
			return;
		}
		int waitCount = waitFlag ? 30 : 0;
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (!(cameraUIObject != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, msgInfo.targetName);
		if (gameObject != null)
		{
			Component component = gameObject.GetComponent(msgInfo.componentName);
			if (component != null)
			{
				WaitSendMessage(component, waitCount, msgInfo.methodName, null);
			}
		}
	}

	public void WaitSendMessage(Component component, int waitCount, string methodName, object value)
	{
		StartCoroutine(WaitSendMessageCoroutine(new WaitSendMessageParam
		{
			m_component = component,
			m_waitCount = waitCount,
			m_methodName = methodName,
			m_value = value
		}));
	}

	private IEnumerator WaitSendMessageCoroutine(WaitSendMessageParam param)
	{
		for (int i = 0; i < param.m_waitCount; i++)
		{
			if (param.m_component.gameObject.activeInHierarchy)
			{
				break;
			}
			yield return null;
		}
		param.m_component.SendMessage(param.m_methodName, param.m_value, SendMessageOptions.DontRequireReceiver);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
