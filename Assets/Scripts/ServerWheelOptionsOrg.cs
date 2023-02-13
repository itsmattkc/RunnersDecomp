using System.Collections.Generic;

public abstract class ServerWheelOptionsOrg
{
	protected bool m_init;

	protected RouletteUtility.WheelType m_type;

	protected RouletteCategory m_category;

	protected Dictionary<long, string[]> m_itemOdds;

	public RouletteUtility.WheelType wheelType
	{
		get
		{
			return m_type;
		}
	}

	public RouletteCategory category
	{
		get
		{
			return m_category;
		}
	}

	public virtual bool isValid
	{
		get
		{
			return false;
		}
	}

	public virtual bool isRemainingRefresh
	{
		get
		{
			return false;
		}
	}

	public virtual int itemWon
	{
		get
		{
			return -1;
		}
	}

	public virtual ServerItem itemWonData
	{
		get
		{
			return default(ServerItem);
		}
	}

	public virtual int rouletteId
	{
		get
		{
			return -1;
		}
	}

	public virtual int multi
	{
		get
		{
			return 1;
		}
	}

	public virtual int numJackpotRing
	{
		get
		{
			return 0;
		}
	}

	public abstract void Setup(ServerChaoWheelOptions data);

	public abstract void Setup(ServerWheelOptions data);

	public abstract void Setup(ServerWheelOptionsGeneral data);

	public virtual bool ChangeMulti(int multi)
	{
		return false;
	}

	public virtual bool IsMulti(int multi)
	{
		if (multi == 1)
		{
			return true;
		}
		return false;
	}

	public abstract int GetRouletteBoardPattern();

	public abstract string GetRouletteArrowSprite();

	public abstract string GetRouletteBgSprite();

	public abstract string GetRouletteBoardSprite();

	public abstract string GetRouletteTicketSprite();

	public abstract RouletteUtility.WheelRank GetRouletteRank();

	public abstract float GetCellWeight(int cellIndex);

	public abstract int GetCellEgg(int cellIndex);

	public abstract ServerItem GetCellItem(int cellIndex, out int num);

	public abstract void PlayBgm(float delay = 0f);

	public abstract void PlaySe(ServerWheelOptionsData.SE_TYPE seType, float delay = 0f);

	public abstract ServerWheelOptionsData.SPIN_BUTTON GetSpinButtonSeting(out int count, out bool btnActive);

	public abstract ServerWheelOptionsData.SPIN_BUTTON GetSpinButtonSeting();

	public int GetSpinCostItemId()
	{
		int result = -1;
		switch (GetSpinButtonSeting())
		{
		case ServerWheelOptionsData.SPIN_BUTTON.FREE:
			result = 0;
			break;
		case ServerWheelOptionsData.SPIN_BUTTON.RING:
			result = 910000;
			break;
		case ServerWheelOptionsData.SPIN_BUTTON.RSRING:
			result = 900000;
			break;
		case ServerWheelOptionsData.SPIN_BUTTON.RAID:
			result = 960000;
			break;
		case ServerWheelOptionsData.SPIN_BUTTON.TICKET:
		{
			ServerWheelOptionsGeneral orgGeneralData = GetOrgGeneralData();
			if (orgGeneralData != null)
			{
				int currentCostItemId = orgGeneralData.GetCurrentCostItemId();
				if (currentCostItemId > 0)
				{
					int costItemNum = orgGeneralData.GetCostItemNum(currentCostItemId);
					int costItemCost = orgGeneralData.GetCostItemCost(currentCostItemId);
					if (costItemNum >= costItemCost)
					{
						result = currentCostItemId;
					}
				}
				break;
			}
			ServerWheelOptions orgRankupData = GetOrgRankupData();
			ServerChaoWheelOptions orgNormalData = GetOrgNormalData();
			if (orgRankupData != null)
			{
				if (orgRankupData.m_numRouletteToken > 0)
				{
					result = 240000;
				}
			}
			else if (orgNormalData != null)
			{
				result = 230000;
			}
			break;
		}
		}
		return result;
	}

	public abstract List<int> GetSpinCostItemIdList();

	public abstract int GetSpinCostItemCost(int costItemId);

	public abstract int GetSpinCostItemNum(int costItemId);

	public virtual int GetSpinCostCurrentIndex()
	{
		return 0;
	}

	public virtual bool ChangeSpinCost(int selectIndex)
	{
		return false;
	}

	public virtual bool IsChangeSpinCost()
	{
		return false;
	}

	public abstract bool GetEggSeting(out int count);

	public abstract ServerWheelOptions GetOrgRankupData();

	public abstract ServerChaoWheelOptions GetOrgNormalData();

	public abstract ServerWheelOptionsGeneral GetOrgGeneralData();

	public List<Constants.Campaign.emType> GetCampaign()
	{
		return RouletteUtility.GetCampaign(category);
	}

	public bool IsCampaign()
	{
		bool result = false;
		List<Constants.Campaign.emType> campaign = GetCampaign();
		if (campaign != null)
		{
			result = true;
		}
		return result;
	}

	public bool IsCampaign(Constants.Campaign.emType campaign)
	{
		bool result = false;
		List<Constants.Campaign.emType> campaign2 = GetCampaign();
		if (campaign2 != null)
		{
			result = campaign2.Contains(campaign);
		}
		return result;
	}

	public abstract Dictionary<long, string[]> UpdateItemWeights();

	public List<string[]> GetItemOdds()
	{
		if (m_itemOdds == null)
		{
			UpdateItemWeights();
		}
		List<string[]> list = new List<string[]>();
		Dictionary<long, string[]>.KeyCollection keys = m_itemOdds.Keys;
		foreach (long item in keys)
		{
			list.Add(m_itemOdds[item]);
		}
		return list;
	}

	public abstract string ShowSpinErrorWindow();

	public virtual List<ServerItem> GetAttentionItemList()
	{
		List<ServerItem> list = null;
		EventManager instance = EventManager.Instance;
		if (instance != null)
		{
			EyeCatcherChaoData[] eyeCatcherChaoDatas = instance.GetEyeCatcherChaoDatas();
			EyeCatcherCharaData[] eyeCatcherCharaDatas = instance.GetEyeCatcherCharaDatas();
			if (eyeCatcherCharaDatas != null)
			{
				EyeCatcherCharaData[] array = eyeCatcherCharaDatas;
				foreach (EyeCatcherCharaData eyeCatcherCharaData in array)
				{
					ServerItem item = new ServerItem((ServerItem.Id)eyeCatcherCharaData.id);
					if (list == null)
					{
						list = new List<ServerItem>();
					}
					list.Add(item);
				}
			}
			if (eyeCatcherChaoDatas != null)
			{
				EyeCatcherChaoData[] array2 = eyeCatcherChaoDatas;
				foreach (EyeCatcherChaoData eyeCatcherChaoData in array2)
				{
					ServerItem item2 = new ServerItem((ServerItem.Id)(eyeCatcherChaoData.chao_id + 400000));
					if (list == null)
					{
						list = new List<ServerItem>();
					}
					list.Add(item2);
				}
			}
		}
		return list;
	}

	public virtual bool IsPrizeDataList()
	{
		return false;
	}
}
