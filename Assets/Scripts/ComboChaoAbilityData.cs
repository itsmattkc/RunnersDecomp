public class ComboChaoAbilityData
{
	public ChaoAbility m_chaoAbility;

	public ComboChaoAbilityType m_type;

	public float m_timeMax;

	public ComboChaoAbilityData(ChaoAbility chaoAbility, ComboChaoAbilityType type, float timeMax)
	{
		m_chaoAbility = chaoAbility;
		m_type = type;
		m_timeMax = timeMax;
	}
}
