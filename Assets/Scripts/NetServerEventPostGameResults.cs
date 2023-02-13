using LitJson;

public class NetServerEventPostGameResults : NetBase
{
	private int m_eventId = -1;

	private int m_numRaidBossRings;

	private ServerEventUserRaidBossState m_userRaidBossState;

	public ServerEventUserRaidBossState UserRaidBossState
	{
		get
		{
			return m_userRaidBossState;
		}
	}

	public NetServerEventPostGameResults(int eventId, int numRaidBossRings)
	{
		m_eventId = eventId;
		m_numRaidBossRings = numRaidBossRings;
	}

	protected override void DoRequest()
	{
		SetAction("Event/eventPostGameResults");
		SetParameter_EventId();
		SetParameter_RaidBossRings();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_EventUserRaidBossState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_EventId()
	{
		WriteActionParamValue("eventId", m_eventId);
	}

	private void SetParameter_RaidBossRings()
	{
		WriteActionParamValue("numRaidbossRings", m_numRaidBossRings);
	}

	private void GetResponse_EventUserRaidBossState(JsonData jdata)
	{
		m_userRaidBossState = NetUtil.AnalyzeEventUserRaidBossState(jdata);
	}
}
