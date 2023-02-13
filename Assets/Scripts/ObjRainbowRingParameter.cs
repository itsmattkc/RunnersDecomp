using System;

[Serializable]
public class ObjRainbowRingParameter : SpawnableParameter
{
	public float firstSpeed;

	public float outOfcontrol;

	public ObjRainbowRingParameter()
		: base("ObjRainbowRing")
	{
		firstSpeed = 20f;
		outOfcontrol = 0.5f;
	}
}
