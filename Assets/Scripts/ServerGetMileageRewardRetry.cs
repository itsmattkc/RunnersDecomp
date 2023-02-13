using UnityEngine;

public class ServerGetMileageRewardRetry : ServerRetryProcess
{
	private int m_episode;

	private int m_chapter;

	public ServerGetMileageRewardRetry(int episode, int chapter, GameObject callbackObject)
		: base(callbackObject)
	{
		m_episode = episode;
		m_chapter = chapter;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetMileageReward(m_episode, m_chapter, m_callbackObject);
		}
	}
}
