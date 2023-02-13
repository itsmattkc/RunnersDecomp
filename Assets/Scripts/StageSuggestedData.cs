using System;

public class StageSuggestedData : IComparable
{
	public int id
	{
		get;
		set;
	}

	public CharacterAttribute[] charaAttribute
	{
		get;
		set;
	}

	public int CompareTo(object obj)
	{
		if (this == (StageSuggestedData)obj)
		{
			return 0;
		}
		return -1;
	}
}
