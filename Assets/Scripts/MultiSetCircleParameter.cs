using System;
using UnityEngine;

[Serializable]
public class MultiSetCircleParameter : SpawnableParameter
{
	public int m_count;

	public float m_radius;

	public GameObject m_object;

	public MultiSetCircleParameter()
		: base("MultiSetCircle")
	{
		m_count = 2;
		m_radius = 1f;
	}
}
