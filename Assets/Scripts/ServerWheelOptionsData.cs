using System.Collections.Generic;

public class ServerWheelOptionsData
{
	public enum DATA_TYPE
	{
		NONE,
		NORMAL,
		RANKUP,
		GENERAL
	}

	public enum SPIN_BUTTON
	{
		FREE,
		RING,
		RSRING,
		TICKET,
		RAID,
		NONE
	}

	public enum SE_TYPE
	{
		NONE,
		Open,
		Close,
		Click,
		Spin,
		SpinError,
		Arrow,
		Skip,
		GetItem,
		GetRare,
		GetRankup,
		GetJackpot,
		Multi,
		Change
	}

	private DATA_TYPE m_dataType;

	private ServerWheelOptionsOrg m_wheelOption;

	public RouletteUtility.WheelType wheelType
	{
		get
		{
			return m_wheelOption.wheelType;
		}
	}

	public DATA_TYPE dataType
	{
		get
		{
			if (m_dataType == DATA_TYPE.NONE && m_wheelOption != null)
			{
				if (m_wheelOption.GetOrgGeneralData() != null)
				{
					m_dataType = DATA_TYPE.GENERAL;
				}
				else if (m_wheelOption.GetOrgNormalData() != null)
				{
					m_dataType = DATA_TYPE.NORMAL;
				}
				else if (m_wheelOption.GetOrgRankupData() != null)
				{
					m_dataType = DATA_TYPE.RANKUP;
				}
			}
			return m_dataType;
		}
	}

	public RouletteCategory category
	{
		get
		{
			return m_wheelOption.category;
		}
	}

	public bool isValid
	{
		get
		{
			return m_wheelOption.isValid;
		}
	}

	public bool isRemainingRefresh
	{
		get
		{
			return m_wheelOption.isRemainingRefresh;
		}
	}

	public int itemWon
	{
		get
		{
			return m_wheelOption.itemWon;
		}
	}

	public ServerItem itemWonData
	{
		get
		{
			return m_wheelOption.itemWonData;
		}
	}

	public int rouletteId
	{
		get
		{
			return m_wheelOption.rouletteId;
		}
	}

	public int multi
	{
		get
		{
			return m_wheelOption.multi;
		}
	}

	public bool isGeneral
	{
		get
		{
			return m_wheelOption.GetOrgGeneralData() != null;
		}
	}

	public int numJackpotRing
	{
		get
		{
			return m_wheelOption.numJackpotRing;
		}
	}

	public ServerWheelOptionsData(ServerWheelOptionsData data)
	{
		if (data.GetOrgGeneralData() != null)
		{
			m_wheelOption = new ServerWheelOptionsOrgGen(data.GetOrgGeneralData());
		}
		else if (data.GetOrgNormalData() != null)
		{
			m_wheelOption = new ServerWheelOptionsNormal(data.GetOrgNormalData());
		}
		else if (data.GetOrgRankupData() != null)
		{
			m_wheelOption = new ServerWheelOptionsRankup(data.GetOrgRankupData());
		}
	}

	public ServerWheelOptionsData(ServerChaoWheelOptions data)
	{
		m_wheelOption = new ServerWheelOptionsNormal(data);
	}

	public ServerWheelOptionsData(ServerWheelOptions data)
	{
		m_wheelOption = new ServerWheelOptionsRankup(data);
	}

	public ServerWheelOptionsData(ServerWheelOptionsGeneral data)
	{
		m_wheelOption = new ServerWheelOptionsOrgGen(data);
	}

	public void Setup(ServerChaoWheelOptions data)
	{
		m_wheelOption.Setup(data);
	}

	public void Setup(ServerWheelOptions data)
	{
		m_wheelOption.Setup(data);
	}

	public void Setup(ServerWheelOptionsGeneral data)
	{
		m_wheelOption.Setup(data);
	}

	public bool ChangeMulti(int multi)
	{
		return m_wheelOption.ChangeMulti(multi);
	}

	public bool IsMulti(int multi)
	{
		return m_wheelOption.IsMulti(multi);
	}

	public int GetRouletteBoardPattern()
	{
		if (m_wheelOption == null)
		{
			return 0;
		}
		return m_wheelOption.GetRouletteBoardPattern();
	}

	public string GetRouletteArrowSprite()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetRouletteArrowSprite();
	}

	public string GetRouletteBgSprite()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetRouletteBgSprite();
	}

	public string GetRouletteBoardSprite()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetRouletteBoardSprite();
	}

	public string GetRouletteTicketSprite()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetRouletteTicketSprite();
	}

	public RouletteUtility.WheelRank GetRouletteRank()
	{
		if (m_wheelOption == null)
		{
			return RouletteUtility.WheelRank.Normal;
		}
		return m_wheelOption.GetRouletteRank();
	}

	public int GetCellEgg(int cellIndex)
	{
		if (m_wheelOption == null)
		{
			return 0;
		}
		return m_wheelOption.GetCellEgg(cellIndex);
	}

	public ServerItem GetCellItem(int cellIndex, out int num)
	{
		if (m_wheelOption == null)
		{
			num = 0;
			return default(ServerItem);
		}
		return m_wheelOption.GetCellItem(cellIndex, out num);
	}

	public ServerItem GetCellItem(int cellIndex)
	{
		if (m_wheelOption == null)
		{
			return default(ServerItem);
		}
		int num = 0;
		return m_wheelOption.GetCellItem(cellIndex, out num);
	}

	public float GetCellWeight(int cellIndex)
	{
		if (m_wheelOption == null)
		{
			return 0f;
		}
		return m_wheelOption.GetCellWeight(cellIndex);
	}

	public void PlayBgm(float delay = 0f)
	{
		if (m_wheelOption != null)
		{
			m_wheelOption.PlayBgm(delay);
		}
	}

	public void PlaySe(SE_TYPE seType, float delay = 0f)
	{
		if (m_wheelOption != null)
		{
			m_wheelOption.PlaySe(seType, delay);
		}
	}

	public SPIN_BUTTON GetSpinButtonSeting(out int count, out bool btnActive)
	{
		if (m_wheelOption == null)
		{
			count = 0;
			btnActive = false;
			return SPIN_BUTTON.NONE;
		}
		return m_wheelOption.GetSpinButtonSeting(out count, out btnActive);
	}

	public SPIN_BUTTON GetSpinButtonSeting()
	{
		if (m_wheelOption == null)
		{
			return SPIN_BUTTON.NONE;
		}
		return m_wheelOption.GetSpinButtonSeting();
	}

	public int GetSpinCostItemId()
	{
		if (m_wheelOption == null)
		{
			return 0;
		}
		return m_wheelOption.GetSpinCostItemId();
	}

	public List<int> GetSpinCostItemIdList()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetSpinCostItemIdList();
	}

	public int GetSpinCostItemNum(int costItemId)
	{
		if (m_wheelOption == null)
		{
			return 0;
		}
		return m_wheelOption.GetSpinCostItemNum(costItemId);
	}

	public int GetSpinCostItemCost(int costItemId)
	{
		if (m_wheelOption == null)
		{
			return 0;
		}
		return m_wheelOption.GetSpinCostItemCost(costItemId);
	}

	public bool ChangeSpinCost(int selectIndex)
	{
		if (m_wheelOption == null)
		{
			return false;
		}
		return m_wheelOption.ChangeSpinCost(selectIndex);
	}

	public int GetCurrentSpinCostIndex()
	{
		if (m_wheelOption == null)
		{
			return 0;
		}
		return m_wheelOption.GetSpinCostCurrentIndex();
	}

	public bool GetEggSeting(out int count)
	{
		if (m_wheelOption == null)
		{
			count = 0;
			return false;
		}
		return m_wheelOption.GetEggSeting(out count);
	}

	public ServerWheelOptions GetOrgRankupData()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetOrgRankupData();
	}

	public ServerChaoWheelOptions GetOrgNormalData()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetOrgNormalData();
	}

	public ServerWheelOptionsGeneral GetOrgGeneralData()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetOrgGeneralData();
	}

	public List<Constants.Campaign.emType> GetCampaign()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetCampaign();
	}

	public bool IsCampaign()
	{
		if (m_wheelOption == null)
		{
			return false;
		}
		return m_wheelOption.IsCampaign();
	}

	public bool IsCampaign(Constants.Campaign.emType campaign)
	{
		if (m_wheelOption == null)
		{
			return false;
		}
		return m_wheelOption.IsCampaign(campaign);
	}

	public List<string[]> GetItemOdds()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetItemOdds();
	}

	public string ShowSpinErrorWindow()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.ShowSpinErrorWindow();
	}

	public List<ServerItem> GetAttentionItemList()
	{
		if (m_wheelOption == null)
		{
			return null;
		}
		return m_wheelOption.GetAttentionItemList();
	}

	public bool IsPrizeDataList()
	{
		if (m_wheelOption == null)
		{
			return false;
		}
		return m_wheelOption.IsPrizeDataList();
	}

	public void CopyTo(ServerWheelOptionsData to)
	{
		to.Setup(m_wheelOption.GetOrgGeneralData());
		to.Setup(m_wheelOption.GetOrgNormalData());
		to.Setup(m_wheelOption.GetOrgRankupData());
	}
}
