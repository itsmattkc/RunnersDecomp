namespace Message
{
	public class MsgGetWeeklyLeaderboardOptions : MessageBase
	{
		public ServerWeeklyLeaderboardOptions m_weeklyLeaderboardOptions;

		public MsgGetWeeklyLeaderboardOptions()
			: base(61500)
		{
		}
	}
}
