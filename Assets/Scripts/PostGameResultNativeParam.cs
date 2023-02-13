using System.Runtime.InteropServices;

public struct PostGameResultNativeParam
{
	public long score;

	public long distance;

	public long numRings;

	public long numFailureRings;

	public long numRedStarRings;

	public bool dailyChallengeComplete;

	public long dailyChallengeValue;

	public bool closed;

	public long numAnimals;

	public int reachPoint;

	public bool chapterClear;

	public int numBossAttack;

	public bool getChaoEgg;

	public long stageMaxScore;

	public int eventId;

	public long eventValue;

	public bool bossDestroyed;

	public int maxComboCount;

	public void Init(ServerGameResults resultData)
	{
		if (resultData != null)
		{
			closed = resultData.m_isSuspended;
			score = resultData.m_score;
			stageMaxScore = resultData.m_maxChapterScore;
			numRings = resultData.m_numRings;
			numFailureRings = resultData.m_numFailureRings;
			numRedStarRings = resultData.m_numRedStarRings;
			distance = resultData.m_distance;
			dailyChallengeValue = resultData.m_dailyMissionValue;
			dailyChallengeComplete = resultData.m_dailyMissionComplete;
			numAnimals = resultData.m_numAnimals;
			reachPoint = resultData.m_reachPoint;
			chapterClear = resultData.m_clearChapter;
			numBossAttack = resultData.m_numBossAttack;
			getChaoEgg = resultData.m_chaoEggPresent;
			int? num = resultData.m_eventId;
			if (num.HasValue)
			{
				eventId = resultData.m_eventId.Value;
				eventValue = resultData.m_eventValue.Value;
			}
			else
			{
				eventId = -1;
				eventValue = -1L;
			}
			bossDestroyed = resultData.m_isBossDestroyed;
			maxComboCount = resultData.m_maxComboCount;
		}
	}
}
