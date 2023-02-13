public class ServerGameResults
{
	public long m_score;

	public long m_numRings;

	public long m_numFailureRings;

	public long m_numRedStarRings;

	public long m_distance;

	public long m_dailyMissionValue;

	public bool m_dailyMissionComplete;

	public bool m_isSuspended;

	public long m_numAnimals;

	public int m_reachPoint;

	public bool m_clearChapter;

	public int m_numBossAttack;

	public long m_maxChapterScore;

	public bool m_chaoEggPresent;

	public bool m_isBossDestroyed;

	public int m_maxComboCount;

	public int? m_eventId;

	public long? m_eventValue;

	public ServerGameResults(bool isSuspended, bool tutorialStage, bool chaoEggPresent, bool bossStage, int oldNumBossAttack, int? eventId, long? eventValue)
	{
		Initialize(oldNumBossAttack);
		m_isSuspended = isSuspended;
		m_eventId = eventId;
		m_eventValue = eventValue;
		LevelInformation levelInformation = GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
		if (!m_isSuspended)
		{
			StageScoreManager instance = StageScoreManager.Instance;
			PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
			if (instance != null && playerInformation != null && levelInformation != null)
			{
				m_score = ((!bossStage) ? instance.FinalScore : 0);
				m_numRings = instance.FinalCountData.ring;
				m_numFailureRings = playerInformation.NumLostRings;
				m_numRedStarRings = instance.FinalCountData.red_ring;
				m_distance = ((!bossStage) ? instance.FinalCountData.distance : 0);
				m_numAnimals = instance.FinalCountData.animal;
				m_reachPoint = 0;
				m_clearChapter = false;
				if (levelInformation.NowBoss)
				{
					m_numBossAttack = ((!levelInformation.BossDestroy) ? levelInformation.NumBossAttack : 0);
				}
				m_maxComboCount = StageComboManager.Instance.MaxComboCount;
			}
			m_chaoEggPresent = chaoEggPresent;
		}
		if (levelInformation != null)
		{
			m_isBossDestroyed = levelInformation.BossDestroy;
		}
		StageMissionManager stageMissionManager = GameObjectUtil.FindGameObjectComponent<StageMissionManager>("StageMissionManager");
		if (stageMissionManager != null)
		{
			m_dailyMissionComplete = stageMissionManager.Completed;
			if (SaveDataManager.Instance != null)
			{
				DailyMissionData dailyMission = SaveDataManager.Instance.PlayerData.DailyMission;
				m_dailyMissionValue = dailyMission.progress;
			}
		}
	}

	public void SetMapProgress(MileageMapState prevMapInfo, ref long[] pointScore, bool existBossInChapter)
	{
		if (EventManager.Instance != null && EventManager.Instance.IsSpecialStage())
		{
			m_clearChapter = false;
			m_maxChapterScore = 0L;
			m_reachPoint = 0;
			return;
		}
		m_maxChapterScore = pointScore[5];
		if (m_isSuspended)
		{
			return;
		}
		LevelInformation levelInformation = GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
		if (prevMapInfo != null)
		{
			long score = prevMapInfo.m_score;
			long num = score + m_score;
			for (int num2 = 5; num2 >= 0; num2--)
			{
				if (num >= pointScore[num2])
				{
					m_reachPoint = num2;
					break;
				}
			}
			if (m_reachPoint >= 5 && !existBossInChapter)
			{
				m_clearChapter = true;
			}
		}
		if (levelInformation != null && levelInformation.NowBoss && levelInformation.BossDestroy)
		{
			m_clearChapter = true;
		}
	}

	private void Initialize(int oldNumBossAttack)
	{
		m_score = 0L;
		m_numRings = 0L;
		m_numFailureRings = 0L;
		m_numRedStarRings = 0L;
		m_distance = 0L;
		m_dailyMissionValue = 0L;
		m_dailyMissionComplete = false;
		m_numAnimals = 0L;
		m_reachPoint = 0;
		m_clearChapter = false;
		m_numBossAttack = oldNumBossAttack;
		m_chaoEggPresent = false;
		m_isSuspended = false;
	}
}
