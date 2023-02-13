namespace Message
{
	public class MsgTransformPhantom : MessageBase
	{
		public readonly PhantomType m_type;

		public MsgTransformPhantom(PhantomType type)
			: base(12304)
		{
			m_type = type;
		}
	}
}
