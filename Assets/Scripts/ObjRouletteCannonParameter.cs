using System;

[Serializable]
public class ObjRouletteCannonParameter : SpawnableParameter
{
	public float firstSpeed;

	public float outOfcontrol;

	public float moveSpeed;

	public float angle;

	public ObjRouletteCannonParameter()
		: base("ObjRouletteCannon")
	{
		firstSpeed = 10f;
		outOfcontrol = 0.5f;
		moveSpeed = 1f;
		angle = 60f;
	}
}
