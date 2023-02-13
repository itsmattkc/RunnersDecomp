namespace Message
{
	public class MsgTutorialItem : MessageBase
	{
		public HudTutorial.Id m_id;

		public MsgTutorialItem(HudTutorial.Id id)
			: base(12341)
		{
			m_id = id;
		}
	}
}
