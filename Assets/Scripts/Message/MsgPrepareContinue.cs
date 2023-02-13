namespace Message
{
	public class MsgPrepareContinue : MessageBase
	{
		public bool m_bossStage;

		public bool m_timeUp;

		public MsgPrepareContinue(bool bossStage, bool timeUp)
			: base(12353)
		{
			m_bossStage = bossStage;
			m_timeUp = timeUp;
		}
	}
}
