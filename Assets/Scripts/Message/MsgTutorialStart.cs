using UnityEngine;

namespace Message
{
	public class MsgTutorialStart : MessageBase
	{
		public readonly Vector3 m_pos;

		public MsgTutorialStart(Vector3 pos)
			: base(12330)
		{
			m_pos = pos;
		}
	}
}
