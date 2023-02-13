using UnityEngine;

namespace Player
{
	public class JumpSpringParameter : StateEnteringParameter
	{
		public Vector3 m_position = Vector3.zero;

		public Quaternion m_rotation = Quaternion.identity;

		public float m_outOfControlTime;

		public float m_firstSpeed;

		public override void Reset()
		{
			m_outOfControlTime = 0f;
			m_firstSpeed = 0f;
		}

		public void Set(Vector3 pos, Quaternion rot, float speed, float time)
		{
			m_position = pos;
			m_rotation = rot;
			m_outOfControlTime = time;
			m_firstSpeed = speed;
		}
	}
}
