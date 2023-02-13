using System.Collections.Generic;

public class ServerLoginBonusReward
{
	public List<ServerItemState> m_itemList;

	public ServerLoginBonusReward()
	{
		m_itemList = new List<ServerItemState>();
	}

	public void CopyTo(ServerLoginBonusReward dest)
	{
		foreach (ServerItemState item in m_itemList)
		{
			dest.m_itemList.Add(item);
		}
	}
}
