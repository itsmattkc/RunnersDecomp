namespace Message
{
	public class MsgBossEnd : MessageBase
	{
		public bool m_dead;

		public MsgBossEnd(bool dead)
			: base(12307)
		{
			m_dead = dead;
		}
	}
}
