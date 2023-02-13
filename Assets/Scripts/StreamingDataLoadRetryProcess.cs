using UnityEngine;

internal class StreamingDataLoadRetryProcess : ServerRetryProcess
{
	private TitleDataLoader m_loader;

	private int m_retryCount;

	public StreamingDataLoadRetryProcess(int retriedCount, GameObject returnObject, TitleDataLoader loader)
		: base(returnObject)
	{
		m_retryCount = retriedCount;
		m_loader = loader;
	}

	public override void Retry()
	{
		m_retryCount++;
		if (m_loader != null)
		{
			m_loader.RetryStreamingDataLoad(m_retryCount);
		}
	}
}
