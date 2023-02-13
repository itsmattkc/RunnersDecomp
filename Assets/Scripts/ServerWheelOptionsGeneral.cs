using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerWheelOptionsGeneral
{
	private const long COST_ITEM_REQ_OFFSET = 10000000L;

	private int m_currentCostSelect;

	private List<int> m_itemId;

	private List<int> m_itemNum;

	private List<int> m_itemWeight;

	private int m_rouletteId;

	private int m_rank;

	private int m_jackpotRing;

	private int m_remaining;

	private int m_spEgg;

	private int m_multi;

	private Dictionary<int, long> m_costItem;

	private DateTime m_nextFreeSpin;

	private int m_patternType = -1;

	public int currentCostSelect
	{
		get
		{
			return m_currentCostSelect;
		}
		private set
		{
			m_currentCostSelect = value;
		}
	}

	public int multi
	{
		get
		{
			return m_multi;
		}
		private set
		{
			m_multi = value;
		}
	}

	public int itemLenght
	{
		get
		{
			if (m_itemId == null)
			{
				return 0;
			}
			return m_itemId.Count;
		}
	}

	public int rouletteId
	{
		get
		{
			return m_rouletteId;
		}
	}

	public int remainingTicketTotal
	{
		get
		{
			return GetRemainingTicket();
		}
	}

	public int remainingFree
	{
		get
		{
			return m_remaining - GetRemainingTicket();
		}
	}

	public int jackpotRing
	{
		get
		{
			return m_jackpotRing;
		}
	}

	public int spEgg
	{
		get
		{
			return m_spEgg;
		}
		set
		{
			m_spEgg = value;
		}
	}

	public DateTime nextFreeSpin
	{
		get
		{
			return m_nextFreeSpin;
		}
	}

	public RouletteUtility.WheelType type
	{
		get
		{
			return (!isRankup) ? RouletteUtility.WheelType.Normal : RouletteUtility.WheelType.Rankup;
		}
	}

	public RouletteUtility.WheelRank rank
	{
		get
		{
			return RouletteUtility.GetRouletteRank(m_rank);
		}
	}

	public int patternType
	{
		get
		{
			if (m_patternType < 0)
			{
				return GetRoulettePatternType();
			}
			return m_patternType;
		}
	}

	public string spriteNameBg
	{
		get
		{
			return RouletteUtility.GetRouletteBgSpriteName(this);
		}
	}

	public string spriteNameBoard
	{
		get
		{
			return RouletteUtility.GetRouletteBoardSpriteName(this);
		}
	}

	public string spriteNameArrow
	{
		get
		{
			return RouletteUtility.GetRouletteArrowSpriteName(this);
		}
	}

	public string spriteNameCostItem
	{
		get
		{
			return RouletteUtility.GetRouletteCostItemName(GetCurrentCostItemId());
		}
	}

	public bool isRankup
	{
		get
		{
			bool result = false;
			if (m_rank > 0)
			{
				return true;
			}
			foreach (int item in m_itemId)
			{
				if (new ServerItem((ServerItem.Id)item).idType == ServerItem.IdType.ITEM_ROULLETE_WIN)
				{
					return true;
				}
			}
			return result;
		}
	}

	public ServerWheelOptionsGeneral()
	{
		m_itemId = new List<int>();
		m_itemNum = new List<int>();
		m_itemWeight = new List<int>();
		m_costItem = new Dictionary<int, long>();
		for (int i = 0; i < 8; i++)
		{
			if (i == 0)
			{
				m_itemId.Add(200000);
			}
			else
			{
				m_itemId.Add(120000 + i - 1);
			}
			m_itemNum.Add(1);
			m_itemWeight.Add(1);
		}
		m_remaining = 0;
		m_jackpotRing = 0;
		m_rank = 0;
		m_multi = 1;
		m_nextFreeSpin = DateTime.Now;
		m_nextFreeSpin = m_nextFreeSpin.AddDays(999.0);
	}

	private int GetRoulettePatternType()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 9999999;
		foreach (int item in m_itemWeight)
		{
			if (num2 < item)
			{
				num2 = item;
			}
			if (num3 > item)
			{
				num3 = item;
			}
		}
		return m_patternType = ((!((float)num3 / (float)num2 < 0.35f)) ? 1 : 0);
	}

	public ServerWheelOptionsData.SPIN_BUTTON GetSpinButton()
	{
		ServerWheelOptionsData.SPIN_BUTTON sPIN_BUTTON = ServerWheelOptionsData.SPIN_BUTTON.NONE;
		if (remainingFree > 0)
		{
			sPIN_BUTTON = ServerWheelOptionsData.SPIN_BUTTON.FREE;
		}
		else if (sPIN_BUTTON == ServerWheelOptionsData.SPIN_BUTTON.NONE)
		{
			switch (GetCurrentCostItemId())
			{
			case 900000:
				sPIN_BUTTON = ServerWheelOptionsData.SPIN_BUTTON.RSRING;
				break;
			case 910000:
				sPIN_BUTTON = ServerWheelOptionsData.SPIN_BUTTON.RING;
				break;
			case 960000:
				sPIN_BUTTON = ServerWheelOptionsData.SPIN_BUTTON.RAID;
				break;
			default:
				sPIN_BUTTON = ServerWheelOptionsData.SPIN_BUTTON.TICKET;
				break;
			}
		}
		return sPIN_BUTTON;
	}

	public RouletteUtility.CellType GetCell(int index)
	{
		RouletteUtility.CellType result = RouletteUtility.CellType.Item;
		if (index >= 0 && index < itemLenght)
		{
			int num = m_itemId[index];
			if (num >= 0)
			{
				switch (new ServerItem((ServerItem.Id)num).idType)
				{
				case ServerItem.IdType.CHARA:
				case ServerItem.IdType.CHAO:
					result = RouletteUtility.CellType.Egg;
					break;
				case ServerItem.IdType.ITEM_ROULLETE_WIN:
					result = RouletteUtility.CellType.Rankup;
					break;
				default:
					result = RouletteUtility.CellType.Item;
					break;
				}
			}
		}
		return result;
	}

	public float GetCellWeight(int index)
	{
		float result = 0f;
		if (index >= 0 && index < itemLenght)
		{
			result = m_itemWeight[index];
		}
		return result;
	}

	public RouletteUtility.CellType GetCell(int index, out int itemId, out int itemNum, out float itemRate)
	{
		RouletteUtility.CellType result = RouletteUtility.CellType.Item;
		itemId = 0;
		itemNum = 0;
		itemRate = 0f;
		if (index >= 0 && index < itemLenght)
		{
			int num = m_itemId[index];
			int num2 = m_itemNum[index];
			if (num >= 0)
			{
				itemId = num;
				itemNum = num2;
				itemRate = GetItemRate(index);
				switch (new ServerItem((ServerItem.Id)num).idType)
				{
				case ServerItem.IdType.CHARA:
				case ServerItem.IdType.CHAO:
					result = RouletteUtility.CellType.Egg;
					itemNum = 0;
					break;
				case ServerItem.IdType.ITEM_ROULLETE_WIN:
					result = RouletteUtility.CellType.Rankup;
					itemNum = 0;
					break;
				default:
					result = RouletteUtility.CellType.Item;
					break;
				}
			}
		}
		return result;
	}

	private float GetItemRate(int index)
	{
		float result = 0f;
		if (index >= 0 && index < itemLenght)
		{
			int itemMaxWeightIndex = GetItemMaxWeightIndex();
			int itemTotalWeight = GetItemTotalWeight();
			int num = m_itemWeight[index];
			if (itemTotalWeight > 0 && num > 0)
			{
				if (itemMaxWeightIndex < 0 || index != itemMaxWeightIndex)
				{
					result = Mathf.Round((float)num / (float)itemTotalWeight * 10000f) / 100f;
				}
				else
				{
					float num2 = 0f;
					for (int i = 0; i < m_itemWeight.Count; i++)
					{
						if (i != itemMaxWeightIndex)
						{
							num2 += Mathf.Round((float)num / (float)itemTotalWeight * 10000f) / 100f;
						}
					}
					result = 100f - num2;
				}
			}
		}
		return result;
	}

	private int GetItemTotalWeight()
	{
		int num = 0;
		for (int i = 0; i < m_itemWeight.Count; i++)
		{
			num += m_itemWeight[i];
		}
		return num;
	}

	private int GetItemMaxWeightIndex()
	{
		int result = -1;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < m_itemWeight.Count; i++)
		{
			int num3 = num;
			if (m_itemWeight[i] > num)
			{
				num = m_itemWeight[i];
				result = i;
			}
			if (num3 != num)
			{
				num2++;
			}
		}
		if (num2 <= 1)
		{
			result = -1;
		}
		return result;
	}

	public void SetupItem(int index, int itemId, int weight, int num = 0)
	{
		if (m_itemId == null || m_itemNum == null || m_itemWeight == null || index < 0)
		{
			return;
		}
		if (index < m_itemId.Count)
		{
			m_itemId[index] = itemId;
			m_itemNum[index] = num;
			m_itemWeight[index] = weight;
			return;
		}
		int num2 = index - m_itemId.Count + 1;
		for (int i = 0; i < num2; i++)
		{
			m_itemId.Add(itemId);
			m_itemNum.Add(num);
			m_itemWeight.Add(weight);
		}
	}

	public void ResetupCostItem()
	{
		if (m_costItem != null)
		{
			m_costItem.Clear();
		}
		m_costItem = new Dictionary<int, long>();
	}

	public void AddCostItem(int itemId, int itemNum, int oneCost = 1)
	{
		if (m_costItem == null)
		{
			m_costItem = new Dictionary<int, long>();
		}
		Debug.Log("ServerWheelOptionsGeneral AddCostItem  itemId:" + itemId + "  itemNum:" + itemNum + "  oneCost:" + oneCost);
		if (m_costItem.ContainsKey(itemId))
		{
			Dictionary<int, long> costItem;
			Dictionary<int, long> dictionary = costItem = m_costItem;
			int key;
			int key2 = key = itemId;
			long num = costItem[key];
			dictionary[key2] = num + itemNum;
		}
		else
		{
			m_costItem.Add(itemId, itemNum + 10000000L * (long)oneCost);
		}
		GeneralUtil.SetItemCount((ServerItem.Id)itemId, m_costItem[itemId] % 10000000);
	}

	public void CopyToCostItem(Dictionary<int, long> items)
	{
		if (m_costItem == null)
		{
			m_costItem = new Dictionary<int, long>();
		}
		else
		{
			m_costItem.Clear();
			m_costItem = new Dictionary<int, long>();
		}
		if (items == null || items.Count <= 0)
		{
			return;
		}
		Dictionary<int, long>.KeyCollection keys = items.Keys;
		foreach (int item in keys)
		{
			m_costItem.Add(item, items[item]);
		}
	}

	public List<int> GetCostItemList()
	{
		List<int> result = null;
		if (m_costItem != null && m_costItem.Count > 0)
		{
			result = new List<int>();
			Dictionary<int, long>.KeyCollection keys = m_costItem.Keys;
			{
				foreach (int item in keys)
				{
					result.Add(item);
				}
				return result;
			}
		}
		return result;
	}

	public int GetCostItemNum(int costItemId)
	{
		int result = -1;
		if (m_costItem != null && m_costItem.Count > 0 && m_costItem.ContainsKey(costItemId))
		{
			result = (int)GeneralUtil.GetItemCount((ServerItem.Id)costItemId);
		}
		return result;
	}

	public int GetCostItemCost(int costItemId)
	{
		int result = -1;
		if (m_costItem != null && m_costItem.Count > 0 && m_costItem.ContainsKey(costItemId))
		{
			result = (int)(m_costItem[costItemId] / 10000000);
		}
		return result;
	}

	public int GetDefultCostItemId()
	{
		int result = -1;
		List<int> costItemList = GetCostItemList();
		if (costItemList != null && costItemList.Count > 0)
		{
			result = costItemList[0];
		}
		return result;
	}

	public int GetCurrentCostItemId()
	{
		int num = -1;
		int num2 = 0;
		int num3 = 0;
		List<int> costItemList = GetCostItemList();
		int num4 = 0;
		if (costItemList != null && costItemList.Count > 0)
		{
			if (m_currentCostSelect <= 0)
			{
				for (int i = 0; i < costItemList.Count; i++)
				{
					num2 = GetCostItemNum(costItemList[i]);
					num3 = GetCostItemCost(costItemList[i]);
					if (num2 >= num3)
					{
						num = costItemList[i];
						break;
					}
				}
			}
			else if (m_currentCostSelect <= costItemList.Count)
			{
				for (int j = 0; j < costItemList.Count; j++)
				{
					num4 = (m_currentCostSelect + j - 1) % costItemList.Count;
					if (num4 < costItemList.Count)
					{
						num2 = GetCostItemNum(costItemList[num4]);
						num3 = GetCostItemCost(costItemList[num4]);
						if (num2 >= num3)
						{
							num = costItemList[num4];
							m_currentCostSelect = num4 + 1;
							break;
						}
					}
				}
			}
			if (num == -1)
			{
				num = costItemList[0];
			}
		}
		return num;
	}

	public int GetCurrentCostItemNum()
	{
		int result = 0;
		int currentCostItemId = GetCurrentCostItemId();
		if (currentCostItemId > 0)
		{
			int costItemNum = GetCostItemNum(currentCostItemId);
			int costItemCost = GetCostItemCost(currentCostItemId);
			if (costItemNum >= costItemCost)
			{
				result = costItemNum / costItemCost;
			}
		}
		return result;
	}

	public bool ChangeCostItem(int selectIndex)
	{
		bool result = false;
		if (currentCostSelect == selectIndex)
		{
			return false;
		}
		if (selectIndex <= 0)
		{
			currentCostSelect = 0;
			return true;
		}
		List<int> costItemList = GetCostItemList();
		if (costItemList != null && costItemList.Count > 1)
		{
			if (costItemList.Count > selectIndex - 1)
			{
				int costItemId = costItemList[selectIndex - 1];
				int costItemCost = GetCostItemCost(costItemId);
				int costItemNum = GetCostItemNum(costItemId);
				if (costItemNum >= costItemCost)
				{
					currentCostSelect = selectIndex;
				}
				else
				{
					currentCostSelect = 0;
				}
			}
			else
			{
				currentCostSelect = 99;
			}
			result = true;
		}
		return result;
	}

	public bool ChangeMulti(int multi)
	{
		bool flag = false;
		flag = IsMulti(multi);
		if (flag)
		{
			m_multi = multi;
			if (m_multi < 1)
			{
				m_multi = 1;
			}
		}
		else
		{
			m_multi = 1;
		}
		return flag;
	}

	public bool IsMulti(int multi)
	{
		bool result = false;
		if (multi <= 1)
		{
			result = true;
		}
		else
		{
			int currentCostItemId = GetCurrentCostItemId();
			ServerWheelOptionsData.SPIN_BUTTON spinButton = GetSpinButton();
			int costItemCost = GetCostItemCost(currentCostItemId);
			int num = 0;
			bool flag = true;
			switch (spinButton)
			{
			case ServerWheelOptionsData.SPIN_BUTTON.RING:
				num = (int)SaveDataManager.Instance.ItemData.RingCount;
				break;
			case ServerWheelOptionsData.SPIN_BUTTON.RSRING:
				num = (int)SaveDataManager.Instance.ItemData.RedRingCount;
				break;
			case ServerWheelOptionsData.SPIN_BUTTON.TICKET:
			case ServerWheelOptionsData.SPIN_BUTTON.RAID:
				num = GetCostItemNum(currentCostItemId);
				break;
			default:
				flag = false;
				break;
			}
			if (flag && num >= costItemCost * multi)
			{
				result = true;
			}
		}
		return result;
	}

	public void SetupParam(int rouletteId, int remaining, int jackpotRing, int rank, int spEggNum, DateTime nextFree)
	{
		m_rouletteId = rouletteId;
		m_remaining = remaining;
		m_jackpotRing = jackpotRing;
		m_rank = rank;
		m_spEgg = spEggNum;
		m_nextFreeSpin = nextFree;
	}

	public void SetupParam(int rouletteId, int remaining, int jackpotRing, int rank, int spEggNum)
	{
		m_rouletteId = rouletteId;
		m_remaining = remaining;
		m_jackpotRing = jackpotRing;
		m_rank = rank;
		m_spEgg = spEggNum;
	}

	public void CopyTo(ServerWheelOptionsGeneral to)
	{
		for (int i = 0; i < itemLenght; i++)
		{
			to.SetupItem(i, m_itemId[i], m_itemWeight[i], m_itemNum[i]);
		}
		to.SetupParam(m_rouletteId, m_remaining, m_jackpotRing, m_rank, m_spEgg, m_nextFreeSpin);
		to.CopyToCostItem(m_costItem);
	}

	private int GetRemainingTicket()
	{
		int num = 0;
		if (m_costItem != null && m_costItem.Count > 0)
		{
			Dictionary<int, long>.KeyCollection keys = m_costItem.Keys;
			int num2 = 0;
			int num3 = 0;
			{
				foreach (int item in keys)
				{
					if (item >= 240000 && item < 250000)
					{
						num3 = (int)(m_costItem[item] / 10000000);
						num2 = (int)(m_costItem[item] % 10000000);
						if (num2 >= num3)
						{
							num += num2 / num3;
						}
					}
				}
				return num;
			}
		}
		return num;
	}
}
