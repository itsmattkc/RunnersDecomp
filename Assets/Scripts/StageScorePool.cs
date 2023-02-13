public class StageScorePool
{
	public static readonly int ArrayCount = 30000;

	public StageScoreData[] scoreDatas = new StageScoreData[ArrayCount];

	private int m_objectIndex;

	private int m_lastDistance;

	public int StoredCount
	{
		get
		{
			return m_objectIndex;
		}
		private set
		{
		}
	}

	public void CheckHalfWay()
	{
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null && m_objectIndex != 0)
		{
			if (StageModeManager.Instance != null && StageModeManager.Instance.IsQuickMode())
			{
				ServerQuickModeGameResults gameResult = new ServerQuickModeGameResults(false);
				instance.CheckNativeHalfWayQuickModeResultScore(gameResult);
			}
			else
			{
				ServerGameResults gameResult2 = new ServerGameResults(false, false, false, false, 0, null, null);
				instance.CheckNativeHalfWayResultScore(gameResult2);
			}
		}
		m_objectIndex = 0;
	}

	public void AddScore(StageScoreData scoreData)
	{
		AddScore((ScoreType)scoreData.scoreType, scoreData.scoreValue);
	}

	public void AddScore(ScoreType type, int score)
	{
		if (m_objectIndex >= ArrayCount)
		{
			Debug.Log("StageScorePool arraySize over");
			return;
		}
		scoreDatas[m_objectIndex].scoreType = (byte)type;
		if (type == ScoreType.distance)
		{
			int scoreValue = score - m_lastDistance;
			scoreDatas[m_objectIndex].scoreValue = scoreValue;
			m_lastDistance = score;
		}
		else
		{
			scoreDatas[m_objectIndex].scoreValue = score;
		}
		m_objectIndex++;
	}

	public void DebugLog()
	{
		Debug.Log("StageScorePool.CurrentDataSize = " + m_objectIndex);
	}
}
