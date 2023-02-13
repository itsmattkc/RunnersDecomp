using System;

[Serializable]
public class ObjCannonParameter : SpawnableParameter
{
	public float firstSpeed;

	public float outOfcontrol;

	public float moveSpeed;

	public float moveArea;

	public ObjCannonParameter()
		: base("ObjCannon")
	{
		firstSpeed = 10f;
		outOfcontrol = 0.5f;
		moveSpeed = 0.4f;
		moveArea = 50f;
	}
}
