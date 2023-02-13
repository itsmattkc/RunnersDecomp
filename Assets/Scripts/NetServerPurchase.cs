using LitJson;

public class NetServerPurchase : NetBase
{
	public bool paramPurchaseResult
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public NetServerPurchase()
		: this(false)
	{
	}

	public NetServerPurchase(bool isSuccess)
	{
		paramPurchaseResult = isSuccess;
	}

	protected override void DoRequest()
	{
		SetAction("Store/purchase");
		SetParameter_PurchaseResult();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		if (!paramPurchaseResult)
		{
			base.state = emState.UnavailableFailed;
			base.resultStCd = ServerInterface.StatusCode.HspPurchaseError;
		}
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_PurchaseResult()
	{
		WriteActionParamValue("isSuccess", paramPurchaseResult ? 1 : 0);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}
}
