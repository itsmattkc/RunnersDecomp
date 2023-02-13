using UnityEngine;

namespace Message
{
	public class MsgStageReplace : MessageBase
	{
		public PlayerSpeed m_speedLevel;

		public Vector3 m_position;

		public Quaternion m_rotation;

		public string m_stageName;

		public MsgStageReplace(PlayerSpeed speedLevel, Vector3 pos, Quaternion rot, string stagename)
			: base(12309)
		{
			m_speedLevel = speedLevel;
			m_position = pos;
			m_rotation = rot;
			m_stageName = stagename;
		}
	}
}
