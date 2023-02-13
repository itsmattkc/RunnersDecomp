public class CharaAbility
{
	private uint[] m_ability_level = new uint[10];

	public uint[] Ability
	{
		get
		{
			return m_ability_level;
		}
		set
		{
			m_ability_level = value;
		}
	}

	public CharaAbility()
	{
		for (uint num = 0u; num < 10; num++)
		{
			m_ability_level[num] = 0u;
		}
	}

	public uint GetTotalLevel()
	{
		uint num = 0u;
		for (uint num2 = 0u; num2 < 10; num2++)
		{
			num += m_ability_level[num2];
		}
		return num;
	}
}
