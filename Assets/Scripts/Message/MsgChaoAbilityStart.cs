namespace Message
{
	public class MsgChaoAbilityStart : MessageBase
	{
		public readonly ChaoAbility[] m_ability;

		public bool m_flag;

		public MsgChaoAbilityStart(ChaoAbility[] ability)
			: base(21761)
		{
			m_ability = ability;
			m_flag = false;
		}

		public MsgChaoAbilityStart(ChaoAbility ability)
			: base(21761)
		{
			m_ability = new ChaoAbility[1];
			m_ability[0] = ability;
			m_flag = false;
		}
	}
}
