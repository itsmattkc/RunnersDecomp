using System;

[Serializable]
public class ObjSpringParameter : SpawnableParameter
{
	public float firstSpeed;

	public float outOfcontrol;

	public ObjSpringParameter()
		: base("ObjSpring")
	{
		firstSpeed = 2f;
		outOfcontrol = 0.5f;
	}
}
