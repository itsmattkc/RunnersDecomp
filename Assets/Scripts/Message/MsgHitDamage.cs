using UnityEngine;

namespace Message
{
	public class MsgHitDamage : MessageBase
	{
		public readonly GameObject m_sender;

		public int m_attackPower;

		public uint m_attackAttribute;

		public MsgHitDamage(GameObject sender, AttackPower attack)
			: base(16384)
		{
			m_sender = sender;
			m_attackPower = (int)attack;
		}
	}
}
