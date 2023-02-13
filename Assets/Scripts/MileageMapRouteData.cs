using System;

public class MileageMapRouteData : IComparable
{
	public int id
	{
		get;
		set;
	}

	public MileageBonus ability_type
	{
		get;
		set;
	}

	public float ability_value
	{
		get;
		set;
	}

	public int effect_flag
	{
		get;
		set;
	}

	public string texture_name
	{
		get;
		set;
	}

	public int CompareTo(object obj)
	{
		if (this == (MileageMapRouteData)obj)
		{
			return 0;
		}
		return -1;
	}
}
