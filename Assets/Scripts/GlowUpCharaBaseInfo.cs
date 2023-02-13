public class GlowUpCharaBaseInfo
{
	private CharaType m_charaType;

	private int m_level;

	private int m_levelUpCost;

	private int m_currentExp;

	private bool m_isActive;

	public CharaType charaType
	{
		get
		{
			return m_charaType;
		}
		set
		{
			m_charaType = value;
		}
	}

	public int level
	{
		get
		{
			return m_level;
		}
		set
		{
			m_level = value;
		}
	}

	public int levelUpCost
	{
		get
		{
			return m_levelUpCost;
		}
		set
		{
			m_levelUpCost = value;
		}
	}

	public int currentExp
	{
		get
		{
			return m_currentExp;
		}
		set
		{
			m_currentExp = value;
		}
	}

	public bool IsActive
	{
		get
		{
			return m_isActive;
		}
		set
		{
			m_isActive = value;
		}
	}
}
