namespace Message
{
	public class MsgLoginBonusSelectSucceed : MessageBase
	{
		public ServerLoginBonusReward m_loginBonusReward;

		public ServerLoginBonusReward m_firstLoginBonusReward;

		public MsgLoginBonusSelectSucceed()
			: base(61446)
		{
		}
	}
}
