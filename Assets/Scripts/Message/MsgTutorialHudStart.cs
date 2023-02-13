namespace Message
{
	public class MsgTutorialHudStart : MessageBase
	{
		public HudTutorial.Id m_id;

		public BossType m_bossType;

		public MsgTutorialHudStart(HudTutorial.Id id, BossType bossType)
			: base(12348)
		{
			m_id = id;
			m_bossType = bossType;
		}
	}
}
