namespace Message
{
	public class MsgGetDailyMissionDataSucceed : MessageBase
	{
		public ServerDailyChallengeState m_dailyChallengeState;

		public MsgGetDailyMissionDataSucceed()
			: base(61486)
		{
		}
	}
}
