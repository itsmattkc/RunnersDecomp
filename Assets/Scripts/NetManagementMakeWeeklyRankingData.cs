using LitJson;

public class NetManagementMakeWeeklyRankingData : NetBase
{
	protected override void DoRequest()
	{
		SetAction("Management/makeWeeklyRankingData");
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}
}
