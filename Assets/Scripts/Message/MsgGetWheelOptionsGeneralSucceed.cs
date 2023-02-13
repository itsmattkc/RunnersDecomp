namespace Message
{
	public class MsgGetWheelOptionsGeneralSucceed : MessageBase
	{
		public ServerWheelOptionsGeneral m_wheelOptionsGeneral;

		public MsgGetWheelOptionsGeneralSucceed()
			: base(61459)
		{
		}
	}
}
