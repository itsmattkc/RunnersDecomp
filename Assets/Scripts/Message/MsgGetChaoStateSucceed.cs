namespace Message
{
	public class MsgGetChaoStateSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public MsgGetChaoStateSucceed()
			: base(61452)
		{
		}
	}
}
