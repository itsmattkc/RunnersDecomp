public class ServerEventRaidBossUserState
{
	private string m_wrestleId;

	private string m_name;

	private int m_grade;

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

	private int m_wrestleCount;

	private int m_wrestleDamage;

	private bool m_wrestleBeatFlg;

	public string WrestleId
	{
		get
		{
			return m_wrestleId;
		}
		set
		{
			m_wrestleId = value;
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

	public int Grade
	{
		get
		{
			return m_grade;
		}
		set
		{
			m_grade = value;
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

	public int WrestleCount
	{
		get
		{
			return m_wrestleCount;
		}
		set
		{
			m_wrestleCount = value;
		}
	}

	public int WrestleDamage
	{
		get
		{
			return m_wrestleDamage;
		}
		set
		{
			m_wrestleDamage = value;
		}
	}

	public bool WrestleBeatFlg
	{
		get
		{
			return m_wrestleBeatFlg;
		}
		set
		{
			m_wrestleBeatFlg = value;
		}
	}

	public ServerEventRaidBossUserState()
	{
		m_wrestleId = string.Empty;
		m_name = string.Empty;
		m_grade = 0;
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
		m_wrestleCount = 0;
		m_wrestleDamage = 0;
		m_wrestleBeatFlg = false;
	}

	public void CopyTo(ServerEventRaidBossUserState to)
	{
		to.m_wrestleId = m_wrestleId;
		to.m_name = m_name;
		to.m_grade = m_grade;
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
		to.m_wrestleCount = m_wrestleCount;
		to.m_wrestleDamage = m_wrestleDamage;
		to.m_wrestleBeatFlg = m_wrestleBeatFlg;
	}
}
