using System.Collections.Generic;
using Text;

public class ServerWheelOptionsRankup : ServerWheelOptionsOrg
{
	private ServerWheelOptions m_orgData;

	public override bool isValid
	{
		get
		{
			bool result = false;
			if ((m_orgData.m_nextFreeSpin - NetBase.GetCurrentTime()).Ticks > 0)
			{
				result = true;
			}
			return result;
		}
	}

	public override bool isRemainingRefresh
	{
		get
		{
			bool result = false;
			if (m_orgData != null && m_orgData.m_itemWon >= 0 && m_orgData.m_items.Length > m_orgData.m_itemWon)
			{
				int id = m_orgData.m_items[m_orgData.m_itemWon];
				if (new ServerItem((ServerItem.Id)id).idType == ServerItem.IdType.ITEM_ROULLETE_WIN && m_orgData.m_rouletteRank != RouletteUtility.WheelRank.Super)
				{
					result = true;
				}
			}
			return result;
		}
	}

	public override int itemWon
	{
		get
		{
			int result = -1;
			if (m_orgData != null)
			{
				result = m_orgData.m_itemWon;
			}
			return result;
		}
	}

	public override ServerItem itemWonData
	{
		get
		{
			if (m_orgData != null && m_orgData.m_itemWon >= 0 && m_orgData.m_items.Length > m_orgData.m_itemWon)
			{
				int id = m_orgData.m_items[m_orgData.m_itemWon];
				return new ServerItem((ServerItem.Id)id);
			}
			return default(ServerItem);
		}
	}

	public override int numJackpotRing
	{
		get
		{
			return m_orgData.m_numJackpotRing;
		}
	}

	public ServerWheelOptionsRankup(ServerWheelOptions data)
	{
		if (data != null)
		{
			m_category = RouletteCategory.ITEM;
			m_init = true;
			m_type = RouletteUtility.WheelType.Rankup;
			if (m_orgData == null)
			{
				m_orgData = new ServerWheelOptions();
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
		if (data != null)
		{
			m_category = RouletteCategory.ITEM;
			m_init = true;
			m_type = RouletteUtility.WheelType.Rankup;
			if (m_orgData == null)
			{
				m_orgData = new ServerWheelOptions();
			}
			data.CopyTo(m_orgData);
			UpdateItemWeights();
		}
	}

	public override void Setup(ServerWheelOptionsGeneral data)
	{
	}

	public override int GetRouletteBoardPattern()
	{
		int result = 0;
		if (m_init)
		{
			result = 1;
		}
		return result;
	}

	public override string GetRouletteArrowSprite()
	{
		string result = null;
		if (m_init)
		{
			switch (m_orgData.m_rouletteRank)
			{
			case RouletteUtility.WheelRank.Normal:
				result = "ui_roulette_arrow_sil";
				break;
			case RouletteUtility.WheelRank.Big:
				result = "ui_roulette_arrow_gol";
				break;
			case RouletteUtility.WheelRank.Super:
				result = "ui_roulette_arrow_gol";
				break;
			}
		}
		return result;
	}

	public override string GetRouletteBgSprite()
	{
		string text = null;
		return "ui_roulette_tablebg_gre";
	}

	public override string GetRouletteBoardSprite()
	{
		string result = null;
		switch (m_orgData.m_rouletteRank)
		{
		case RouletteUtility.WheelRank.Normal:
			result = "ui_roulette_table_gre_1";
			break;
		case RouletteUtility.WheelRank.Big:
			result = "ui_roulette_table_sil_1";
			break;
		case RouletteUtility.WheelRank.Super:
			result = "ui_roulette_table_gol_1r";
			break;
		}
		return result;
	}

	public override string GetRouletteTicketSprite()
	{
		if (m_orgData != null)
		{
			return "ui_cmn_icon_item_240000";
		}
		return null;
	}

	public override RouletteUtility.WheelRank GetRouletteRank()
	{
		RouletteUtility.WheelRank result = RouletteUtility.WheelRank.Normal;
		if (m_init && m_orgData != null)
		{
			result = m_orgData.m_rouletteRank;
		}
		return result;
	}

	public override float GetCellWeight(int cellIndex)
	{
		return 1f;
	}

	public override int GetCellEgg(int cellIndex)
	{
		int num = -1;
		if (m_orgData != null && m_orgData.m_items.Length > cellIndex)
		{
			num = m_orgData.m_items[cellIndex];
			if (num >= 1000)
			{
				if (num >= 300000 && num < 400000)
				{
					num = 100;
				}
				else if (num >= 400000 && num < 500000)
				{
					num = ((num >= 402000) ? 2 : ((num >= 401000) ? 1 : 0));
				}
			}
		}
		return num;
	}

	public override ServerItem GetCellItem(int cellIndex, out int num)
	{
		ServerItem result = default(ServerItem);
		num = 1;
		if (m_orgData != null && m_orgData.m_items.Length > cellIndex)
		{
			result = new ServerItem((ServerItem.Id)m_orgData.m_items[cellIndex]);
			num = m_orgData.m_itemQuantities[cellIndex];
		}
		return result;
	}

	public override void PlayBgm(float delay = 0f)
	{
		Debug.Log("ServerWheelOptionsRankup PlayBgm   no play bgm !");
	}

	public override void PlaySe(ServerWheelOptionsData.SE_TYPE seType, float delay = 0f)
	{
		if (!m_init)
		{
			return;
		}
		string text = null;
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
			text = "sys_menu_decide";
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
			text = "sys_roulette_change";
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			if (delay <= 0f)
			{
				SoundManager.SePlay(text);
			}
			else
			{
				RouletteManager.PlaySe(text, delay);
			}
		}
	}

	public override ServerWheelOptionsData.SPIN_BUTTON GetSpinButtonSeting(out int count, out bool btnActive)
	{
		ServerWheelOptionsData.SPIN_BUTTON result = ServerWheelOptionsData.SPIN_BUTTON.NONE;
		count = 0;
		btnActive = false;
		if (m_init && SaveDataManager.Instance != null && SaveDataManager.Instance.ItemData != null)
		{
			int ringCount = (int)SaveDataManager.Instance.ItemData.RingCount;
			result = ServerWheelOptionsData.SPIN_BUTTON.TICKET;
			int numRemaining = m_orgData.m_numRemaining;
			int numRouletteToken = m_orgData.m_numRouletteToken;
			if (numRemaining > numRouletteToken)
			{
				result = ServerWheelOptionsData.SPIN_BUTTON.FREE;
				count = numRemaining - numRouletteToken;
				btnActive = true;
			}
			else
			{
				count = 1;
				if (numRemaining > 0)
				{
					btnActive = true;
				}
			}
		}
		return result;
	}

	public override ServerWheelOptionsData.SPIN_BUTTON GetSpinButtonSeting()
	{
		ServerWheelOptionsData.SPIN_BUTTON result = ServerWheelOptionsData.SPIN_BUTTON.NONE;
		if (m_init && SaveDataManager.Instance != null && SaveDataManager.Instance.ItemData != null)
		{
			result = ServerWheelOptionsData.SPIN_BUTTON.TICKET;
			int numRemaining = m_orgData.m_numRemaining;
			int numRouletteToken = m_orgData.m_numRouletteToken;
			if (numRemaining > numRouletteToken)
			{
				result = ServerWheelOptionsData.SPIN_BUTTON.FREE;
			}
		}
		return result;
	}

	public override List<int> GetSpinCostItemIdList()
	{
		List<int> list = null;
		list = new List<int>();
		if (m_orgData != null)
		{
			int numRouletteToken = m_orgData.m_numRouletteToken;
			if (numRouletteToken >= 0)
			{
				list.Add(240000);
			}
		}
		return list;
	}

	public override int GetSpinCostItemCost(int costItemId)
	{
		int result = 0;
		if (m_orgData != null)
		{
			if (costItemId == 0)
			{
				result = 1;
			}
			else
			{
				switch (costItemId)
				{
				case 900000:
				case 910000:
					result = m_orgData.m_spinCost;
					break;
				case 240000:
					result = 1;
					break;
				}
			}
		}
		return result;
	}

	public override int GetSpinCostItemNum(int costItemId)
	{
		int result = 0;
		if (m_orgData != null)
		{
			if (costItemId == 0)
			{
				result = m_orgData.m_numRemaining - m_orgData.m_numRouletteToken;
			}
			else
			{
				switch (costItemId)
				{
				case 910000:
					result = (int)SaveDataManager.Instance.ItemData.RingCount;
					break;
				case 900000:
					result = (int)SaveDataManager.Instance.ItemData.RedRingCount;
					break;
				case 240000:
					result = m_orgData.m_numRouletteToken;
					break;
				}
			}
		}
		return result;
	}

	public override bool GetEggSeting(out int count)
	{
		bool result = false;
		count = 0;
		return result;
	}

	public override ServerWheelOptions GetOrgRankupData()
	{
		return m_orgData;
	}

	public override ServerChaoWheelOptions GetOrgNormalData()
	{
		return null;
	}

	public override ServerWheelOptionsGeneral GetOrgGeneralData()
	{
		return null;
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
		float num = 0f;
		if (m_orgData != null)
		{
			for (int i = 0; i < m_orgData.m_items.Length; i++)
			{
				long num2 = m_orgData.m_items[i];
				float num3 = m_orgData.m_itemWeight[i];
				int num4 = m_orgData.m_itemQuantities[i];
				if (num2 >= 400000 && num2 < 500000)
				{
					num2 = ((num2 >= 402000) ? 402000 : ((num2 < 401000) ? 400000 : 401000));
				}
				num2 = num2 * 100000 + num4;
				num += num3;
				if (!list.Contains(num2))
				{
					list.Add(num2);
				}
				if (!dictionary.ContainsKey(num2))
				{
					dictionary.Add(num2, num3);
				}
				else
				{
					Dictionary<long, float> dictionary3;
					Dictionary<long, float> dictionary4 = dictionary3 = dictionary;
					long key;
					long key2 = key = num2;
					float num5 = dictionary3[key];
					dictionary4[key2] = num5 + num3;
				}
				if (!dictionary2.ContainsKey(num2))
				{
					dictionary2.Add(num2, num4);
				}
				else
				{
					dictionary2[num2] = num4;
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				List<string> list2 = new List<string>();
				long num6 = list[j];
				float num7 = dictionary[num6] / num * 100f;
				int num8 = dictionary2[num6];
				int id = (int)(num6 / 100000);
				ServerItem serverItem = new ServerItem((ServerItem.Id)id);
				string empty = string.Empty;
				string str = string.Empty;
				switch (serverItem.id)
				{
				case ServerItem.Id.BIG:
				case ServerItem.Id.SUPER:
				case ServerItem.Id.JACKPOT:
					empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "reward_" + serverItem.id.ToString().ToLower()).text;
					break;
				default:
					if (serverItem.idType == ServerItem.IdType.CHAO || serverItem.idType == ServerItem.IdType.CHARA)
					{
						string cellName = "ui_Lbl_rarity_0";
						if (serverItem.id >= ServerItem.Id.CHAO_BEGIN && serverItem.id < (ServerItem.Id)500000)
						{
							cellName = "ui_Lbl_rarity_" + (int)((float)serverItem.id / 1000f) % 10;
						}
						else if (serverItem.id >= ServerItem.Id.CHARA_BEGIN && serverItem.id < ServerItem.Id.CHAO_BEGIN)
						{
							cellName = "ui_Lbl_rarity_100";
						}
						empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Roulette", cellName).text;
					}
					else
					{
						empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "RewardType", "reward_type_" + (int)serverItem.rewardType).text;
						str = " x " + num8;
					}
					break;
				}
				list2.Add(empty + str);
				string format = "F" + RouletteUtility.OddsDisplayDecimal;
				string empty2 = string.Empty;
				empty2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "odds").text.Replace("{ODDS}", num7.ToString(format));
				list2.Add(empty2);
				m_itemOdds.Add(num6, list2.ToArray());
			}
		}
		return m_itemOdds;
	}

	public override string ShowSpinErrorWindow()
	{
		ServerWheelOptionsData.SPIN_BUTTON spinButtonSeting = GetSpinButtonSeting();
		string text = null;
		switch (spinButtonSeting)
		{
		case ServerWheelOptionsData.SPIN_BUTTON.FREE:
		{
			text = "SpinRemainingError";
			GeneralWindow.CInfo info3 = default(GeneralWindow.CInfo);
			info3.name = "SpinRemainingError";
			info3.buttonType = GeneralWindow.ButtonType.Ok;
			info3.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_remain_caption").text;
			info3.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_remain_text").text;
			info3.isPlayErrorSe = true;
			GeneralWindow.Create(info3);
			break;
		}
		case ServerWheelOptionsData.SPIN_BUTTON.RING:
		{
			text = "SpinCostErrorRing";
			GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
			info2.caption = TextUtility.GetCommonText("ItemRoulette", "gw_cost_caption");
			info2.message = TextUtility.GetCommonText("ItemRoulette", "gw_cost_text");
			info2.buttonType = GeneralWindow.ButtonType.ShopCancel;
			info2.name = "SpinCostErrorRing";
			info2.isPlayErrorSe = true;
			GeneralWindow.Create(info2);
			break;
		}
		case ServerWheelOptionsData.SPIN_BUTTON.TICKET:
		{
			text = "SpinTicketError";
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "SpinTicketError";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_remain_caption").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_ticket_text").text;
			info.isPlayErrorSe = true;
			GeneralWindow.Create(info);
			break;
		}
		default:
			text = "SpinDefaultError";
			Debug.Log("ServerWheelOptionsRankup ShowSpinErrorWindow error !!!");
			break;
		}
		return text;
	}

	public override List<ServerItem> GetAttentionItemList()
	{
		return null;
	}

	public override bool IsPrizeDataList()
	{
		return false;
	}
}
