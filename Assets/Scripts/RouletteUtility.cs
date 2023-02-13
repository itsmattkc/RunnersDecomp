using System;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class RouletteUtility
{
	public enum AchievementType
	{
		NONE,
		PlayerGet,
		ChaoGet,
		LevelUp,
		LevelMax,
		Multi
	}

	public enum NextType
	{
		NONE,
		EQUIP,
		CHARA_EQUIP
	}

	public enum CellType
	{
		Item,
		Egg,
		Rankup
	}

	public enum WheelRank
	{
		Normal,
		Big,
		Super,
		MAX
	}

	public enum WheelType
	{
		NONE,
		Normal,
		Rankup
	}

	public enum RouletteColor
	{
		NONE,
		Blue,
		Purple,
		Green,
		Silver,
		Gold
	}

	public const bool ROULETTE_PARTS_DSTROY = false;

	public const bool ITEM_ROULETTE_USE_RING = false;

	public const bool ROULETTE_CHANGE_EFFECT = false;

	public const float ROULETTE_BASIC_RELOAD_SPAN = 5f;

	public const float ROULETTE_MULTI_GET_EFFECT_TIME = 5f;

	public const float ROULETTE_SPIN_WAIT_LIMIT_TIME = 10f;

	public const int ROULETTE_MULTI_NUM_0 = 1;

	public const int ROULETTE_MULTI_NUM_1 = 3;

	public const int ROULETTE_MULTI_NUM_2 = 5;

	public const int ROULETTE_TUTORIAL_ADD_SP_EGG = 10;

	private const string ROULETTE_CHANGE_ICON_SPRITE_NAME = "ui_roulette_pager_icon_{CATEGORY}";

	private const string ROULETTE_BG_SPRITE_NAME = "ui_roulette_tablebg_{COLOR}";

	private const string ROULETTE_BOARD_SPRITE_NAME = "ui_roulette_table_{COLOR}_{TYPE}";

	private const string ROULETTE_ARROW_SPRITE_NAME = "ui_roulette_arrow_{COLOR}";

	private const string ROULETTE_COST_ITEM_SPRITE_NAME = "ui_cmn_icon_item_{ID}";

	private const string ROULETTE_HEADER_NAME = "ui_Header_{TYPE}_Roulette";

	public static readonly int OddsDisplayDecimal = 2;

	private static bool s_itemRouletteUse;

	private static bool s_loginRoulette;

	private static RouletteCategory s_rouletteDefault = RouletteCategory.ITEM;

	public static ServerSpinResultGeneral s_spinResult;

	public static int s_spinResultCount;

	private static bool s_rouletteTurtorialEnd;

	private static bool s_rouletteTurtorial;

	private static bool s_rouletteTurtorialLock;

	private static string s_jackpotFeedText = string.Empty;

	public static RouletteCategory rouletteDefault
	{
		get
		{
			return s_rouletteDefault;
		}
		set
		{
			s_rouletteDefault = value;
		}
	}

	public static bool loginRoulette
	{
		get
		{
			return s_loginRoulette;
		}
		set
		{
			s_loginRoulette = value;
		}
	}

	public static string jackpotFeedText
	{
		get
		{
			return s_jackpotFeedText;
		}
	}

	public static bool rouletteTurtorialEnd
	{
		get
		{
			return s_rouletteTurtorialEnd;
		}
		set
		{
			s_rouletteTurtorial = false;
			s_rouletteTurtorialEnd = value;
			if (!s_rouletteTurtorialEnd)
			{
				s_rouletteTurtorialLock = true;
			}
		}
	}

	public static bool isTutorial
	{
		get
		{
			bool flag = false;
			if (s_rouletteTurtorialLock)
			{
				return false;
			}
			if (s_rouletteTurtorial)
			{
				flag = true;
			}
			else if (ServerInterface.ChaoWheelOptions != null && !s_rouletteTurtorialEnd)
			{
				flag = ServerInterface.ChaoWheelOptions.IsTutorial;
				if (flag)
				{
					s_rouletteTurtorial = true;
				}
			}
			return flag;
		}
	}

	public static RouletteTicketCategory GetRouletteTicketCategory(int itemId)
	{
		RouletteTicketCategory result = RouletteTicketCategory.NONE;
		if (itemId > 229999 && itemId <= 299999)
		{
			if (itemId >= 230000 && itemId < 240000)
			{
				result = RouletteTicketCategory.PREMIUM;
			}
			else if (itemId >= 240000 && itemId < 250000)
			{
				result = RouletteTicketCategory.ITEM;
			}
			else if (itemId >= 250000 && itemId < 260000)
			{
				result = RouletteTicketCategory.RAID;
			}
			else if (itemId >= 260000 && itemId < 270000)
			{
				result = RouletteTicketCategory.EVENT;
			}
		}
		return result;
	}

	public static bool SetItemRouletteUseRing(bool use)
	{
		return false;
	}

	public static string GetRouletteCostItemName(int costItemId)
	{
		string text = "ui_cmn_icon_item_{ID}";
		return text.Replace("{ID}", costItemId.ToString());
	}

	public static string GetRouletteColorName(RouletteColor rcolor)
	{
		string result = null;
		switch (rcolor)
		{
		case RouletteColor.Blue:
			result = "blu";
			break;
		case RouletteColor.Purple:
			result = "pur";
			break;
		case RouletteColor.Green:
			result = "gre";
			break;
		case RouletteColor.Gold:
			result = "gol";
			break;
		case RouletteColor.Silver:
			result = "sil";
			break;
		}
		return result;
	}

	public static WheelRank GetRouletteRank(int rank)
	{
		WheelRank result = WheelRank.Normal;
		switch (rank % 100)
		{
		case 0:
			result = WheelRank.Normal;
			break;
		case 1:
			result = WheelRank.Big;
			break;
		case 2:
			result = WheelRank.Super;
			break;
		}
		return result;
	}

	public static string GetRouletteChangeIconSpriteName(RouletteCategory category)
	{
		string text = "ui_roulette_pager_icon_{CATEGORY}";
		int num = (int)category;
		return text.Replace("{CATEGORY}", num.ToString());
	}

	public static string GetRouletteBgSpriteName(ServerWheelOptionsGeneral wheel)
	{
		string text = "ui_roulette_tablebg_{COLOR}";
		string rouletteColorName = GetRouletteColorName(RouletteColor.Green);
		switch (wheel.type)
		{
		case WheelType.Normal:
			rouletteColorName = GetRouletteColorName(RouletteColor.Blue);
			break;
		case WheelType.Rankup:
			rouletteColorName = GetRouletteColorName(RouletteColor.Green);
			break;
		}
		return text.Replace("{COLOR}", rouletteColorName);
	}

	public static string GetRouletteBoardSpriteName(ServerWheelOptionsGeneral wheel)
	{
		RouletteCategory rouletteCategory = GetRouletteCategory(wheel);
		string text = "ui_roulette_table_{COLOR}_{TYPE}";
		string newValue = GetRouletteColorName(RouletteColor.Green);
		int patternType = wheel.patternType;
		string str = string.Empty;
		WheelRank rank = wheel.rank;
		switch (wheel.type)
		{
		case WheelType.Normal:
			newValue = ((rouletteCategory == RouletteCategory.SPECIAL) ? GetRouletteColorName(RouletteColor.Gold) : GetRouletteColorName(RouletteColor.Silver));
			break;
		case WheelType.Rankup:
			switch (rank)
			{
			case WheelRank.Normal:
				newValue = GetRouletteColorName(RouletteColor.Green);
				break;
			case WheelRank.Big:
				newValue = GetRouletteColorName(RouletteColor.Silver);
				break;
			case WheelRank.Super:
				newValue = GetRouletteColorName(RouletteColor.Gold);
				str = "r";
				break;
			}
			break;
		}
		text = text.Replace("{COLOR}", newValue);
		text = text.Replace("{TYPE}", patternType.ToString());
		return text + str;
	}

	public static string GetRouletteArrowSpriteName(ServerWheelOptionsGeneral wheel)
	{
		string text = "ui_roulette_arrow_{COLOR}";
		string newValue = GetRouletteColorName(RouletteColor.Silver);
		switch (wheel.type)
		{
		case WheelType.Normal:
			newValue = ((wheel.rank != 0) ? GetRouletteColorName(RouletteColor.Gold) : GetRouletteColorName(RouletteColor.Silver));
			break;
		case WheelType.Rankup:
			newValue = ((wheel.rank != WheelRank.Super) ? GetRouletteColorName(RouletteColor.Silver) : GetRouletteColorName(RouletteColor.Gold));
			break;
		}
		return text.Replace("{COLOR}", newValue);
	}

	public static RouletteCategory GetRouletteCategory(ServerWheelOptionsGeneral wheel)
	{
		RouletteCategory result = RouletteCategory.NONE;
		if (wheel != null && wheel.rouletteId >= 0)
		{
			result = RouletteCategory.RAID;
		}
		return result;
	}

	public static bool ChangeRouletteHeader(RouletteCategory category)
	{
		bool result = false;
		if (category != RouletteCategory.ALL)
		{
			string rouletteCategoryName = GetRouletteCategoryName(category);
			if (!string.IsNullOrEmpty(rouletteCategoryName))
			{
				string text = "ui_Header_{TYPE}_Roulette";
				text = text.Replace("{TYPE}", rouletteCategoryName);
				HudMenuUtility.SendChangeHeaderText(text);
				result = true;
			}
		}
		else
		{
			string cellName = "ui_Header_Roulette_top";
			HudMenuUtility.SendChangeHeaderText(cellName);
			result = true;
		}
		return result;
	}

	public static string GetRouletteCategoryHeaderText(RouletteCategory category)
	{
		string result = string.Empty;
		string cellName = string.Empty;
		if (category != RouletteCategory.ALL)
		{
			string rouletteCategoryName = GetRouletteCategoryName(category);
			if (!string.IsNullOrEmpty(rouletteCategoryName))
			{
				cellName = "ui_Header_{TYPE}_Roulette";
				cellName = cellName.Replace("{TYPE}", rouletteCategoryName);
			}
		}
		else
		{
			cellName = "ui_Header_Roulette_top";
		}
		TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", cellName);
		if (text != null)
		{
			result = text.text;
		}
		return result;
	}

	public static string GetRouletteCategoryName(RouletteCategory category)
	{
		string result = null;
		switch (category)
		{
		case RouletteCategory.PREMIUM:
			result = "Premium";
			break;
		case RouletteCategory.SPECIAL:
			result = "Special";
			break;
		case RouletteCategory.ITEM:
			result = "Item";
			break;
		case RouletteCategory.RAID:
			result = "Raidboss";
			break;
		case RouletteCategory.EVENT:
			result = "Event";
			break;
		}
		return result;
	}

	private static void ShowItem(int id, int num)
	{
		if (new ServerItem((ServerItem.Id)id).idType != ServerItem.IdType.ITEM_ROULLETE_WIN)
		{
			ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
			if (itemGetWindow != null)
			{
				itemGetWindow.Create(new ItemGetWindow.CInfo
				{
					name = "ItemGet",
					caption = GetText("gw_item_caption"),
					serverItemId = id,
					imageCount = GetText("gw_item_text", "{COUNT}", HudUtility.GetFormatNumString(num))
				});
			}
		}
		else
		{
			ShowJackpot(num);
		}
	}

	private static void ShowJackpot(int jackpotRing)
	{
		RouletteManager.isShowGetWindow = true;
		int serverItemId = 910000;
		ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
		if (itemGetWindow != null)
		{
			itemGetWindow.Create(new ItemGetWindow.CInfo
			{
				name = "Jackpot",
				buttonType = ItemGetWindow.ButtonType.TweetCancel,
				caption = GetText("gw_jackpot_caption"),
				serverItemId = serverItemId,
				imageCount = GetText("gw_jackpot_text", "{COUNT}", HudUtility.GetFormatNumString(RouletteManager.numJackpotRing))
			});
			s_jackpotFeedText = GetText("feed_jackpot_text", "{COUNT}", HudUtility.GetFormatNumString(RouletteManager.numJackpotRing));
			RouletteManager.numJackpotRing = jackpotRing;
		}
	}

	public static void ShowGetWindow(ServerWheelOptions data)
	{
		GameObject x = GameObject.Find("UI Root (2D)");
		if (!(x != null))
		{
			return;
		}
		RouletteManager.isShowGetWindow = true;
		BackKeyManager.InvalidFlag = false;
		int id = data.m_items[data.m_itemWon];
		int num = data.m_itemQuantities[data.m_itemWon];
		ServerItem serverItem = new ServerItem((ServerItem.Id)id);
		if (serverItem.idType == ServerItem.IdType.ITEM_ROULLETE_WIN && data.m_rouletteRank == WheelRank.Super)
		{
			ShowJackpot(data.m_numJackpotRing);
		}
		else if (serverItem.idType == ServerItem.IdType.CHAO || serverItem.idType == ServerItem.IdType.CHARA)
		{
			ServerChaoState chao = data.GetChao();
			if (chao != null)
			{
				ShowOtomo(chao, !data.IsItemList(), data.m_itemList, data.NumRequiredSpEggs, false);
			}
		}
		else
		{
			ShowItem(id, num);
		}
	}

	public static void ShowGetWindow(ServerSpinResultGeneral data)
	{
		RouletteManager.isShowGetWindow = false;
		s_spinResult = null;
		s_spinResultCount = -1;
		GameObject x = GameObject.Find("UI Root (2D)");
		if (!(x != null))
		{
			return;
		}
		Debug.Log("ShowGetWindow ItemWon:" + data.ItemWon + " !!!!!!!!");
		if (data.ItemWon >= 0)
		{
			if (data.AcquiredChaoData.Count > 0)
			{
				Dictionary<int, ServerChaoData>.KeyCollection keys = data.AcquiredChaoData.Keys;
				using (Dictionary<int, ServerChaoData>.KeyCollection.Enumerator enumerator = keys.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						int current = enumerator.Current;
						ShowOtomo(data.AcquiredChaoData[current], data.IsRequiredChao[current], data.ItemState, data.NumRequiredSpEggs, false);
					}
				}
			}
			else if (data.ItemState.Count > 0)
			{
				Dictionary<int, ServerItemState>.KeyCollection keys2 = data.ItemState.Keys;
				using (Dictionary<int, ServerItemState>.KeyCollection.Enumerator enumerator2 = keys2.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						int current2 = enumerator2.Current;
						ShowItem(data.ItemState[current2].m_itemId, data.ItemState[current2].m_num);
					}
				}
			}
			else
			{
				Debug.Log("RouletteUtility ShowGetWindow G  single error?");
			}
			return;
		}
		s_spinResult = data;
		s_spinResultCount = data.GetOtomoAndCharaMax() - 1;
		RouletteManager.isShowGetWindow = false;
		Debug.Log("ShowGetWindow ResultCount:" + s_spinResultCount + " !!!!!!!!");
		if (s_spinResultCount >= 0)
		{
			RouletteManager.isMultiGetWindow = true;
			ShowGetAllOtomoAndChara();
			return;
		}
		RouletteManager.isMultiGetWindow = false;
		string acquiredListText = data.AcquiredListText;
		if (!string.IsNullOrEmpty(acquiredListText))
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "RouletteGetAllList";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "GeneralWindow", "ui_Lbl_get_list").text;
			info.message = acquiredListText;
			GeneralWindow.Create(info);
		}
	}

	public static bool IsGetOtomoOrCharaWindow()
	{
		bool flag = false;
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			ChaoGetWindow chaoGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(gameObject, "ro_PlayerGetWindowUI");
			if (chaoGetWindow != null && chaoGetWindow.gameObject.activeSelf)
			{
				flag = true;
			}
			if (!flag)
			{
				chaoGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(gameObject, "chao_get_Window");
				if (chaoGetWindow != null && chaoGetWindow.gameObject.activeSelf)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				chaoGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(gameObject, "chao_rare_get_Window");
				if (chaoGetWindow != null && chaoGetWindow.gameObject.activeSelf)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				ChaoMergeWindow chaoMergeWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoMergeWindow>(gameObject, "chao_merge_Window");
				if (chaoMergeWindow != null && chaoMergeWindow.gameObject.activeSelf)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				PlayerMergeWindow playerMergeWindow = GameObjectUtil.FindChildGameObjectComponent<PlayerMergeWindow>(gameObject, "player_merge_Window");
				if (playerMergeWindow != null && playerMergeWindow.gameObject.activeSelf)
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	public static bool ShowGetAllOtomoAndChara()
	{
		bool result = false;
		if (s_spinResult != null && s_spinResultCount >= 0)
		{
			ServerChaoData showData = s_spinResult.GetShowData(s_spinResultCount);
			if (showData != null)
			{
				ShowOtomo(showData, true, null, 0, true);
				result = true;
			}
			s_spinResultCount--;
		}
		return result;
	}

	public static void ShowGetAllListEnd()
	{
		string name = "RouletteGetAllListEnd";
		string message = string.Empty;
		string empty = string.Empty;
		GeneralWindow.ButtonType buttonType = GeneralWindow.ButtonType.YesNo;
		bool flag = false;
		if (s_spinResult != null)
		{
			empty = s_spinResult.AcquiredListText;
			flag = s_spinResult.CheckGetChara();
			if (!string.IsNullOrEmpty(empty))
			{
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Roulette", "get_item_list_text").text;
				if (!string.IsNullOrEmpty(text))
				{
					message = text.Replace("{PARAN}", empty);
					if (flag)
					{
						name = "RouletteGetAllListEndChara";
						string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_Header_PlayerSet").text;
						message = message.Replace("{PAGE}", text2);
					}
					else
					{
						name = "RouletteGetAllListEndChao";
						string text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_Header_ChaoSet").text;
						message = message.Replace("{PAGE}", text3);
					}
				}
				else
				{
					message = empty;
					buttonType = GeneralWindow.ButtonType.Ok;
				}
			}
		}
		RouletteManager.isShowGetWindow = true;
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = name;
		info.buttonType = buttonType;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "GeneralWindow", "ui_Lbl_get_list").text;
		info.message = message;
		GeneralWindow.Create(info);
	}

	private static void ShowOtomo(ServerChaoData data, bool required, Dictionary<int, ServerItemState> itemState, int numRequiredSpEggs, bool multi)
	{
		ServerItem serverItem = new ServerItem((ServerItem.Id)data.Id);
		GameObject uiRoot = GameObject.Find("UI Root (2D)");
		if (data.Rarity == 100 || serverItem.idType == ServerItem.IdType.CHARA)
		{
			ShowGetWindowChara(data, uiRoot, itemState, multi);
		}
		else if (data.Level == 0)
		{
			ShowGetWindowOtomo(data, uiRoot, multi);
		}
		else if (IsLevelMaxChao(data.Id) && !required)
		{
			if (numRequiredSpEggs > 0)
			{
				ShowGetWindowOtomoMax(data, uiRoot, numRequiredSpEggs);
			}
			else
			{
				ShowGetWindowOtomoLvup(data, uiRoot, multi);
			}
		}
		else if (!multi && !required)
		{
			ShowGetWindowOtomoMax(data, uiRoot, numRequiredSpEggs);
		}
		else
		{
			ShowGetWindowOtomoLvup(data, uiRoot, multi);
		}
	}

	public static void ShowGetWindow(ServerChaoSpinResult data)
	{
		RouletteManager.isShowGetWindow = false;
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			ServerItem serverItem = new ServerItem((ServerItem.Id)data.AcquiredChaoData.Id);
			if (data.AcquiredChaoData.Rarity == 100 || serverItem.idType == ServerItem.IdType.CHARA)
			{
				ShowGetWindowChara(data.AcquiredChaoData, gameObject, data.ItemState, false);
			}
			else if (data.AcquiredChaoData.Level == 0)
			{
				ShowGetWindowOtomo(data.AcquiredChaoData, gameObject, false);
			}
			else if (IsLevelMaxChao(data.AcquiredChaoData.Id) && !data.IsRequiredChao)
			{
				ShowGetWindowOtomoMax(data.AcquiredChaoData, gameObject, data.NumRequiredSpEggs);
			}
			else
			{
				ShowGetWindowOtomoLvup(data.AcquiredChaoData, gameObject, false);
			}
		}
	}

	private static void ShowGetWindowChara(ServerChaoData data, GameObject uiRoot, Dictionary<int, ServerItemState> itemState, bool multi)
	{
		int id = data.Id;
		int rarity = data.Rarity;
		int level = data.Level;
		AchievementType achievement = AchievementType.PlayerGet;
		ServerPlayerState playerState = ServerInterface.PlayerState;
		CharacterDataNameInfo.Info dataByServerID = CharacterDataNameInfo.Instance.GetDataByServerID(id);
		ServerCharacterState serverCharacterState = playerState.CharacterState(dataByServerID.m_ID);
		if ((itemState == null || (itemState != null && itemState.Count == 0)) && serverCharacterState.star > 0)
		{
			PlayerMergeWindow playerMergeWindow = GameObjectUtil.FindChildGameObjectComponent<PlayerMergeWindow>(uiRoot, "player_merge_Window");
			if (playerMergeWindow != null)
			{
				playerMergeWindow.PlayStart(id, achievement);
			}
			return;
		}
		ChaoGetWindow chaoGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(uiRoot, "ro_PlayerGetWindowUI");
		if (!(chaoGetWindow != null))
		{
			return;
		}
		ChaoGetPartsBase chaoGetPartsBase = null;
		if (multi)
		{
			achievement = AchievementType.Multi;
		}
		if (itemState != null && itemState.Count > 0)
		{
			PlayerGetPartsOverlap playerGetPartsOverlap = chaoGetWindow.gameObject.GetComponent<PlayerGetPartsOverlap>();
			if (playerGetPartsOverlap == null)
			{
				playerGetPartsOverlap = chaoGetWindow.gameObject.AddComponent<PlayerGetPartsOverlap>();
			}
			playerGetPartsOverlap.Init(id, rarity, level, itemState);
			chaoGetPartsBase = playerGetPartsOverlap;
		}
		else
		{
			if (isTutorial && RouletteTop.Instance != null && RouletteTop.Instance.category == RouletteCategory.PREMIUM)
			{
				TutorialCursor.StartTutorialCursor(TutorialCursor.Type.ROULETTE_OK);
			}
			PlayerGetPartsOverlap playerGetPartsOverlap2 = chaoGetWindow.gameObject.GetComponent<PlayerGetPartsOverlap>();
			if (playerGetPartsOverlap2 == null)
			{
				playerGetPartsOverlap2 = chaoGetWindow.gameObject.AddComponent<PlayerGetPartsOverlap>();
			}
			playerGetPartsOverlap2.Init(id, rarity, level, null);
			chaoGetPartsBase = playerGetPartsOverlap2;
		}
		if (multi)
		{
			chaoGetWindow.isSetuped = false;
		}
		chaoGetWindow.PlayStart(chaoGetPartsBase, isTutorial, false, achievement);
	}

	private static void ShowGetWindowOtomo(ServerChaoData data, GameObject uiRoot, bool multi)
	{
		ChaoGetPartsBase chaoGetParts = null;
		int rarity = data.Rarity;
		ChaoGetWindow chaoGetWindow = null;
		if (rarity == 0 || rarity == 1)
		{
			chaoGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(uiRoot, "chao_get_Window");
			if (chaoGetWindow != null)
			{
				ChaoGetPartsNormal chaoGetPartsNormal = chaoGetWindow.GetComponent<ChaoGetPartsNormal>();
				if (chaoGetPartsNormal == null)
				{
					chaoGetPartsNormal = chaoGetWindow.gameObject.AddComponent<ChaoGetPartsNormal>();
				}
				chaoGetPartsNormal.Init(data.Id, rarity);
				chaoGetParts = chaoGetPartsNormal;
			}
		}
		else
		{
			chaoGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(uiRoot, "chao_rare_get_Window");
			if (chaoGetWindow != null)
			{
				ChaoGetPartsRare chaoGetPartsRare = chaoGetWindow.gameObject.GetComponent<ChaoGetPartsRare>();
				if (chaoGetPartsRare == null)
				{
					chaoGetPartsRare = chaoGetWindow.gameObject.AddComponent<ChaoGetPartsRare>();
				}
				chaoGetPartsRare.Init(data.Id, rarity);
				chaoGetParts = chaoGetPartsRare;
			}
		}
		if (chaoGetWindow != null)
		{
			if (multi)
			{
				chaoGetWindow.isSetuped = false;
				chaoGetWindow.PlayStart(chaoGetParts, isTutorial, false, AchievementType.Multi);
			}
			else
			{
				chaoGetWindow.PlayStart(chaoGetParts, isTutorial, false, AchievementType.ChaoGet);
			}
		}
	}

	private static void ShowGetWindowOtomoLvup(ServerChaoData data, GameObject uiRoot, bool multi)
	{
		ChaoMergeWindow chaoMergeWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoMergeWindow>(uiRoot, "chao_merge_Window");
		if (chaoMergeWindow != null)
		{
			if (multi)
			{
				chaoMergeWindow.isSetuped = false;
				chaoMergeWindow.PlayStart(data.Id, data.Level, data.Rarity, AchievementType.Multi);
			}
			else
			{
				chaoMergeWindow.PlayStart(data.Id, data.Level, data.Rarity, AchievementType.LevelUp);
			}
		}
	}

	private static void ShowGetWindowOtomoMax(ServerChaoData data, GameObject uiRoot, int numRequiredSpEggs)
	{
		SpEggGetPartsBase spEggGetPartsBase = null;
		int rarity = data.Rarity;
		SpEggGetWindow spEggGetWindow = null;
		if (rarity == 0)
		{
			spEggGetWindow = GameObjectUtil.FindChildGameObjectComponent<SpEggGetWindow>(uiRoot, "chao_egg_transform_Window");
			spEggGetPartsBase = new SpEggGetPartsNormal(data.Id, numRequiredSpEggs);
		}
		else
		{
			spEggGetWindow = GameObjectUtil.FindChildGameObjectComponent<SpEggGetWindow>(uiRoot, "chao_rare_egg_transform_Window");
			spEggGetPartsBase = new SpEggGetPartsRare(data.Id, rarity, numRequiredSpEggs);
		}
		if (spEggGetWindow != null)
		{
			spEggGetWindow.PlayStart(spEggGetPartsBase, AchievementType.LevelMax);
		}
	}

	public static void ShowLoginBounsInfoWindow(string param = "")
	{
		string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "today_roulette_caption").text;
		string message = (!string.IsNullOrEmpty(param)) ? param : TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "today_roulette_text").text;
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "LoginBouns";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = text;
		info.message = message;
		GeneralWindow.Create(info);
	}

	private static string GetText(string cellName, Dictionary<string, string> dicReplaces = null)
	{
		string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", cellName).text;
		if (dicReplaces != null)
		{
			text = TextUtility.Replaces(text, dicReplaces);
		}
		return text;
	}

	private static string GetText(string cellName, string srcText, string dstText)
	{
		return GetText(cellName, new Dictionary<string, string>
		{
			{
				srcText,
				dstText
			}
		});
	}

	public static string GetPrizeList(ServerPrizeState prizeState)
	{
		string text = string.Empty;
		int num = 0;
		int num2 = -1;
		Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
		List<int> list = new List<int>();
		foreach (ServerPrizeData prize in prizeState.prizeList)
		{
			if (prize.priority >= 0)
			{
				if (!list.Contains(prize.priority))
				{
					list.Add(prize.priority);
				}
				if (dictionary.ContainsKey(prize.priority))
				{
					dictionary[prize.priority].Add(prize.GetItemName());
					continue;
				}
				List<string> list2 = new List<string>();
				list2.Add(prize.GetItemName());
				dictionary.Add(prize.priority, list2);
			}
		}
		list.Sort();
		for (int i = 0; i < list.Count; i++)
		{
			int num3 = list[i];
			List<string> list3 = new List<string>();
			num = 0;
			foreach (string item in dictionary[num3])
			{
				if (list3.Contains(item))
				{
					continue;
				}
				if (num2 != num3)
				{
					num2 = num3;
					if (!string.IsNullOrEmpty(text))
					{
						if (num != 0)
						{
							text += Environment.NewLine;
						}
						text += Environment.NewLine;
					}
					num = 0;
					string cellName = "ui_Lbl_rarity_" + num3;
					text += "[00ff00]";
					text += TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Roulette", cellName).text;
					text += "[-]";
					text += Environment.NewLine;
				}
				else if (num > 0)
				{
					text += ", ";
				}
				text += item;
				list3.Add(item);
				num++;
				if (num >= 3)
				{
					text += Environment.NewLine;
					num = 0;
				}
			}
		}
		return text;
	}

	public static List<Constants.Campaign.emType> GetCampaign(RouletteCategory category)
	{
		List<Constants.Campaign.emType> list = null;
		if (isTutorial && category == RouletteCategory.PREMIUM)
		{
			return null;
		}
		ServerCampaignState campaignState = ServerInterface.CampaignState;
		if (campaignState != null)
		{
			switch (category)
			{
			case RouletteCategory.ITEM:
				if (campaignState.InSession(Constants.Campaign.emType.FreeWheelSpinCount))
				{
					if (list == null)
					{
						list = new List<Constants.Campaign.emType>();
					}
					list.Add(Constants.Campaign.emType.FreeWheelSpinCount);
				}
				if (campaignState.InSession(Constants.Campaign.emType.JackPotValueBonus))
				{
					if (list == null)
					{
						list = new List<Constants.Campaign.emType>();
					}
					list.Add(Constants.Campaign.emType.JackPotValueBonus);
				}
				break;
			case RouletteCategory.PREMIUM:
				if (campaignState.InSession(Constants.Campaign.emType.PremiumRouletteOdds))
				{
					if (list == null)
					{
						list = new List<Constants.Campaign.emType>();
					}
					list.Add(Constants.Campaign.emType.PremiumRouletteOdds);
				}
				if (campaignState.InSession(Constants.Campaign.emType.ChaoRouletteCost))
				{
					if (list == null)
					{
						list = new List<Constants.Campaign.emType>();
					}
					list.Add(Constants.Campaign.emType.ChaoRouletteCost);
				}
				break;
			}
		}
		return list;
	}

	public static string GetChaoGroupName(int chaoId)
	{
		if (new ServerItem((ServerItem.Id)chaoId).idType == ServerItem.IdType.CHARA)
		{
			return "CharaName";
		}
		return "Chao";
	}

	public static string GetChaoCellName(int chaoId)
	{
		string empty = string.Empty;
		if (new ServerItem((ServerItem.Id)chaoId).idType == ServerItem.IdType.CHARA)
		{
			int charaType = (int)new ServerItem((ServerItem.Id)chaoId).charaType;
			return CharaName.Name[charaType];
		}
		int num = chaoId - 400000;
		return string.Format("name{0:D4}", num);
	}

	public static ServerChaoState.ChaoStatus GetChaoStatus(int chaoId)
	{
		ServerChaoState.ChaoStatus result = ServerChaoState.ChaoStatus.NotOwned;
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState == null)
		{
			return result;
		}
		ServerChaoState serverChaoState = playerState.ChaoStateByItemID(chaoId);
		if (serverChaoState == null)
		{
			return result;
		}
		return serverChaoState.Status;
	}

	public static bool IsLevelMaxChao(int chaoId)
	{
		ServerChaoState.ChaoStatus chaoStatus = GetChaoStatus(chaoId);
		if (chaoStatus == ServerChaoState.ChaoStatus.MaxLevel)
		{
			return true;
		}
		return false;
	}
}
