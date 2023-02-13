using UnityEngine;

namespace Message
{
	public class MsgOnCannonImpulse : MessageBase
	{
		public Vector3 m_position;

		public Quaternion m_rotation;

		public float m_firstSpeed;

		public float m_outOfControl;

		public bool m_roulette;

		public bool m_succeed;

		public MsgOnCannonImpulse(Vector3 pos, Quaternion rot, float firstSpeed, float outOfControl, bool roulette)
			: base(24578)
		{
			m_position = pos;
			m_rotation = rot;
			m_firstSpeed = firstSpeed;
			m_outOfControl = outOfControl;
			m_roulette = roulette;
			m_succeed = false;
		}
	}
}
