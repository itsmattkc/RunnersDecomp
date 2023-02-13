namespace Message
{
	public class MsgGetVariousParameterSucceed : MessageBase
	{
		public int m_energyRefreshTime;

		public int m_energyRecoveryMax;

		public int m_onePlayCmCount;

		public int m_onePlayContinueCount;

		public int m_cmSkipCount;

		public bool m_isPurchased;

		public MsgGetVariousParameterSucceed()
			: base(61444)
		{
		}
	}
}
