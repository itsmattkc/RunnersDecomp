using UnityEngine;

namespace Message
{
	public class MsgOnDrawingRings : MessageBase
	{
		public ChaoAbility m_chaoAbility;

		public GameObject m_target;

		public MsgOnDrawingRings()
			: base(24583)
		{
			m_chaoAbility = ChaoAbility.UNKNOWN;
		}

		public MsgOnDrawingRings(ChaoAbility chaoAbility)
			: base(24583)
		{
			m_chaoAbility = chaoAbility;
		}

		public MsgOnDrawingRings(GameObject target)
			: base(24583)
		{
			m_target = target;
			m_chaoAbility = ChaoAbility.UNKNOWN;
		}
	}
}
