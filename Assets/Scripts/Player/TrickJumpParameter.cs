using UnityEngine;

namespace Player
{
	public class TrickJumpParameter : StateEnteringParameter
	{
		public bool m_succeed = true;

		public Vector3 m_position = Vector3.zero;

		public Quaternion m_rotation = Quaternion.identity;

		public float m_outOfControlTime;

		public float m_firstSpeed;

		public Quaternion m_succeedRotation = Quaternion.identity;

		public float m_succeedOutOfcontrol;

		public float m_succeedFirstSpeed;

		public override void Reset()
		{
			m_succeed = false;
			m_outOfControlTime = 0f;
			m_firstSpeed = 0f;
		}

		public void Set(Vector3 pos, Quaternion rot, float speed, float time, Quaternion succeedRot, float succeedSpeed, float succeedTime, bool succeed)
		{
			m_position = pos;
			m_rotation = rot;
			m_outOfControlTime = time;
			m_firstSpeed = speed;
			m_succeedRotation = succeedRot;
			m_succeedOutOfcontrol = succeedTime;
			m_succeedFirstSpeed = succeedSpeed;
			m_succeed = succeed;
		}
	}
}
