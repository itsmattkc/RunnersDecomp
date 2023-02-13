using UnityEngine;

internal class StageStreamingDataLoadRetryProcess : ServerRetryProcess
{
	private GameModeStage m_gameModeStage;

	private int m_retryCount;

	public StageStreamingDataLoadRetryProcess(GameObject returnObject, GameModeStage gameModeStage)
		: base(returnObject)
	{
		m_gameModeStage = gameModeStage;
	}

	public override void Retry()
	{
		m_retryCount++;
		if (m_gameModeStage != null)
		{
			m_gameModeStage.RetryStreamingDataLoad(m_retryCount);
		}
	}
}
