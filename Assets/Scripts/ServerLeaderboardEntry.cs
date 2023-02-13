using App;

public class ServerLeaderboardEntry
{
	public string m_hspId;

	public long m_score;

	public long m_hiScore;

	public int m_userData;

	public string m_name;

	public string m_url;

	public bool m_energyFlg;

	public int m_grade;

	public int m_gradeChanged;

	public long m_expireTime;

	public int m_numRank;

	public long m_loginTime;

	public int m_charaId;

	public int m_charaLevel;

	public int m_subCharaId;

	public int m_subCharaLevel;

	public int m_mainChaoId;

	public int m_mainChaoLevel;

	public int m_subChaoId;

	public int m_subChaoLevel;

	public Env.Language m_language;

	public int m_leagueIndex;

	public ServerLeaderboardEntry()
	{
		m_hspId = "0123456789abcdef";
		m_score = 0L;
		m_hiScore = 0L;
		m_userData = 0;
		m_name = "0123456789abcdef";
		m_url = "0123456789abcdef";
		m_energyFlg = false;
		m_grade = 0;
		m_gradeChanged = 0;
		m_expireTime = 0L;
	}

	public void CopyTo(ServerLeaderboardEntry to)
	{
		to.m_gradeChanged = ((to.m_grade != 0) ? (to.m_grade - m_grade) : 0);
		to.m_hspId = m_hspId;
		to.m_score = m_score;
		to.m_hiScore = m_hiScore;
		to.m_userData = m_userData;
		to.m_name = m_name;
		to.m_url = m_url;
		to.m_energyFlg = m_energyFlg;
		to.m_grade = m_grade;
		to.m_expireTime = m_expireTime;
		to.m_numRank = m_numRank;
		to.m_loginTime = m_loginTime;
		to.m_charaId = m_charaId;
		to.m_charaLevel = m_charaLevel;
		to.m_subCharaId = m_subCharaId;
		to.m_subCharaLevel = m_subCharaLevel;
		to.m_mainChaoId = m_mainChaoId;
		to.m_mainChaoLevel = m_mainChaoLevel;
		to.m_subChaoId = m_subChaoId;
		to.m_subChaoLevel = m_subChaoLevel;
		to.m_language = m_language;
		to.m_leagueIndex = m_leagueIndex;
	}

	public ServerLeaderboardEntry Clone()
	{
		ServerLeaderboardEntry serverLeaderboardEntry = new ServerLeaderboardEntry();
		serverLeaderboardEntry.m_gradeChanged = m_gradeChanged;
		serverLeaderboardEntry.m_hspId = m_hspId;
		serverLeaderboardEntry.m_score = m_score;
		serverLeaderboardEntry.m_hiScore = m_hiScore;
		serverLeaderboardEntry.m_userData = m_userData;
		serverLeaderboardEntry.m_name = m_name;
		serverLeaderboardEntry.m_url = m_url;
		serverLeaderboardEntry.m_energyFlg = m_energyFlg;
		serverLeaderboardEntry.m_grade = m_grade;
		serverLeaderboardEntry.m_expireTime = m_expireTime;
		serverLeaderboardEntry.m_numRank = m_numRank;
		serverLeaderboardEntry.m_loginTime = m_loginTime;
		serverLeaderboardEntry.m_charaId = m_charaId;
		serverLeaderboardEntry.m_charaLevel = m_charaLevel;
		serverLeaderboardEntry.m_subCharaId = m_subCharaId;
		serverLeaderboardEntry.m_subCharaLevel = m_subCharaLevel;
		serverLeaderboardEntry.m_mainChaoId = m_mainChaoId;
		serverLeaderboardEntry.m_mainChaoLevel = m_mainChaoLevel;
		serverLeaderboardEntry.m_subChaoId = m_subChaoId;
		serverLeaderboardEntry.m_subChaoLevel = m_subChaoLevel;
		serverLeaderboardEntry.m_language = m_language;
		serverLeaderboardEntry.m_leagueIndex = m_leagueIndex;
		return serverLeaderboardEntry;
	}

	public bool IsSentEnergy()
	{
		return m_energyFlg;
	}
}
