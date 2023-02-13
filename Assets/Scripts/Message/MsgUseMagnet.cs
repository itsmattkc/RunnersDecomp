using UnityEngine;

namespace Message
{
	public class MsgUseMagnet : MessageBase
	{
		public readonly GameObject m_obj;

		public readonly GameObject m_target;

		public readonly float m_time;

		public MsgUseMagnet(GameObject obj, float time)
			: base(12360)
		{
			m_obj = obj;
			m_target = null;
			m_time = time;
		}

		public MsgUseMagnet(GameObject obj, GameObject target, float time)
			: base(12360)
		{
			m_obj = obj;
			m_target = target;
			m_time = time;
		}
	}
}
