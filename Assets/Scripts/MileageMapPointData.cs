using System;

public class MileageMapPointData : IComparable
{
	public int id
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
		if (this == (MileageMapPointData)obj)
		{
			return 0;
		}
		return -1;
	}
}
