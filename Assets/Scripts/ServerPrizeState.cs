using System.Collections.Generic;
using Text;

public class ServerPrizeState
{
	private List<ServerPrizeData> m_prizeList;

	private string m_prizeText;

	public List<ServerPrizeData> prizeList
	{
		get
		{
			return m_prizeList;
		}
	}

	public ServerPrizeState()
	{
		m_prizeList = new List<ServerPrizeData>();
		m_prizeText = null;
	}

	public ServerPrizeState(ServerWheelOptionsData data)
	{
		m_prizeList = null;
		m_prizeText = null;
		if (data.dataType != ServerWheelOptionsData.DATA_TYPE.RANKUP)
		{
			return;
		}
		ServerWheelOptions orgRankupData = data.GetOrgRankupData();
		if (orgRankupData != null)
		{
			int num = orgRankupData.m_items.Length;
			for (int i = 0; i < num; i++)
			{
				AddPrize(new ServerPrizeData
				{
					itemId = orgRankupData.m_items[i],
					num = orgRankupData.m_itemQuantities[i],
					weight = orgRankupData.m_itemWeight[i]
				});
			}
		}
	}

	public bool AddPrize(ServerPrizeData data)
	{
		if (m_prizeList == null)
		{
			m_prizeList = new List<ServerPrizeData>();
		}
		m_prizeList.Add(data);
		return true;
	}

	public void ResetPrizeList()
	{
		if (m_prizeList != null)
		{
			m_prizeList.Clear();
		}
		m_prizeList = new List<ServerPrizeData>();
	}

	public bool IsExpired()
	{
		return false;
	}

	public bool IsData()
	{
		bool result = false;
		if (m_prizeList != null && m_prizeList.Count > 0)
		{
			result = true;
		}
		return result;
	}

	public List<string[]> GetItemOdds(ServerWheelOptionsData data)
	{
		return data.GetItemOdds();
	}

	public string GetPrizeText(ServerWheelOptionsData data)
	{
		string result = null;
		RouletteCategory category = data.category;
		if (category != 0 && category != RouletteCategory.ALL && category != RouletteCategory.GENERAL)
		{
			m_prizeText = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Roulette", "note_" + RouletteUtility.GetRouletteCategoryName(category)).text;
			string prizeList = RouletteUtility.GetPrizeList(this);
			result = m_prizeText.Replace("{PARAM}", prizeList);
		}
		return result;
	}

	public List<ServerItem> GetAttentionList()
	{
		List<ServerItem> list = null;
		if (m_prizeList != null && m_prizeList.Count > 0)
		{
			List<ServerItem> listData = new List<ServerItem>();
			List<ServerItem> listData2 = new List<ServerItem>();
			foreach (ServerPrizeData prize in m_prizeList)
			{
				ServerItem item = new ServerItem((ServerItem.Id)prize.itemId);
				if (item.idType == ServerItem.IdType.CHARA)
				{
					bool flag = true;
					if (listData.Count > 0)
					{
						foreach (ServerItem item2 in listData)
						{
							if (item2.id == item.id)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						listData.Add(item);
					}
				}
				else
				{
					if (item.idType != ServerItem.IdType.CHAO || prize.itemId < 402000)
					{
						continue;
					}
					bool flag2 = true;
					if (listData2.Count > 0)
					{
						foreach (ServerItem item3 in listData2)
						{
							if (item3.id == item.id)
							{
								flag2 = false;
								break;
							}
						}
					}
					if (flag2)
					{
						listData2.Add(item);
					}
				}
			}
			if (listData.Count > 0 || listData2.Count > 0)
			{
				list = new List<ServerItem>();
				int num = 4;
				int num2 = listData.Count;
				int num3 = listData2.Count;
				if (num2 > 2)
				{
					num2 = 2;
				}
				if (num3 > num - num2)
				{
					num3 = num - num2;
				}
				GeneralUtil.RandomList(ref listData);
				GeneralUtil.RandomList(ref listData2);
				if (num2 > 0)
				{
					for (int i = 0; i < num2; i++)
					{
						list.Add(listData[i]);
					}
				}
				if (num3 > 0)
				{
					for (int j = 0; j < num3; j++)
					{
						list.Add(listData2[j]);
					}
				}
			}
		}
		return list;
	}

	public void CopyTo(ServerPrizeState to)
	{
		if (to == null || prizeList == null || prizeList.Count <= 0)
		{
			return;
		}
		to.ResetPrizeList();
		foreach (ServerPrizeData prize in prizeList)
		{
			if (prize != null)
			{
				to.AddPrize(prize);
			}
		}
	}
}
