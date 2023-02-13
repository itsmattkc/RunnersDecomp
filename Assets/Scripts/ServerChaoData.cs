public class ServerChaoData
{
	public enum RarityType
	{
		NORMAL = 0,
		RARE = 1,
		SRARE = 2,
		PLAYER = 100,
		CAMPAIGN = 101
	}

	public int Id
	{
		get;
		set;
	}

	public int Level
	{
		get;
		set;
	}

	public int Rarity
	{
		get;
		set;
	}

	public ServerChaoData()
	{
		Id = 0;
		Level = 0;
		Rarity = -1;
	}

	public void Dump()
	{
	}
}
