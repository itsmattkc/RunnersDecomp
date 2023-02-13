using LitJson;
using System.Collections.Generic;

public class NetServerGetEventUserRaidBossList : NetBase
{
	private int m_eventId;

	private List<ServerEventRaidBossState> m_userRaidBossList;

	private ServerEventUserRaidBossState m_userRaidBossState;

	public List<ServerEventRaidBossState> UserRaidBossList
	{
		get
		{
			return m_userRaidBossList;
		}
	}

	public ServerEventUserRaidBossState UserRaidBossState
	{
		get
		{
			return m_userRaidBossState;
		}
	}

	public NetServerGetEventUserRaidBossList(int eventId)
	{
		m_eventId = eventId;
	}

	protected override void DoRequest()
	{
		SetAction("Event/getEventUserRaidbossList");
		WriteActionParamValue("eventId", m_eventId);
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		m_userRaidBossList = NetUtil.AnalyzeRaidBossStateList(jdata);
		m_userRaidBossState = NetUtil.AnalyzeEventUserRaidBossState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}
}
