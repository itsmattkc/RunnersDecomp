using UnityEngine;

namespace Message
{
	public class MsgOnJumpBoardHit : MessageBase
	{
		public Vector3 m_position;

		public Quaternion m_rotation;

		public MsgOnJumpBoardHit(Vector3 pos, Quaternion rot)
			: base(24584)
		{
			m_position = pos;
			m_rotation = rot;
		}
	}
}
