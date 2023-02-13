using System;

[Serializable]
public class ObjEnemySwingParameter : SpawnableParameter
{
	public float moveSpeed;

	public float moveDistanceX;

	public float moveDistanceY;

	public int tblID;

	public ObjEnemySwingParameter()
		: base("ObjEnemySwing")
	{
		moveSpeed = 0f;
		moveDistanceX = 0f;
		moveDistanceY = 0f;
		tblID = 0;
	}
}
