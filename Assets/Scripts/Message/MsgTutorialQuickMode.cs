namespace Message
{
	public class MsgTutorialQuickMode : MessageBase
	{
		public HudTutorial.Id m_id;

		public MsgTutorialQuickMode(HudTutorial.Id id)
			: base(12345)
		{
			m_id = id;
		}
	}
}
