public class ServerMileageEvent
{
	public enum emEventType
	{
		Incentive,
		BeginBonus,
		EndBonus,
		Goal
	}

	public int Distance
	{
		get;
		set;
	}

	public emEventType EventType
	{
		get;
		set;
	}

	public int Content
	{
		get;
		set;
	}

	public ServerConstants.NumType NumType
	{
		get;
		set;
	}

	public int Num
	{
		get;
		set;
	}

	public int Level
	{
		get;
		set;
	}

	public ServerMileageEvent()
	{
		Distance = 0;
		EventType = emEventType.Incentive;
		Content = 0;
		NumType = ServerConstants.NumType.Number;
		Num = 0;
		Level = 0;
	}
}
