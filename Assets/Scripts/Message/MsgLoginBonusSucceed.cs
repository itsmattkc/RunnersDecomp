namespace Message
{
	public class MsgLoginBonusSucceed : MessageBase
	{
		public ServerLoginBonusData m_loginBonusData;

		public MsgLoginBonusSucceed()
			: base(61445)
		{
		}
	}
}
