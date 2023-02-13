namespace Message
{
	public class MsgAbilityEffectEnd : MessageBase
	{
		public readonly ChaoAbility m_ability;

		public readonly string m_effectName;

		public MsgAbilityEffectEnd(ChaoAbility ability, string effectName)
			: base(16388)
		{
			m_ability = ability;
			m_effectName = effectName;
		}
	}
}
