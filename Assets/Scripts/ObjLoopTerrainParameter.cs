using System;
using UnityEngine;

[Serializable]
public class ObjLoopTerrainParameter : SpawnableParameter
{
	public string m_pathName;

	public float m_pathZOffset;

	private float m_scalex;

	private float m_scaley;

	private float m_scalez;

	private float m_centerx;

	private float m_centery;

	private float m_centerz;

	public Vector3 Center
	{
		get
		{
			return new Vector3(m_centerx, m_centery, m_centerz);
		}
		set
		{
			m_centerx = value.x;
			m_centery = value.y;
			m_centerz = value.z;
		}
	}

	public Vector3 Size
	{
		get
		{
			return new Vector3(m_scalex, m_scaley, m_scalez);
		}
		set
		{
			m_scalex = value.x;
			m_scaley = value.y;
			m_scalez = value.z;
		}
	}

	public ObjLoopTerrainParameter()
		: base("ObjLoopTerrain")
	{
	}
}
