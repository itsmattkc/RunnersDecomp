namespace Message
{
	public class MsgEventActStartSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public MsgEventActStartSucceed()
			: base(61508)
		{
		}
	}
}
