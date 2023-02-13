namespace Message
{
	public class MsgTutorialChara : MessageBase
	{
		public HudTutorial.Id m_id;

		public MsgTutorialChara(HudTutorial.Id id)
			: base(12343)
		{
			m_id = id;
		}
	}
}
