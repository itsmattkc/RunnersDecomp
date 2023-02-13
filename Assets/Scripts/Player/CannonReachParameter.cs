using UnityEngine;

namespace Player
{
	public class CannonReachParameter : StateEnteringParameter
	{
		public Vector3 m_position = Vector3.zero;

		public Quaternion m_rotation = Quaternion.identity;

		public float m_height;

		public GameObject m_catchedObject;

		public override void Reset()
		{
		}

		public void Set(Vector3 pos, Quaternion rot, float height, GameObject catchedObject)
		{
			m_position = pos;
			m_rotation = rot;
			m_height = height;
			m_catchedObject = catchedObject;
		}
	}
}
