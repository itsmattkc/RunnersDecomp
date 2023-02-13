namespace Message
{
	public class MsgQuickModeActStartSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public MsgQuickModeActStartSucceed()
			: base(61513)
		{
		}
	}
}
