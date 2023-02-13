namespace Message
{
	public class MsgGetCharacterStateSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public MsgGetCharacterStateSucceed()
			: base(61451)
		{
		}
	}
}
