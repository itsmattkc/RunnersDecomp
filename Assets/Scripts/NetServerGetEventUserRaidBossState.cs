using LitJson;

public class NetServerGetEventUserRaidBossState : NetBase
{
	private int m_eventId;

	private ServerEventUserRaidBossState m_userRaidBossState;

	public ServerEventUserRaidBossState UserRaidBossState
	{
		get
		{
			return m_userRaidBossState;
		}
	}

	public NetServerGetEventUserRaidBossState(int eventId)
	{
		m_eventId = eventId;
	}

	protected override void DoRequest()
	{
		SetAction("Event/getEventUserRaidboss");
		WriteActionParamValue("eventId", m_eventId);
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		m_userRaidBossState = NetUtil.AnalyzeEventUserRaidBossState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}
}
