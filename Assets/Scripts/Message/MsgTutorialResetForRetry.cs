using UnityEngine;

namespace Message
{
	public class MsgTutorialResetForRetry : MessageBase
	{
		public Vector3 m_position;

		public Quaternion m_rotation;

		public int m_ring;

		public bool m_blink;

		public MsgTutorialResetForRetry(Vector3 position, Quaternion rotation, bool blink)
			: base(12347)
		{
			m_position = position;
			m_rotation = rotation;
			m_blink = blink;
		}

		public MsgTutorialResetForRetry(int ring, bool blink)
			: base(12347)
		{
			m_ring = ring;
			m_blink = blink;
		}
	}
}
