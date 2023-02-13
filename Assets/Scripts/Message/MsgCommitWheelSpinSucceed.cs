namespace Message
{
	public class MsgCommitWheelSpinSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public ServerWheelOptions m_wheelOptions;

		public ServerSpinResultGeneral m_resultSpinResultGeneral;

		public MsgCommitWheelSpinSucceed()
			: base(61461)
		{
		}
	}
}
