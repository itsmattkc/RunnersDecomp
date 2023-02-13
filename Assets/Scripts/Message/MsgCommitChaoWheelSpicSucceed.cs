namespace Message
{
	public class MsgCommitChaoWheelSpicSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public ServerChaoWheelOptions m_chaoWheelOptions;

		public ServerSpinResultGeneral m_resultSpinResultGeneral;

		public MsgCommitChaoWheelSpicSucceed()
			: base(61467)
		{
		}
	}
}
