using LitJson;

public class NetDebugUpdRouletteData : NetBase
{
	public int paramRank
	{
		get;
		set;
	}

	public int paramNumRemaining
	{
		get;
		set;
	}

	public int paramItemWon
	{
		get;
		set;
	}

	public NetDebugUpdRouletteData()
		: this(0, 0, 0)
	{
	}

	public NetDebugUpdRouletteData(int rank, int numRemaining, int itemWon)
	{
		paramRank = rank;
		paramNumRemaining = numRemaining;
		paramItemWon = itemWon;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/updRouletteData");
		SetParameter_Roulette();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Roulette()
	{
		WriteActionParamValue("rouletteRank", paramRank);
		WriteActionParamValue("numRemainingRoulette", paramNumRemaining);
		WriteActionParamValue("itemWon", paramItemWon);
	}
}
