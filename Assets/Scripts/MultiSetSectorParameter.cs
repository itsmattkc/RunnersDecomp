using System;
using UnityEngine;

[Serializable]
public class MultiSetSectorParameter : SpawnableParameter
{
	public int m_count;

	public float m_radius;

	public float m_angle;

	public GameObject m_object;

	public MultiSetSectorParameter()
		: base("MultiSetSector")
	{
		m_count = 2;
		m_radius = 1f;
		m_angle = 180f;
	}
}
