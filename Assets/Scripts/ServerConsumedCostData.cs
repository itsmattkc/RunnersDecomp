public class ServerConsumedCostData
{
	public int consumedItemId
	{
		get;
		set;
	}

	public int itemId
	{
		get;
		set;
	}

	public int numItem
	{
		get;
		set;
	}

	public ServerConsumedCostData()
	{
		consumedItemId = 0;
		itemId = 0;
		numItem = 0;
	}

	public void CopyTo(ServerConsumedCostData dest)
	{
		dest.consumedItemId = consumedItemId;
		dest.itemId = itemId;
		dest.numItem = numItem;
	}
}
