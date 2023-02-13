using System.Collections.Generic;

public class ServerChaoSpinResult
{
	public ServerChaoData AcquiredChaoData
	{
		get;
		set;
	}

	public bool IsRequiredChao
	{
		get;
		set;
	}

	public int NumRequiredSpEggs
	{
		get;
		set;
	}

	public bool IsRequiredSpEgg
	{
		get
		{
			return 0 < NumRequiredSpEggs;
		}
	}

	public Dictionary<int, ServerItemState> ItemState
	{
		get;
		private set;
	}

	public int ItemWon
	{
		get;
		set;
	}

	public bool IsGotAlreadyChaoLevelMax
	{
		get;
		set;
	}

	public ServerChaoSpinResult()
	{
		AcquiredChaoData = null;
		IsRequiredChao = true;
		NumRequiredSpEggs = 0;
		ItemState = new Dictionary<int, ServerItemState>();
		ItemWon = 0;
		IsGotAlreadyChaoLevelMax = false;
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

	public void CopyTo(ServerChaoSpinResult to)
	{
		to.AcquiredChaoData = AcquiredChaoData;
		to.IsRequiredChao = IsRequiredChao;
		to.NumRequiredSpEggs = NumRequiredSpEggs;
		to.ItemState.Clear();
		foreach (ServerItemState value in ItemState.Values)
		{
			to.ItemState.Add(value.m_itemId, value);
		}
		to.ItemWon = ItemWon;
		to.IsGotAlreadyChaoLevelMax = IsGotAlreadyChaoLevelMax;
	}

	public void Dump()
	{
	}
}
