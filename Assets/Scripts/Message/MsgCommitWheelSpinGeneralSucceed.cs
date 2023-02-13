namespace Message
{
	public class MsgCommitWheelSpinGeneralSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public ServerWheelOptionsGeneral m_wheelOptionsGeneral;

		public ServerSpinResultGeneral m_resultSpinResultGeneral;

		public MsgCommitWheelSpinGeneralSucceed()
			: base(61460)
		{
		}
	}
}
