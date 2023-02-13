using UnityEngine;

internal class StreamingDataKeyRetryProcess : ServerRetryProcess
{
	private GameModeTitle m_title;

	public StreamingDataKeyRetryProcess(GameObject returnObject, GameModeTitle title)
		: base(returnObject)
	{
		m_title = title;
	}

	public override void Retry()
	{
		if (m_title != null)
		{
			m_title.StreamingKeyDataRetry();
		}
	}
}
