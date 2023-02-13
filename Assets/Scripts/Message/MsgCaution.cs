using UnityEngine;

namespace Message
{
	public class MsgCaution : MessageBase
	{
		public readonly HudCaution.Type m_cautionType;

		public readonly int m_number;

		public readonly int m_second;

		public readonly float m_rate;

		public readonly ItemType m_itemType;

		public readonly Vector3 m_position;

		public readonly bool m_flag;

		public readonly ObjBossEventBossParameter m_bossParam;

		public MsgCaution(HudCaution.Type cautionType)
			: base(12317)
		{
			m_cautionType = cautionType;
		}

		public MsgCaution(HudCaution.Type cautionType, int number)
			: base(12317)
		{
			m_cautionType = cautionType;
			m_number = number;
		}

		public MsgCaution(HudCaution.Type cautionType, int number, bool flag)
			: base(12317)
		{
			m_cautionType = cautionType;
			m_number = number;
			m_flag = flag;
		}

		public MsgCaution(HudCaution.Type cautionType, float rate)
			: base(12317)
		{
			m_cautionType = cautionType;
			m_rate = rate;
		}

		public MsgCaution(HudCaution.Type cautionType, ObjBossEventBossParameter bossParam)
			: base(12317)
		{
			m_cautionType = cautionType;
			m_bossParam = bossParam;
		}

		public MsgCaution(HudCaution.Type cautionType, ItemType itemType)
			: base(12317)
		{
			m_cautionType = cautionType;
			m_itemType = itemType;
		}

		public MsgCaution(HudCaution.Type cautionType, int number, int second)
			: base(12317)
		{
			m_cautionType = cautionType;
			m_number = number;
			m_second = second;
		}

		public MsgCaution(HudCaution.Type cautionType, int score, Vector3 position)
			: base(12317)
		{
			m_cautionType = cautionType;
			m_number = score;
			m_position = position;
		}
	}
}
