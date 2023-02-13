public class ServerMileageBonus
{
	public enum emBonusType
	{
		Score,
		Ring
	}

	public emBonusType BonusType
	{
		get;
		set;
	}

	public ServerConstants.NumType NumType
	{
		get;
		set;
	}

	public int NumBonus
	{
		get;
		set;
	}

	public ServerMileageBonus()
	{
		BonusType = emBonusType.Score;
		NumType = ServerConstants.NumType.Number;
		NumBonus = 0;
	}
}
