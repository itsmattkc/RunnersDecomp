public struct RewardData
{
	public int serverId;

	public int reward_id;

	public int reward_count;

	public RewardData(int reward_id, int reward_count)
	{
		serverId = reward_id;
		ServerItem serverItem = new ServerItem((ServerItem.Id)reward_id);
		int num = reward_id;
		switch (serverItem.idType)
		{
		case ServerItem.IdType.EQUIP_ITEM:
			num = serverItem.idIndex;
			break;
		case ServerItem.IdType.RING:
			num = 8;
			break;
		case ServerItem.IdType.RSRING:
			num = 9;
			break;
		}
		this.reward_id = num;
		this.reward_count = reward_count;
	}

	public void Set(RewardData src)
	{
		serverId = src.serverId;
		reward_id = src.reward_id;
		reward_count = src.reward_count;
	}
}
