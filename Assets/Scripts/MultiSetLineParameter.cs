using System;
using UnityEngine;

[Serializable]
public class MultiSetLineParameter : SpawnableParameter
{
	public int m_count;

	public float m_distance;

	public int m_type;

	public GameObject m_object;

	public MultiSetLineParameter()
		: base("MultiSetLine")
	{
		m_count = 2;
		m_distance = 1f;
		m_type = 0;
	}
}
