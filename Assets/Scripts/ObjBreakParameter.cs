using System;
using UnityEngine;

[Serializable]
public class ObjBreakParameter : SpawnableParameter
{
	public GameObject m_modelObject;

	public ObjBreakParameter()
		: base("ObjBreak")
	{
	}
}
