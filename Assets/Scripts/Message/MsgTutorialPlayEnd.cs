using Tutorial;
using UnityEngine;

namespace Message
{
	public class MsgTutorialPlayEnd : MessageBase
	{
		public readonly bool m_complete;

		public readonly bool m_retry;

		public readonly EventID m_nextEventID;

		public readonly Vector3 m_pos;

		public readonly Quaternion m_rot;

		public MsgTutorialPlayEnd(bool complete, bool retry, EventID nextEventID, Vector3 pos, Quaternion rot)
			: base(12335)
		{
			m_complete = complete;
			m_retry = retry;
			m_nextEventID = nextEventID;
			m_pos = pos;
			m_rot = rot;
		}
	}
}
