public class ComboChaoParam
{
	public ChaoAbility m_chaoAbility;

	public ComboChaoAbilityType m_type;

	public float m_extraValue;

	public int m_comboNum;

	public int m_nextCombo;

	public int m_continuationNum;

	public bool m_movement;

	public ComboChaoParam(ChaoAbility chaoAbility, ComboChaoAbilityType type, float extra, int comboNum, int nextCombo)
	{
		m_chaoAbility = chaoAbility;
		m_type = type;
		m_extraValue = extra;
		m_comboNum = comboNum;
		m_nextCombo = nextCombo;
		m_continuationNum = 0;
		m_movement = false;
	}
}
