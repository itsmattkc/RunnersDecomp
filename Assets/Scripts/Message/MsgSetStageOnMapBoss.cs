using UnityEngine;

namespace Message
{
	public class MsgSetStageOnMapBoss : MessageBase
	{
		public Vector3 m_position;

		public Quaternion m_rotation;

		public string m_stageName;

		public BossType m_bossType;

		public MsgSetStageOnMapBoss(Vector3 pos, Quaternion rot, string stagename, BossType bossType)
			: base(12312)
		{
			m_position = pos;
			m_rotation = rot;
			m_stageName = stagename;
			m_bossType = bossType;
		}
	}
}
