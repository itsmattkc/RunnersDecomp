using System;

[Serializable]
public class ObjDashRingParameter : SpawnableParameter
{
	public float firstSpeed;

	public float outOfcontrol;

	public ObjDashRingParameter()
		: base("ObjDashRing")
	{
		firstSpeed = 8f;
		outOfcontrol = 0.5f;
	}
}
