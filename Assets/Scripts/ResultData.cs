using System.Collections.Generic;

public class ResultData
{
	public bool m_validResult;

	public string m_stageName;

	public bool m_bossStage;

	public bool m_bossDestroy;

	public bool m_rivalHighScore;

	public bool m_fromOptionTutorial;

	public bool m_eventStage;

	public bool m_quickMode;

	public bool m_missionComplete;

	public MileageMapState m_oldMapState;

	public MileageMapState m_newMapState;

	public List<ServerMileageIncentive> m_mileageIncentiveList;

	public List<ServerItemState> m_dailyMissionIncentiveList;

	public long m_highScore;

	public long m_totalScore;
}
