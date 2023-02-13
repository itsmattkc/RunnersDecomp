using System;
using UnityEngine;

[Serializable]
public class ObjCollisionParameter : SpawnableParameter
{
	private float m_size_x;

	private float m_size_y;

	private float m_size_z;

	public ObjCollisionParameter()
		: base("ObjCollision")
	{
		m_size_x = 1f;
		m_size_y = 1f;
		m_size_z = 1f;
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
}
