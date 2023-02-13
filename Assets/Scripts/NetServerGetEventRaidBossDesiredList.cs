using LitJson;
using System.Collections.Generic;

public class NetServerGetEventRaidBossDesiredList : NetBase
{
	private int m_eventId;

	private long m_raidBossId;

	private List<string> m_friendIdList = new List<string>();

	private List<ServerEventRaidBossDesiredState> m_desiredList;

	public List<ServerEventRaidBossDesiredState> DesiredList
	{
		get
		{
			return m_desiredList;
		}
	}

	public NetServerGetEventRaidBossDesiredList(int eventId, long raidBossId, List<string> friendIdList)
	{
		m_eventId = eventId;
		m_raidBossId = raidBossId;
		if (friendIdList == null)
		{
			return;
		}
		foreach (string friendId in friendIdList)
		{
			m_friendIdList.Add(friendId);
		}
	}

	protected override void DoRequest()
	{
		SetAction("Event/getEventRaidbossDesiredList");
		WriteActionParamValue("raidbossId", m_raidBossId);
		WriteActionParamValue("eventId", m_eventId);
		List<object> list = new List<object>();
		foreach (string friendId in m_friendIdList)
		{
			if (!string.IsNullOrEmpty(friendId))
			{
				list.Add(friendId);
			}
		}
		WriteActionParamArray("friendIdList", list);
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		m_desiredList = NetUtil.AnalyzeEventRaidbossDesiredList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}
}
