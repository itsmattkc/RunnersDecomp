using System;

public class MileageMapBGData : IComparable
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

	public string window_texture_name
	{
		get;
		set;
	}

	public int CompareTo(object obj)
	{
		if (this == (MileageMapBGData)obj)
		{
			return 0;
		}
		return -1;
	}
}
