namespace Message
{
	public class MsgActivePointMarker : MessageBase
	{
		public PointMarkerType m_type;

		public bool m_activated;

		public MsgActivePointMarker(PointMarkerType type)
			: base(12328)
		{
			m_type = type;
		}
	}
}
