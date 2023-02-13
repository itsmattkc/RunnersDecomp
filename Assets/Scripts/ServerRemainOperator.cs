using System.Collections.Generic;

public class ServerRemainOperator
{
	public int operatorData
	{
		get;
		set;
	}

	public int number
	{
		get;
		set;
	}

	public Dictionary<int, ServerItemState> ItemState
	{
		get;
		private set;
	}

	public ServerRemainOperator()
	{
		operatorData = 0;
		number = 0;
		ItemState = new Dictionary<int, ServerItemState>();
	}

	public void CopyTo(ServerRemainOperator to)
	{
		to.operatorData = operatorData;
		to.number = number;
		to.ItemState.Clear();
		foreach (ServerItemState value in ItemState.Values)
		{
			to.ItemState.Add(value.m_itemId, value);
		}
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

	public void Dump()
	{
	}
}
