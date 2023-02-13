using System.Collections.Generic;

public class ServerLoginBonusRewardList
{
	public List<ServerLoginBonusReward> m_selectRewardList;

	public ServerLoginBonusRewardList()
	{
		m_selectRewardList = new List<ServerLoginBonusReward>();
	}

	public void CopyTo(ServerLoginBonusRewardList dest)
	{
		foreach (ServerLoginBonusReward selectReward in m_selectRewardList)
		{
			dest.m_selectRewardList.Add(selectReward);
		}
	}
}
