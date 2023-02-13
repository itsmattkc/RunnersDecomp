using UnityEngine;

public class TransformParam
{
	public Vector3 m_pos
	{
		get;
		private set;
	}

	public Vector3 m_rot
	{
		get;
		private set;
	}

	public TransformParam(Vector3 pos, Vector3 rot)
	{
		m_pos = pos;
		m_rot = rot;
	}
}
