using UnityEngine;

public class PlaceObjectData
{
	public int x;

	public int y;

	public int count;

	public Bounds bound;

	public PlaceObjectData(int _x, int _y, int _count, Bounds _b)
	{
		x = _x;
		y = _y;
		count = _count;
		bound = _b;
	}
}
