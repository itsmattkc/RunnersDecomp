using System.Runtime.InteropServices;

public struct QuickModePostGameResultNativeParam
{
	public long score;

	public long distance;

	public long numRings;

	public long numFailureRings;

	public long numRedStarRings;

	public bool dailyChallengeComplete;

	public long dailyChallengeValue;

	public long numAnimals;

	public bool closed;

	public int maxComboCount;

	public void Init(ServerQuickModeGameResults resultData)
	{
		if (resultData != null)
		{
			score = resultData.m_score;
			numRings = resultData.m_numRings;
			numFailureRings = resultData.m_numFailureRings;
			numRedStarRings = resultData.m_numRedStarRings;
			distance = resultData.m_distance;
			dailyChallengeValue = resultData.m_dailyMissionValue;
			numAnimals = resultData.m_numAnimals;
			maxComboCount = resultData.m_maxComboCount;
			closed = resultData.m_isSuspended;
			dailyChallengeComplete = resultData.m_dailyMissionComplete;
		}
	}
}
