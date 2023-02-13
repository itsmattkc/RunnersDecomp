using System;
using UnityEngine;

[Serializable]
public class MultiSetParaloopCircleParameter : SpawnableParameter
{
	public int m_count;

	public float m_radius;

	public GameObject m_object;

	private float m_size_x;

	private float m_size_y;

	private float m_size_z;

	private float m_center_x;

	private float m_center_y;

	private float m_center_z;

	public MultiSetParaloopCircleParameter()
		: base("MultiSetParaloopCircle")
	{
		m_count = 2;
		m_radius = 1f;
		m_size_x = 0f;
		m_size_y = 0f;
		m_size_z = 0f;
		m_center_x = 0f;
		m_center_y = 0f;
		m_center_z = 0f;
	}

	public void SetSize(Vector3 size)
	{
		m_size_x = size.x;
		m_size_y = size.y;
		m_size_z = size.z;
	}

	public Vector3 GetSize()
	{
		return new Vector3(m_size_x, m_size_y, m_size_z);
	}

	public void SetCenter(Vector3 center)
	{
		m_center_x = center.x;
		m_center_y = center.y;
		m_center_z = center.z;
	}

	public Vector3 GetCenter()
	{
		return new Vector3(m_center_x, m_center_y, m_center_z);
	}
}
