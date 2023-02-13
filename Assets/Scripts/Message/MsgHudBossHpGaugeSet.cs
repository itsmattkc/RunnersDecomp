namespace Message
{
	public class MsgHudBossHpGaugeSet : MessageBase
	{
		public int m_hp;

		public int m_hpMax;

		public MsgHudBossHpGaugeSet(int hp, int hpMax)
			: base(49153)
		{
			m_hp = hp;
			m_hpMax = hpMax;
		}
	}
}
