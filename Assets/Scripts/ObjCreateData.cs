using UnityEngine;

public class ObjCreateData
{
	public GameObject m_obj;

	public GameObject m_src;

	public Vector3 m_pos;

	public Quaternion m_rot;

	public bool m_create;

	public ObjCreateData(GameObject src, Vector3 pos, Quaternion rot)
	{
		m_obj = null;
		m_src = src;
		m_pos = pos;
		m_rot = rot;
		m_create = false;
	}
}
