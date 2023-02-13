using System.Collections.Generic;
using Text;
using UnityEngine;

public class ServerWheelOptionsOrgGen : ServerWheelOptionsOrg
{
	private ServerWheelOptionsGeneral m_orgData;

	public override bool isValid
	{
		get
		{
			return true;
		}
	}

	public override bool isRemainingRefresh
	{
		get
		{
			bool result = false;
			if (m_orgData != null && m_orgData.type == RouletteUtility.WheelType.Rankup && RouletteManager.Instance != null)
			{
				result = RouletteManager.Instance.currentRankup;
			}
			return result;
		}
	}

	public override int itemWon
	{
		get
		{
			return -1;
		}
	}

	public override ServerItem itemWonData
	{
		get
		{
			return default(ServerItem);
		}
	}

	public override int rouletteId
	{
		get
		{
			return m_orgData.rouletteId;
		}
	}

	public override int multi
	{
		get
		{
			return m_orgData.multi;
		}
	}

	public override int numJackpotRing
	{
		get
		{
			return m_orgData.jackpotRing;
		}
	}

	public ServerWheelOptionsOrgGen(ServerWheelOptionsGeneral data)
	{
		if (data != null)
		{
			m_category = RouletteUtility.GetRouletteCategory(data);
			m_init = true;
			m_type = RouletteUtility.WheelType.Rankup;
			if (m_orgData == null)
			{
				m_orgData = new ServerWheelOptionsGeneral();
			}
			data.CopyTo(m_orgData);
			UpdateItemWeights();
		}
	}

	public override void Setup(ServerChaoWheelOptions data)
	{
	}

	public override void Setup(ServerWheelOptions data)
	{
	}

	public override void Setup(ServerWheelOptionsGeneral data)
	{
		if (data != null)
		{
			m_category = RouletteUtility.GetRouletteCategory(data);
			m_init = true;
			m_type = RouletteUtility.WheelType.Rankup;
			int selectIndex = 0;
			int multi = 1;
			if (m_orgData == null)
			{
				m_orgData = new ServerWheelOptionsGeneral();
			}
			else
			{
				selectIndex = m_orgData.currentCostSelect;
				multi = m_orgData.multi;
			}
			data.CopyTo(m_orgData);
			data.ChangeCostItem(selectIndex);
			if (!data.ChangeMulti(multi) || data.rank != 0)
			{
				data.ChangeMulti(1);
			}
			UpdateItemWeights();
		}
	}

	public override bool ChangeMulti(int multi)
	{
		if (m_orgData == null)
		{
			return false;
		}
		return m_orgData.ChangeMulti(multi);
	}

	public override bool IsMulti(int multi)
	{
		if (m_orgData == null)
		{
			return false;
		}
		return m_orgData.IsMulti(multi);
	}

	public override int GetRouletteBoardPattern()
	{
		int result = 0;
		if (m_init)
		{
			result = m_orgData.patternType;
		}
		return result;
	}

	public override string GetRouletteArrowSprite()
	{
		if (m_orgData != null)
		{
			return m_orgData.spriteNameArrow;
		}
		return null;
	}

	public override string GetRouletteBgSprite()
	{
		if (m_orgData != null)
		{
			return m_orgData.spriteNameBg;
		}
		return null;
	}

	public override string GetRouletteBoardSprite()
	{
		if (m_orgData != null)
		{
			return m_orgData.spriteNameBoard;
		}
		return null;
	}

	public override string GetRouletteTicketSprite()
	{
		if (m_orgData != null)
		{
			return m_orgData.spriteNameCostItem;
		}
		return null;
	}

	public override RouletteUtility.WheelRank GetRouletteRank()
	{
		RouletteUtility.WheelRank result = RouletteUtility.WheelRank.Normal;
		if (m_init && m_orgData != null)
		{
			result = m_orgData.rank;
		}
		return result;
	}

	public override float GetCellWeight(int cellIndex)
	{
		float result = 0f;
		if (m_orgData != null && m_orgData.itemLenght > cellIndex)
		{
			result = m_orgData.GetCellWeight(cellIndex);
		}
		return result;
	}

	public override int GetCellEgg(int cellIndex)
	{
		int result = -1;
		if (m_orgData != null && m_orgData.itemLenght > cellIndex)
		{
			int itemId = 0;
			int itemNum = 0;
			float itemRate = 0f;
			m_orgData.GetCell(cellIndex, out itemId, out itemNum, out itemRate);
			ServerItem serverItem = new ServerItem((ServerItem.Id)itemId);
			if (serverItem.idType == ServerItem.IdType.CHAO)
			{
				result = (int)serverItem.id / 1000 % 10;
			}
		}
		return result;
	}

	public override ServerItem GetCellItem(int cellIndex, out int num)
	{
		ServerItem result = default(ServerItem);
		num = 1;
		if (m_orgData != null && m_orgData.itemLenght > cellIndex)
		{
			int itemId = 0;
			float itemRate = 0f;
			m_orgData.GetCell(cellIndex, out itemId, out num, out itemRate);
			result = new ServerItem((ServerItem.Id)itemId);
		}
		else
		{
			num = -1;
		}
		return result;
	}

	public override void PlayBgm(float delay = 0f)
	{
		if (!m_init || !(EventManager.Instance != null))
		{
			return;
		}
		EventManager.EventType type = EventManager.Instance.Type;
		string text = null;
		string cueSheetName = "BGM";
		if ((!RouletteUtility.isTutorial || m_category != RouletteCategory.PREMIUM) && type != EventManager.EventType.NUM && type != EventManager.EventType.UNKNOWN && EventManager.Instance.IsInEvent() && EventCommonDataTable.Instance != null)
		{
			string text2 = null;
			switch (base.category)
			{
			case RouletteCategory.SPECIAL:
				text2 = EventCommonDataTable.Instance.GetData(EventCommonDataItem.RouletteS_BgmName);
				break;
			default:
				text2 = EventCommonDataTable.Instance.GetData(EventCommonDataItem.Roulette_BgmName);
				break;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				cueSheetName = "BGM_" + EventManager.GetEventTypeName(EventManager.Instance.Type);
				text = text2;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			switch (base.category)
			{
			case RouletteCategory.SPECIAL:
				text = "bgm_sys_s_roulette";
				break;
			default:
				text = "bgm_sys_roulette";
				break;
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			RouletteManager.PlayBgm(text, delay, cueSheetName);
		}
	}

	public override void PlaySe(ServerWheelOptionsData.SE_TYPE seType, float delay = 0f)
	{
		if (!m_init || !(EventManager.Instance != null))
		{
			return;
		}
		string text = null;
		string cueSheetName = "SE";
		EventManager.EventType type = EventManager.Instance.Type;
		switch (seType)
		{
		case ServerWheelOptionsData.SE_TYPE.Open:
			text = "sys_window_open";
			break;
		case ServerWheelOptionsData.SE_TYPE.Close:
			text = "sys_window_close";
			break;
		case ServerWheelOptionsData.SE_TYPE.Click:
			text = "sys_menu_decide";
			break;
		case ServerWheelOptionsData.SE_TYPE.Spin:
			if (!RouletteUtility.isTutorial || m_category != RouletteCategory.PREMIUM)
			{
				switch (base.category)
				{
				case RouletteCategory.PREMIUM:
				case RouletteCategory.SPECIAL:
					if ((base.category == RouletteCategory.PREMIUM || base.category == RouletteCategory.SPECIAL) && type != EventManager.EventType.NUM && type != EventManager.EventType.UNKNOWN && EventManager.Instance.IsInEvent())
					{
						string data2 = EventCommonDataTable.Instance.GetData(EventCommonDataItem.RouletteDecide_SeCueName);
						if (!string.IsNullOrEmpty(data2))
						{
							cueSheetName = "SE_" + EventManager.GetEventTypeName(EventManager.Instance.Type);
							text = data2;
						}
					}
					break;
				default:
					text = "sys_menu_decide";
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "sys_menu_decide";
			}
			break;
		case ServerWheelOptionsData.SE_TYPE.SpinError:
			text = "sys_error";
			break;
		case ServerWheelOptionsData.SE_TYPE.Arrow:
			text = "sys_roulette_arrow";
			break;
		case ServerWheelOptionsData.SE_TYPE.Skip:
			text = "sys_page_skip";
			break;
		case ServerWheelOptionsData.SE_TYPE.GetItem:
			text = "sys_roulette_itemget";
			break;
		case ServerWheelOptionsData.SE_TYPE.GetRare:
			text = "sys_roulette_itemget_rare";
			break;
		case ServerWheelOptionsData.SE_TYPE.GetRankup:
			text = "sys_roulette_levelup";
			break;
		case ServerWheelOptionsData.SE_TYPE.GetJackpot:
			text = "sys_roulette_jackpot";
			break;
		case ServerWheelOptionsData.SE_TYPE.Multi:
			text = "boss_scene_change";
			break;
		case ServerWheelOptionsData.SE_TYPE.Change:
			if (!RouletteUtility.isTutorial || m_category != RouletteCategory.PREMIUM)
			{
				switch (base.category)
				{
				case RouletteCategory.PREMIUM:
				case RouletteCategory.SPECIAL:
					if ((base.category == RouletteCategory.PREMIUM || base.category == RouletteCategory.SPECIAL) && type != EventManager.EventType.NUM && type != EventManager.EventType.UNKNOWN && EventManager.Instance.IsInEvent())
					{
						string data = EventCommonDataTable.Instance.GetData(EventCommonDataItem.RouletteChange_SeCueName);
						if (!string.IsNullOrEmpty(data))
						{
							cueSheetName = "SE_" + EventManager.GetEventTypeName(EventManager.Instance.Type);
							text = data;
						}
					}
					break;
				default:
					text = "sys_roulette_change";
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "sys_roulette_change";
			}
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			if (delay <= 0f)
			{
				SoundManager.SePlay(text, cueSheetName);
			}
			else
			{
				RouletteManager.PlaySe(text, delay, cueSheetName);
			}
		}
	}

	public override ServerWheelOptionsData.SPIN_BUTTON GetSpinButtonSeting(out int count, out bool btnActive)
	{
		ServerWheelOptionsData.SPIN_BUTTON sPIN_BUTTON = ServerWheelOptionsData.SPIN_BUTTON.NONE;
		count = 0;
		btnActive = false;
		if (m_init && SaveDataManager.Instance != null && SaveDataManager.Instance.ItemData != null && m_orgData != null)
		{
			if (RouletteUtility.isTutorial && m_category == RouletteCategory.PREMIUM)
			{
				count = -1;
				sPIN_BUTTON = ServerWheelOptionsData.SPIN_BUTTON.FREE;
				btnActive = true;
			}
			else
			{
				int currentCostItemId = m_orgData.GetCurrentCostItemId();
				int multi = m_orgData.multi;
				int costItemNum = m_orgData.GetCostItemNum(currentCostItemId);
				count = m_orgData.GetCostItemCost(currentCostItemId) * multi;
				sPIN_BUTTON = m_orgData.GetSpinButton();
				if (costItemNum >= count)
				{
					btnActive = true;
				}
				if (sPIN_BUTTON == ServerWheelOptionsData.SPIN_BUTTON.FREE)
				{
					btnActive = true;
					count = m_orgData.remainingFree;
				}
			}
		}
		return sPIN_BUTTON;
	}

	public override ServerWheelOptionsData.SPIN_BUTTON GetSpinButtonSeting()
	{
		ServerWheelOptionsData.SPIN_BUTTON result = ServerWheelOptionsData.SPIN_BUTTON.NONE;
		if (m_init && SaveDataManager.Instance != null && SaveDataManager.Instance.ItemData != null && m_orgData != null)
		{
			result = ((!RouletteUtility.isTutorial || m_category != RouletteCategory.PREMIUM) ? m_orgData.GetSpinButton() : ServerWheelOptionsData.SPIN_BUTTON.FREE);
		}
		return result;
	}

	public override List<int> GetSpinCostItemIdList()
	{
		List<int> result = null;
		if (m_orgData != null)
		{
			result = m_orgData.GetCostItemList();
		}
		return result;
	}

	public override int GetSpinCostItemCost(int costItemId)
	{
		int result = 0;
		if (m_orgData != null)
		{
			result = ((costItemId <= 0) ? 1 : m_orgData.GetCostItemCost(costItemId));
		}
		return result;
	}

	public override int GetSpinCostItemNum(int costItemId)
	{
		int result = 0;
		if (m_orgData != null)
		{
			result = ((costItemId > 0) ? m_orgData.GetCostItemNum(costItemId) : m_orgData.remainingFree);
		}
		return result;
	}

	public override bool ChangeSpinCost(int selectIndex)
	{
		bool result = false;
		if (IsChangeSpinCost())
		{
			result = m_orgData.ChangeCostItem(selectIndex);
		}
		return result;
	}

	public override bool IsChangeSpinCost()
	{
		bool result = false;
		if (m_orgData.GetSpinButton() != 0)
		{
			List<int> spinCostItemIdList = GetSpinCostItemIdList();
			if (spinCostItemIdList.Count > 1)
			{
				result = true;
			}
		}
		return result;
	}

	public override int GetSpinCostCurrentIndex()
	{
		return m_orgData.currentCostSelect;
	}

	public override bool GetEggSeting(out int count)
	{
		bool result = false;
		count = RouletteManager.Instance.specialEgg;
		if (m_init && m_orgData != null && base.category != RouletteCategory.SPECIAL && base.category != RouletteCategory.RAID)
		{
			result = true;
			count = RouletteManager.Instance.specialEgg;
		}
		return result;
	}

	public override ServerWheelOptions GetOrgRankupData()
	{
		return null;
	}

	public override ServerChaoWheelOptions GetOrgNormalData()
	{
		return null;
	}

	public override ServerWheelOptionsGeneral GetOrgGeneralData()
	{
		return m_orgData;
	}

	public override Dictionary<long, string[]> UpdateItemWeights()
	{
		if (m_itemOdds != null)
		{
			m_itemOdds.Clear();
		}
		m_itemOdds = new Dictionary<long, string[]>();
		List<long> list = new List<long>();
		Dictionary<long, float> dictionary = new Dictionary<long, float>();
		Dictionary<long, int> dictionary2 = new Dictionary<long, int>();
		Dictionary<long, float> dictionary3 = new Dictionary<long, float>();
		bool flag = false;
		ServerCampaignState serverCampaignState = null;
		if (m_orgData != null)
		{
			int num = 0;
			if (IsCampaign(Constants.Campaign.emType.PremiumRouletteOdds) && base.category == RouletteCategory.PREMIUM)
			{
				flag = true;
				serverCampaignState = ServerInterface.CampaignState;
				for (int i = 0; i < m_orgData.itemLenght; i++)
				{
					ServerCampaignData campaignInSession = serverCampaignState.GetCampaignInSession(Constants.Campaign.emType.PremiumRouletteOdds, i);
					if (campaignInSession != null)
					{
						num += campaignInSession.iContent;
					}
				}
			}
			for (int j = 0; j < m_orgData.itemLenght; j++)
			{
				int itemId;
				int itemNum;
				float itemRate;
				m_orgData.GetCell(j, out itemId, out itemNum, out itemRate);
				long num2 = itemId;
				float num3 = itemRate;
				float num4 = itemRate;
				int num5 = itemNum;
				num2 = num2 * 100000 + num5;
				if (flag && flag && serverCampaignState != null)
				{
					float num6 = -1f;
					ServerCampaignData campaignInSession2 = serverCampaignState.GetCampaignInSession(Constants.Campaign.emType.PremiumRouletteOdds, j);
					if (campaignInSession2 != null)
					{
						num6 = campaignInSession2.iContent;
					}
					if (num6 >= 0f)
					{
						num4 = (float)Mathf.RoundToInt(num6 / (float)num * 10000f) / 100f;
					}
				}
				if (!list.Contains(num2))
				{
					list.Add(num2);
				}
				if (!dictionary.ContainsKey(num2))
				{
					dictionary.Add(num2, num4);
				}
				else
				{
					Dictionary<long, float> dictionary4;
					Dictionary<long, float> dictionary5 = dictionary4 = dictionary;
					long key;
					long key2 = key = num2;
					float num7 = dictionary4[key];
					dictionary5[key2] = num7 + num4;
				}
				if (!dictionary3.ContainsKey(num2))
				{
					dictionary3.Add(num2, num3);
				}
				else
				{
					Dictionary<long, float> dictionary6;
					Dictionary<long, float> dictionary7 = dictionary6 = dictionary3;
					long key;
					long key3 = key = num2;
					float num7 = dictionary6[key];
					dictionary7[key3] = num7 + num3;
				}
				if (!dictionary2.ContainsKey(num2))
				{
					dictionary2.Add(num2, num5);
				}
				else
				{
					dictionary2[num2] = num5;
				}
			}
			list.Sort();
			for (int k = 0; k < list.Count; k++)
			{
				List<string> list2 = new List<string>();
				long num8 = list[k];
				float num9 = dictionary3[num8];
				float num10 = dictionary[num8];
				int num11 = dictionary2[num8];
				int id = (int)(num8 / 100000);
				ServerItem serverItem = new ServerItem((ServerItem.Id)id);
				string empty = string.Empty;
				string str = string.Empty;
				if (serverItem.idType == ServerItem.IdType.CHARA)
				{
					empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Roulette", "ui_Lbl_rarity_100").text;
				}
				else if (serverItem.idType == ServerItem.IdType.CHAO)
				{
					int id2 = (int)serverItem.id;
					int num12 = id2 / 1000 % 10;
					empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Roulette", "ui_Lbl_rarity_" + num12).text;
				}
				else
				{
					switch (serverItem.id)
					{
					case ServerItem.Id.BIG:
					case ServerItem.Id.SUPER:
					case ServerItem.Id.JACKPOT:
						empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "reward_" + serverItem.id.ToString().ToLower()).text;
						break;
					default:
						empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "RewardType", "reward_type_" + (int)serverItem.rewardType).text;
						str = " x " + num11;
						break;
					}
				}
				list2.Add(empty + str);
				string format = "F" + RouletteUtility.OddsDisplayDecimal;
				string empty2 = string.Empty;
				if (num10 != num9)
				{
					empty2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "odds").text.Replace("{ODDS}", num10.ToString(format));
					float num13 = num10 - num9;
					string text = num13.ToString(format);
					string empty3 = string.Empty;
					if (num13 > 0f)
					{
						text = "+" + text;
						empty3 = "campaign_odds_up";
					}
					else
					{
						empty3 = "campaign_odds_down";
					}
					string str2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", empty3).text.Replace("{ODDS}", text);
					empty2 += str2;
				}
				else
				{
					empty2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "odds").text.Replace("{ODDS}", num10.ToString(format));
				}
				list2.Add(empty2);
				m_itemOdds.Add(num8, list2.ToArray());
			}
		}
		return m_itemOdds;
	}

	public override string ShowSpinErrorWindow()
	{
		ServerWheelOptionsData.SPIN_BUTTON spinButtonSeting = GetSpinButtonSeting();
		string result = null;
		switch (spinButtonSeting)
		{
		case ServerWheelOptionsData.SPIN_BUTTON.FREE:
		{
			result = "SpinRemainingError";
			GeneralWindow.CInfo info4 = default(GeneralWindow.CInfo);
			info4.name = "SpinRemainingError";
			info4.buttonType = GeneralWindow.ButtonType.Ok;
			info4.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_remain_caption").text;
			info4.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_remain_text").text;
			info4.isPlayErrorSe = true;
			GeneralWindow.Create(info4);
			break;
		}
		case ServerWheelOptionsData.SPIN_BUTTON.RING:
		{
			result = "SpinCostErrorRing";
			GeneralWindow.CInfo info3 = default(GeneralWindow.CInfo);
			info3.caption = TextUtility.GetCommonText("ItemRoulette", "gw_cost_caption");
			info3.message = TextUtility.GetCommonText("ItemRoulette", "gw_cost_text");
			info3.buttonType = GeneralWindow.ButtonType.ShopCancel;
			info3.name = "SpinCostErrorRing";
			info3.isPlayErrorSe = true;
			GeneralWindow.Create(info3);
			break;
		}
		case ServerWheelOptionsData.SPIN_BUTTON.RSRING:
		{
			result = "SpinCostErrorRSRing";
			bool flag = ServerInterface.IsRSREnable();
			GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
			info2.caption = TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption");
			info2.message = ((!flag) ? TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption_text_2") : TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption_text"));
			info2.buttonType = ((!flag) ? GeneralWindow.ButtonType.Ok : GeneralWindow.ButtonType.ShopCancel);
			info2.name = "SpinCostErrorRSRing";
			info2.isPlayErrorSe = true;
			GeneralWindow.Create(info2);
			break;
		}
		case ServerWheelOptionsData.SPIN_BUTTON.RAID:
		{
			result = "SpinCostErrorRaidRing";
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextUtility.GetCommonText("Roulette", "gw_raid_cost_caption");
			info.message = TextUtility.GetCommonText("Roulette", "gw_raid_cost_caption_text");
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.name = "SpinCostErrorRaidRing";
			info.isPlayErrorSe = true;
			GeneralWindow.Create(info);
			break;
		}
		default:
			Debug.Log("ServerWheelOptionsRankup ShowSpinErrorWindow error !!!");
			break;
		}
		return result;
	}

	public override List<ServerItem> GetAttentionItemList()
	{
		List<ServerItem> result = null;
		if (base.category == RouletteCategory.RAID && RouletteManager.Instance != null)
		{
			ServerPrizeState prizeList = RouletteManager.Instance.GetPrizeList(base.category);
			if (prizeList != null)
			{
				result = prizeList.GetAttentionList();
			}
		}
		return result;
	}

	public override bool IsPrizeDataList()
	{
		return true;
	}
}
