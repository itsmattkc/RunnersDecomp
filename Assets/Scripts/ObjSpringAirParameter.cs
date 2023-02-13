using System;

[Serializable]
public class ObjSpringAirParameter : SpawnableParameter
{
	public float firstSpeed;

	public float outOfcontrol;

	public ObjSpringAirParameter()
		: base("ObjSpringAir")
	{
		firstSpeed = 2f;
		outOfcontrol = 0.5f;
	}
}
