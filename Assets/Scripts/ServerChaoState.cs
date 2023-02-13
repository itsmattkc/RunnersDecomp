public class ServerChaoState : ServerChaoData
{
	public enum ChaoStatus
	{
		NotOwned,
		Owned,
		MaxLevel
	}

	public enum ChaoDealing
	{
		None,
		Leader,
		Sub
	}

	public ChaoStatus Status
	{
		get;
		set;
	}

	public ChaoDealing Dealing
	{
		get;
		set;
	}

	public int NumInvite
	{
		get;
		set;
	}

	public int NumAcquired
	{
		get;
		set;
	}

	public bool Hidden
	{
		get;
		set;
	}

	public bool IsLvUp
	{
		get;
		set;
	}

	public bool IsNew
	{
		get;
		set;
	}

	public bool IsInvite
	{
		get
		{
			return 0 < NumInvite;
		}
	}

	public bool IsOwned
	{
		get
		{
			return ChaoStatus.NotOwned != Status;
		}
	}

	public ServerChaoState()
	{
		Status = ChaoStatus.NotOwned;
		Dealing = ChaoDealing.None;
		NumInvite = 0;
		NumAcquired = 0;
		Hidden = false;
		IsLvUp = false;
		IsNew = false;
	}
}
