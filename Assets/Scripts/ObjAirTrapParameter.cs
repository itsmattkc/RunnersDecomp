using System;

[Serializable]
public class ObjAirTrapParameter : SpawnableParameter
{
	public float moveSpeed;

	public float moveDistanceX;

	public float moveDistanceY;

	public ObjAirTrapParameter()
		: base("ObjAirTrap")
	{
		moveSpeed = 0f;
		moveDistanceX = 0f;
		moveDistanceY = 0f;
	}
}
