namespace Message
{
	public class MsgGetPlayerStateSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public MsgGetPlayerStateSucceed()
			: base(61450)
		{
		}
	}
}
