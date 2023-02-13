using LitJson;

public class NetDebugUpdDailyMission : NetBase
{
	public int paramMissionId
	{
		get;
		set;
	}

	public NetDebugUpdDailyMission()
		: this(0)
	{
	}

	public NetDebugUpdDailyMission(int missionId)
	{
		paramMissionId = missionId;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/updDailyMission");
		SetParameter_DailyMission();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_DailyMission()
	{
		WriteActionParamValue("missionId", paramMissionId);
	}
}
