using UnityEngine;

public class FriendSignPointData
{
	public GameObject m_obj;

	public float m_distance;

	public float m_nextDistance;

	public bool m_myPoint;

	public FriendSignPointData(GameObject obj, float distance, float nextDistance, bool myPoint)
	{
		m_obj = obj;
		m_distance = distance;
		m_nextDistance = nextDistance;
		m_myPoint = myPoint;
	}
}
