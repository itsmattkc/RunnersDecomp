using LitJson;

public class NetDebugUpgradeChao : NetBase
{
	public int paramChaoId
	{
		get;
		set;
	}

	public int paramLevel
	{
		get;
		set;
	}

	public NetDebugUpgradeChao()
		: this(0, 0)
	{
	}

	public NetDebugUpgradeChao(int chaoId, int level)
	{
		paramChaoId = chaoId;
		paramLevel = level;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/upgradeChao");
		SetParameter_Chao();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Chao()
	{
		WriteActionParamValue("chaoId", paramChaoId);
		WriteActionParamValue("lv", paramLevel);
	}
}
