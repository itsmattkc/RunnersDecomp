using LitJson;

public class NetServerDrawRaidBoss : NetBase
{
	private int m_eventId;

	private long m_score;

	private ServerEventRaidBossState m_raidBossState;

	public ServerEventRaidBossState RaidBossState
	{
		get
		{
			return m_raidBossState;
		}
	}

	public NetServerDrawRaidBoss(int eventId, long score)
	{
		m_eventId = eventId;
		m_score = score;
	}

	protected override void DoRequest()
	{
		SetAction("Game/drawRaidboss");
		WriteActionParamValue("eventId", m_eventId);
		WriteActionParamValue("score", m_score);
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		m_raidBossState = NetUtil.AnalyzeRaidBossState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}
}
