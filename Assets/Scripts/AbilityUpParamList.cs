using System.Collections.Generic;

public class AbilityUpParamList
{
	private List<AbilityUpParam> m_paramList = new List<AbilityUpParam>();

	public int Count
	{
		get
		{
			return m_paramList.Count;
		}
		private set
		{
		}
	}

	public int GetMaxLevel()
	{
		return m_paramList.Count - 1;
	}

	public AbilityUpParam GetAbilityUpParam(int level)
	{
		int maxLevel = GetMaxLevel();
		if (level > maxLevel)
		{
			return null;
		}
		if (level < 0)
		{
			return null;
		}
		return m_paramList[level];
	}

	public void AddAbilityUpParam(AbilityUpParam param)
	{
		m_paramList.Add(param);
	}
}
