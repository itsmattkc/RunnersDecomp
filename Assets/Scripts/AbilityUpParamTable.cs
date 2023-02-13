using System.Collections.Generic;

public class AbilityUpParamTable
{
	private Dictionary<AbilityType, AbilityUpParamList> m_table;

	public AbilityUpParamTable()
	{
		m_table = new Dictionary<AbilityType, AbilityUpParamList>();
	}

	public void AddList(AbilityType type, AbilityUpParamList list)
	{
		if (m_table != null)
		{
			m_table.Add(type, list);
		}
	}

	public AbilityUpParamList GetList(AbilityType type)
	{
		if (m_table == null)
		{
			return null;
		}
		if (!m_table.ContainsKey(type))
		{
			return null;
		}
		return m_table[type];
	}
}
