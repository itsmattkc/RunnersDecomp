using LitJson;
using System.Collections.Generic;

public class NetServerGetEventRaidBossUserList : NetBase
{
	private int m_eventId;

	private long m_raidBossId;

	private List<ServerEventRaidBossUserState> m_raidBossUserStateList;

	private ServerEventRaidBossBonus m_raidBossBonus;

	private ServerEventRaidBossState m_raidBossState;

	public List<ServerEventRaidBossUserState> RaidBossUserStateList
	{
		get
		{
			return m_raidBossUserStateList;
		}
	}

	public ServerEventRaidBossBonus RaidBossBonus
	{
		get
		{
			return m_raidBossBonus;
		}
	}

	public ServerEventRaidBossState RaidBossState
	{
		get
		{
			return m_raidBossState;
		}
	}

	public NetServerGetEventRaidBossUserList(int eventId, long raidBossId)
	{
		m_eventId = eventId;
		m_raidBossId = raidBossId;
	}

	protected override void DoRequest()
	{
		SetAction("Event/getEventRaidbossUserList");
		WriteActionParamValue("raidbossId", m_raidBossId);
		WriteActionParamValue("eventId", m_eventId);
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		m_raidBossUserStateList = NetUtil.AnalyzeEventRaidBossUserStateList(jdata);
		m_raidBossBonus = NetUtil.AnalyzeEventRaidBossBonus(jdata);
		m_raidBossState = NetUtil.AnalyzeRaidBossState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}
}
