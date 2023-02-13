public class ServerQuickModeGameResults
{
	public long m_score;

	public long m_numRings;

	public long m_numFailureRings;

	public long m_numRedStarRings;

	public long m_distance;

	public long m_numAnimals;

	public int m_maxComboCount;

	public long m_dailyMissionValue;

	public bool m_dailyMissionComplete;

	public bool m_isSuspended;

	public ServerQuickModeGameResults(bool isSuspended)
	{
		Initialize();
		m_isSuspended = isSuspended;
		if (!m_isSuspended)
		{
			StageScoreManager instance = StageScoreManager.Instance;
			PlayerInformation playerInformation = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
			if (instance != null && playerInformation != null)
			{
				m_score = instance.FinalScore;
				m_numRings = instance.FinalCountData.ring;
				m_numFailureRings = playerInformation.NumLostRings;
				m_numRedStarRings = instance.FinalCountData.red_ring;
				m_distance = instance.FinalCountData.distance;
				m_numAnimals = instance.FinalCountData.animal;
				m_maxComboCount = StageComboManager.Instance.MaxComboCount;
			}
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

	private void Initialize()
	{
		m_score = 0L;
		m_numRings = 0L;
		m_numFailureRings = 0L;
		m_numRedStarRings = 0L;
		m_distance = 0L;
		m_dailyMissionValue = 0L;
		m_numAnimals = 0L;
		m_dailyMissionComplete = false;
		m_isSuspended = false;
	}
}
