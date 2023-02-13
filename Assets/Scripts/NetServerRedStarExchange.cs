using LitJson;

public class NetServerRedStarExchange : NetBase
{
	public int paramStoreItemId
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public NetServerRedStarExchange()
		: this(0)
	{
	}

	public NetServerRedStarExchange(int storeItemId)
	{
		paramStoreItemId = storeItemId;
	}

	protected override void DoRequest()
	{
		SetAction("Store/redstarExchange");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string redStarExchangeString = instance.GetRedStarExchangeString(paramStoreItemId);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(redStarExchangeString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_StoreItemId()
	{
		WriteActionParamValue("itemId", paramStoreItemId);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}
}
