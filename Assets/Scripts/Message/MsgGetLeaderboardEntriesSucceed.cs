namespace Message
{
	public class MsgGetLeaderboardEntriesSucceed : MessageBase
	{
		public ServerLeaderboardEntries m_leaderboardEntries;

		public MsgGetLeaderboardEntriesSucceed()
			: base(61479)
		{
		}
	}
}
