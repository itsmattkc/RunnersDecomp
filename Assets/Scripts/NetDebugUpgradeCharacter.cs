using LitJson;

public class NetDebugUpgradeCharacter : NetBase
{
	public int paramCharacterId
	{
		get;
		set;
	}

	public int paramLevel
	{
		get;
		set;
	}

	public NetDebugUpgradeCharacter()
		: this(0, 0)
	{
	}

	public NetDebugUpgradeCharacter(int characterId, int level)
	{
		paramCharacterId = characterId;
		paramLevel = level;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/upgradeCharacter");
		SetParameter_Character();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Character()
	{
		WriteActionParamValue("characterId", paramCharacterId);
		WriteActionParamValue("lv", paramLevel);
	}
}
