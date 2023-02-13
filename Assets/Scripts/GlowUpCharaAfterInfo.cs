using System.Collections.Generic;

public class GlowUpCharaAfterInfo
{
	private int m_level;

	private int m_levelUpCost;

	private int m_exp;

	private List<AbilityType> m_abilityList;

	private List<int> m_abilityListExp;

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

	public int exp
	{
		get
		{
			return m_exp;
		}
		set
		{
			m_exp = value;
		}
	}

	public List<AbilityType> abilityList
	{
		get
		{
			return m_abilityList;
		}
		set
		{
			m_abilityList = value;
		}
	}

	public List<int> abilityListExp
	{
		get
		{
			return m_abilityListExp;
		}
		set
		{
			m_abilityListExp = value;
		}
	}
}
