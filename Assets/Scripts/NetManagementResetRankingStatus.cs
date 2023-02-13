using LitJson;

public class NetManagementResetRankingStatus : NetBase
{
	protected override void DoRequest()
	{
		SetAction("Management/resetRankingStatus");
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}
}
