namespace Message
{
	public class MsgGetChaoWheelOptionsSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public ServerChaoWheelOptions m_options;

		public MsgGetChaoWheelOptionsSucceed()
			: base(61463)
		{
		}
	}
}
