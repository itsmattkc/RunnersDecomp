using UnityEngine;

public class ServerGetLeaderboardEntriesRetry : ServerRetryProcess
{
	public int m_mode;

	public int m_first;

	public int m_count;

	public int m_index;

	public int m_rankingType;

	public int m_eventId;

	public string[] m_friendIdList;

	public ServerGetLeaderboardEntriesRetry(int mode, int first, int count, int index, int rankingType, int eventId, string[] friendIdList, GameObject callbackObject)
		: base(callbackObject)
	{
		m_mode = mode;
		m_first = first;
		m_count = count;
		m_index = index;
		m_rankingType = rankingType;
		m_eventId = eventId;
		m_friendIdList = friendIdList;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetLeaderboardEntries(m_mode, m_first, m_count, m_index, m_rankingType, m_eventId, m_friendIdList, m_callbackObject);
		}
	}
}
