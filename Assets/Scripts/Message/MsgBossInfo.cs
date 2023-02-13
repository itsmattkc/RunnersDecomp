using UnityEngine;

namespace Message
{
	public class MsgBossInfo : MessageBase
	{
		public GameObject m_boss;

		public Vector3 m_position;

		public Quaternion m_rotation;

		public bool m_succeed;

		public MsgBossInfo()
			: base(12322)
		{
		}
	}
}
