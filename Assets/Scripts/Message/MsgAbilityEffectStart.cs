namespace Message
{
	public class MsgAbilityEffectStart : MessageBase
	{
		public readonly ChaoAbility m_ability;

		public readonly string m_effectName;

		public readonly bool m_loop;

		public readonly bool m_center;

		public MsgAbilityEffectStart(ChaoAbility ability, string effectName, bool loop, bool center)
			: base(16387)
		{
			m_ability = ability;
			m_effectName = effectName;
			m_loop = loop;
			m_center = center;
		}
	}
}
