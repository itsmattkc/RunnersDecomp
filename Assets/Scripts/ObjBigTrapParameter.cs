using System;

[Serializable]
public class ObjBigTrapParameter : SpawnableParameter
{
	public float moveSpeedX;

	public float moveSpeedY;

	public float moveDistanceY;

	public float startMoveDistance;

	public ObjBigTrapParameter()
		: base("ObjBigTrap")
	{
		moveSpeedX = -1f;
		moveSpeedY = 0.5f;
		moveDistanceY = 1f;
		startMoveDistance = 20f;
	}
}
