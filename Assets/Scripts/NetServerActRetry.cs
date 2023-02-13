using LitJson;

public class NetServerActRetry : NetBase
{
	protected override void DoRequest()
	{
		SetAction("Game/actRetry");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string actRetryString = instance.GetActRetryString();
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(actRetryString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}
}
