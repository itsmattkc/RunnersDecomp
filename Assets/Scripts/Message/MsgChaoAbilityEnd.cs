namespace Message
{
	public class MsgChaoAbilityEnd : MessageBase
	{
		public readonly ChaoAbility[] m_ability;

		public MsgChaoAbilityEnd(ChaoAbility[] ability)
			: base(21762)
		{
			m_ability = ability;
		}

		public MsgChaoAbilityEnd(ChaoAbility ability)
			: base(21762)
		{
			m_ability = new ChaoAbility[1];
			m_ability[0] = ability;
		}
	}
}
