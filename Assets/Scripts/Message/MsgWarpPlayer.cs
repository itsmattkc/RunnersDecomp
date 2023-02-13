using UnityEngine;

namespace Message
{
	public class MsgWarpPlayer : MessageBase
	{
		public Vector3 m_position;

		public Quaternion m_rotation;

		public bool m_hold;

		public MsgWarpPlayer(Vector3 pos, Quaternion rot, bool hold)
			: base(20485)
		{
			m_position = pos;
			m_rotation = rot;
			m_hold = hold;
		}
	}
}
