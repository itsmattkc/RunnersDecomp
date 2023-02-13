public class ServerEventRaidBossDesiredState
{
	private string m_desireId;

	private string m_name;

	private int m_numRank;

	private int m_loginTime;

	private int m_charaId;

	private int m_charaLevel;

	private int m_subCharaId;

	private int m_subCharaLevel;

	private int m_mainChaoId;

	private int m_mainChaoLevel;

	private int m_subChaoId;

	private int m_subChaoLevel;

	private int m_language;

	private int m_league;

	private int m_numBeatedEnterprise;

	public string DesireId
	{
		get
		{
			return m_desireId;
		}
		set
		{
			m_desireId = value;
		}
	}

	public string UserName
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}

	public int NumRank
	{
		get
		{
			return m_numRank;
		}
		set
		{
			m_numRank = value;
		}
	}

	public int LoginTime
	{
		get
		{
			return m_loginTime;
		}
		set
		{
			m_loginTime = value;
		}
	}

	public int CharaId
	{
		get
		{
			return m_charaId;
		}
		set
		{
			m_charaId = value;
		}
	}

	public int CharaLevel
	{
		get
		{
			return m_charaLevel;
		}
		set
		{
			m_charaLevel = value;
		}
	}

	public int SubCharaId
	{
		get
		{
			return m_subCharaId;
		}
		set
		{
			m_subCharaId = value;
		}
	}

	public int SubCharaLevel
	{
		get
		{
			return m_subCharaLevel;
		}
		set
		{
			m_subCharaLevel = value;
		}
	}

	public int MainChaoId
	{
		get
		{
			return m_mainChaoId;
		}
		set
		{
			m_mainChaoId = value;
		}
	}

	public int MainChaoLevel
	{
		get
		{
			return m_mainChaoLevel;
		}
		set
		{
			m_mainChaoLevel = value;
		}
	}

	public int SubChaoId
	{
		get
		{
			return m_subChaoId;
		}
		set
		{
			m_subChaoId = value;
		}
	}

	public int SubChaoLevel
	{
		get
		{
			return m_subChaoLevel;
		}
		set
		{
			m_subChaoLevel = value;
		}
	}

	public int Language
	{
		get
		{
			return m_language;
		}
		set
		{
			m_language = value;
		}
	}

	public int League
	{
		get
		{
			return m_league;
		}
		set
		{
			m_league = value;
		}
	}

	public int NumBeatedEnterprise
	{
		get
		{
			return m_numBeatedEnterprise;
		}
		set
		{
			m_numBeatedEnterprise = value;
		}
	}

	public ServerEventRaidBossDesiredState()
	{
		m_desireId = string.Empty;
		m_name = string.Empty;
		m_numRank = 0;
		m_loginTime = 0;
		m_charaId = 0;
		m_charaLevel = 0;
		m_subCharaId = 0;
		m_subCharaLevel = 0;
		m_mainChaoId = 0;
		m_mainChaoLevel = 0;
		m_subChaoId = 0;
		m_subChaoLevel = 0;
		m_language = 0;
		m_league = 0;
		m_numBeatedEnterprise = 0;
	}

	public void CopyTo(ServerEventRaidBossDesiredState to)
	{
		to.m_desireId = m_desireId;
		to.m_name = m_name;
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
		to.m_league = m_league;
		to.m_numBeatedEnterprise = m_numBeatedEnterprise;
	}
}
