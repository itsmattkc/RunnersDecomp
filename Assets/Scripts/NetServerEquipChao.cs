using LitJson;

public class NetServerEquipChao : NetBase
{
	public int paramMainChaoId
	{
		get;
		set;
	}

	public int paramSubChaoId
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public NetServerEquipChao()
		: this(0, 0)
	{
	}

	public NetServerEquipChao(int mainChaoId, int subChaoId)
	{
		paramMainChaoId = mainChaoId;
		paramSubChaoId = subChaoId;
	}

	protected override void DoRequest()
	{
		SetAction("Chao/equipChao");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string equipChaoString = instance.GetEquipChaoString(paramMainChaoId, paramSubChaoId);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(equipChaoString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_MainChaoId()
	{
		WriteActionParamValue("mainChaoId", paramMainChaoId);
	}

	private void SetParameter_SubChaoId()
	{
		WriteActionParamValue("subChaoId", paramSubChaoId);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}
}
