using System;

public class RaidBossAttackRateTable : IComparable
{
	public float[] attackRate
	{
		get;
		set;
	}

	public RaidBossAttackRateTable()
	{
	}

	public RaidBossAttackRateTable(float[] data)
	{
		attackRate = data;
	}

	public int CompareTo(object obj)
	{
		if (this == (RaidBossAttackRateTable)obj)
		{
			return 0;
		}
		return -1;
	}
}
