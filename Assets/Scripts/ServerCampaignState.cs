using System.Collections.Generic;

public class ServerCampaignState
{
	private Dictionary<Constants.Campaign.emType, List<ServerCampaignData>> m_dic;

	public ServerCampaignState()
	{
		m_dic = new Dictionary<Constants.Campaign.emType, List<ServerCampaignData>>();
	}

	public bool InSession(int id)
	{
		return InSession(id, 0);
	}

	public bool InSession(int id, int index)
	{
		ServerCampaignData campaign = GetCampaign(id, index);
		if (campaign != null)
		{
			return campaign.InSession();
		}
		return false;
	}

	public bool InAnyIdSession(Constants.Campaign.emType campaignType)
	{
		ServerCampaignData anyIdCampaign = GetAnyIdCampaign(campaignType);
		if (anyIdCampaign != null)
		{
			return anyIdCampaign.InSession();
		}
		return false;
	}

	public bool InSession(Constants.Campaign.emType campaignType)
	{
		return InSession(campaignType, -1);
	}

	public bool InSession(Constants.Campaign.emType campaignType, int id)
	{
		ServerCampaignData campaign = GetCampaign(campaignType, id);
		if (campaign != null)
		{
			return campaign.InSession();
		}
		return false;
	}

	public ServerCampaignData GetCampaign(int id)
	{
		return GetCampaign(id, 0);
	}

	public ServerCampaignData GetCampaign(int id, int index)
	{
		int num = CampaignCount(id);
		if (0 > index || num <= index)
		{
			return null;
		}
		int num2 = 0;
		foreach (KeyValuePair<Constants.Campaign.emType, List<ServerCampaignData>> item in m_dic)
		{
			int count = item.Value.Count;
			for (int i = 0; i < count; i++)
			{
				ServerCampaignData serverCampaignData = item.Value[i];
				if (serverCampaignData.id == id)
				{
					if (num2 == index)
					{
						return serverCampaignData;
					}
					num2++;
					break;
				}
			}
		}
		return null;
	}

	public ServerCampaignData GetAnyIdCampaign(Constants.Campaign.emType campaignType)
	{
		return GetCampaign(campaignType, -1);
	}

	public ServerCampaignData GetCampaign(Constants.Campaign.emType campaignType)
	{
		return GetCampaign(campaignType, -1);
	}

	public ServerCampaignData GetCampaign(Constants.Campaign.emType campaignType, int id)
	{
		List<ServerCampaignData> value = null;
		if (m_dic.TryGetValue(campaignType, out value))
		{
			int count = value.Count;
			if (0 < count)
			{
				ServerCampaignData serverCampaignData = null;
				for (int i = 0; i < count; i++)
				{
					serverCampaignData = value[i];
					if (serverCampaignData.id == id || id == -1)
					{
						return serverCampaignData;
					}
				}
			}
		}
		return null;
	}

	public ServerCampaignData GetCampaignInSession(int id)
	{
		int num = CampaignCount(id);
		for (int i = 0; i < num; i++)
		{
			ServerCampaignData campaignInSession = GetCampaignInSession(id, i);
			if (campaignInSession != null)
			{
				return campaignInSession;
			}
		}
		return null;
	}

	public ServerCampaignData GetCampaignInSession(int id, int index)
	{
		if (InSession(id, index))
		{
			return GetCampaign(id, index);
		}
		return null;
	}

	public ServerCampaignData GetCampaignInSession(Constants.Campaign.emType campaignType)
	{
		return GetCampaignInSession(campaignType, -1);
	}

	public ServerCampaignData GetCampaignInSession(Constants.Campaign.emType campaignType, int id)
	{
		if (InSession(campaignType, id))
		{
			return GetCampaign(campaignType, id);
		}
		return null;
	}

	public int CampaignCount(int id)
	{
		int num = 0;
		foreach (KeyValuePair<Constants.Campaign.emType, List<ServerCampaignData>> item in m_dic)
		{
			int count = item.Value.Count;
			for (int i = 0; i < count; i++)
			{
				ServerCampaignData serverCampaignData = item.Value[i];
				if (serverCampaignData.id == id)
				{
					num++;
					break;
				}
			}
		}
		return num;
	}

	public bool RegistCampaign(ServerCampaignData registData)
	{
		List<ServerCampaignData> value = null;
		if (!m_dic.TryGetValue(registData.campaignType, out value))
		{
			value = new List<ServerCampaignData>();
			value.Add(registData);
			m_dic.Add(registData.campaignType, value);
		}
		else
		{
			ServerCampaignData serverCampaignData = null;
			int count = value.Count;
			bool flag = false;
			for (int i = 0; i < count; i++)
			{
				serverCampaignData = value[i];
				if (serverCampaignData.id == registData.id && serverCampaignData.beginDate == registData.beginDate && serverCampaignData.endDate == registData.endDate)
				{
					registData.CopyTo(serverCampaignData);
					flag = true;
				}
			}
			if (!flag)
			{
				value.Add(registData);
			}
		}
		return true;
	}

	public void RemoveCampaign(ServerCampaignData registData)
	{
		List<ServerCampaignData> value = null;
		if (!m_dic.TryGetValue(registData.campaignType, out value))
		{
			return;
		}
		List<ServerCampaignData> list = new List<ServerCampaignData>();
		int count = value.Count;
		for (int i = 0; i < count; i++)
		{
			ServerCampaignData serverCampaignData = value[i];
			if (serverCampaignData.id == registData.id)
			{
				list.Add(serverCampaignData);
			}
		}
		foreach (ServerCampaignData item in list)
		{
			if (item != null)
			{
				value.Remove(item);
			}
		}
	}
}
