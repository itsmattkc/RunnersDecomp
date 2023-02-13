using System;

[Serializable]
public class ObjEnemyConstantParameter : SpawnableParameter
{
	public float moveSpeed;

	public float moveDistance;

	public float startMoveDistance;

	public int tblID;

	public ObjEnemyConstantParameter()
		: base("ObjEnemyConstant")
	{
		moveSpeed = 0f;
		moveDistance = 0f;
		startMoveDistance = 0f;
		tblID = 0;
	}
}
