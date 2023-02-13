public class AbilityUpParam
{
	private float m_abilityPotential;

	private float m_abilityCost;

	public float Potential
	{
		get
		{
			return m_abilityPotential;
		}
		set
		{
			m_abilityPotential = value;
		}
	}

	public float Cost
	{
		get
		{
			return m_abilityCost;
		}
		set
		{
			m_abilityCost = value;
		}
	}

	public AbilityUpParam()
	{
		m_abilityPotential = 0f;
		m_abilityCost = 0f;
	}
}
