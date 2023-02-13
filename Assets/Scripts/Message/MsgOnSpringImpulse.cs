using UnityEngine;

namespace Message
{
	public class MsgOnSpringImpulse : MessageBase
	{
		public Vector3 m_position;

		public Quaternion m_rotation;

		public float m_firstSpeed;

		public float m_outOfControl;

		public bool m_succeed;

		public MsgOnSpringImpulse(Vector3 pos, Quaternion rot, float firstSpeed, float outOfControl)
			: base(24576)
		{
			m_position = pos;
			m_rotation = rot;
			m_firstSpeed = firstSpeed;
			m_outOfControl = outOfControl;
			m_succeed = false;
		}
	}
}
