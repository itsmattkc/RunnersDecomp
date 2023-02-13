using UnityEngine;

public class ServerEventGameResults
{
	public long m_numRings;

	public long m_numFailureRings;

	public long m_numRedStarRings;

	public bool m_isSuspended;

	public long m_dailyMissionValue;

	public bool m_dailyMissionComplete;

	public int m_eventId;

	public long m_eventValue;

	public long m_raidBossId;

	public int m_raidBossDamage;

	public bool m_isRaidBossBeat;

	public ServerEventGameResults(bool isSuspended, int eventId, long eventValue, long raidBossId)
	{
		m_isSuspended = isSuspended;
		m_numRings = 0L;
		m_numFailureRings = 0L;
		m_numRedStarRings = 0L;
		m_dailyMissionValue = 0L;
		m_dailyMissionComplete = false;
		m_eventId = eventId;
		m_eventValue = eventValue;
		m_raidBossId = raidBossId;
		m_raidBossDamage = 0;
		m_isRaidBossBeat = false;
		if (m_isSuspended)
		{
			return;
		}
		StageScoreManager instance = StageScoreManager.Instance;
		if (instance != null)
		{
			m_numRedStarRings = instance.FinalCountData.red_ring;
		}
		LevelInformation levelInformation = GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
		if (levelInformation != null)
		{
			int num = 0;
			RaidBossData currentRaidData = RaidBossInfo.currentRaidData;
			if (currentRaidData != null)
			{
				num = (int)(currentRaidData.hpMax - currentRaidData.hp);
			}
			m_raidBossDamage = Mathf.Max(levelInformation.NumBossAttack - num, 0);
			m_isRaidBossBeat = levelInformation.BossDestroy;
		}
	}
}
