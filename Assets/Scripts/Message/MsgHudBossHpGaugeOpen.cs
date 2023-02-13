namespace Message
{
	public class MsgHudBossHpGaugeOpen : MessageBase
	{
		public BossType m_bossType;

		public int m_level;

		public int m_hp;

		public int m_hpMax;

		public MsgHudBossHpGaugeOpen(BossType bossType, int level, int hp, int hpMax)
			: base(49152)
		{
			m_bossType = bossType;
			m_level = level;
			m_hp = hp;
			m_hpMax = hpMax;
		}
	}
}
