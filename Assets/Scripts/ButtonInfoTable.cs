using Message;
using UnityEngine;

public class ButtonInfoTable
{
	public enum PageType
	{
		MAIN = 0,
		CHAO = 1,
		EVENT = 2,
		INFOMATION = 3,
		ITEM = 4,
		STAGE = 5,
		OPTION = 6,
		PLAYER_MAIN = 7,
		PLAYER_SUB = 8,
		PRESENT_BOX = 9,
		DAILY_CHALLENGE = 10,
		DAILY_BATTLE = 11,
		ROULETTE = 12,
		SHOP_RSR = 13,
		SHOP_RING = 14,
		SHOP_ENERGY = 0xF,
		SHOP_EVENT = 0x10,
		EPISODE = 17,
		EPISODE_PLAY = 18,
		EPISODE_RANKING = 19,
		QUICK = 20,
		QUICK_RANKING = 21,
		PLAY_AT_EPISODE_PAGE = 22,
		NUM = 23,
		NON = -1
	}

	public class AnimInfo
	{
		public string animName;

		public string targetName;

		public AnimInfo(string targetName, string animName)
		{
			this.targetName = targetName;
			this.animName = animName;
		}
	}

	public class MessageInfo
	{
		public string targetName;

		public string methodName;

		public string componentName;

		public bool uiFlag;

		public MessageInfo(string target, string method, string component, bool ui = true)
		{
			targetName = target;
			methodName = method;
			componentName = component;
			uiFlag = ui;
		}
	}

	public class ButtonInfo
	{
		public MsgMenuSequence.SequeneceType nextMenuId = MsgMenuSequence.SequeneceType.NON;

		public PageType nextPageType = PageType.NON;

		public MessageInfo btnMsgInfo;

		public string clickButtonPath = string.Empty;

		public string seName = string.Empty;

		public ButtonInfo(MsgMenuSequence.SequeneceType menuId, PageType pageType, string path, string se, MessageInfo info = null)
		{
			nextMenuId = menuId;
			nextPageType = pageType;
			btnMsgInfo = info;
			clickButtonPath = path;
			seName = se;
		}

		public ButtonInfo(MsgMenuSequence.SequeneceType menuId, string path, string se)
		{
			nextMenuId = menuId;
			nextPageType = PageType.NON;
			btnMsgInfo = null;
			clickButtonPath = path;
			seName = se;
		}

		public ButtonInfo(MsgMenuSequence.SequeneceType menuId, PageType pageType)
		{
			nextMenuId = menuId;
			nextPageType = pageType;
			btnMsgInfo = null;
			clickButtonPath = string.Empty;
			seName = string.Empty;
		}
	}

	public enum ButtonType
	{
		PRESENT_BOX = 0,
		DAILY_CHALLENGE = 1,
		DAILY_BATTLE = 2,
		CHARA_MAIN = 3,
		CHARA_SUB = 4,
		CHAO = 5,
		VIRTUAL_NEW_ITEM = 6,
		PLAY_ITEM = 7,
		OPTION = 8,
		INFOMATION = 9,
		ROULETTE = 10,
		CHAO_ROULETTE = 11,
		REWARDLIST_TO_CHAO_ROULETTE = 12,
		ITEM_ROULETTE = 13,
		CHAO_TO_ROULETTE = 14,
		EPISODE = 0xF,
		EPISODE_PLAY = 0x10,
		EPISODE_RANKING = 17,
		QUICK = 18,
		QUICK_RANKING = 19,
		PLAY_AT_EPISODE_PAGE = 20,
		PLAY_EVENT = 21,
		PRESENT_BOX_BACK = 22,
		DAILY_CHALLENGE_BACK = 23,
		DAILY_BATTLE_BACK = 24,
		CHARA_BACK = 25,
		ITEM_BACK = 26,
		CHAO_BACK = 27,
		SHOP_BACK = 28,
		EPISODE_BACK = 29,
		EPISODE_PLAY_BACK = 30,
		EPISODE_RANKING_BACK = 0x1F,
		QUICK_BACK = 0x20,
		QUICK_RANKING_BACK = 33,
		PLAY_AT_EPISODE_PAGE_BACK = 34,
		INFOMATION_BACK = 35,
		ROULETTE_BACK = 36,
		OPTION_BACK = 37,
		REDSTAR_TO_SHOP = 38,
		RING_TO_SHOP = 39,
		CHALLENGE_TO_SHOP = 40,
		RAIDENERGY_TO_SHOP = 41,
		EVENT_RAID = 42,
		EVENT_SPECIAL = 43,
		EVENT_COLLECT = 44,
		EVENT_BACK = 45,
		FORCE_MAIN_BACK = 46,
		TITLE_BACK = 47,
		GO_STAGE = 48,
		NUM = 49,
		UNKNOWN = -1
	}

	public readonly MsgMenuSequence.SequeneceType[] m_sequences = new MsgMenuSequence.SequeneceType[23]
	{
		MsgMenuSequence.SequeneceType.MAIN,
		MsgMenuSequence.SequeneceType.CHAO,
		MsgMenuSequence.SequeneceType.EVENT_TOP,
		MsgMenuSequence.SequeneceType.INFOMATION,
		MsgMenuSequence.SequeneceType.PLAY_ITEM,
		MsgMenuSequence.SequeneceType.NON,
		MsgMenuSequence.SequeneceType.OPTION,
		MsgMenuSequence.SequeneceType.CHARA_MAIN,
		MsgMenuSequence.SequeneceType.CHARA_MAIN,
		MsgMenuSequence.SequeneceType.PRESENT_BOX,
		MsgMenuSequence.SequeneceType.DAILY_CHALLENGE,
		MsgMenuSequence.SequeneceType.DAILY_BATTLE,
		MsgMenuSequence.SequeneceType.ROULETTE,
		MsgMenuSequence.SequeneceType.SHOP,
		MsgMenuSequence.SequeneceType.SHOP,
		MsgMenuSequence.SequeneceType.SHOP,
		MsgMenuSequence.SequeneceType.SHOP,
		MsgMenuSequence.SequeneceType.EPISODE,
		MsgMenuSequence.SequeneceType.EPISODE_PLAY,
		MsgMenuSequence.SequeneceType.EPISODE_RANKING,
		MsgMenuSequence.SequeneceType.QUICK,
		MsgMenuSequence.SequeneceType.QUICK_RANKING,
		MsgMenuSequence.SequeneceType.PLAY_AT_EPISODE_PAGE
	};

	public readonly ButtonType[] m_platformBackButtonType = new ButtonType[23]
	{
		ButtonType.TITLE_BACK,
		ButtonType.CHAO_BACK,
		ButtonType.EVENT_BACK,
		ButtonType.INFOMATION_BACK,
		ButtonType.ITEM_BACK,
		ButtonType.ITEM_BACK,
		ButtonType.OPTION_BACK,
		ButtonType.CHARA_BACK,
		ButtonType.CHARA_BACK,
		ButtonType.PRESENT_BOX_BACK,
		ButtonType.DAILY_CHALLENGE_BACK,
		ButtonType.DAILY_BATTLE_BACK,
		ButtonType.ROULETTE_BACK,
		ButtonType.SHOP_BACK,
		ButtonType.SHOP_BACK,
		ButtonType.SHOP_BACK,
		ButtonType.SHOP_BACK,
		ButtonType.EPISODE_BACK,
		ButtonType.EPISODE_PLAY_BACK,
		ButtonType.EPISODE_RANKING_BACK,
		ButtonType.QUICK_BACK,
		ButtonType.QUICK_RANKING_BACK,
		ButtonType.PLAY_AT_EPISODE_PAGE_BACK
	};

	public static readonly AnimInfo[] m_animInfos = new AnimInfo[23]
	{
		new AnimInfo("MainMenuUI4", "ui_mm_Anim"),
		new AnimInfo("ChaoSetUI", "ui_mm_chao_Anim"),
		null,
		new AnimInfo("InformationUI", "ui_daily_challenge_infomation_intro_Anim"),
		new AnimInfo("ItemSet_3_UI", "ui_itemset_3_intro_Anim"),
		null,
		new AnimInfo("OptionUI", "ui_menu_option_intro_Anim"),
		new AnimInfo("PlayerSet_3_UI", "ui_mm_player_set_2_intro_Anim"),
		new AnimInfo("PlayerSet_2_UI", "ui_mm_player_set_2_intro_Anim"),
		new AnimInfo("PresentBoxUI", "ui_menu_presentbox_intro_Anim"),
		null,
		new AnimInfo("DailyInfoUI", "ui_daily_challenge_infomation_intro_Anim"),
		new AnimInfo("RouletteTopUI", null),
		new AnimInfo("ShopUI2", "ui_mm_shop_intro_Anim"),
		new AnimInfo("ShopUI2", "ui_mm_shop_intro_Anim"),
		new AnimInfo("ShopUI2", "ui_mm_shop_intro_Anim"),
		new AnimInfo("ShopUI2", "ui_mm_shop_intro_Anim"),
		null,
		new AnimInfo("ItemSet_3_UI", "ui_itemset_3_intro_Anim"),
		null,
		new AnimInfo("ItemSet_3_UI", "ui_itemset_3_intro_Anim"),
		null,
		new AnimInfo("ItemSet_3_UI", "ui_itemset_3_intro_Anim")
	};

	public readonly MessageInfo[] m_msgInfosForPages = new MessageInfo[23]
	{
		null,
		new MessageInfo("ChaoSetUI", "OnStartChaoSet", "ChaoSetUI"),
		null,
		new MessageInfo("InformationUI", "OnStartInformation", "InformationUI"),
		null,
		null,
		new MessageInfo("OptionUI", "OnStartOptionUI", "OptionUI"),
		new MessageInfo("PlayerSet_3_UI", "Setup", "PlayerCharaList"),
		new MessageInfo("PlayerSet_2_UI", "StartSubCharacter", "MenuPlayerSet"),
		new MessageInfo("PresentBoxUI", "OnStartPresentBox", "PresentBoxUI"),
		null,
		new MessageInfo("DailyInfoUI", "Setup", "DailyInfo"),
		new MessageInfo("RouletteTopUI", "OnRouletteOpenDefault", "RouletteTop"),
		new MessageInfo("ShopUI2", "OnStartShopRedStarRing", "ShopUI"),
		new MessageInfo("ShopUI2", "OnStartShopRing", "ShopUI"),
		new MessageInfo("ShopUI2", "OnStartShopChallenge", "ShopUI"),
		new MessageInfo("ShopUI2", "OnStartShopEvent", "ShopUI"),
		new MessageInfo("ui_mm_mileage2_page", "OnStartMileage", "ui_mm_mileage_page"),
		null,
		new MessageInfo("ui_mm_ranking_page", "SetDisplayEndlessModeOn", "RankingUI"),
		null,
		new MessageInfo("ui_mm_ranking_page", "SetDisplayQuickModeOn", "RankingUI"),
		null
	};

	public readonly MessageInfo[] m_msgInfosForEndPages = new MessageInfo[23]
	{
		null,
		new MessageInfo("ChaoSetUI", "OnMsgMenuBack", "ChaoSetUI"),
		null,
		new MessageInfo("InformationUI", "OnEndInformation", "InformationUI"),
		new MessageInfo("ItemSet_3_UI", "OnMsgMenuBack", "ItemSetMenu"),
		null,
		new MessageInfo("MainMenuButtonEvent", "OnOptionBackButtonClicked", string.Empty, false),
		null,
		null,
		new MessageInfo("PresentBoxUI", "OnEndPresentBox", "PresentBoxUI"),
		null,
		new MessageInfo("DailyInfoUI", "OnClickBackButton", "DailyInfo"),
		new MessageInfo("RouletteTopUI", "OnRouletteEnd", "RouletteTop"),
		new MessageInfo("ShopUI2", "OnShopBackButtonClicked", "ShopUI"),
		new MessageInfo("ShopUI2", "OnShopBackButtonClicked", "ShopUI"),
		new MessageInfo("ShopUI2", "OnShopBackButtonClicked", "ShopUI"),
		new MessageInfo("ShopUI2", "OnShopBackButtonClicked", "ShopUI"),
		new MessageInfo("ui_mm_mileage2_page", "OnEndMileage", "ui_mm_mileage_page"),
		null,
		new MessageInfo("ui_mm_ranking_page", "SetDisplayEndlessModeOff", "RankingUI"),
		null,
		new MessageInfo("ui_mm_ranking_page", "SetDisplayQuickModeOff", "RankingUI"),
		null
	};

	public readonly ButtonInfo[] m_button_info = new ButtonInfo[49]
	{
		new ButtonInfo(MsgMenuSequence.SequeneceType.PRESENT_BOX, PageType.PRESENT_BOX, "MainMenuUI4/Anchor_7_BL/Btn_2_presentbox", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.DAILY_CHALLENGE, PageType.DAILY_CHALLENGE, "MainMenuUI4/Anchor_9_BR/Btn_1_challenge", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.DAILY_BATTLE, PageType.DAILY_BATTLE, "MainMenuUI4/Anchor_5_MC/1_Quick/Btn_2_battle", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.CHARA_MAIN, PageType.PLAYER_MAIN, "MainMenuUI4/Anchor_5_MC/2_Character/Btn_2_player", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.CHARA_MAIN, PageType.PLAYER_SUB, "MainMenuUI4/Anchor_5_MC/mainmenu_contents/grid/page_3/slot/ui_mm_main2_page(Clone)/player_set/Btn_player_sub", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.CHAO, PageType.CHAO, "MainMenuUI4/Anchor_5_MC/2_Character/Btn_1_chao", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.PLAY_ITEM, PageType.ITEM),
		new ButtonInfo(MsgMenuSequence.SequeneceType.STAGE_CHECK, PageType.NON),
		new ButtonInfo(MsgMenuSequence.SequeneceType.OPTION, PageType.OPTION, "MainMenuUI4/Anchor_7_BL/Btn_1_Option", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.INFOMATION, PageType.INFOMATION, "MainMenuUI4/Anchor_7_BL/Btn_0_info", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.ROULETTE, PageType.ROULETTE, "MainMenuUI4/Anchor_8_BC/Btn_roulette", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.CHAO_ROULETTE, PageType.ROULETTE),
		new ButtonInfo(MsgMenuSequence.SequeneceType.CHAO_ROULETTE, PageType.ROULETTE),
		new ButtonInfo(MsgMenuSequence.SequeneceType.ITEM_ROULETTE, PageType.ROULETTE),
		new ButtonInfo(MsgMenuSequence.SequeneceType.CHAO_ROULETTE, PageType.ROULETTE, "ChaoSetUIPage/ChaoSetUI/Anchor_7_BL/mainmenu_btn_1c/Btn_roulette", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.EPISODE, PageType.EPISODE, "MainMenuUI4/Anchor_5_MC/0_Endless/Btn_2_mileage", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.MAIN_PLAY_BUTTON, PageType.EPISODE_PLAY, "MainMenuUI4/Anchor_5_MC/0_Endless/Btn_3_play", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.EPISODE_RANKING, PageType.EPISODE_RANKING, "MainMenuUI4/Anchor_5_MC/0_Endless/Btn_1_ranking", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.QUICK, PageType.QUICK, "MainMenuUI4/Anchor_5_MC/1_Quick/Btn_3_play", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.QUICK_RANKING, PageType.QUICK_RANKING, "MainMenuUI4/Anchor_5_MC/1_Quick/Btn_1_ranking", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.PLAY_AT_EPISODE_PAGE, PageType.PLAY_AT_EPISODE_PAGE, "ui_mm_mileage2_page/Anchor_9_BR/Btn_play", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.EVENT_TOP, PageType.EVENT, string.Empty, "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "PresentBoxUI/Anchor_7_BL/mainmenu_btn_1b/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, string.Empty, "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "DailyInfoUI/Anchor_7_BL/mainmenu_btn_1b/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "PlayerSet_3_UI/Anchor_7_BL/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "ItemSet_3_UI/Anchor_7_BL/mainmenu_btn_1b/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "ChaoSetUIPage/ChaoSetUI/Anchor_7_BL/mainmenu_btn_1c/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "ShopPage/ShopUI2/Anchor_7_BL/mainmenu_btn_1b/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "ShopPage/ShopUI2/Anchor_7_BL/mainmenu_btn_1b/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "ui_mm_mileage2_page/Anchor_7_BL/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "ui_mm_ranking_page/Anchor_7_BL/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "ShopPage/ShopUI2/Anchor_7_BL/mainmenu_btn_1b/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "ui_mm_ranking_page/Anchor_7_BL/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "ShopPage/ShopUI2/Anchor_7_BL/mainmenu_btn_1b/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "InformationUI/Anchor_7_BL/mainmenu_btn_1b/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "RouletteUI/Anchor_7_BL/roulette_btn_2/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, "OptionUI/Anchor_7_BL/option_btn/Btn_cmn_back", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.SHOP, PageType.SHOP_RSR, "MainMenuCmnUI/Anchor_3_TR/mainmenu_info_quantum/Btn_shop/Btn_charge_rsring", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.SHOP, PageType.SHOP_RING, "MainMenuCmnUI/Anchor_3_TR/mainmenu_info_quantum/Btn_shop/Btn_charge_stock", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.SHOP, PageType.SHOP_ENERGY, "MainMenuCmnUI/Anchor_3_TR/mainmenu_info_quantum/Btn_shop/Btn_charge_challenge", "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.SHOP, PageType.SHOP_EVENT, string.Empty, "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.EVENT_RAID, PageType.EVENT, string.Empty, "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.EVENT_SPECIAL, PageType.EVENT, string.Empty, "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.EVENT_COLLECT, PageType.EVENT, string.Empty, "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, PageType.NON, string.Empty, "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.MAIN, PageType.MAIN, string.Empty, "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.BACK, PageType.NON, string.Empty, "sys_menu_decide"),
		new ButtonInfo(MsgMenuSequence.SequeneceType.STAGE, PageType.STAGE, string.Empty, "sys_menu_decide")
	};

	public void PlaySE(ButtonType button_type)
	{
		if (button_type < ButtonType.NUM && !string.IsNullOrEmpty(m_button_info[(int)button_type].seName))
		{
			SoundManager.SePlay(m_button_info[(int)button_type].seName);
		}
	}

	public PageType GetPageType(ButtonType button_type)
	{
		if (button_type < ButtonType.NUM)
		{
			return m_button_info[(int)button_type].nextPageType;
		}
		return PageType.NON;
	}

	public MsgMenuSequence.SequeneceType GetSequeneceType(ButtonType button_type)
	{
		if (button_type < ButtonType.NUM)
		{
			return m_button_info[(int)button_type].nextMenuId;
		}
		return MsgMenuSequence.SequeneceType.NON;
	}

	public MsgMenuSequence.SequeneceType GetSequeneceType(PageType pageType)
	{
		if (pageType != PageType.NON && pageType < PageType.NUM)
		{
			return m_sequences[(int)pageType];
		}
		return MsgMenuSequence.SequeneceType.NON;
	}

	public AnimInfo GetPageAnimInfo(PageType pageType)
	{
		if (pageType != PageType.NON && pageType < PageType.NUM)
		{
			return m_animInfos[(int)pageType];
		}
		return null;
	}

	public MessageInfo GetPageMessageInfo(PageType page, bool start)
	{
		if (page != PageType.NON && page < PageType.NUM)
		{
			if (start)
			{
				return m_msgInfosForPages[(int)page];
			}
			return m_msgInfosForEndPages[(int)page];
		}
		return null;
	}

	public GameObject GetDisplayObj(PageType nextPageType)
	{
		if (nextPageType != PageType.NON && nextPageType < PageType.NUM && m_animInfos[(int)nextPageType] != null)
		{
			GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
			if (cameraUIObject != null)
			{
				return GameObjectUtil.FindChildGameObject(cameraUIObject, m_animInfos[(int)nextPageType].targetName);
			}
		}
		return null;
	}
}
