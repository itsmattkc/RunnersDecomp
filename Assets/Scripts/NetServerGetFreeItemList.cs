using LitJson;

public class NetServerGetFreeItemList : NetBase
{
	public ServerFreeItemState resultFreeItemState;

	protected override void DoRequest()
	{
		SetAction("Game/getFreeItemList");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_FreeItemList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_FreeItemList(JsonData jdata)
	{
		resultFreeItemState = NetUtil.AnalyzeFreeItemList(jdata);
	}
}
