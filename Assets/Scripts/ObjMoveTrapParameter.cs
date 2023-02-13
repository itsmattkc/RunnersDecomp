using System;

[Serializable]
public class ObjMoveTrapParameter : SpawnableParameter
{
	public float moveSpeed;

	public float moveDistance;

	public float startMoveDistance;

	public ObjMoveTrapParameter()
		: base("ObjMoveTrap")
	{
		moveSpeed = 3f;
		moveDistance = 20f;
		startMoveDistance = 20f;
	}
}
