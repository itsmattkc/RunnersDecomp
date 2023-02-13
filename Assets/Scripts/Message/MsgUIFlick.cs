namespace Message
{
	public class MsgUIFlick : MessageBase
	{
		private FlickType m_flick_type;

		public FlickType Flick
		{
			get
			{
				return m_flick_type;
			}
		}

		public MsgUIFlick(FlickType type)
			: base(57344)
		{
			m_flick_type = type;
		}
	}
}
