using LitJson;

public class NetDebugForceDrawRaidboss : NetBase
{
	private ServerEventRaidBossState m_raidBossState;

	public int paramEventId
	{
		get;
		set;
	}

	public long paramScore
	{
		get;
		set;
	}

	public ServerEventRaidBossState RaidBossState
	{
		get
		{
			return m_raidBossState;
		}
	}

	public NetDebugForceDrawRaidboss()
		: this(0, 0L)
	{
	}

	public NetDebugForceDrawRaidboss(int eventId, long score)
	{
		paramEventId = eventId;
		paramScore = score;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/forceDrawRaidboss");
		SetParameter_User();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		m_raidBossState = NetUtil.AnalyzeRaidBossState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_User()
	{
		WriteActionParamValue("eventId", paramEventId);
		WriteActionParamValue("score", paramScore);
	}
}
