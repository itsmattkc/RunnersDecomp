using LitJson;

public class NetServerGetCampaignList : NetBase
{
	protected override void DoRequest()
	{
		SetAction("Game/getCampaignList");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_CampaignList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_CampaignList(JsonData jdata)
	{
		NetUtil.GetResponse_CampaignList(jdata);
	}
}
