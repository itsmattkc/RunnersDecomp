using LitJson;

public class NetDebugResetGameData : NetBase
{
	protected override void DoRequest()
	{
		SetAction("Debug/resetGameData");
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}
}
