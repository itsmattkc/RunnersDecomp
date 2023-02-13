using UnityEngine;

namespace Message
{
	public class MsgOnJumpBoardJump : MessageBase
	{
		public Vector3 m_position;

		public Quaternion m_succeedRotation;

		public Quaternion m_missRotation;

		public float m_succeedFirstSpeed;

		public float m_missFirstSpeed;

		public float m_succeedOutOfcontrol;

		public float m_missOutOfcontrol;

		public bool m_succeed;

		public MsgOnJumpBoardJump(Vector3 pos, Quaternion rot1, Quaternion rot2, float spd1, float spd2, float ooc1, float ooc2)
			: base(24585)
		{
			m_position = pos;
			m_succeedRotation = rot1;
			m_missRotation = rot2;
			m_succeedFirstSpeed = spd1;
			m_missFirstSpeed = spd2;
			m_succeedOutOfcontrol = ooc1;
			m_missOutOfcontrol = ooc2;
			m_succeed = false;
		}
	}
}
