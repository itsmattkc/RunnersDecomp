using System.Collections.Generic;
using Text;

public class ServerWheelOptionsNormal : ServerWheelOptionsOrg
{
	private static int s_multi = 1;

	private ServerChaoWheelOptions m_orgData;

	public override bool isValid
	{
		get
		{
			bool result = false;
			if (m_orgData != null && m_orgData.IsValid)
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
			return false;
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

	public override int multi
	{
		get
		{
			return s_multi;
		}
	}

	public ServerWheelOptionsNormal(ServerChaoWheelOptions data)
	{
		if (data != null)
		{
			m_category = RouletteCategory.PREMIUM;
			if (data.SpinType == ServerChaoWheelOptions.ChaoSpinType.Special)
			{
				m_category = RouletteCategory.SPECIAL;
			}
			m_init = true;
			m_type = RouletteUtility.WheelType.Normal;
			int multi = s_multi;
			if (m_orgData == null)
			{
				m_orgData = new ServerChaoWheelOptions();
			}
			data.CopyTo(m_orgData);
			if (!data.ChangeMulti(multi) || m_category == RouletteCategory.SPECIAL)
			{
				data.ChangeMulti(1);
			}
			UpdateItemWeights();
		}
	}

	public override void Setup(ServerChaoWheelOptions data)
	{
		if (data != null)
		{
			m_category = RouletteCategory.PREMIUM;
			if (data.SpinType == ServerChaoWheelOptions.ChaoSpinType.Special)
			{
				m_category = RouletteCategory.SPECIAL;
			}
			m_init = true;
			m_type = RouletteUtility.WheelType.Normal;
			int multi = 1;
			if (m_orgData == null)
			{
				m_orgData = new ServerChaoWheelOptions();
			}
			else
			{
				multi = m_orgData.multi;
			}
			data.CopyTo(m_orgData);
			if (!data.ChangeMulti(multi) || m_category == RouletteCategory.SPECIAL)
			{
				data.ChangeMulti(1);
			}
			UpdateItemWeights();
		}
	}

	public override void Setup(ServerWheelOptions data)
	{
	}

	public override void Setup(ServerWheelOptionsGeneral data)
	{
	}

	public override bool ChangeMulti(int multi)
	{
		if (m_orgData == null)
		{
			return false;
		}
		bool result = m_orgData.ChangeMulti(multi);
		s_multi = m_orgData.multi;
		return result;
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
			result = 0;
			if (m_orgData != null && m_orgData.SpinType == ServerChaoWheelOptions.ChaoSpinType.Special)
			{
				result = 1;
			}
		}
		return result;
	}

	public override string GetRouletteArrowSprite()
	{
		string result = null;
		if (m_init)
		{
			result = "ui_roulette_arrow_gol";
		}
		return result;
	}

	public override string GetRouletteBgSprite()
	{
		string result = null;
		if (m_init)
		{
			result = "ui_roulette_tablebg_blu";
		}
		return result;
	}

	public override string GetRouletteBoardSprite()
	{
		string result = null;
		if (m_init)
		{
			result = "ui_roulette_table_sil_0";
			if (m_orgData != null && m_orgData.SpinType == ServerChaoWheelOptions.ChaoSpinType.Special)
			{
				result = "ui_roulette_table_gol_1";
			}
		}
		return result;
	}

	public override string GetRouletteTicketSprite()
	{
		if (m_orgData != null)
		{
			return "ui_cmn_icon_item_230000";
		}
		return null;
	}

	public override RouletteUtility.WheelRank GetRouletteRank()
	{
		return RouletteUtility.WheelRank.Normal;
	}

	public override float GetCellWeight(int cellIndex)
	{
		float result = 0f;
		if (m_init && m_orgData != null && m_orgData.ItemWeight.Length > cellIndex)
		{
			result = m_orgData.ItemWeight[cellIndex];
		}
		return result;
	}

	public override int GetCellEgg(int cellIndex)
	{
		int result = -1;
		if (m_init && m_orgData != null && m_orgData.Rarities.Length > cellIndex)
		{
			result = m_orgData.Rarities[cellIndex];
		}
		return result;
	}

	public override ServerItem GetCellItem(int cellIndex, out int num)
	{
		ServerItem result = default(ServerItem);
		num = -1;
		if (m_init && m_orgData != null && m_orgData.Rarities.Length > cellIndex)
		{
			int num2 = m_orgData.Rarities[cellIndex];
			if (num2 >= 0)
			{
				if (num2 >= 10)
				{
					result = new ServerItem(ServerItem.Id.CHARA_BEGIN);
				}
				else
				{
					switch (num2)
					{
					case 0:
						result = new ServerItem(ServerItem.Id.CHAO_BEGIN);
						break;
					case 1:
						result = new ServerItem(ServerItem.Id.CHAO_BEGIN_RARE);
						break;
					case 2:
						result = new ServerItem(ServerItem.Id.CHAO_BEGIN_SRARE);
						break;
					default:
						result = new ServerItem(ServerItem.Id.CHAO_BEGIN);
						break;
					}
				}
				num = 0;
			}
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
		if ((!RouletteUtility.isTutorial || m_category != RouletteCategory.PREMIUM) && IsBGM_SEChangeEvent(type) && EventManager.Instance.IsInEvent() && EventCommonDataTable.Instance != null)
		{
			string text2 = null;
			text2 = ((m_orgData == null || m_orgData.SpinType != ServerChaoWheelOptions.ChaoSpinType.Special) ? EventCommonDataTable.Instance.GetData(EventCommonDataItem.Roulette_BgmName) : EventCommonDataTable.Instance.GetData(EventCommonDataItem.RouletteS_BgmName));
			if (!string.IsNullOrEmpty(text2))
			{
				cueSheetName = "BGM_" + EventManager.GetEventTypeName(EventManager.Instance.Type);
				text = text2;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			text = ((m_orgData == null || m_orgData.SpinType != ServerChaoWheelOptions.ChaoSpinType.Special) ? "bgm_sys_roulette" : "bgm_sys_s_roulette");
		}
		if (!string.IsNullOrEmpty(text))
		{
			RouletteManager.PlayBgm(text, delay, cueSheetName);
		}
	}

	private bool IsBGM_SEChangeEvent(EventManager.EventType eventType)
	{
		if (eventType != EventManager.EventType.NUM && eventType != EventManager.EventType.UNKNOWN && eventType != EventManager.EventType.ADVERT)
		{
			return true;
		}
		return false;
	}

	public override void PlaySe(ServerWheelOptionsData.SE_TYPE seType, float delay = 0f)
	{
		if (!m_init || !(EventManager.Instance != null))
		{
			return;
		}
		EventManager.EventType type = EventManager.Instance.Type;
		string text = null;
		string cueSheetName = "SE";
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
			if ((!RouletteUtility.isTutorial || m_category != RouletteCategory.PREMIUM) && IsBGM_SEChangeEvent(type) && EventManager.Instance.IsInEvent() && EventCommonDataTable.Instance != null)
			{
				string data2 = EventCommonDataTable.Instance.GetData(EventCommonDataItem.RouletteDecide_SeCueName);
				if (!string.IsNullOrEmpty(data2))
				{
					cueSheetName = "SE_" + EventManager.GetEventTypeName(EventManager.Instance.Type);
					text = data2;
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
			if ((!RouletteUtility.isTutorial || m_category != RouletteCategory.PREMIUM) && (base.category == RouletteCategory.PREMIUM || base.category == RouletteCategory.SPECIAL) && IsBGM_SEChangeEvent(type) && EventManager.Instance.IsInEvent() && EventCommonDataTable.Instance != null)
			{
				string data = EventCommonDataTable.Instance.GetData(EventCommonDataItem.RouletteChange_SeCueName);
				if (!string.IsNullOrEmpty(data))
				{
					cueSheetName = "SE_" + EventManager.GetEventTypeName(EventManager.Instance.Type);
					text = data;
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
		ServerWheelOptionsData.SPIN_BUTTON result = ServerWheelOptionsData.SPIN_BUTTON.NONE;
		count = 0;
		btnActive = false;
		if (m_init && SaveDataManager.Instance != null && SaveDataManager.Instance.ItemData != null)
		{
			if (RouletteUtility.isTutorial && m_category == RouletteCategory.PREMIUM)
			{
				count = -1;
				result = ServerWheelOptionsData.SPIN_BUTTON.FREE;
				btnActive = true;
			}
			else
			{
				int redRingCount = (int)SaveDataManager.Instance.ItemData.RedRingCount;
				if (m_orgData != null && m_orgData.SpinType == ServerChaoWheelOptions.ChaoSpinType.Special)
				{
					result = ServerWheelOptionsData.SPIN_BUTTON.FREE;
					count = -1;
					btnActive = true;
				}
				else
				{
					List<Constants.Campaign.emType> campaign = GetCampaign();
					result = ServerWheelOptionsData.SPIN_BUTTON.RSRING;
					if (m_orgData != null)
					{
						if (m_orgData.NumRouletteToken > 0 && m_orgData.NumRouletteToken >= m_orgData.Cost)
						{
							result = ServerWheelOptionsData.SPIN_BUTTON.TICKET;
							count = m_orgData.Cost * m_orgData.multi;
							if (m_orgData.NumRouletteToken >= count)
							{
								btnActive = true;
							}
						}
						else
						{
							int num = m_orgData.Cost;
							if (campaign != null && campaign.Count > 0 && base.category == RouletteCategory.PREMIUM && campaign.Contains(Constants.Campaign.emType.ChaoRouletteCost))
							{
								ServerCampaignState campaignState = ServerInterface.CampaignState;
								if (campaignState != null)
								{
									ServerCampaignData campaignInSession = campaignState.GetCampaignInSession(Constants.Campaign.emType.ChaoRouletteCost, 0);
									if (campaignInSession != null)
									{
										num = campaignInSession.iContent;
									}
								}
							}
							count = num * m_orgData.multi;
							if (redRingCount >= count)
							{
								btnActive = true;
							}
						}
					}
					else
					{
						btnActive = false;
					}
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
			if (RouletteUtility.isTutorial && m_category == RouletteCategory.PREMIUM)
			{
				result = ServerWheelOptionsData.SPIN_BUTTON.FREE;
			}
			else if (m_orgData != null && m_orgData.SpinType == ServerChaoWheelOptions.ChaoSpinType.Special)
			{
				result = ServerWheelOptionsData.SPIN_BUTTON.FREE;
			}
			else
			{
				result = ServerWheelOptionsData.SPIN_BUTTON.RSRING;
				if (m_orgData.NumRouletteToken > 0 && m_orgData.NumRouletteToken >= m_orgData.Cost)
				{
					result = ServerWheelOptionsData.SPIN_BUTTON.TICKET;
				}
			}
		}
		return result;
	}

	public override List<int> GetSpinCostItemIdList()
	{
		List<int> list = null;
		list = new List<int>();
		list.Add(GetSpinCostItemId());
		return list;
	}

	public override int GetSpinCostItemCost(int costItemId)
	{
		int result = 0;
		if (m_orgData != null)
		{
			int num = m_orgData.Cost;
			if (costItemId == 900000 && base.category == RouletteCategory.PREMIUM)
			{
				List<Constants.Campaign.emType> campaign = GetCampaign();
				if (campaign != null && campaign.Count > 0 && campaign.Contains(Constants.Campaign.emType.ChaoRouletteCost))
				{
					ServerCampaignState campaignState = ServerInterface.CampaignState;
					if (campaignState != null)
					{
						ServerCampaignData campaignInSession = campaignState.GetCampaignInSession(Constants.Campaign.emType.ChaoRouletteCost, 0);
						if (campaignInSession != null)
						{
							num = campaignInSession.iContent;
						}
					}
				}
			}
			result = num;
		}
		return result;
	}

	public override int GetSpinCostItemNum(int costItemId)
	{
		int result = 0;
		switch (costItemId)
		{
		case 910000:
			result = (int)SaveDataManager.Instance.ItemData.RingCount;
			break;
		case 900000:
			result = (int)SaveDataManager.Instance.ItemData.RedRingCount;
			break;
		case 230000:
			if (m_orgData != null)
			{
				result = m_orgData.NumRouletteToken;
			}
			break;
		}
		return result;
	}

	public override bool GetEggSeting(out int count)
	{
		bool result = false;
		count = 0;
		if (m_init && m_orgData != null && m_orgData.SpinType == ServerChaoWheelOptions.ChaoSpinType.Normal)
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
		return m_orgData;
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
		List<int> list = new List<int>();
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		Dictionary<int, float> dictionary2 = new Dictionary<int, float>();
		float num = 0f;
		float num2 = 0f;
		bool flag = false;
		ServerCampaignState serverCampaignState = null;
		if (m_orgData != null)
		{
			if (IsCampaign(Constants.Campaign.emType.PremiumRouletteOdds) && base.category == RouletteCategory.PREMIUM)
			{
				flag = true;
				serverCampaignState = ServerInterface.CampaignState;
			}
			for (int i = 0; i < m_orgData.Rarities.Length; i++)
			{
				int num3 = m_orgData.Rarities[i];
				float num4 = m_orgData.ItemWeight[i];
				float num5 = num4;
				if (flag && serverCampaignState != null)
				{
					float num6 = -1f;
					ServerCampaignData campaignInSession = serverCampaignState.GetCampaignInSession(Constants.Campaign.emType.PremiumRouletteOdds, i);
					if (campaignInSession != null)
					{
						num6 = campaignInSession.iContent;
					}
					if (num6 >= 0f)
					{
						num5 = num6;
					}
				}
				num += num5;
				num2 += num4;
				if (!list.Contains(num3))
				{
					list.Add(num3);
				}
				int key;
				float num7;
				if (!dictionary.ContainsKey(num3))
				{
					dictionary.Add(num3, num5);
				}
				else
				{
					Dictionary<int, float> dictionary3;
					Dictionary<int, float> dictionary4 = dictionary3 = dictionary;
					int key2 = key = num3;
					num7 = dictionary3[key];
					dictionary4[key2] = num7 + num5;
				}
				if (!dictionary2.ContainsKey(num3))
				{
					dictionary2.Add(num3, num4);
					continue;
				}
				Dictionary<int, float> dictionary5;
				Dictionary<int, float> dictionary6 = dictionary5 = dictionary2;
				int key3 = key = num3;
				num7 = dictionary5[key];
				dictionary6[key3] = num7 + num4;
			}
			list.Sort();
			bool flag2 = IsCampaign(Constants.Campaign.emType.PremiumRouletteOdds);
			for (int j = 0; j < list.Count; j++)
			{
				List<string> list2 = new List<string>();
				int num8 = list[j];
				float num9 = dictionary2[num8] / num2 * 100f;
				float num10 = dictionary[num8] / num * 100f;
				string cellName = "ui_Lbl_rarity_" + num8;
				list2.Add(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Roulette", cellName).text);
				string format = "F" + RouletteUtility.OddsDisplayDecimal;
				string empty = string.Empty;
				if (num10 != num9 && flag2)
				{
					empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "odds").text.Replace("{ODDS}", num10.ToString(format));
					float num11 = num10 - num9;
					string text = num11.ToString(format);
					string empty2 = string.Empty;
					if (num11 > 0f)
					{
						text = "+" + text;
						empty2 = "campaign_odds_up";
					}
					else
					{
						empty2 = "campaign_odds_down";
					}
					string str = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", empty2).text.Replace("{ODDS}", text);
					empty += str;
				}
				else
				{
					empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "odds").text.Replace("{ODDS}", num9.ToString(format));
				}
				list2.Add(empty);
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
			result = "SpinCostErrorRing";
			GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
			info2.caption = TextUtility.GetCommonText("ItemRoulette", "gw_cost_caption");
			info2.message = TextUtility.GetCommonText("ItemRoulette", "gw_cost_text");
			info2.buttonType = GeneralWindow.ButtonType.ShopCancel;
			info2.name = "SpinCostErrorRing";
			info2.isPlayErrorSe = true;
			GeneralWindow.Create(info2);
			break;
		}
		case ServerWheelOptionsData.SPIN_BUTTON.RSRING:
		{
			result = "SpinCostErrorRSRing";
			bool flag = ServerInterface.IsRSREnable();
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption");
			info.message = ((!flag) ? TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption_text_2") : TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption_text"));
			info.buttonType = ((!flag) ? GeneralWindow.ButtonType.Ok : GeneralWindow.ButtonType.ShopCancel);
			info.name = "SpinCostErrorRSRing";
			info.isPlayErrorSe = true;
			GeneralWindow.Create(info);
			break;
		}
		default:
			Debug.Log("ServerWheelOptionsNormal ShowSpinErrorWindow error !!!");
			break;
		}
		return result;
	}

	public override bool IsPrizeDataList()
	{
		return true;
	}
}
