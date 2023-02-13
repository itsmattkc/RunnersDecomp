using System.Collections.Generic;

public class ServerDailyBattlePrizeData
{
	public int operatorData;

	public int number;

	public Dictionary<int, ServerItemState> ItemState
	{
		get;
		private set;
	}

	public ServerDailyBattlePrizeData()
	{
		operatorData = 0;
		number = 0;
		ItemState = new Dictionary<int, ServerItemState>();
	}

	public void Dump()
	{
	}

	public void AddItemState(ServerItemState itemState)
	{
		if (ItemState.ContainsKey(itemState.m_itemId))
		{
			ItemState[itemState.m_itemId].m_num += itemState.m_num;
		}
		else
		{
			ItemState.Add(itemState.m_itemId, itemState);
		}
	}

	public void CopyTo(ServerDailyBattlePrizeData dest)
	{
		dest.operatorData = operatorData;
		dest.number = number;
		dest.ItemState.Clear();
		foreach (ServerItemState value in ItemState.Values)
		{
			dest.ItemState.Add(value.m_itemId, value);
		}
	}

	public void CopyTo(ServerRemainOperator to)
	{
		to.operatorData = operatorData;
		to.number = number;
		to.ItemState.Clear();
		to.ItemState.Clear();
		foreach (ServerItemState value in ItemState.Values)
		{
			to.ItemState.Add(value.m_itemId, value);
		}
	}

	public static List<ServerRemainOperator> ConvertRemainOperatorList(List<ServerDailyBattlePrizeData> prizeList)
	{
		if (prizeList == null || prizeList.Count <= 0)
		{
			return null;
		}
		List<ServerRemainOperator> list = new List<ServerRemainOperator>();
		foreach (ServerDailyBattlePrizeData prize in prizeList)
		{
			ServerRemainOperator serverRemainOperator = new ServerRemainOperator();
			prize.CopyTo(serverRemainOperator);
			list.Add(serverRemainOperator);
		}
		return list;
	}
}
