using UnityEngine;

public class AbilityButtonParams
{
	private CharaType m_charaType;

	private AbilityType m_abilityType;

	private GameObject m_buttonObject;

	public CharaType Character
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

	public AbilityType Ability
	{
		get
		{
			return m_abilityType;
		}
		set
		{
			m_abilityType = value;
		}
	}
}
