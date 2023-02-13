namespace Message
{
	public class MsgRequestEnergySucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public long m_resultExpireTime;

		public MsgRequestEnergySucceed()
			: base(61481)
		{
		}
	}
}
