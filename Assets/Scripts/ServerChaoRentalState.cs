public class ServerChaoRentalState
{
	public string FriendId
	{
		get;
		set;
	}

	public string Name
	{
		get;
		set;
	}

	public string Url
	{
		get;
		set;
	}

	public ServerChaoData ChaoData
	{
		get;
		set;
	}

	public int RentalState
	{
		get;
		set;
	}

	public long NextRentalAt
	{
		get;
		set;
	}

	public bool IsRented
	{
		get
		{
			return 1 == RentalState;
		}
	}

	public bool IsRentalable
	{
		get
		{
			return 0 == NextRentalAt;
		}
	}

	public float TimeSinceStartup
	{
		get;
		set;
	}

	public ServerChaoRentalState()
	{
		FriendId = string.Empty;
		Name = string.Empty;
		Url = string.Empty;
		ChaoData = null;
		RentalState = 0;
		NextRentalAt = 0L;
		TimeSinceStartup = 0f;
	}

	public void Dump()
	{
	}
}
