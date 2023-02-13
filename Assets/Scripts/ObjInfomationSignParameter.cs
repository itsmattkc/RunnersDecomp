using System;
using UnityEngine;

[Serializable]
public class ObjInfomationSignParameter : SpawnableParameter
{
	public GameObject m_infomationObject;

	public ObjInfomationSignParameter()
		: base("ObjInfomationSign")
	{
	}
}
