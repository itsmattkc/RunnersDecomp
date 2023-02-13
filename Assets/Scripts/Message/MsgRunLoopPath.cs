namespace Message
{
	public class MsgRunLoopPath : MessageBase
	{
		public PathComponent m_component;

		public MsgRunLoopPath(PathComponent component)
			: base(20481)
		{
			m_component = component;
		}
	}
}
