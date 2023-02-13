using System;
using UnityEngine;

[Serializable]
public class ObjFloorParameter : SpawnableParameter
{
	public GameObject m_modelObject;

	public ObjFloorParameter()
		: base("ObjAirFloor")
	{
	}
}
